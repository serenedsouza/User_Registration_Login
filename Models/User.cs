namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Xml.Linq;

    public partial class User
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Enter Username")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Pass { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile")]
        [Phone(ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        public string Img { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
