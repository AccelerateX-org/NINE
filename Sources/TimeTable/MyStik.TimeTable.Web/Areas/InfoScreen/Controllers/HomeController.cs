using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using HtmlAgilityPack;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Areas.InfoScreen.Models;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json.Linq;

namespace MyStik.TimeTable.Web.Areas.InfoScreen.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            var screen = Db.Infoscreens.SingleOrDefault(x => x.Tag.Equals(id));
            if (screen == null)
            {
                return HttpNotFound("Kein Infoscreen mit dem Tag " + id + " gefunden");
            }

            return View("Main", screen);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 10)]
        public PartialViewResult ContentForPage(Guid id, int page)
        {
            return RoomSchedule(id, page);

            //return PartialView("_Dummy");
        }





        #region LeftPanel
        /// <summary>
        /// Aktuelle Abfahrtszeiten MVG
        /// Haltestelle Lothstrasse (Hochschule München)
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Aktueller Mensaspeiseplan
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Mensa(string location)
        {
            var model = new InfoscreenModel();


            // TODO: Abfrage des Mensaplans
            var m = new MensaViewModel();

            var daten = "";

            // download mensa daten
            // Speiseplan das aktuellen Tages
            try
            {
                if (location.Equals("FK 10") || location.Equals("FK 11"))
                {
                    daten = GetResult("http://openmensa.org/api/v2/canteens/142/meals");
                }
                else
                {
                    daten = GetResult("http://openmensa.org/api/v2/canteens/141/meals");
                }
            }
            catch (Exception ex)
            {
                daten = ""; // Fehlerfall
                ViewBag.Datum = ex.Message.ToString();
            }

            // Wenn Inhalt in daten vorhanden
            if (daten != "")
            {
                var alleElemente = JToken.Parse(daten); // Text als JSON parsen

                // Neue Liste erstellen
                m.Tage = new List<MensaView_Tag>();

                // alle Elemente auslesen
                foreach (var Element in alleElemente)
                {
                    // Tage auslesen
                    var tag = new MensaView_Tag
                    {
                        date = (string)Element["date"],
                        closed = (string)Element["closed"],
                        meals = new List<MensaView_Meal>()
                    };

                    // Meals auslesen
                    foreach (var Meal in Element["meals"])
                    {
                        var tmp_notes = new List<string>();
                        foreach (string value in Meal["notes"].Values())
                        {
                            tmp_notes.Add((string)value);
                        }

                        var pri = new MensaView_Prices
                        {
                            price_student = parsePrice((string)Meal["prices"]["students"]),
                            price_employees = parsePrice((string)Meal["prices"]["employees"]),
                            price_others = parsePrice((string)Meal["prices"]["others"]),
                            price_pupils = parsePrice((string)Meal["prices"]["pupils"])
                        };

                        var meal = new MensaView_Meal
                        {
                            id = (int)Meal["id"],
                            name = (string)Meal["name"],
                            category = (string)Meal["category"],
                            prices = pri,
                            notes = tmp_notes
                        };
                        tag.meals.Add(meal);
                    }

                    m.Tage.Add(tag);
                }

                model.SpeiseplanHeute = m.Tage.FirstOrDefault();
            }

            // 3. Model an den View übergeben
            return PartialView("_Mensa", model);
        }


        #endregion


        #region RightPanel

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult RoomSchedule(Guid id, int page)
        {
            var screen = Db.Infoscreens.SingleOrDefault(x => x.Id == id);

            if (screen == null)
            {
                return PartialView("_RoomSchedule", new InfoscreenModel{ RoomSchedules = new List<RoomScheduleViewModel>() });
            }   

            var screenPage = screen?.Pages.SingleOrDefault(p => p.Index == page);

            if (screenPage == null)
            {
                return PartialView("_RoomSchedule", new InfoscreenModel { RoomSchedules = new List<RoomScheduleViewModel>() });
            }   

            var now = DateTime.Now;
            var diff = 45 - now.Minute % 15;
            var next = now.AddMinutes(diff);

            var model = new InfoscreenModel { 
                RoomSchedules= new List<RoomScheduleViewModel>(),
                Page = screenPage
            };

            //foreach (var room in rooms)
            foreach (var r in screenPage.RoomAllocationGroup.RoomAllocations.Select(ra => ra.Room))
            {
                if (r != null)
                {
                    // was läuft jetzt in dem Raum
                    var currentDates = Db.ActivityDates.Where(d => d.Begin <= now  && d.End >= now && d.Rooms.Any(rm => rm.Id == r.Id))
                        .OrderBy(d => d.Begin).ThenBy(d => d.End).Include(d => d.Activity).ToList();

                    var nextDates = Db.ActivityDates.Where(d => d.Begin > now && d.Begin <= next && d.Rooms.Any(rm => rm.Id == r.Id))
                        .OrderBy(d => d.Begin).ThenBy(d => d.End).Include(d => d.Activity).ToList();

                    // kein Nachfolger im Zeitfenster => nächster Termin heute
                    if (!nextDates.Any())
                    {
                        var nextDay = now.Date.AddDays(1);
                        nextDates = Db.ActivityDates.Where(d => d.Begin > now && d.Begin < nextDay && d.Rooms.Any(rm => rm.Id == r.Id))
                            .OrderBy(d => d.Begin).ThenBy(d => d.End).Include(d => d.Activity).ToList();

                    }

                    // nur die nächsten 2 anzeigen
                    nextDates = nextDates.Take(2).ToList();

                    var sched = new RoomScheduleViewModel { CurrentDates = currentDates, NextDates = nextDates, Room = r };

                    model.RoomSchedules.Add(sched);
                }
            }


            return PartialView("_RoomSchedule", model);
        }

        /// <summary>
        /// https://www.mvv-muenchen.de/fahrplanauskunft/fuer-entwickler/index.html
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult PublicTransport(string id)
        {
            // das mit dem dynamischen Nachladen der Daten klappt nicht
            return PartialView("_PublicTransport" );
        }

        public PartialViewResult Ads(string id)
        {
            var model = new InfoscreenModel();
            return PartialView("_Ads", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CurrentCourses(string location = "FK 09")
        {
            var now = DateTime.Now;
            var maxEntries = 14;

            var model = new InfoscreenModel();

            var fk = location;


            var nowPlaying = Db.ActivityDates.Where(d =>
                    d.Activity.Organiser.ShortName.Equals(fk) &&
                    d.Begin <= now && now < d.End).OrderBy(d => d.Begin).ThenBy(d => d.End)
                .Include(activityDate => activityDate.Activity).ToList();

            model.NowPlayingDates = nowPlaying.Where(date => date.Activity is Course).Take(maxEntries).ToList();

            return PartialView("_CurrentCourses", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NextCourses(string location = "FK 09")
        {
            var now = DateTime.Now;
            var maxEntries = 14;

            var model = new InfoscreenModel();

            var fk = location;

            // die nächste sind die, die am selben Tag noch beginnen
            var endOfDay = DateTime.Today.AddDays(1);

            var upComing = Db.ActivityDates.Where(d =>
                d.Activity.Organiser.ShortName.Equals(fk) &&
                d.Begin > now && d.Begin < endOfDay).OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();

            model.UpcomingDates = upComing.Where(date => date.Activity is Course).Take(maxEntries).ToList();


            return PartialView("_NextCourses", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult FreeRooms(string location = "FK 09")
        {
            var model = new InfoscreenModel();

            var fk = location;
            var b = "R";
            if (location.Equals("FK 10"))
            {
                b = "L";
            }
            else if (location.Equals("FK 11"))
            {
                b = "K";
            }

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var fk09 = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals(fk));
            if (fk09 != null)
            {
                var allRooms = roomService.GetAvaliableRoomsNow(fk09.Id, 45);
                // nur R-Bau
                model.CurrentFreeRooms = allRooms.Where(r => r.Room.Number.StartsWith(b)).OrderBy(r => r.Room.Number).Take(14).ToList();

                /*
                var nextRooms = roomService.GetAvaliableRoomsNext(fk09.Id, 15, 45);

                // aus nextRooms alle rauswerfen, die in allrooms schon drin sind
                var additionalFreeRooms = nextRooms.Where(room => allRooms.All(r => r.Room.Id != room.Room.Id)).ToList();
                model.NextFreeRooms = additionalFreeRooms.Where(r => r.Room.Number.StartsWith(b)).OrderBy(r => r.Room.Number).Take(14).ToList();
                */
            }
            else
            {
                model.CurrentFreeRooms = new List<RoomInfoModel>();
                model.NextFreeRooms = new List<RoomInfoModel>();
            }




            return PartialView("_FreeRooms", model);
        }


        #endregion




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Announcements()
        {
            // lese Daten aus Datenbank

            var fs09 = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FS 09"));
           // var fk09 = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals("Fakultät 09"));

            // Liste aller Events, die mindestens 1 termin in der Zukunft haben
            // erst einmal alle events
            //var eventList =  Db.Activities.OfType<Event>().Where(ev => ev.Published == true && ev.Organiser.Id == fs09.Id &&
            //    ev.Dates.Any(d => d.Begin >= DateTime.Now)).ToList();
            var eventList = Db.Activities.OfType<Event>().Where(ev => ev.Dates.Any(d => d.Begin >= DateTime.Now)).ToList();

            // var jobList = Db.Activities.OfType<JOB>().Where(ev => ev.Published == true && ev.Organiser.Id == fk09.Id && 
            // ev.Dates.Any(d => d.Begin >= DateTime.Today)).ToList();

            
            var model = new NachmittagRechtsModel();

            model.Events = new List<InfoscreenEventViewModel>();

            foreach(var ev in eventList)
            {
                var ev1 = new InfoscreenEventViewModel
                {
                    Titel = ev.Name,
                    Beschreibung = ev.Description,
                    Ort = ev.Dates.FirstOrDefault().Rooms.FirstOrDefault().Name,
                    Datum = ev.Dates.OrderBy(d => d.Begin).FirstOrDefault(d => d.Begin >= DateTime.Now).Begin

                };

                model.Events.Add(ev1);

            }

            // sollten keine Daten in der DB sein, dann machnen wir uns selbst welche
            if (!eventList.Any())
            {

            // baue das Modellobjekt aus Dummydaten auf
            var ev1 = new InfoscreenEventViewModel
            {
                    Bild = "http://4.bp.blogspot.com/-e6D4R-wtDek/VWhejYdIDSI/AAAAAAAAFFE/8JNjovkmyuQ/s1600/gradu.jpg",
                    Titel = "Titel_1",
                    Beschreibung = "Beschreibung Testevent_1",
            };

                var ev2 = new InfoscreenEventViewModel
                {
                    Bild = "http://www.bayern.by/data/mediadb/cms_pictures/%7B7e6e2681-f4c3-5538-661d-80f649a659a9%7D.jpeg",
                    Titel = "Titel_2",
                    Beschreibung = "Beschreibung Testevent_2",
                };
           
                var ev3 = new InfoscreenEventViewModel
                {
                    Bild = "http://www.windwardboardshop.com/wp/wp-content/uploads/2013/09/burton-welcome-to-winter.jpg",
                    Titel = "Titel_3",
                    Beschreibung = "Beschreibung Testevent_3",
                };

            model.Events.Add(ev1);
                model.Events.Add(ev2);
                model.Events.Add(ev3);
            }


            return PartialView("_RechtsCarousel", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Announcements2()
        {
            var model = new NachmittagRechtsModel();

            model.Events = new List<InfoscreenEventViewModel>();

            var ev1 = new InfoscreenEventViewModel
            {
                Bild = "https://w3-mediapool.hm.edu/mediapool/media/dachmarke/dm_lokal/presse/news_1/bilder_48/2015_2/12_14/Urkunden.jpg",
                Titel = "Announcement_1",
                Beschreibung = "Beschreibung Announcement_1",
            };

            var ev2 = new InfoscreenEventViewModel
            {
                Bild = "https://w3-mediapool.hm.edu/mediapool/media/dachmarke/dm_lokal/presse/news_1/bilder_48/2015_2/12_14/einladungskarte-2.jpg",
                Titel = "Announcement_2",
                Beschreibung = "Beschreibung Announcement_2",
            };

            var ev3 = new InfoscreenEventViewModel
            {
                Bild = "https://w3-mediapool.hm.edu/mediapool/media/dachmarke/dm_lokal/presse/news_1/bilder_48/2015_2/12_14/SUNY_Albany_Praesidenten_2.jpg",
                Titel = "Announcementl_3",
                Beschreibung = "Beschreibung Announcement_3",
            };

            model.Events.Add(ev1);
            model.Events.Add(ev2);
            model.Events.Add(ev3);

            return PartialView("_RechtsCarouselAnnouncements", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Events()
        {
            // TODO: Abfrage des Backends
            
            ViewBag.Datum = DateTime.Now.ToString("dddd,d.MMMM");

            // 1. Modell anlegen
            
            var m = new RechtsEventsModel();
            m.InfoscreenEventsRight = new List<InfoscreenEventsRightViewModel>();

 
            // 2. Modell befüllen


            //alle Veranstaltungen der Woche anzeigen
            // Abfragen der Datenbank ob Event am Infoscreen veröffentlicht werden soll

            var events = Db.Activities.OfType<Event>().Where(ev => ev.Dates.Any(d => d.Begin >= DateTime.Today) && ev.Published.Equals(true)).Take(3).ToList();
            

            // TODO: Befülle das Objekt
            foreach (var ev in events)
            {

                var k = new InfoscreenEventsRightViewModel();
                
                    k.Infotext = ev.Info;
                k.Wochentag = ev.Dates.FirstOrDefault().Begin.DayOfWeek.ToString();
                k.Datum = ev.Dates.FirstOrDefault();
                k.Titel = ev.Name;
                k.Fakultät = "Fakultät 09";

                    if (ev.Dates.Any() && ev.Dates.First().Rooms.Any())
                    {
                        k.Raumnummer = ev.Dates.FirstOrDefault().Rooms.FirstOrDefault().Number;
                    }
                    else
                    {
                        k.Raumnummer = "unbekannt";
                    }
                    k.Kurzbeschreibung = ev.ShortName;
                    k.Beschreibung = ev.Description;
        
            m.InfoscreenEventsRight.Add(k);


            // TODO: Verzweigung nach Wochentag => andere Sicht, anderes Modell
                
            }
            return PartialView("_RechtsEvents",m);


        }
        string GetResult(string url)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json";
            var res = (HttpWebResponse)req.GetResponse();
            var streamReader = new StreamReader(res.GetResponseStream());
            var result = streamReader.ReadToEnd();
            return result;
        }

        double parsePrice(string input)
        {
            double value;
            if (input == null) return 0.0;
            input = input.Replace(".", ","); // interner parser checkt kein "." => low
            if (Double.TryParse(input, out value))
            {
                return value;
            }
            else
            {
                return 0.0;
            }
        }
        
    }
}