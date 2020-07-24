﻿using Company.Project.Authorization.Users;

using Riven.Dependency;
using Riven.Uow;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Company.Project.Authorization.Roles;
using System.Linq;
using Company.Project.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Company.Project.Seeder
{
    public interface ITenantSeeder : IScopeDependency
    {
        Task Create(DbContext dbContext);
    }

    public class TenantSeeder : SeederBase, ITenantSeeder
    {
        public TenantSeeder(ILookupNormalizer lookupNormalizer, IPasswordHasher<User> passwordHasher)
            : base(lookupNormalizer, passwordHasher)
        {
        }

        public virtual async Task Create(DbContext dbContext)
        {
            if (!(dbContext is AppDbContext))
            {
                throw new Exception("The database context type is incorrect!");
            }


            var defaultRole = await this.CreateRoles(dbContext, AppConsts.MultiTenancy.DefaultTenantName);

            var defaultUser = await this.CreateTenantUsers(dbContext, defaultRole);


            await dbContext.SaveChangesAsync();
        }

    }
}
