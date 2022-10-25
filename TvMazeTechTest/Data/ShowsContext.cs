using Microsoft.EntityFrameworkCore;
using TvMazeTechTest.Models;

namespace TvMazeTechTest.Data
{
    public class ShowsContext : DbContext
    {
        public ShowsContext(DbContextOptions<ShowsContext> options) : base(options)
        {
        }        

        public DbSet<Shows> Shows {get; set;}
        public DbSet<Settings> Settings { get; set; }

    }
}
