using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eproject.Models
{
    public class FeedBack
    {
        public FeedBack()
        {
            //this.role = "ROLE_USER";
           // this.enabled = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [MaxLength(150)]
        [Required(ErrorMessage = "not blank!")]
        public string content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime createAt { get; set; }
        [ForeignKey("user")]
        public Guid own { get; set; }
        public virtual User user { get; set; }

        [ForeignKey("Recipe")]
        public Guid recipe_id { get; set; }
        public virtual Recipe Recipe { get; set; }
        [NotMapped]
        public string ago { get; set; }
        [NotMapped]
        public string name { get; set; }
    }
}