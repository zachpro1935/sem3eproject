using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eproject.Models
{
    public class Recipe
    {
        public Recipe()
        {
            //this.role = "ROLE_USER";
            this.enabled = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "name not blank!")]
        public string name { get; set; }
        [Required(ErrorMessage = "not blank!")]
        public string category { get; set; }
        [Required(ErrorMessage = "not blank!")]
        public string content { get; set; }
        [Required(ErrorMessage = "not blank")]
        public string type { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createAt { get; set; }
        public bool enabled { get; set; }

        public string image { get; set; }

        [ForeignKey("User")]
        public Guid manager { get; set; }
        public virtual User User { get; set; }
        [NotMapped]
        public HttpPostedFileBase imgFile { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }

        [ForeignKey("Contest")]
        public Guid? contest_id { get; set; }
        public virtual Contest Contest { get; set; }

        [NotMapped]
        public double rate { get; set; }

        public int viewCount { get; set; } = 0;
    }
}