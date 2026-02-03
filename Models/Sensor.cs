using System;
using System.Collections.Generic;
using System.Text;

namespace HondaSensorChecker.Models
{
    public class Sensor
    {
        public int SensorId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime ScannedTime { get; set; }
        public bool InProgress { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OperatorId { get; set; }
        public Operator Operator { get; set; }
        public int SupplierBoxId { get; set; }
        public SupplierBox SupplierBox { get; set; }
        public int SapWorkOrderId { get; set; }
        public SapWorkOrder SapWorkOrder { get; set; }
        public int ZfBoxId { get; set; }
        public ZfBox ZfBox { get; set; }
    }
}
