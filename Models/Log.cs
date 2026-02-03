namespace HondaSensorChecker.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public DateTime Data { get; set; }
        public string Description { get; set; }

        public int OperatorId { get; set; }
        public Operator Operator { get; set; }
    }
}