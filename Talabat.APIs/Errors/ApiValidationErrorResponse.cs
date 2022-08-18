using System.Collections;
using System.Collections.Generic;

namespace Talabat.APIs.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; } = new List<string>();
        public ApiValidationErrorResponse():base(400)
        {

        }
    }
}
