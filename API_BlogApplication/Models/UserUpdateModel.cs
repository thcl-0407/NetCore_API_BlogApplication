﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_BlogApplication.Models
{
    public class UserUpdateModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
