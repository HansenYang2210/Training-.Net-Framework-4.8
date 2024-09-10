using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using FluentValidation;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(HttpActionExecutedContext context)
    {
        // Log the exception (You can use your logging mechanism here)
        LogException(context.Exception);

        if (context.Exception is ValidationException validationException)
        {
            // Handle FluentValidation.ValidationException
            var errors = validationException.Errors;
            var errorMessages = errors.Select(e => e.ErrorMessage).ToList();
            var errorMessageString = string.Join(" & ", errorMessages);

            context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                new
                {
                    Title = "Validation failed",
                    Message = errorMessageString
                });
        }
        else if (context.Exception is HttpException httpException && httpException.GetHttpCode() == 500)
        {
            context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                new { Title = "Internal Server Error", 
                    Message = "Bad Request. An unexpected error occurred." });
        }
        else
        {
            // Handle other exceptions or keep the original behavior
            context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                new
                {
                    Title = "Information",
                    Message = context.Exception.Message
                });
        }
    }

    private void LogException(Exception exception)
    {
        // Implement your logging logic here. For example:
        // Logger.LogError(exception);
        Console.WriteLine(exception.ToString()); // Example logging to console
    }
}
