using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class SupplierBoxRepository : RepositoryBase<SupplierBox>, ISupplierBoxRepository
    {
        private readonly DbSet<SupplierBox> _dbSetSupplierBox;
        public SupplierBoxRepository(DataContext context) : base(context)
        {
            _dbSetSupplierBox = context.Set<SupplierBox>();
        }
    }
}