using Microsoft.EntityFrameworkCore;
using MyBlog.Data.Interfaces;
using MyBlog.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data
{
    public class MyBlogApiServerSide : IMyBlogAPI
    {

        IDbContextFactory<MyBlogDbContext> factory;

        public MyBlogApiServerSide(IDbContextFactory<MyBlogDbContext> factory)
        {
            this.factory = factory;
        }

        public async Task DeleteItem(IMyBlogItem item)
        {
            using var context = factory.CreateDbContext();
            context.Remove(item);
            await context.SaveChangesAsync();
        }


        private async Task<IMyBlogItem> SaveItem(IMyBlogItem item)
        {
            using var context = factory.CreateDbContext();
            if (item.Id == 0)
            {
                context.Add(item);
            }
            else
            {
                if (item is BlogPost)
                {
                    var post = item as BlogPost;
                    var currentpost = await
                        context.BlogPosts.Include
                        (p => p.Category).Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);
                    currentpost.PublishDate = post.PublishDate;
                    currentpost.Title = post.Title;
                    currentpost.Text = post.Text;
                    var ids = post.Tags.Select(t => t.Id);
                    currentpost.Tags = context.Tags.Where(t => ids.Contains(t.Id)).ToList();
                    currentpost.Category = await context.Categories.FirstOrDefaultAsync(c => c.Id == post.Category.Id);
                    await context.SaveChangesAsync();
                }
                else 
                {
                    context.Entry(item).State = EntityState.Modified;
                }
            }
            await context.SaveChangesAsync();
            return item;
        }


        public async Task DeleteBlogPostAsync(BlogPost item)
        {
            await DeleteItem(item);
        }

        public async Task DeleteCategoryAsync(Category item)
        {
            await DeleteItem(item);
        }

        public async Task DeleteTagAsync(Tag item)
        {
            await DeleteItem(item);
        }

        public async Task<BlogPost> GetBlogPostAsync(int id)
        {
            using var context = factory.CreateDbContext();
            return await context.BlogPosts.Include(p => p.Category).Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<BlogPost>> GetBlogPostsAsync(int numberofPosts, int startindex)
        {
            using var context = factory.CreateDbContext();
            return await context.BlogPosts.OrderByDescending(p => p.PublishDate).Skip(startindex).Take(numberofPosts).ToListAsync();
        }

        public async Task<int> GetBLogPostsCountAsync()
        {
            using var context = factory.CreateDbContext();
            return await context.BlogPosts.CountAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using var context = factory.CreateDbContext();
            return await context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            using var context = factory.CreateDbContext();
            return await context.Categories.Include(p => p.BlogPosts).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Tag> GetTagAsync(int id)
        {
            using var context = factory.CreateDbContext();
            return await context.Tags.Include(t => t.BlogPosts).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            using var context = factory.CreateDbContext();
            return await context.Tags.ToListAsync();
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost item)
        {
            return (await SaveItem(item) as BlogPost);
        }

        public async Task<Category> SaveCategoryAsync(Category item)
        {
            return (await SaveItem(item) as Category);
        }

        public async Task<Tag> SaveTagAsync(Tag item)
        {
            return (await SaveItem(item) as Tag);
        }
    }
}
