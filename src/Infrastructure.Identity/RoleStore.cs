using Dapper;
using Microsoft.AspNetCore.Identity;
using RU.Challenge.Domain.Entities.Auth;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Identity
{
    public class RoleStore : IRoleStore<Role>
    {
        private readonly IDbConnection _dbConnection;

        public RoleStore(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _dbConnection.ExecuteAsync(
                sql: "INSERT INTO role_auth (id, role_name, normalized_role_name) VALUES (@Id, @RoleName, @NormalizedRoleName)",
                param: new { role.Id, role.RoleName, role.NormalizedRoleName });

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _dbConnection.ExecuteAsync(
                sql: "DELETE FROM role_auth WHERE id = @Id", param: new { role.Id });

            return IdentityResult.Success;
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(roleId, out Guid id))
                throw new ArgumentException($"Id was not a valid Guid: {roleId}", nameof(roleId));

            return await _dbConnection.QueryFirstOrDefaultAsync<Role>(
                sql: "SELECT * FROM role_auth WHERE id = @Id", param: new { Id = roleId });
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Role>(
                sql: "SELECT * FROM role_auth WHERE normalized_role_name LIKE @NormalizedRoleName", param: new { NormalizedRoleName = $"%{normalizedRoleName}%" });
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _dbConnection.ExecuteAsync(
                sql: "UPDATE role_auth SET role_name = @RoleName, normalized_role_name = @NormalizedRoleName WHERE id = @Id",
                param: new { role.Id, role.RoleName, role.NormalizedRoleName });

            return IdentityResult.Success;
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedRoleName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.RoleName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedRoleName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.RoleName = roleName;
            return Task.CompletedTask;
        }

        public void Dispose()
            => _dbConnection.Dispose();
    }
}