using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MyStik.TimeTable.Web.Hubs
{
    public class ImportHub : Hub
    {
        public string msg = "Initializing and Preparing...";
        public int count = 100;
 
        public void Hello()
        {
            for (int x = 0; x <= count; x++)
            {

                // delay the process to see things clearly
                Thread.Sleep(100);

                if (x == 20)
                    msg = "Loading Application Settings...";

                else if (x == 40)
                    msg = "Applying Application Settings...";

                else if (x == 60)
                    msg = "Loading User Settings...";

                else if (x == 80)
                    msg = "Applying User Settings...";

                else if (x == 100)
                    msg = "Process Completed!...";

                // call client-side SendMethod method
                Clients.Caller.hello(string.Format
                        (msg + " {0}% of {1}%", x, count), x);
            } 

        }
    }
}