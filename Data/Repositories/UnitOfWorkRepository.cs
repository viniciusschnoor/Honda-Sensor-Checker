using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.UnitOfWork
{
    public class UnitOfWorkRepository : IUnitOfWork
    {
        private readonly DataContext _context;

        public IOperatorRepository Operators { get; }
        public IProductRepository Products { get; }
        public ISensorRepository Sensors { get; }
        public IZfBoxRepository ZfBoxes { get; }
        public ISupplierBoxRepository SupplierBoxes { get; }
        public ISapWorkOrderRepository SapWorkOrders { get; }
        public ILogRepository Logs { get; }

        public UnitOfWorkRepository(DataContext context)
        {
            _context = context;

            Operators = new OperatorRepository(_context);
            Products = new ProductRepository(_context);
            Sensors = new SensorRepository(_context);
            ZfBoxes = new ZfBoxRepository(_context);
            SupplierBoxes = new SupplierBoxRepository(_context);
            SapWorkOrders = new SapWorkOrderRepository(_context);
            Logs = new LogRepository(_context);
        }
        public bool Commit(out string error)
        {
            error = string.Empty;

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;

                if (msg.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase))
                    error = "FOREIGN_KEY_VIOLATION";
                else
                    error = msg;

                return false;
            }
            catch (Exception ex)
            {
                error = ex.InnerException?.Message ?? ex.Message;
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}