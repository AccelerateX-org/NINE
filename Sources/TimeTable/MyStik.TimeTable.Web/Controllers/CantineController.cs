using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CantineController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            ViewBag.MenuId = "menu-cantine";
            ViewBag.Datum = DateTime.Now.ToString("dddd, d.M.yyyy");

            // Hier das Model aufbauen
            MensaViewModel model = new MensaViewModel();

            string daten = "";

            var url = $"http://openmensa.org/api/v2/canteens/{id}";

            // download mensa daten
            try
            {
                daten = GetResult(url);
            }
            catch (Exception ex)
            {
                daten = ""; // Fehlerfall
                ViewBag.Datum = ex.Message.ToString();
                return View(model);
            }

            JToken mensa = JToken.Parse(daten); // Text als JSON parsen

            model.Name = (string)mensa["name"];
            model.Address = (string)mensa["address"];



            url = $"http://openmensa.org/api/v2/canteens/{id}/meals";

            // download mensa daten
            try
            {
                daten = GetResult(url);
            }
            catch (Exception ex)
            {
                daten = ""; // Fehlerfall
                ViewBag.Datum = ex.Message.ToString();
            }

            // Wenn Inhalt in daten vorhanden
            if (daten != "")
            {
                JToken alleElemente = JToken.Parse(daten); // Text als JSON parsen

                // Neue Liste erstellen
                model.Tage = new List<MensaView_Tag>();

                // alle Elemente auslesen
                foreach (JToken Element in alleElemente){
                        // Tage auslesen
                        var tag = new MensaView_Tag
                        {
                            date = (string)Element["date"],
                            closed = (string)Element["closed"],
                            meals = new List<MensaView_Meal>()
                        };
                        // Meals auslesen
                        foreach (JToken Meal in Element["meals"])
                        {
                        // Notizen
                        List<string> tmp_notes = new List<string>();
                        foreach (string value in Meal["notes"].Values()){
                            tmp_notes.Add((string)value);
                        }

                            var meal = new MensaView_Meal
                            {
                                id = (int)Meal["id"],
                                name = (string)Meal["name"],
                                category = (string)Meal["category"],
                            prices = new MensaView_Prices {
                                price_student = parsePrice((string)Meal["prices"]["students"]),
                                price_employees = parsePrice((string)Meal["prices"]["employees"]),
                                price_others = parsePrice((string)Meal["prices"]["others"]),
                                price_pupils = parsePrice((string)Meal["prices"]["pupils"])
                            },
                            notes = tmp_notes
                            };
                            tag.meals.Add(meal);
                        }
                        model.Tage.Add(tag);
                }
            }

            // Vorschau

            JToken allElements = JRaw.Parse(daten); // Text als JSON parsen
            model.Vorschautage = new List<VorschauView_Tag>();

            foreach (JToken Element in allElements)
            {
              if (allElements.First != Element)
              { 
                  var str = (string)Element["date"];
                  var date = DateTime.Parse(str);

                           

                // Vorschautage auslesen
                var Vorschautag = new VorschauView_Tag
                {
                    Datum = date.ToString("dd.MM.yyyy"),
                    closed = (bool)Element["closed"],
                    Name =   date.DayOfWeek.ToString() ,

                    Gerichte = new List<VorschauView_Gerichte>()
                };

                // Meals auslesen
                foreach (JToken gericht in Element["meals"])
                {
                    // Notizen
                    List<string> tmp_notes = new List<string>();
                    foreach (string value in gericht["notes"].Values())
                    {
                        tmp_notes.Add((string)value);
                    }

                     var Tagesgericht = new VorschauView_Gerichte
                     {
                         Name = (string)gericht["name"],
                         Kategorie = (string)gericht["category"],
                         PreisStudent = (string) gericht["prices"]["students"],
                         PreisMitarbeiter = (string)gericht["prices"]["employees"],
                         PreisGaeste = (string)gericht["prices"]["others"],
                         Notizen = tmp_notes
                     };
                    
                     
                     Vorschautag.Gerichte.Add(Tagesgericht);
                     
                }

                model.Vorschautage.Add(Vorschautag);
              }
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StuCafe()
        {
            StuCafeViewModel model = new StuCafeViewModel();

            model.Name = Resources.StuCafeHeading;
//Gerichte
            model.Gerichte = new List<StuCafeGerichteViewModel>();

            var ls = new StuCafeGerichteViewModel
            {
                Name = Resources.MeatLoafBreadString,
                Preis = "1,50 €"
            };
            model.Gerichte.Add(ls);
            var ss = new StuCafeGerichteViewModel
            {
                Name = Resources.SchnitzelBreadString,
                Preis = "2,00 €"
            };

            model.Gerichte.Add(ss);



//Snacks
            model.Snacks = new List<StuCafeSnacksViewModel>();

            var balisto = new StuCafeSnacksViewModel
            {
                Name = Resources.SnacksTableContent,
                Preis = "0,80 €"
            };

            model.Snacks.Add(balisto);
// Getränke
            model.Getraenke = new List<StuCafeGetraenkViewModel>();

            var cola = new StuCafeGetraenkViewModel
            {
                Name = "Coca Cola",
                Preis = "1,50 €"
            };

            var fanta = new StuCafeGetraenkViewModel
            {
                Name = "Fanta",
                Preis = "1,50 €"
            };


            model.Getraenke.Add(cola);
            model.Getraenke.Add(fanta);



            return View(model);
        }

        string GetResult(string url)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader streamReader = new StreamReader(res.GetResponseStream());
            string result = streamReader.ReadToEnd();
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