using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email or Mobile")]
        public string EmailOrMobile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Pass { get; set; }
    }
}