using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweet_Book.Domain
{

    public class PostTag
    {


        //public int Id { get; set; }
        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }
        public string TagName { get; set; }


        public virtual Post Post { get; set; }
        public Guid PostId { get; set; }
    }
}

