using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MyStik.TimeTable.Web.Hubs
{
    public class FamosHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}