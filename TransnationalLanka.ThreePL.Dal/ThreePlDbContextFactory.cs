using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TransnationalLanka.ThreePL.Dal
{
    public class ThreePlDbContextFactory : IDesignTimeDbContextFactory<ThreePlDbContext>
    {
        public ThreePlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ThreePlDbContext>();
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-UHEQ6FB;Initial Catalog=threepl-db-local;Integrated Security=True;");
            return new ThreePlDbContext(optionsBuilder.Options);
        }
    }
}
