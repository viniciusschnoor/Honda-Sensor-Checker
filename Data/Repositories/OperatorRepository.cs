using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class OperatorRepository : RepositoryBase<Operator>, IOperatorRepository
    {
        private readonly DbSet<Operator> _dbSetOperator;
        public OperatorRepository(DataContext context) : base(context)
        {
            _dbSetOperator = context.Set<Operator>();
        }
    }
}