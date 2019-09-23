using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eproject.Models
{
    public class Contest
    {
        public Contest()
        {
            //this.role = "ROLE_USER";
            // this.enabled = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [StringLength(30, ErrorMessage = "The {0} must be at least 3 characters long.", MinimumLength = 3)]
        [Required(ErrorMessage = "Contest Name is Required")]
        [DisplayName("Contest Name")]
        [RegularExpression(@"^[a-zA-Z\s\d]+$", ErrorMessage = "Contest name must contain only letters and numbers")]
        public string name { get; set; }

        [Required(ErrorMessage = "Content is Required")]
        [StringLength(3000, ErrorMessage = "The {0} must be at least 3 characters long.", MinimumLength = 10)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Content")]
        [AllowHtml]
        public string content { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Create Date")]
        public DateTime createAt { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start Date is Required")]
        [DisplayName("Start Date")]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "End Date is Required")]
        [DisplayName("End Date")]
        public DateTime endDate { get; set; }

        public Guid? winner { get; set; }

        public virtual ICollection<Recipe> recipes { get; set; }
    }
}