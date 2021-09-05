
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyBlog.Data.Models;

namespace MyBlog.Data
{
    public class MyBlogContext : DbContext
    {

        public MyBlogContext(DbContextOptions<MyBlogContext> context) : base(context)
        { 
        
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public class MyBlogDbContextFactory : IDesignTimeDbContextFactory<MyBlogContext>
        {
            public MyBlogContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<MyBlogContext>();
                optionsBuilder.UseSqlite("Data Source = test.db");

                return new MyBlogContext(optionsBuilder.Options);
            }
        }
    }
}
