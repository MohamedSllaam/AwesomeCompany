namespace AwesomeCompany.Options
{
    public class DataBaseOptions
    {
        public string ConnectionString {  get; set; } =string.Empty;

        public int DataMaxRetryCointbaseName { get; set; }
        public int CommandTimeOut { get; set; }
        public int EnableDetailsErrors {  get; set; }
        public int EnableSensitiveLogging {  get; set; }

    }
}
