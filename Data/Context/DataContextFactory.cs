using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HondaSensorChecker.Data.Context
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // Design-time DB só para gerar migrations (evita WinForms/Host/Program)
            optionsBuilder.UseSqlite("Data Source=HondaSensorChecker.design.db");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
