using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TransferMS.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseOracle("User Id=backmore_transfer;Password=SenhaForte123;Data Source=localhost:1521/XEPDB1;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
