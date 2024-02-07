namespace api.svici.sys.Utilities.Data.Constants
{
    public class ResponseServiceConfig
    {
        public bool isCheck { get; set; }
        public dynamic Config { get; set; }
    }

    public class ResConfig
    {
        public ConfigModel? Data { get; set; }
        public string? KBZRefNo { get; set; }
    }
    public class ConfigModel
    {
        public string? serviceid { get; set; }
        public string? servicename { get; set; }
        public string? serviceDesc { get; set; }
        public string? serviceUrl { get; set; }
        public string? status { get; set; }
        public string? reqFormatValue { get; set; }
        public string? userid { get; set; }
        public string? value { get; set; }
    }
}
