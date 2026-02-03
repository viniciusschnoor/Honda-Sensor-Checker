using System;
using System.Collections.Generic;
using System.Text;

namespace HondaSensorChecker.Models
{
    public class ZfBox
    {
        public int ZfBoxId { get; set; }
        public int QtyToSend { get; set; }
        public string UniqueNumber { get; set; }
        public string? Batch { get; set; }
        public bool InProgress { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SapWorkOrderId { get; set; }
        public SapWorkOrder SapWorkOrder { get; set; }
        public int OperatorId { get; set; }
        public Operator Operator { get; set; }
        public List<Sensor> Sensors { get; } = new();
    }
}
