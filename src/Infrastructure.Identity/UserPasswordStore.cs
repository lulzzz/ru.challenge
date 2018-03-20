using Dapper;
using Microsoft.AspNetCore.Identity;
using RU.Challenge.Domain.Entities.Auth;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Identity
{
    public class UserPasswordStore : IUserPasswordStore<User>
    {
        private readonly IDbConnection _dbConnection;

        public UserPasswordStore(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _dbConnection.ExecuteAsync(
                sql: "INSERT INTO user_auth (id, user_name, normalized_user_name, email, password_hash) VALUES (@Id, @UserName, @NormalizedUserName, @Email, @PasswordHash)",
                param: new { user.Id, user.UserName, user.NormalizedUserName, user.Email, user.PasswordHash });

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _dbConnection.ExecuteAsync(
                sql: "DELETE FROM user_auth WHERE id = @Id", param: new { user.Id });

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out Guid id))
                throw new ArgumentException($"Id was not a valid Guid: {userId}", nameof(userId));

            return await _dbConnection.QueryFirstOrDefaultAsync<User>(
                sql: "SELECT * FROM user_auth WHERE id = @Id", param: new { Id = userId });
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(
                sql: "SELECT * FROM user_auth WHERE normalized_user_name LIKE @NormalizedUserName", param: new { NormalizedUserName = $"%{normalizedUserName}%" });
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _dbConnection.ExecuteAsync(
                sql: "UPDATE user_auth SET user_name = @UserName, normalized_user_name = @NormalizedUserName, email = @Email, password_hash = @PasswordHash WHERE id = @Id",
                param: new { user.Id, user.UserName, user.NormalizedUserName, user.Email, user.PasswordHash });

            return IdentityResult.Success;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public void Dispose()
            => _dbConnection.Dispose();
    }
}