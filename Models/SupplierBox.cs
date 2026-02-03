using System.Collections.Generic;

namespace HondaSensorChecker.Models
{
    public class SupplierBox
    {
        public int SupplierBoxId { get; set; }
        public string UniqueNumber { get; set; }
        public int QtySupplied { get; set; }
        public int QtyRemaining { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public List<Sensor> Sensors { get; } = new();
    }
}
