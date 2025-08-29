using DataTypeMapping.Utilities;

namespace AuthServer.PipeLineExt
{
    public class CustomAuthResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized || 
                context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                context.Response.Headers.ContentLength = null;
                context.Response.Body.SetLength(0);

                var BaseResponse =context.Response.StatusCode == StatusCodes.Status401Unauthorized ?
                    BaseResponse<string>.Failure(new List<string> { "You are not authorized to access this resource." })
                    : BaseResponse<string>.Failure(new List<string> { "You do not have permission to access this resource." });
                await context.Response.WriteAsJsonAsync(BaseResponse);
            }
        }
    }
}
