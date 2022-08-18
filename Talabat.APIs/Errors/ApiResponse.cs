using System;

namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultStatusCodeMessage(statusCode);
        }

        private string GetDefaultStatusCodeMessage(int statusCode)
          => statusCode switch
          {
              400 => "A Bad Request, You Have Made",
              401 => "Authorzied, You Are Not",
              404 => "Resourse Was Not Found",
              500 => "Errors are the path to the dark side ....",
              _ => null
          };
    }
}
