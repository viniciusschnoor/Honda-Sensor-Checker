using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class LogRepository : RepositoryBase<Log>, ILogRepository
    {
        private readonly DbSet<Log> _dbSetLog;
        public LogRepository(DataContext context) : base(context)
        {
            _dbSetLog = context.Set<Log>();
        }
    }
}