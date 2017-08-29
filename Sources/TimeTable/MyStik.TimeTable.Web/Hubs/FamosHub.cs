using Microsoft.AspNet.SignalR;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class FamosHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}