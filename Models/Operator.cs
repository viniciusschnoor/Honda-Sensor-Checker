using System.Collections.Generic;

namespace HondaSensorChecker.Models
{
    public class Operator
    {
        public int OperatorId { get; set; }
        public string Re { get; set; }
        public string ZfId { get; set; }
        public string Name { get; set; }
        public bool Admin { get; set; }

        public List<Sensor> Sensors { get; } = new();
        public List<ZfBox> ZfBoxes { get; } = new();
        public List<Log> Logs { get; } = new();
    }
}
