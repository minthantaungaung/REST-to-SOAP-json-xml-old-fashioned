namespace app.api.Utilities.Helpers;

public class HttpHeaderValue
{
    public readonly string? KbzRefNo;
    public readonly string? Authorization;
    public HttpHeaderValue(IHttpContextAccessor httpContextAccessor)
    {
        this.KbzRefNo = httpContextAccessor?.HttpContext?.Request?.Headers["KBZ_REF_NO"];
        if (string.IsNullOrEmpty(KbzRefNo))
        {
            this.KbzRefNo = httpContextAccessor?.HttpContext?.Request?.Headers["LOGID"];
            if (string.IsNullOrEmpty(KbzRefNo))
            {
                this.KbzRefNo = Guid.NewGuid().ToString();
            }
        }
        this.Authorization = httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"];
    }
}
