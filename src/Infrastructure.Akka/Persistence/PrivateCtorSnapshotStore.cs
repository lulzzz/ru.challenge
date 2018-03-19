﻿using Akka.Configuration;
using Akka.Persistence.Sql.Common.Snapshot;
using Akka.Serialization;
using Akka.Util;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Data.Common;

namespace Akka.Persistence.PostgreSql.Snapshot
{
    public class PrivateCtorSnapshotStore : SqlSnapshotStore
    {
        public readonly PostgreSqlPersistence Extension = PostgreSqlPersistence.Get(Context.System);
        public PostgreSqlSnapshotStoreSettings SnapshotSettings { get; }

        public PrivateCtorSnapshotStore(Config snapshotConfig) : base(snapshotConfig)
        {
            var config = snapshotConfig.WithFallback(Extension.DefaultJournalConfig);
            var storedAsString = config.GetString("stored-as");
            if (!Enum.TryParse(storedAsString, true, out StoredAsType storedAs))
            {
                throw new ConfigurationException($"Value [{storedAsString}] of the 'stored-as' HOCON config key is not valid. Valid values: bytea, json, jsonb.");
            }

            QueryExecutor = new PrivateCtorPostgreSqlQueryExecutor(new PrivateCtorPostgreSqlQueryConfiguration(
                schemaName: config.GetString("schema-name"),
                snapshotTableName: config.GetString("table-name"),
                persistenceIdColumnName: "persistence_id",
                sequenceNrColumnName: "sequence_nr",
                payloadColumnName: "payload",
                manifestColumnName: "manifest",
                timestampColumnName: "created_at",
                serializerIdColumnName: "serializer_id",
                timeout: config.GetTimeSpan("connection-timeout"),
                storedAs: storedAs,
                defaultSerializer: config.GetString("serializer")),
                    Context.System.Serialization);

            SnapshotSettings = new PostgreSqlSnapshotStoreSettings(config);
        }

        protected override DbConnection CreateDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        public override ISnapshotQueryExecutor QueryExecutor { get; }
    }

    public class PrivateCtorPostgreSqlQueryExecutor : AbstractQueryExecutor
    {
        private readonly Func<object, SerializationResult> _serialize;
        private readonly Func<Type, object, string, int?, object> _deserialize;

        public PrivateCtorPostgreSqlQueryExecutor(PrivateCtorPostgreSqlQueryConfiguration configuration, Akka.Serialization.Serialization serialization) : base(configuration, serialization)
        {
            CreateSnapshotTableSql = $@"
                DO
                $do$
                BEGIN
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{Configuration.SchemaName}' AND TABLE_NAME = '{Configuration.SnapshotTableName}') THEN
                    CREATE TABLE {Configuration.FullSnapshotTableName} (
                        {Configuration.PersistenceIdColumnName} VARCHAR(255) NOT NULL,
                        {Configuration.SequenceNrColumnName} BIGINT NOT NULL,
                        {Configuration.TimestampColumnName} BIGINT NOT NULL,
                        {Configuration.ManifestColumnName} VARCHAR(500) NOT NULL,
                        {Configuration.PayloadColumnName} {configuration.StoredAs.ToString().ToUpperInvariant()} NOT NULL,
                        {Configuration.SerializerIdColumnName} INTEGER NULL,
                        CONSTRAINT {Configuration.SnapshotTableName}_pk PRIMARY KEY ({Configuration.PersistenceIdColumnName}, {Configuration.SequenceNrColumnName})
                    );
                    CREATE INDEX {Configuration.SnapshotTableName}_{Configuration.SequenceNrColumnName}_idx ON {Configuration.FullSnapshotTableName}({Configuration.SequenceNrColumnName});
                    CREATE INDEX {Configuration.SnapshotTableName}_{Configuration.TimestampColumnName}_idx ON {Configuration.FullSnapshotTableName}({Configuration.TimestampColumnName});
                END IF;
                END
                $do$";

            InsertSnapshotSql = $@"
                WITH upsert AS (
                    UPDATE {Configuration.FullSnapshotTableName}
                    SET
                        {Configuration.TimestampColumnName} = @Timestamp,
                        {Configuration.PayloadColumnName} = @Payload
                    WHERE {Configuration.PersistenceIdColumnName} = @PersistenceId
                    AND {Configuration.SequenceNrColumnName} = @SequenceNr
                    RETURNING *)
                INSERT INTO {Configuration.FullSnapshotTableName} (
                    {Configuration.PersistenceIdColumnName},
                    {Configuration.SequenceNrColumnName},
                    {Configuration.TimestampColumnName},
                    {Configuration.ManifestColumnName},
                    {Configuration.PayloadColumnName},
                    {Configuration.SerializerIdColumnName})
                SELECT @PersistenceId, @SequenceNr, @Timestamp, @Manifest, @Payload, @SerializerId
                WHERE NOT EXISTS (SELECT * FROM upsert)";

            switch (configuration.StoredAs)
            {
                case StoredAsType.ByteA:
                    _serialize = ss =>
                    {
                        var serializer = Serialization.FindSerializerFor(ss);
                        return new SerializationResult(NpgsqlDbType.Bytea, serializer.ToBinary(ss), serializer);
                    };
                    _deserialize = (type, serialized, manifest, serializerId) =>
                    {
                        if (serializerId.HasValue)
                        {
                            return Serialization.Deserialize((byte[])serialized, serializerId.Value, manifest);
                        }
                        else
                        {
                            // Support old writes that did not set the serializer id
                            var deserializer = Serialization.FindSerializerForType(type, Configuration.DefaultSerializer);
                            return deserializer.FromBinary((byte[])serialized, type);
                        }
                    };
                    break;

                case StoredAsType.JsonB:
                    _serialize = ss => new SerializationResult(NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(ss, configuration.JsonSerializerSettings), null);
                    _deserialize = (type, serialized, manifest, serializerId) => JsonConvert.DeserializeObject((string)serialized, type, configuration.JsonSerializerSettings);
                    break;

                case StoredAsType.Json:
                    _serialize = ss => new SerializationResult(NpgsqlDbType.Json, JsonConvert.SerializeObject(ss, configuration.JsonSerializerSettings), null);
                    _deserialize = (type, serialized, manifest, serializerId) => JsonConvert.DeserializeObject((string)serialized, type, configuration.JsonSerializerSettings);
                    break;

                default:
                    throw new NotSupportedException($"{configuration.StoredAs} is not supported Db type for a payload");
            }
        }

        protected override string InsertSnapshotSql { get; }

        protected override DbCommand CreateCommand(DbConnection connection)
        {
            return ((NpgsqlConnection)connection).CreateCommand();
        }

        protected override void SetTimestampParameter(DateTime timestamp, DbCommand command) => AddParameter(command, "@Timestamp", DbType.Int64, timestamp.Ticks);

        protected override void SetPayloadParameter(object snapshot, DbCommand command)
        {
            var serializationResult = _serialize(snapshot);
            command.Parameters.Add(new NpgsqlParameter("@Payload", serializationResult.DbType) { Value = serializationResult.Payload });
        }

        protected override SelectedSnapshot ReadSnapshot(DbDataReader reader)
        {
            var persistenceId = reader.GetString(0);
            var sequenceNr = reader.GetInt64(1);
            var timestamp = new DateTime(reader.GetInt64(2));
            var manifest = reader.GetString(3);

            int? serializerId = null;
            Type type = null;

            if (!string.IsNullOrEmpty(manifest))
                type = Type.GetType(manifest, true);

            if (!reader.IsDBNull(5))
                serializerId = reader.GetInt32(5);

            var snapshot = _deserialize(type, reader[4], manifest, serializerId);

            var metadata = new SnapshotMetadata(persistenceId, sequenceNr, timestamp);
            return new SelectedSnapshot(metadata, snapshot);
        }

        protected override string CreateSnapshotTableSql { get; }

        protected override void SetManifestParameters(object snapshot, DbCommand command)
        {
            var snapshotType = snapshot.GetType();
            var serializer = Serialization.FindSerializerForType(snapshotType, Configuration.DefaultSerializer);

            string manifest = "";
            if (serializer is SerializerWithStringManifest)
            {
                manifest = ((SerializerWithStringManifest)serializer).Manifest(snapshot);
            }
            else
            {
                if (!serializer.IncludeManifest)
                {
                    manifest = snapshotType.TypeQualifiedName();
                }
            }
            AddParameter(command, "@Manifest", DbType.String, manifest);
            AddParameter(command, "@SerializerId", DbType.Int32, serializer.Identifier);
        }
    }

    public class PrivateCtorPostgreSqlQueryConfiguration : QueryConfiguration
    {
        public readonly StoredAsType StoredAs;
        public readonly JsonSerializerSettings JsonSerializerSettings;

        public PrivateCtorPostgreSqlQueryConfiguration(
            string schemaName,
            string snapshotTableName,
            string persistenceIdColumnName,
            string sequenceNrColumnName,
            string payloadColumnName,
            string manifestColumnName,
            string timestampColumnName,
            string serializerIdColumnName,
            TimeSpan timeout,
            StoredAsType storedAs,
            string defaultSerializer,
            JsonSerializerSettings jsonSerializerSettings = null)
            : base(schemaName, snapshotTableName, persistenceIdColumnName, sequenceNrColumnName, payloadColumnName, manifestColumnName, timestampColumnName, serializerIdColumnName, timeout, defaultSerializer)
        {
            StoredAs = storedAs;
            JsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings
            {
                ContractResolver = new AkkaContractResolver()
            };
        }
    }

    public class SerializationResult
    {
        public SerializationResult(NpgsqlDbType dbType, object payload, Serializer serializer)
        {
            DbType = dbType;
            Payload = payload;
            Serializer = serializer;
        }

        public NpgsqlDbType DbType { get; private set; }
        public object Payload { get; private set; }
        public Serializer Serializer { get; private set; }
    }
}