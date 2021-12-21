using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweet_Book.Domain
{   
    public class Tags
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Tag { get; set; }
        public string TagName { get; set; }

        public string PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
