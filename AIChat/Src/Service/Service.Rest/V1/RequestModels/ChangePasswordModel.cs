﻿using System.ComponentModel.DataAnnotations;

namespace Service.Rest.V1.RequestModels
{
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
