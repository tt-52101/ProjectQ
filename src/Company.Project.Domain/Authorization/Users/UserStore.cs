﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Riven.Repositories;
using Riven;
using Microsoft.EntityFrameworkCore;
using Riven.Uow;

namespace Company.Project.Authorization.Users
{
    public class UserStore : IUserStore<User>
    {
        private bool _disposed;

        protected readonly IUnitOfWorkManager _unitOfWorkManager;

        protected readonly IRepository<User> _userRepo;

        protected IdentityErrorDescriber ErrorDescriber { get; }

        public UserStore(IUnitOfWorkManager unitOfWorkManager, IRepository<User> userRepo)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userRepo = userRepo;

            ErrorDescriber = new IdentityErrorDescriber();
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            Check.NotNull(user, nameof(user));

            await _userRepo.InsertAsync(user);
            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            await _userRepo.DeleteAsync(user.Id);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }



        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Check.NotNullOrWhiteSpace(userId, nameof(userId));

            if (!long.TryParse(userId, out long userIdWithLong))
            {
                throw new ArgumentException("");
            }

            return await this._userRepo.GetAll()
                .Where(o => o.Id == userIdWithLong)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _userRepo.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            return Task.FromResult(ConvertIdToString(user.Id));
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            await this._userRepo.UpdateAsync(user);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            _disposed = true;
        }

        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual async Task SaveChanges(CancellationToken cancellationToken)
        {
            await this._unitOfWorkManager.Current.SaveChangesAsync(cancellationToken);
        }

        protected virtual string ConvertIdToString(long id)
        {
            if (object.Equals(id, default(long)))
            {
                return null;
            }
            return id.ToString();
        }
    }
}
