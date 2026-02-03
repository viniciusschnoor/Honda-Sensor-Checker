using System.Collections.Generic;

namespace HondaSensorChecker.Models
{
    public class SapWorkOrder
    {
        public int SapWorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public List<ZfBox> ZfBoxes { get; } = new();
        public List<Sensor> Sensors { get; } = new();
    }
}
