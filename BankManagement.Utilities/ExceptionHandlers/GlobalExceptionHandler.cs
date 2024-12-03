using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankManagement.Utilities.ExceptionHandlers;


public class GlobalExceptionHandler : Attribute, IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        Exception exception = context.Exception;

        if (exception is InvalidOperationException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid operation performed!" });
        }
        else if (exception is NullReferenceException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Null reference is found! Please check it." });
        }
        else if (exception is ArgumentException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid argument is found!" });
        }
        else if (exception is KeyNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new { error = "The provided key is not found." });
        }
        else if (exception is FormatException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid format is found!" });
        }
        else if (exception is ArgumentNullException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Null reference for argument is found! Please check it once." });
        }
        // else
        // {
        //     context.Result = new StatusCodeResult(500);
        // }
        // context.ExceptionHandled = true;
        await Task.CompletedTask;
    }
}

