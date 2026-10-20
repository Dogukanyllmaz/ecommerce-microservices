using Microsoft.AspNetCore.Http;

namespace eCommerceSharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            //Extract spesific header from the request

            var signedHeader = context.Request.Headers["Api-Gateway"];

            //null means, the request is not coming from the api gateway //503 service is unavailable
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }

        }

    }
}
