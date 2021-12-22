using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Domain;

namespace Tweet_Book.Services
{
    public class TagService : ITagSerivce
    {
        private readonly List<Tag> tags;
        public TagService()
        {
            tags = new List<Tag>();
            tags.Add(new Tag { CreatorId = Guid.NewGuid(), Name = "Tag1",CreatedOn=DateTime.Now});
            tags.Add(new Tag { CreatorId = Guid.NewGuid(), Name = "Tag2", CreatedOn = DateTime.Now });
        }

        public List<Tag> GetTags()
        {
            return tags;
        }

        public Tag GetTagById(string tagName)
        {
            var tag = tags.SingleOrDefault(x => x.Name == tagName);
            return tag;
        }
        public Tag CreateTag(Tag tag)
        {
            tags.Add(tag);
            return tag;
        }
        public bool DeleteTag(String tagName)
        {
            var post = GetTagById(tagName);
            if (post == null) return false;
            tags.Remove(post);
            return true;

        }
    }
}
