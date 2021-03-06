﻿using Riven.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Project.Authorization.Roles.Dtos
{
    public class RoleDto : EntityDto<Guid?>
    {

        /// <summary>
        /// 名称(编码)-不可被修改
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public virtual bool IsStatic { get; set; }
    }
}
