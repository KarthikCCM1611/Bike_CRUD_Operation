using BikeWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeWebsite.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
        {

        }

        public DbSet<Brand> Brands { get; set; }



    }
}
