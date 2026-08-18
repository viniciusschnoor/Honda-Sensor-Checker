namespace HondaSensorChecker.Configuration
{
    public sealed class AccSettings
    {
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string DllVersion { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
    }
}
