using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Web.Api.Responses
{
    /// <summary>
    /// Response zur Abfrage der UserId
    /// </summary>
    public class UserIdResponse
    {
        /// <summary>
        /// Die Guid UserId als String
        /// </summary>
        public string UserId { get; set; }
    }
}
