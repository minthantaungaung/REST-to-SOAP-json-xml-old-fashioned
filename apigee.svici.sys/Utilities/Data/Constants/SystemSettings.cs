namespace api.svici.sys.Utilities.Data.Constants;

public class SystemSettings
{
    public string? Issuer { get; set; }
    public string? AudienceId { get; set; }
    public string? Thumbprint { get; set; }
}
public class ConnectionString
{
    public string? CardWorksSoapService { get; set; }
    public string? CONFIG_URL { get; set; }
    public string? LOGAPI { get; set; }
    public string? VISTA_Oracle { get; set; }
    public string? VISTA_FE { get; set; }
}

public class LogSetting
{
    public string? SVBackend { get; set; }
    public string? TEXTLOG { get; set; }
    public string? APILOG { get; set; }
}

public class ConfigSetting
{
    public string? ServiceName { get; set; }
    public string? ConfigUrl { get; set; }
    public HttpClient? HttpClient { get; set; }
    public string? Scheme { get; set; }
    public string? Parameter { get; set; }
    public string? KbzRefNo { get; set; }
}