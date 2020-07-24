using System;
using System.Collections.Generic;
using System.Text;

using Riven.Dependency;
using Riven.Localization;

namespace Company.Project.Authorization
{
    public interface IAppSession : ITransientDependency
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// 用户Id - 字符串类型
        /// </summary>
        string UserIdString { get; }

        /// <summary>
        /// 用户账号
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 租户Id - 字符串类型
        /// </summary>
        string TenantName { get; }

        /// <summary>
        /// 模拟登录用户Id
        /// </summary>
        long? ImpersonatedUserId { get; }

        /// <summary>
        /// 模拟登录用户Id - 字符串
        /// </summary>
        string ImpersonatedUserIdString { get; }

        /// <summary>
        /// 模拟登录租户名称 - 字符串
        /// </summary>
        string ImpersonatedTenantName { get; }

        /// <summary>
        /// 当前语言信息
        /// </summary>
        LanguageInfo CurrentLanguage { get; }
    }
}
