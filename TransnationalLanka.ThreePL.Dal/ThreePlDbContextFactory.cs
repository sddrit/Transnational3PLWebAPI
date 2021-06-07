using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TransnationalLanka.ThreePL.Dal
{
    public class ThreePlDbContextFactory : IDesignTimeDbContextFactory<ThreePlDbContext>
    {
        public ThreePlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ThreePlDbContext>();
            optionsBuilder.UseSqlServer("Data Source=192.168.1.103;Initial Catalog=threepl;Integrated Security=True;User ID=sa;Password=Tirddds@123");
            return new ThreePlDbContext(optionsBuilder.Options);
        }
    }
}
