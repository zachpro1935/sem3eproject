using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eproject.Models
{
    public class User
    {
        public User()
        {
            this.role = "ROLE_USER";
            this.enabled = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "user name not blank!")]
        public string name { get; set; }       
        [Required(ErrorMessage = "not blank!")]
        public string pass { get; set; }
        [Required(ErrorMessage = "not blank!")]
        [MaxLength(30)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Index(IsUnique = true)]
        public string email { get; set; }
        [Required(ErrorMessage = "not blank!")]
        public string gender { get; set; }
        [Required(ErrorMessage = "You must provide a phone number")]
        //[Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10,12})$", ErrorMessage = "Not a valid phone number")]
        public string phone { get; set; }
        public string role { get; set; }
      
        [DataType(DataType.DateTime)]
        public DateTime expireDate { get; set; }
        public bool enabled { get; set; }
        public string emailVerify { get; set; }

        public virtual ICollection<Recipe> recipes { get; set; }

        public virtual ICollection<FeedBack> feedback { get; set; }
    }
}