﻿namespace AwesomeCompany.Options
{
    public class DataBaseOptions
    {
        public string ConnectionString {  get; set; } =string.Empty;
        public int  MaxRetryCount { get; set; }
        public int CommandTimeOut { get; set; }
        public bool EnableDetailedErrors {  get; set; }
        public bool EnableSensitiveDataLogging {  get; set; }

    }
}
