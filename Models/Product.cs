using System.Collections.Generic;

namespace HondaSensorChecker.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Prefix { get; set; }
        public string StartPartNumber { get; set; }
        public string EndPartNumber { get; set; }

        public List<Sensor> Sensors { get; } = new();
        public List<SupplierBox> SupplierBoxes { get; } = new();
        public List<SapWorkOrder> SapWorkOrders { get; } = new();
        public List<ZfBox> ZfBoxes { get; } = new();
    }
}