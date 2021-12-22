using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Domain;

namespace Tweet_Book.Services
{
    public interface ITagSerivce
    {
        List<Tag> GetTags();
        Tag GetTagById(string tagName);
        Tag CreateTag(Tag tag);
        bool DeleteTag(string tagName);
    }
}
