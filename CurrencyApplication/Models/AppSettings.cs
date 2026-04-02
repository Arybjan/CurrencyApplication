using System;

namespace CurrencyApplication.Models
{
    public class AppSettings
    {
        public DateTime LastSessionTime { get; set; }
        public string StorageType { get; set; } = "JSON";
    }
}