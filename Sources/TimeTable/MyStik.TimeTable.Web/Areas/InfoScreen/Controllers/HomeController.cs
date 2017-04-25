﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using HtmlAgilityPack;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Areas.InfoScreen.Models;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json.Linq;

namespace MyStik.TimeTable.Web.Areas.InfoScreen.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        // GET: InfoScreen/Home
        public ActionResult Index()
        {
            // leere Seite nur mir Uhr
            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
        public PartialViewResult LeftPanel(string token)
        {
            // Während der Öffnungszeiten der Mensa 
            // abwechselnd MVG und Mensa
            // sonst
            // nur MVG

            // Bei Start immer MVG
            if (string.IsNullOrEmpty(token))
                return MVG();

            // wenn es MVG war, dann Mensa
            if (token.Equals("MVG"))
            {
                // Nach 14:00 keinen Speiseplan mehr anzeigen
                if (DateTime.Now > DateTime.Today.AddHours(14))
                    return MVG();
                
                return Mensa();
            }

            // default MVG
            return MVG();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
        public PartialViewResult RightPanel(string token)
        {
            // Im Wechsel
            // - aktuelle Lehrveranstaltungen
            // - freie Räume
            // - "Werbung", z.B. ppt oder Bild 

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Token = "CurrentCourses@HOKO_Weeks";
                return CurrentCourses();
            }

            string[] elems = token.Split('@');

            if (elems.Length < 2)
            {
                // Theoretisch darf das nicht passieren
                // Default: Zeig die aktuellen Kurse
                ViewBag.Token = "CurrentCourses@HOKO_Weeks";
                return CurrentCourses();
            }

            var lastView = elems[0];
            var nextView = elems[1];

            if (IsCommercial(lastView))
            {
                var nextCommerical = GetNextCommercial(lastView);
                return ShowContent(nextView, nextCommerical);
            }
            else
            {
                var nextContent = GetNextContent(lastView);
                return ShowCommercial(nextView, nextContent);
            }

        }

        private bool IsCommercial(string token)
        {
            return token.Equals("HOKO_Weeks") || token.Equals("HOKO_Screen");
        }

        private string GetNextCommercial(string token)
        {
            if (token.Equals("HOKO_Weeks"))
                return "HOKO_Screen";
            return "HOKO_Weeks";
        }

        private string GetNextContent(string token)
        {
            if (token.Equals("CurrentCourses"))
                return"NextCourses";

            if (token.Equals("NextCourses"))
                return "FreeRooms";

            return "CurrentCourses";
        }

        public PartialViewResult ShowContent(string contentView, string commercialView)
        {
            string token = string.Format("{0}@{1}", contentView, commercialView);
            ViewBag.Token = token;
            
            if (contentView.Equals("CurrentCourses"))
                return CurrentCourses();

            if (contentView.Equals("NextCourses"))
                return NextCourses();

            if (contentView.Equals("FreeRooms"))
                return FreeRooms();

            // Theoretisch darf das nicht passieren
            // Default: Zeig die aktuellen Kurse
            return CurrentCourses();
        }

        public PartialViewResult ShowCommercial(string commercialView, string contentView)
        {
            string token = string.Format("{0}@{1}", commercialView, contentView);
            ViewBag.Token = token;


            // zuletzt HOKO_Weeks
            if (commercialView.Equals("HOKO_Weeks"))
            {
                ViewBag.ImgFileName = "~/Assets/fillter/img/HOKO_Weeks_Stundenplan.jpg";
            }
            else // zuletzt HOKO_Screen
            {
                ViewBag.ImgFileName = "~/Assets/fillter/img/Hoko Screenwerbung.jpg";
            }

            return PartialView("_Commercial");
        }



        #region LeftPanel
        /// <summary>
        /// Aktuelle Abfahrtszeiten MVG
        /// Haltestelle Lothstrasse (Hochschule München)
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MVG()
        {
            var model = new InfoscreenModel();


            // TODO: Abfrage MVV
            model.MVVallDepartures = new List<MvvViewModel>();

            var url =
                "http://www.mvg-live.de/ims/dfiStaticAnzeige.svc?haltestelle=Hochschule+M%fcnchen+(Lothstra%dfe)&ubahn=checked&bus=checked&tram=checked&sbahn=checked";

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            var res = (HttpWebResponse)req.GetResponse();

            // Verwendung des HTML Agility Package zum Parsen des HTMLs
            var doc = new HtmlDocument();
            doc.Load(res.GetResponseStream());

            // geht!
            // erste und letzte Zeile der Tabelle müssen ignoriert werden

            // Alle <tr> Elemente im ganzen HTML-Dokument auswählen
            //var trNodes = doc.DocumentNode.SelectNodes("//tr").Take(10).ToList(); 
            var trNodes = doc.DocumentNode.SelectNodes("//tr").ToList();

            // Es ist bekannt: das erste Element und die beiden letzten Elemente enthalten keine Abfahrtszeiten
            for (var i = 1; i < trNodes.Count - 2; i++)
            {
                // pro <tr>-Element alle <td>-Elemente auswählen
                var tdNodes = trNodes[i].SelectNodes("td");

                // Zur Sicherheit: nur <tr>-Elemente, die exakt 3 <td>-Elemente besitzen nehmen
                if (tdNodes.Count == 3)
                {
                    // im Ziel steht ggf. noch Leerzeichen drin => löschen
                    var ziel = tdNodes[1].InnerText;
                    ziel = ziel.Replace("&nbsp;", "").Trim();

                    // Das Objekt aufbauem
                    var mvv = new MvvViewModel
                    {
                        Liniennummer = tdNodes[0].InnerText,
                        Richtung = ziel,
                        AbfahrtszeitInMin = int.Parse(tdNodes[2].InnerText)
                    };
                    // Das Objekt zur Liste hinzufügen
                    model.MVVallDepartures.Add(mvv);
                }
            }

            return PartialView("_MVG", model);
        }

        /// <summary>
        /// Aktueller Mensaspeiseplan
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Mensa()
        {
            var model = new InfoscreenModel();


            // TODO: Abfrage des Mensaplans
            var m = new MensaViewModel();

            var daten = "";

            // download mensa daten
            // Speiseplan das aktuellen Tages
            try
            {
                daten = GetResult("http://openmensa.org/api/v2/canteens/141/meals");
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

            // wenn es keine Daten gibt, dann doch lieber MVG aufrufen
            if (model.SpeiseplanHeute == null)
                return MVG();

            // 3. Model an den View übergeben
            return PartialView("_Mensa", model);
        }


        #endregion


        #region RightPanel

        public PartialViewResult CurrentCourses()
        {
            var now = DateTime.Now;
            var maxEntries = 14;

            var model = new InfoscreenModel();

            var nowPlaying = Db.ActivityDates.Where(d => d.Begin <= now && now < d.End).OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();

            model.NowPlayingDates = nowPlaying.Where(date => date.Activity is Course).Take(maxEntries).ToList();

            return PartialView("_CurrentCourses", model);
        }

        public PartialViewResult NextCourses()
        {
            var now = DateTime.Now;
            var maxEntries = 14;

            var model = new InfoscreenModel();

            // die nächste sind die, die am selben Tag noch beginnen
            var endOfDay = DateTime.Today.AddDays(1);

            var upComing = Db.ActivityDates.Where(d => d.Begin > now && d.Begin < endOfDay).OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();

            model.UpcomingDates = upComing.Where(date => date.Activity is Course).Take(maxEntries).ToList();


            return PartialView("_NextCourses", model);
        }

        public PartialViewResult FreeRooms()
        {
            var model = new InfoscreenModel();

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var fk09 = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));
            if (fk09 != null)
            {
                var allRooms = roomService.GetAvaliableRoomsNow(fk09.Id, 45);
                // nur R-Bau
                model.CurrentFreeRooms = allRooms.Where(r => r.Room.Number.StartsWith("R")).OrderBy(r => r.Room.Number).Take(14).ToList();

                var nextRooms = roomService.GetAvaliableRoomsNext(fk09.Id, 15, 45);

                // aus nextRooms alle rauswerfen, die in allrooms schon drin sind
                var additionalFreeRooms = nextRooms.Where(room => allRooms.All(r => r.Room.Id != room.Room.Id)).ToList();
                model.NextFreeRooms = additionalFreeRooms.Where(r => r.Room.Number.StartsWith("R")).OrderBy(r => r.Room.Number).Take(14).ToList();
            }
            else
            {
                model.CurrentFreeRooms = new List<RoomInfoModel>();
                model.NextFreeRooms = new List<RoomInfoModel>();
            }




            return PartialView("_FreeRooms", model);
        }


        #endregion





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


        public PartialViewResult Events()
        {
            // TODO: Abfrage des Backends
            
            ViewBag.Datum = DateTime.Now.ToString("dddd,d.MMMM");

            // 1. Modell anlegen
            
            var m = new RechtsEventsModel();
            m.InfoscreenEventsRight = new List<InfoscreenEventsRightViewModel>();

 
            // 2. Modell befüllen


            //alle Veranstaltungen der Woche anzeigen
            var events = Db.Activities.OfType<Event>().Where(ev => ev.Dates.Any(d => d.Begin >= DateTime.Today)).ToList();

            

            // TODO: Befülle das Objekt
            foreach(var ev in events)
            {
                var k = new InfoscreenEventsRightViewModel();
                

                k.Wochentag = ev.Dates.FirstOrDefault().Begin.DayOfWeek.ToString();
                k.Datum = ev.Dates.FirstOrDefault();
                k.Titel = ev.Name;
                //k.Raumnummer = ev.Dates.FirstOrDefault().Rooms.FirstOrDefault().Number;
                



        
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