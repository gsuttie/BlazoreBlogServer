using MyBlog.Data.Interfaces;
using System.Collections.Generic;

namespace MyBlog.Data.Models
{
    public class Tag : IMyBlogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
