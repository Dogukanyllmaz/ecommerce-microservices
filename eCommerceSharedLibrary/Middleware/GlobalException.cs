using eCommerceSharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerceSharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {

            //Decalare default variables
            string message = "Sorry, internal server error occurred. Try Again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //check if response is too many request //429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);

                }

                //if response is unAuthorized //401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {

                    title = "Alert";
                    message = "You are not  authorized to access";
                    await ModifyHeader(context, title, message, statusCode);

                }

                //if response is Forbidden //403 status code 
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {

                    title = "Out of Access";
                    message = "You are not allowed / required to access";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }

            }
            catch (Exception ex)
            {
                //Log Original Exceptions / file,debugger,Console

                LogException.LogExceptions(ex);

                //check if Exception is Timeout //408 status code
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout... Try Again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                //if exception is caught.
                //if none of the exceptions then do the default
                await ModifyHeader(context, title, message, statusCode);

            }


        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);

            return;


        }
    }
}
