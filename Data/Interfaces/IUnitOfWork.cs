using HondaSensorChecker.Data.Interfaces;

namespace HondaSensorChecker.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IOperatorRepository Operators { get; }
        IProductRepository Products { get; }
        ISensorRepository Sensors { get; }
        IZfBoxRepository ZfBoxes { get; }
        ISupplierBoxRepository SupplierBoxes { get; }
        ISapWorkOrderRepository SapWorkOrders { get; }
        ILogRepository Logs { get; }

        bool Commit(out string error);
    }
}
