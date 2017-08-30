using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();
        private ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CreateAllNotifications()
        {
            // Liste aller Changes die Veranstaltungen betreffen die innerhalb einer Woche in der Zukunft liegen
            DateTime untilDate = DateTime.Now.AddDays(7);
            
            List<ActivityDateChange> changeList = Db.DateChanges.Where(d => d.IsNotificationGenerated == false 
                && DateTime.Compare(DateTime.Now, d.NewEnd) < 0
                && DateTime.Compare(d.NewEnd, untilDate) < 0).ToList();

            foreach (ActivityDateChange a in changeList)
            {
                GenerateNotificationStates(a.Id.ToString());

                // NotificationContent hinterlegen
                a.NotificationContent = GenerateNotificationText(a.Id.ToString());
                a.IsNotificationGenerated = true;

                Db.SaveChanges();

                SendPushNotification(a.NotificationContent, a.Id.ToString());
            }
            return changeList.Count();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeId"></param>
        /// <returns></returns>
        public bool CreateSingleNotification(string changeId)
        {
            bool notificationGenerated = false;

            ActivityDateChange change = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId));

            //Überprüfen ob die Veranstaltung innerhalb des gegebenen Zeitraums stattfindet
            DateTime activityDate = change.Date.Begin;
            DateTime currentDate = DateTime.Now;

            //Zeitraum für die erstellung der Notification definieren
            DateTime untilDate = currentDate.AddDays(7);

            if (DateTime.Compare(currentDate, activityDate) < 0 && DateTime.Compare(activityDate, untilDate) < 0
                && change.IsNotificationGenerated == false)
            {
                string notificationText = GenerateNotificationText(changeId);

                GenerateNotificationStates(changeId);

            // NotificationContent hinterlegen
            change.NotificationContent = notificationText;
            change.IsNotificationGenerated = true;

            Db.SaveChanges();

            SendPushNotification(change.NotificationContent, change.Id.ToString());

            notificationGenerated = true;

            }
            return notificationGenerated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeId"></param>
        /// <returns></returns>
        public string GenerateNotificationText(string changeId)
        {
            ActivityDateChange change = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId));
 
            string notificationText = "";

            // Notification Text erstellen für den Fall einer Raumänderung
            if (change.HasRoomChange && !change.Date.Occurrence.IsCanceled && !change.HasTimeChange && change.Date.Rooms.Count() > 0)
            {
                if (change.Date.Rooms.Count() > 1)
                {
                    List<Room> raumListe = change.Date.Rooms.ToList();
                    string raumNamen = "";
                    foreach (Room r in raumListe)
                    {
                        raumNamen = raumNamen + r.Number.ToString() + ", ";
                    }
                    raumNamen = raumNamen.Remove(raumNamen.Length - 2);
                    notificationText = "Die Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.OldBegin.ToString("dd/MM") + " findet in den Räumen " + raumNamen + " statt.";
                }
                else
                {
                    notificationText = "Die Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.OldBegin.ToString("dd/MM") + " findet im Raum " + change.Date.Rooms.FirstOrDefault().Number.ToString() + " statt.";
                }
            }

            // Notification Text erstellen für den Fall einer Terminverschiebung
            else if (change.HasTimeChange)
            {
                if (change.Date.Rooms.Count() > 1)
                {
                    List<Room> raumListe = change.Date.Rooms.ToList();
                    string raumNamen = "";
                    foreach (Room r in raumListe)
                    {
                        raumNamen = raumNamen + r.Number.ToString() + ", ";
                    }
                    raumNamen = raumNamen.Remove(raumNamen.Length - 2);
                    notificationText = "Die Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.OldBegin.ToString("dd/MM") + " wurde auf den " + change.NewBegin.ToString("dd/MM") + " um " + change.NewBegin.ToString("HH:mm") + " Uhr verschoben und findet in den Räumen " + raumNamen + " statt.";
                }
                else
                {
                    notificationText = "Die Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.OldBegin.ToString("dd/MM") + " wurde auf den " + change.NewBegin.ToString("dd/MM") + " um " + change.NewBegin.ToString("HH:mm") + " Uhr verschoben und findet im Raum " + change.Date.Rooms.FirstOrDefault().Number.ToString() + " statt.";
                }
            }
            // Notification Text erstellen im Fall einer Absage
            else if (change.HasStateChange && change.Date.Occurrence.IsCanceled)
            {
                notificationText = "Die Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.Date.Begin.ToString("dd/MM") + " wurde abgesagt.";
            }

            // Notification Text erstellen im Fall einer Reaktivierung des Termins
            else if (change.HasStateChange && !change.Date.Occurrence.IsCanceled)
            {
                notificationText = "Die zuvor abgesagte Veranstaltung " + change.Date.Activity.Name.ToString() + " vom " + change.Date.Begin.ToString("dd/MM") + " findet wieder statt.";
            }
            else
            {
                notificationText = "Es konnte kein Text generiert werden.";
            }

            return notificationText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeId"></param>
        public void GenerateNotificationStates(string changeId)
        {

            ActivityDateChange change = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId));

            // Liste aller in betroffenen Kurs eingetragenen User generieren
            List<OccurrenceSubscription> subscriptionList = new List<OccurrenceSubscription>();
            subscriptionList = change.Date.Activity.Occurrence.Subscriptions.ToList();

            // schon bestehende NotificationStates löschen (nur solange die Notifications manuell erzeugt werden)
            change.NotificationStates.Clear();

            //Für jeden User einen notificationState anlegen 
            foreach (OccurrenceSubscription s in subscriptionList)
            {
                NotificationState nState = new NotificationState();
                nState.ActivityDateChange = change;
                nState.IsNew = true;
                nState.UserId = s.UserId;
                change.NotificationStates.Add(nState);
            }

            Db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeId"></param>
        public void MarkAsRead(string userId, string changeId)
        {
            var state = Db.DateChanges.SingleOrDefault(x => x.Id.ToString().Equals(changeId)).NotificationStates.SingleOrDefault(y => y.UserId.Equals(userId));
            
            if (state != null){
                state.IsNew = false;
            }
            Db.SaveChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="changeId"></param>
        public void SendPushNotification(string message, string changeId)
        {
            string stringregIds = null;
            List<string> regIDs = new List<string>();

            //RegistrationId Liste befüllen
            List<OccurrenceSubscription> subscriptionList = new List<OccurrenceSubscription>();


            subscriptionList = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId)).Date.Activity.Occurrence.Subscriptions.ToList();

            foreach (OccurrenceSubscription os in subscriptionList)
            {
                var user = _db.Users.SingleOrDefault(u => u.Id.Equals(os.UserId));
                if (user != null){
                    
                    if(user.Devices.ToList().Count() > 0)
                    {
                        foreach (UserDevice d in user.Devices.ToList())
                            {
                                if (d.IsActive)
                                {
                                    regIDs.Add(d.DeviceId);
                                }
                            }
                    }
                } 
            }

            //To Join the values (if ever there are more than 1) with quotes and commas for the Json format below    
            stringregIds = string.Join("\",\"", regIDs);            
            
            // Time To Live für den GCM Service ermitteln und überprüfen ob die notification schon erstellt werden soll
            DateTime activityDate = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId)).Date.Begin;
            DateTime currentDate = DateTime.Now;

            TimeSpan timeDifference = activityDate - currentDate;
            int timeToLive = 0;

            if (TimeSpan.Compare(timeDifference, TimeSpan.Zero) > 0 && TimeSpan.Compare(timeDifference, TimeSpan.FromMinutes(60)) > 0)
            {
                timeToLive = (int)timeDifference.TotalSeconds;
                if (timeToLive > 2419200)
                {
                    timeToLive = 2419200;
                }
            }
            else if (TimeSpan.Compare(timeDifference, TimeSpan.Zero) > 0 && TimeSpan.Compare(timeDifference, TimeSpan.FromMinutes(60)) < 0)
            {
                timeToLive = 3600;
            }

            if (timeToLive > 0)
            {     
                try
                {
                    string GoogleAppID = "AIzaSyA5aqcrzRLIDs_dqPIZRgrVtHnthU_SUXs";
                    var SENDER_ID = "563733444568";
                    var value = message;
                    WebRequest tRequest;
                    tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

                    tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                    string postData = "{\"time_to_live\":" + timeToLive + ",\"data\": { \"message\" : " + "\"" + value + "\",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\",\"title\":\"" + Db.DateChanges.SingleOrDefault(d => d.Id.ToString().Equals(changeId)).Date.Activity.Name.ToString() + "\"},\"registration_ids\":[\"" + stringregIds + "\"]}";

                    Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    tRequest.ContentLength = byteArray.Length;

                    Stream dataStream = tRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse tResponse = tRequest.GetResponse();

                    dataStream = tResponse.GetResponseStream();

                    StreamReader tReader = new StreamReader(dataStream);

                    String sResponseFromServer = tReader.ReadToEnd();

                    HttpWebResponse httpResponse = (HttpWebResponse)tResponse;
                    string statusCode = httpResponse.StatusCode.ToString();

                    tReader.Close();
                    dataStream.Close();
                    tResponse.Close();
                }
                finally { }
            }
        }
    }
}