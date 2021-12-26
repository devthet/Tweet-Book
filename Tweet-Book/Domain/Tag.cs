using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweet_Book.Domain
{   
    public class Tag
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid TagId { get; set; }
      
        [Key]
        public string TagName { get; set; }
        //public Guid TagId { get; set; }
        public Guid CreatorId { get; set; }
        
       // public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual List<PostTag> Tags { get; set; }

        //[ForeignKey(nameof(PostId))]
        //public Guid PostId { get; set; }
        
        //public Post Post { get; set; }
    }
}
