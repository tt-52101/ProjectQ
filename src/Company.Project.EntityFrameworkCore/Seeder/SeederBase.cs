using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Company.Project.Authorization.Roles;
using Company.Project.Authorization.Users;
using Company.Project.Authorization.AppClaims;

namespace Company.Project.Seeder
{
    public abstract class SeederBase
    {
        protected readonly ILookupNormalizer _lookupNormalizer;

        protected readonly IPasswordHasher<User> _passwordHasher;

        protected readonly UserManager _userManager;

        protected readonly IClaimsManager _claimsManager;

        public SeederBase(IServiceProvider serviceProvider)
        {
            _lookupNormalizer = serviceProvider.GetService<ILookupNormalizer>();
            _passwordHasher = serviceProvider.GetService<IPasswordHasher<User>>();
            _userManager = serviceProvider.GetService<UserManager>();
            _claimsManager = serviceProvider.GetService<IClaimsManager>();
        }


        /// <summary>
        /// 创建系统角色
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="tenantName">租户名称</param>
        /// <returns></returns>
        protected virtual async Task<Role> CreateRoles(DbContext dbContext, string tenantName)
        {
            var roleStore = dbContext.Set<Role>();
            var roleClaimStore = dbContext.Set<RoleClaim>();


            var systemRole = await roleStore.IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Name == AppConsts.Authorization.SystemRoleName
                    && o.TenantName == tenantName);

            if (systemRole == null)
            {
                systemRole = new Role()
                {
                    Name = AppConsts.Authorization.SystemRoleName,
                    DisplayName = AppConsts.Authorization.SystemRoleName,
                    Description = AppConsts.Authorization.SystemRoleName,
                    NormalizedName = this._lookupNormalizer.NormalizeName(AppConsts.Authorization.SystemRoleName),
                    IsStatic = true,
                    TenantName = tenantName
                };
                await roleStore.AddAsync(systemRole);
                await dbContext.SaveChangesAsync();
            }

            // 查询现有权限
            var roleClaims = await roleClaimStore.IgnoreQueryFilters()
                .Where(o => o.RoleId == systemRole.Id && o.TenantName == tenantName)
                .ToListAsync();

            // 移除权限
            roleClaimStore.RemoveRange(roleClaims);


            // 添加权限
            roleClaims.Clear();
            var claimItems = _claimsManager.GetAll(
                string.IsNullOrWhiteSpace(tenantName) ? ClaimItemType.Host : ClaimItemType.Tenant
                );
            foreach (var item in claimItems)
            {
                roleClaims.Add(new RoleClaim()
                {
                    RoleId = systemRole.Id,
                    TenantName = tenantName,
                    ClaimType = item.Claim,
                    ClaimValue = item.Claim
                });
            }
            await roleClaimStore.AddRangeAsync(roleClaims);

            await dbContext.SaveChangesAsync();
            return systemRole;
        }

        /// <summary>
        /// 创建系统用户
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="systemRole">系统默认角色</param>
        /// <returns></returns>
        protected virtual async Task<User> CreateUsers(DbContext dbContext, Role systemRole)
        {
            var userStore = dbContext.Set<User>();
            var userRoleStore = dbContext.Set<UserRole>();

            var systemUser = await userStore.IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.UserName == AppConsts.Authorization.SystemUserName
                    && o.TenantName == systemRole.TenantName);
            if (systemUser == null)
            {
                systemUser = new User()
                {
                    UserName = AppConsts.Authorization.SystemUserName,
                    Nickname = AppConsts.Authorization.SystemUserName,
                    PhoneNumber = "13000000007",
                    PhoneNumberConfirmed = true,
                    Email = "msmadaoe@msn.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsStatic = true,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    TenantName = systemRole.TenantName,
                };

                systemUser.NormalizedUserName = this._lookupNormalizer.NormalizeName(systemUser.UserName);
                systemUser.NormalizedEmail = this._lookupNormalizer.NormalizeName(systemUser.Email);

                systemUser.PasswordHash = this._passwordHasher
                    .HashPassword(systemUser, AppConsts.Authorization.SystemUserPassword);

                await userStore.AddAsync(systemUser);
                await dbContext.SaveChangesAsync();

                // 更新密钥
                await _userManager.UpdateSecurityStampAsync(systemUser);
            }


            // 用户添加角色
            var userRole = await userRoleStore.IgnoreQueryFilters()
                            .Where(o => o.UserId == systemUser.Id
                                    && o.RoleId == systemRole.Id
                                    && o.TenantName == systemRole.TenantName)
                            .FirstOrDefaultAsync();
            if (userRole == null)
            {
                await userRoleStore.AddAsync(new UserRole()
                {
                    RoleId = systemRole.Id,
                    UserId = systemUser.Id,
                    TenantName = systemRole.TenantName
                });
            }

            await dbContext.SaveChangesAsync();
            return systemUser;
        }
    }
}
