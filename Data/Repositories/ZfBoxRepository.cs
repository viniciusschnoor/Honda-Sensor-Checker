using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    internal class ZfBoxRepository : RepositoryBase<ZfBox>, IZfBoxRepository
    {
        private readonly DbSet<ZfBox> _dbSetZfBox;
        public ZfBoxRepository(DataContext context) : base(context)
        {
            _dbSetZfBox = context.Set<ZfBox>();
        }
    }
}