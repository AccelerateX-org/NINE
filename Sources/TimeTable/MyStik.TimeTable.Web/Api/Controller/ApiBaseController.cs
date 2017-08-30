using System.Web.Http;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiBaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

    }
}
