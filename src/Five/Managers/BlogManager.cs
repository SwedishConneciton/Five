using Five.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Five.Managers
{
    public interface IBlogManager
    {
        Task<IEnumerable<Blog>> Every();

        Task<bool> AddAsync();
    }

    public class BlogManager : IBlogManager
    {
        private readonly BloggingContext ctx;

        public BlogManager (BloggingContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<bool> AddAsync()
        {
            ctx.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            var count = await ctx.SaveChangesAsync();

            return count == 1;
        }

        public async Task<IEnumerable<Blog>> Every()
        {
            return await ctx.Blogs.ToListAsync();
        }
    }
}
