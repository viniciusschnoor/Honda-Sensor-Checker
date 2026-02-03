using HondaSensorChecker.Models;

namespace HondaSensorChecker
{
    public interface IFinishedBoxFactory
    {
        FinishedBox Create(
            SapWorkOrder workOrder,
            Product product,
            ZfBox zfBox,
            int operatorId);
    }
}
