

namespace Touchsides_Assignment.Middleware
{
    public class GlobalExceptionHandler
    {

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            string result;
            if (exception is ArgumentNullException)
            {
                result = "No file was uploaded. Please select a file to upload.";
            }
            else
            {
                result = "An unexpected error occurred. Please try again later.";
            }

            return context.Response.WriteAsync(new { error = result }.ToString());
        }
    }
}
