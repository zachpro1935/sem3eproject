using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eproject.Models
{
    public class Ratting
    {
        public Ratting()
        {
            //this.role = "ROLE_USER";
            // this.enabled = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
              
        public int rate { get; set; }

        [ForeignKey("user")]
        public Guid own { get; set; }
        public virtual User user { get; set; }

        [ForeignKey("Recipe")]
        public Guid recipe_id { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
