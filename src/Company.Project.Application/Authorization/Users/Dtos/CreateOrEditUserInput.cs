﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Project.Authorization.Users.Dtos
{
    public class CreateOrEditUserInput
    {
        public UserEditDto EntityDto { get; set; }

        public string Password { get; set; }
    }
}
