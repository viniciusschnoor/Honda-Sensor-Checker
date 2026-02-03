using HondaSensorChecker.Models;

namespace HondaSensorChecker
{
    public interface IFinishedBoxFactory
    {
        FinishedBox Create(
            SapWorkOrder workOrder,
            SupplierBox supplierBox,
            Product product,
            int qty,
            List<Sensor> sensors,
            int operatorId);
    }
}