using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Models;

namespace HondaSensorChecker
{
    public class FinishedBoxFactory : IFinishedBoxFactory
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinishedBoxFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public FinishedBox Create(
            SapWorkOrder workOrder,
            SupplierBox supplierBox,
            Product product,
            int qty,
            List<Sensor> sensors,
            int operatorId)
        {
            // Important:
            // do NOT pass entities from another DbContext as-is for updates.
            // FinishedBox will reload entities by ID using this same UnitOfWork context.
            return new FinishedBox(
                _unitOfWork,
                operatorId,
                workOrder,
                supplierBox,
                product,
                qty,
                sensors);
        }
    }
}
