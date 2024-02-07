using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace app.api.Utilities.Helpers;

public class JWTEvents : JwtBearerEvents
{
    public JWTEvents()
    {
        OnChallenge = async context =>
        {
            try
            {
                MainResponse mainResponse = context.Request.HttpContext.RequestServices.GetRequiredService<MainResponse>();
                HttpHeaderValue? httpHeaderValue = context.Request.HttpContext.RequestServices.GetRequiredService<HttpHeaderValue>();
                mainResponse.Error = ReturnMessage.Unauthorized;
                mainResponse.KBZRefNo = httpHeaderValue.KbzRefNo;

                ErrorResponseDetails errorResponseDetails = new ErrorResponseDetails();
                List<ErrorResponseDetails> lsDetails = new List<ErrorResponseDetails>();
                errorResponseDetails.ErrorCode = "Auth";
                errorResponseDetails.ErrorDescription = !string.IsNullOrEmpty(context.ErrorDescription) ? context.ErrorDescription : "Token is null or invalid";

                lsDetails.Add(errorResponseDetails);
                mainResponse.Error.Details = lsDetails;
                string strMainResponse = JsonConvert.SerializeObject(mainResponse);

                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = Application.Json;
                await context.Response.WriteAsync(strMainResponse);
            }
            catch (Exception ex)
            {
                MainResponse mainResponse = new MainResponse();
                context.Request.Headers.TryGetValue("LOGID", out var KbzRefNo);
                mainResponse.Error = ReturnMessage.Unauthorized;
                mainResponse.KBZRefNo = KbzRefNo;

                ErrorResponseDetails errorResponseDetails = new ErrorResponseDetails();
                List<ErrorResponseDetails> lsDetails = new List<ErrorResponseDetails>();
                errorResponseDetails.ErrorCode = "Auth";
                errorResponseDetails.ErrorDescription = !string.IsNullOrEmpty(context.ErrorDescription) ? context.ErrorDescription : "Token expired or null!";

                lsDetails.Add(errorResponseDetails);
                mainResponse.Error.Details = lsDetails;
                string strMainResponse = JsonConvert.SerializeObject(mainResponse);

                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = Application.Json;
                await context.Response.WriteAsync(strMainResponse);
            }
        };
    }
}
