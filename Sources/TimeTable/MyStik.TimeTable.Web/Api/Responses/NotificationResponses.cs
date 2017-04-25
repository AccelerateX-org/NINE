using MyStik.TimeTable.Web.Api.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Responses
{

    /// <summary>
    /// Response zur Abfrage der persönlichen Notifications
    /// </summary>
    public class PersonalNotificationResponse
    {
        /// <summary>
        /// Liste der einzelnen Notifications
        /// </summary>
        public IEnumerable<NotificationContract> Notifications { get; set; }
    }

    public class TokenRegistryResponse
    {   
        // Bool, ob der Token erfolgreich in der DB abgelegt wurde
        public bool tokenSaved { get; set; }
    }
}