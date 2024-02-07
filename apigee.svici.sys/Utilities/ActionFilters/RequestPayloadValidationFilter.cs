using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;

namespace api.svici.sys.Utilities.ActionFilters
{
    public class RequestPayloadValidationFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Request.EnableBuffering();
            string KBZRefNo = context.HttpContext.Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : context.HttpContext.Request.Headers["LOGID"];

            if (context.Result is BadRequestObjectResult badRequestObjectResult)
                if (badRequestObjectResult.Value is ValidationProblemDetails)
                {
                    BaseRespModel returndata = new()
                    {
                        KBZRefNo = KBZRefNo,
                        Error = ErrorCodeModel.InvalidRequestPayload
                    };
                    context.Result = new BadRequestObjectResult(returndata);
                }
            context.HttpContext.Request.Body.Position = 0;
            base.OnResultExecuting(context);
        }
    }
}
