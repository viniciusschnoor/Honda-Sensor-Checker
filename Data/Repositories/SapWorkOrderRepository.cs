using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class SapWorkOrderRepository : RepositoryBase<SapWorkOrder>, ISapWorkOrderRepository
    {
        private readonly DbSet<SapWorkOrder> _dbSetSapWorkOrder;
        public SapWorkOrderRepository(DataContext context) : base(context)
        {
            _dbSetSapWorkOrder = context.Set<SapWorkOrder>();
        }
    }
}