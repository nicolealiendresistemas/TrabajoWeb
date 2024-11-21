using Microsoft.EntityFrameworkCore;
using TrabajoClasesVirtuales.Models;
using TrabajoClasesVirtuales.Models.Proyecto.Models;

namespace TrabajoClasesVirtuales.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
    }

}
