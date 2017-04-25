using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.FamosImport.Data;

namespace MyStik.TimeTable.FamosImport
{
    class FamosImport
    {
        TimeTableDbContext _db = new TimeTableDbContext();

        List<FamosEvent> _eventList = new List<FamosEvent>();
        List<FamosActivity> _activityList = new List<FamosActivity>();
        List<Room> _roomCache = new List<Room>();
        List<ActivityOrganiser> _organiserCache = new List<ActivityOrganiser>();

        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");


        public void ReadFile(string file, int offset)
        {
            sw.Start();

            _Logger.InfoFormat("Lese Bytes der Datei {0}", file);
            //var content = File.ReadAllText(file, Encoding.Default);
            var bytes = File.ReadAllBytes(file);

            var l = bytes.LongLength;
            for (var j=0; j<l; j++)
            {
                if (bytes[j] == 10)
                {
                    if (j > 0 && bytes[j - 1] == 13)
                    {
                        
                    }
                    else
                    {
                        bytes[j] = 32;
                    }
                }
            }

            var tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, bytes);
            _Logger.Info("Bytes ausgetauscht");


            _Logger.InfoFormat("Lese Textdatei {0}", tempFile);
            var lines = File.ReadAllLines(tempFile, Encoding.Default);

            File.Delete(tempFile);

            var i = 0;
            const int expectedCols = 12;
            foreach (var line in lines)
            {
                if (i > 0)
                {
                    var words = line.Split(';');


                    var raum = words[0].Replace("\"", "").Trim();
                    var datum = words[1].Replace("\"", "").Trim();
                    var von = words[2].Replace("\"", "").Trim();
                    var bis = words[3].Replace("\"", "").Trim();
                    var vid = words[4].Replace("\"", "").Trim();
                    string beschreibung;
                    string org;

                    // Wenn in der Beschreibung selbst ";" drin sind, dann gibt es mehr Wörter als die
                    // erwarteten expectedCols
                    if (words.Length > expectedCols)
                    {
                        beschreibung = words[7].Replace("\"", "").Trim();
                        for (var k = 1; k <= words.Length - expectedCols; k++)
                        {
                            beschreibung += " " + words[7 + k].Replace("\"", "").Trim();
                        }
                        org = words[8 + words.Length - expectedCols].Replace("\"", "").Trim();
                    }
                    else
                    {
                        beschreibung = words[7].Replace("\"", "").Trim();
                        org = words[8].Replace("\"", "").Trim();
                    }



                    var date = DateTime.Parse(datum);
                    var from = DateTime.Parse(von);
                    var to = DateTime.Parse(bis);

                    var eventStart = date.AddDays(offset).AddHours(from.Hour).AddMinutes(from.Minute);
                    var eventEnd = date.AddDays(offset).AddHours(to.Hour).AddMinutes(to.Minute);


                    _eventList.Add(new FamosEvent
                    {
                        EventId = vid,
                        Begin = eventStart,
                        End = eventEnd,
                        Description = beschreibung,
                        Organiser = org,
                        RoomNumber = raum,
                    });
                }
                i++;
            }

            _Logger.InfoFormat("Datei {0} in {1} ms gelesen", file, sw.ElapsedMilliseconds);
        }

        public void CheckRooms()
        {
            _Logger.Info("Überprüfe Räume");
            var msStart = sw.ElapsedMilliseconds;

            var roomList = _eventList.Select(e => e.RoomNumber).Distinct().ToList();

            var roomCount = 0;
            var existingCount = 0;
            var newCount = 0;

            foreach (var roomNo in roomList)
            {
                roomCount++;
                var room = _db.Rooms.SingleOrDefault(r => r.Number.Equals(roomNo));
                if (room == null)
                {
                    newCount++;
                    _Logger.InfoFormat("neuer Raum: {0}", roomNo);

                    room = new Room
                    {
                        Capacity = 0,
                        Description = string.Empty,
                        Name = string.Empty,
                        Number = roomNo,
                    };

                    _db.Rooms.Add(room);
                }
                else
                {
                    existingCount++;
                    _Logger.DebugFormat("Bekannter Raum: {0}", roomNo);
                }

                _roomCache.Add(room);
            }

            _Logger.InfoFormat("Vorhandene Räume: {0}", existingCount);
            _Logger.InfoFormat("Neue Räume: {0}", newCount);
            _Logger.InfoFormat("Räume gesamt: {0}", roomCount);

            _db.SaveChanges();

            var msEnd = sw.ElapsedMilliseconds;
            _Logger.InfoFormat("Überprüfung der Räume in {0} ms", msEnd - msStart);
        }

        public void CheckOrganiser()
        {
            _Logger.Info("Überprüfe Veranstalter");
            var msStart = sw.ElapsedMilliseconds;

            var orgList = _eventList.Select(e => e.Organiser).Distinct().ToList();

            var roomCount = 0;
            var existingCount = 0;
            var newCount = 0;

            foreach (var orgName in orgList)
            {
                roomCount++;
                var org = _db.Organisers.SingleOrDefault(o => o.Name.Equals(orgName));
                if (org == null)
                {
                    if (string.IsNullOrEmpty(orgName) || orgName.Equals("noch zu bearbeiten"))
                    {
                        _Logger.WarnFormat("wird nicht importiert: {0}", orgName);
                    }
                    else
                    {
                        newCount++;
                        _Logger.InfoFormat("neuer Veranstalter: {0}", orgName);

                        // Generierung der Kurznamen
                        // Fakultät => ok
                        // sonstiges?
                        // ersten 3 Buchstaben in Groß mit Prüfung!
                        // Fakultät 00
                        string name = orgName;
                        string shortName = orgName;
                        bool isFaculty = false;

                        if (orgName.StartsWith("Fakultät"))
                        {
                            var elem = orgName.Split(' ');
                            if (elem.Length == 2)
                            {
                                shortName = "FK " + elem[1];
                            }

                            isFaculty = true;
                        }

                        org = new ActivityOrganiser
                        {
                            Name = name,
                            ShortName = shortName,
                            IsFaculty = isFaculty,
                            IsStudent = false, // in FAMOS werden keine Fachschaften etc. verwaltet => DOCH
                        };

                       _db.Organisers.Add(org);
                    }
                }
                else
                {
                    existingCount++;
                    _Logger.DebugFormat("Bekannter Veranstalter: {0}", orgName);
                }

                if (org != null)
                    _organiserCache.Add(org);

            }

            _Logger.InfoFormat("Vorhandene Veranstalter: {0}", existingCount);
            _Logger.InfoFormat("Neue Veranstalter: {0}", newCount);
            _Logger.InfoFormat("Veranstalter gesamt: {0}", roomCount);

            _db.SaveChanges();

            var msEnd = sw.ElapsedMilliseconds;
            _Logger.InfoFormat("Überprüfung der Veranstalter in {0} ms", msEnd - msStart);

        }

        public void CheckActivities()
        {
            _Logger.Info("Überprüfe Aktivitäten");
            var msStart = sw.ElapsedMilliseconds;

            var sortedEventList = _eventList.OrderBy(e => e.EventId).ToList();

            // Alle bisher importierten Activities laden
            var activies = _db.Activities.OfType<Reservation>().Where(a => a.ExternalSource.Equals("FAMOS")).ToList();
            foreach (var activy in activies)
            {
                _activityList.Add(new FamosActivity
                {
                    Activity = activy,
                    EventId = activy.ExternalId,
                    Organiser = activy.Organiser.Name,
                    IsValid = true,
                    IsTouched = false,
                });
            }

            var exitingActivities = activies.Count();
            var newActivities = 0;

            // jedes zu importierende Event durchgehen
            foreach (var famosEvent in sortedEventList)
            {
                // Habe ich die Activity bereits geladen?
                var famosActivity = _activityList.SingleOrDefault(a => a.EventId.Equals(famosEvent.EventId));

                if (famosActivity == null)
                {
                    // für diese EventId gibt es noch keinen Eintrag in der Datenbank
                    newActivities++;

                    famosActivity = new FamosActivity
                    {
                        EventId = famosEvent.EventId,
                        Organiser = famosEvent.Organiser,
                        Activity = null,
                    };
                    _activityList.Add(famosActivity);
                }

                famosActivity.Events.Add(famosEvent);
                famosActivity.IsTouched = true;

                // passen Organiser und EventId zusammen?
                if (famosEvent.EventId.Equals(famosActivity.EventId) &&
                    famosEvent.Organiser.Equals(famosActivity.Organiser))
                {
                    famosActivity.IsValid = true;
                }
                else
                {
                    famosActivity.IsValid = false;
                    _Logger.Error("Da passt was nicht");
                }
            }

            // jetzt habe ich alle Aktivitäten geladen, zu denen es Import-Termine gibt
            // => ich kann alle Aktivitäten löschen zu denen es keine Termine mehr gibt!
            var activitiesToDelete = _activityList.Where(a => a.IsTouched == false).ToList();
            foreach (var famosActivity in activitiesToDelete)
            {
                DeleteEvent(famosActivity.Activity);
                _activityList.Remove(famosActivity);
            }

            _Logger.InfoFormat("Veranstaltungen in DB {0}", exitingActivities);
            _Logger.InfoFormat("Neue Veranstaltungen {0}", newActivities);
            _Logger.InfoFormat("Lösche {0} Veranstaltung aus DB, weil keine Termine mehr im Import", activitiesToDelete.Count());

            var msEnd = sw.ElapsedMilliseconds;
            _Logger.InfoFormat("Überprüfung der Aktivitäten in {0} ms", msEnd - msStart);
        }


        public void DeleteImport()
        {
            _Logger.Info("Lösche alle FAMOS-Daten...");
            /*
            var events = _db.Activities.OfType<Event>().Where(a => a.ExternalSource.Equals("FAMOS")).ToList();
            foreach (var activy in events)
            {
                DeleteEvent(activy);
            }
             */

            var reservations = _db.Activities.OfType<Reservation>().Where(a => a.ExternalSource.Equals("FAMOS")).ToList();
            var n = reservations.Count();
            var i = 0;
            _Logger.InfoFormat("Lösche {0} Reservierungen", n);
            foreach (var activy in reservations)
            {
                DeleteEvent(activy);
                i++;
                if (i%50 == 0)
                {
                    _Logger.InfoFormat("{0} von {1} Reservierungen gelöscht", i, n);                    
                }
            }
            
            _Logger.Info("beendet");
        }


        public void ImportDates()
        {

            var nActivity = _activityList.Count;
            var iActivity = 0;
            var iDates = 0;
            _Logger.InfoFormat("Es werden {0} Aktivitäten importiert", nActivity);
            var msStart = sw.ElapsedMilliseconds;


            // Jede Aktivität durchgehen
            // Was wissen wir? Die Activties sind sauber, d.h. es sind nur noch, die import werden sollen
            foreach (var famosActivity in _activityList)
            {
                var org = GetOrganiser(famosActivity.Organiser);

                if (org != null)
                {
                    if (famosActivity.Activity == null)
                    {
                        // die neue Veranstaltung in der DB anlegen
                        var activity = new Reservation
                        {
                            ExternalSource = "FAMOS",
                            ExternalId = famosActivity.EventId,
                            Organiser = GetOrganiser(famosActivity.Organiser),
                            Name = famosActivity.EventId,
                            ShortName = famosActivity.EventId,
                            Occurrence = CreateDefaultOccurrence(),
                            // Description gibt es nicht, die wird bei den Dates dazugefügt
                        };
                        famosActivity.Activity = activity;
                        _db.Activities.Add(activity);
                        //_db.SaveChanges();
                    }

                    // alle Dates, wir löschen die die wir gefunden haben
                    var touchList = famosActivity.Activity.Dates.OrderBy(d => d.Begin).ToList();


                    // jetzt gehen wir alle Events durch
                    var eventList = famosActivity.Events.OrderBy(d => d.Begin).ToList();
                    foreach (var famosEvent in eventList)
                    {
                        // das sind alle Events, die exakt zu dem importierten passen
                        var matchList = touchList.Where(d => d.Begin == famosEvent.Begin &&
                                                             d.End == famosEvent.End &&
                                                             d.Rooms.Any(r => r.Number.Equals(famosEvent.RoomNumber)))
                            .ToList();

                        // Fälle:
                        var bFound = true;
                        if (matchList.Any())
                        {
                            var date = matchList.First();

                            if (matchList.Count() == 1)
                            {
                                // exakt 1 gefunden => Description aktualiseren (falls sich geändert hat), aus Touchliste löschen

                                if (!date.Description.Equals(famosEvent.Description))
                                {
                                    // Die Beschreibung hat sich geändert
                                    date.Description = famosEvent.Description;
                                }

                                touchList.Remove(date);
                            }
                            else
                            {
                                // mehrere gefunden => Description checken
                                // wenn gefunden, dann wie oben
                                // wenn keiner passm dann neu anlegen und in touchListe lassen
                                var deepMatchList =
                                    matchList.Where(d => d.Description.Equals(famosEvent.Description)).ToList();
                                if (deepMatchList.Any())
                                {
                                    // es gibt einen
                                    touchList.Remove(deepMatchList.First());
                                }
                                else
                                {
                                    // neues Date anlegen
                                    bFound = false;
                                }
                            }
                        }
                        else
                        {
                            // nicht gefunden => dann in touchListe lassen, neu anlegen
                            bFound = false;
                        }

                        if (!bFound)
                        {
                            var date = new ActivityDate
                            {
                                Activity = famosActivity.Activity,
                                Begin = famosEvent.Begin,
                                End = famosEvent.End,
                                Description = famosEvent.Description,
                                Occurrence = CreateDefaultOccurrence(),
                                //ShortName = 
                                //Title = 
                            };
                            date.Rooms.Add(GetRoom(famosEvent.RoomNumber));
                            _db.ActivityDates.Add(date);
                            iDates++;
                        }
                        //_db.SaveChanges();

                        // jetzt alle Dates löschen, die noch in der touchListe sind
                        foreach (var date in touchList)
                        {
                            DeleteDate(famosActivity.Activity, date, false);
                        }

                    }

                    _db.SaveChanges();
                }

                iActivity++;
                if (iActivity%50 == 0)
                {
                    _Logger.InfoFormat("{0} von {1} Aktivitäten mit {2} Terminen importiert", iActivity, nActivity, iDates);
                    iDates = 0;
                }
            }

            var msEnd = sw.ElapsedMilliseconds;
            _Logger.InfoFormat("Termine für {0} in {1} ms importiert", nActivity,  msEnd - msStart);
        }

        private static Occurrence CreateDefaultOccurrence()
        {
            return new Occurrence
            {
                Capacity = -1,
                IsAvailable = true,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
            };
        }

        private Room GetRoom(string roomNumber)
        {
            return _roomCache.SingleOrDefault(r => r.Number.Equals(roomNumber));
            //return _db.Rooms.SingleOrDefault(r => r.Number.Equals(roomNumber));
        }

        private ActivityOrganiser GetOrganiser(string orgName)
        {
            return _organiserCache.SingleOrDefault(o => o.Name.Equals(orgName));
            //return _db.Organisers.SingleOrDefault(o => o.Name.Equals(orgName));
        }

        private void DeleteDate(Activity activity, ActivityDate date, bool saveState = true)
        {
            foreach (var slot in date.Slots.ToList())
            {
                foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                {
                    slot.Occurrence.Subscriptions.Remove(sub);
                    _db.Subscriptions.Remove(sub);
                }
                _db.Occurrences.Remove(slot.Occurrence);
                date.Slots.Remove(slot);
                _db.ActivitySlots.Remove(slot);
            }

            foreach (var sub in date.Occurrence.Subscriptions.ToList())
            {
                date.Occurrence.Subscriptions.Remove(sub);
                _db.Subscriptions.Remove(sub);
            }

            _db.Occurrences.Remove(date.Occurrence);
            activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();
            _db.ActivityDates.Remove(date);

            if (saveState)
                _db.SaveChanges();
        }

        private void DeleteEvent(Activity activity)
        {
            if (activity != null)
            {
                foreach (var date in activity.Dates.ToList())
                {
                    foreach (var slot in date.Slots.ToList())
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                        {
                            slot.Occurrence.Subscriptions.Remove(sub);
                            _db.Subscriptions.Remove(sub);
                        }
                        _db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        _db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        _db.Subscriptions.Remove(sub);
                    }

                    _db.Occurrences.Remove(date.Occurrence);
                    activity.Dates.Remove(date);
                    date.Hosts.Clear();
                    date.Rooms.Clear();
                    _db.ActivityDates.Remove(date);
                }

                foreach (var sub in activity.Occurrence.Subscriptions.ToList())
                {
                    activity.Occurrence.Subscriptions.Remove(sub);
                    _db.Subscriptions.Remove(sub);
                }

                _db.Occurrences.Remove(activity.Occurrence);
                _db.Activities.Remove(activity);

                _db.SaveChanges();
            }
            
        }
    }
}
