using System;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationContract
    {
        /// <summary>
        /// Guid des NotificationStates als string
        /// </summary>
        public string NotificationStateId { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Notification Text
        /// </summary>
        public string NotificationContent { get; set; }
        /// <summary>
        /// Schon gelesen?
        /// </summary>
        public bool isNew { get; set; }
        /// <summary>
        /// Id des ActivityDateChanges
        /// </summary>
        public string ChangeId { get; set; }
        /// <summary>
        /// Erstellungsdatum des ActivityDateChanges
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}