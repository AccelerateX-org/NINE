using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomReportController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ChartTest()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Days"></param>
        /// <param name="fromCalendar"></param>
        /// <param name="untilCalendar"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TimeLine(RoomReportRequestModel model, string Days, string fromCalendar, string untilCalendar)
        {


            //Deklaration der Variablen für die Korrektur von Auswahlen
            // Entfernung aller Vorgestellten Leerzeichen
            model.RoomNumber = model.RoomNumber.Trim();
            var length = model.RoomNumber.Length;
            var punkt = ".";
            var leerzeichen = " ";
            var ausgabe = "Fehler";
            var ende = "Fehler";
            var anfang = "das ist";

            //Variablen für Einfügen des R-Gebäudes 
            var NummernStart = false;
            var testwert = 0;
            var korrekturwert = 0;

            bool result = Int32.TryParse(model.RoomNumber.Remove(1), out testwert);
            if (result)
            {
                NummernStart = true;
            }


            if (NummernStart == true)
            {
                //prüft, ob der Raum mit einem Gebäudebuchstaben startet und die sonstige Länge korrekt ist, und setzt ein R, wenn das nicht der Fall ist
                model.RoomNumber = "R" + model.RoomNumber;
                korrekturwert = 1;
            }


            //Ansatz auch ein Leerzeichen am Anfang zu akzeptieren; mit Ausgabe als HTML-Datei testen; wahrscheinlich nur einen Wert zu korrigieren nötig
            /* if(String.Equals(model.RoomNumber.Remove(1), " "))
                 {
                     model.RoomNumber = model.RoomNumber.Substring(1);
                 }
             */
            if (model.RoomNumber.Contains(punkt) == false && model.RoomNumber.Contains(leerzeichen) == true)
            {
                //prüft ob ein Punkt fehlt, ergänzt ihn wenn ja
                ende = model.RoomNumber.Substring(length - 3);
                anfang = model.RoomNumber.Remove(length - 3);
                ausgabe = anfang + punkt + ende;
                model.RoomNumber = ausgabe;
            }
            else if (model.RoomNumber.Contains(punkt) == true && model.RoomNumber.Contains(leerzeichen) == false)
            {
                //prüft ob ein Leerzeichen fehlt, der Punkt aber nicht, ergänzt es wenn ja
                ende = model.RoomNumber.Substring(1);
                anfang = model.RoomNumber.Remove(1);
                ausgabe = anfang + leerzeichen + ende;
                model.RoomNumber = ausgabe;
            }
            else if (model.RoomNumber.Contains(punkt) == false && model.RoomNumber.Contains(leerzeichen) == false)
            {
                //prüft ob ein Leerzeichen und der Punkt fehlen, ergänzt sie falls ja; korrekturwert für den Fall eines Aufrufs durch die Buchstabengebung
                ende = model.RoomNumber.Substring(length - 3 + korrekturwert);
                var mitte = model.RoomNumber.Substring(1);
                mitte = mitte.Remove(1);
                anfang = model.RoomNumber.Remove(1);
                ausgabe = anfang + leerzeichen + mitte + punkt + ende;
                model.RoomNumber = ausgabe;
            }


            // Den Raum suchen
            var room = Db.Rooms.SingleOrDefault(r => r.Number.Equals(model.RoomNumber));

            // wenn es den Raum nicht gibt, dann eine Fehlermeldung anzeigen
            if (room == null)
            {
                // IsValid: Ist der Wert "leer"?
                if (ModelState.IsValid)
                {
                    //Ist nicht leer => Raum muss falsch eingegeben worden sein
                    ModelState.AddModelError("RoomNumber", "Falsche Raumnummer");
                    return View("Index");
                }
                else
                {
                    //ist leer. Fehlermeldung wird "automatisch" angezeigt.
                    // ModelState.AddModelError(string.Empty, "Falsche Eingaben");
                    return View("Index");
                }

            }


            //BERECHNUNG DER RAUMDATEN

            //Wenn Daten in beide Kalender eingegeben worden sind, hat dies Vorrang, wenn nur in eins oder keins etwas eingetragen wurde, passiert hier nichts
            if (fromCalendar != "" && untilCalendar != "")
            {
                DateTime fromC = Convert.ToDateTime(fromCalendar);
                DateTime untilC = Convert.ToDateTime(untilCalendar);
                DateTime untilC2 = Convert.ToDateTime(fromCalendar);

                int Zeitraum2 = (untilC - fromC).Days;
                if (Zeitraum2 < 0)
                {
                    Zeitraum2 = Zeitraum2 * (-1);
                }

                var responeC = new RoomReportResponseModel();
                responeC.Room = room;

                //Übergabe der Werte aus dem RoomReportRequestModel 
                responeC.fromCalendarTransport = model.fromCalendar;
                responeC.untilCalendarTransport = model.untilCalendar;

                for (int n = 0; n <= Zeitraum2; n++)
                {
                    untilC = fromC.AddDays(n);
                    untilC2 = fromC.AddDays(n + 1);

                    double TagDauerC = 930;
                    int AnzahlBelegungenC = 0;
                    int GesamtdauerC = 0;
                    var datesTodayC = room.Dates.Where(d => d.Begin >= untilC && d.End <= untilC2).ToList();

                    AnzahlBelegungenC += datesTodayC.Count;

                    foreach (var date in datesTodayC)
                    {
                        GesamtdauerC += (int)(date.End - date.Begin).TotalMinutes;
                    }



                    var dayStatistics = new RoomStatisticsModel();
                    dayStatistics.Date = untilC.ToShortDateString();
                    dayStatistics.TotalMinutes = GesamtdauerC;
                    dayStatistics.DateCount = AnzahlBelegungenC;

                    dayStatistics.Wochentag = untilC.ToString("dddd");
                    dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauerC;

                    if ((int)fromC.DayOfWeek < 6 & (int)fromC.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                    {
                        dayStatistics.Werktag = true;
                    }

                    responeC.Statistics.Add(dayStatistics);

                }
                return View(responeC);

            }



            else
            {



                // jetzt das Modell für die Antwort aufbauen
                var respone = new RoomReportResponseModel();
                respone.Room = room;

                //Übergabe der Werte aus dem RoomReportRequestModel 
                respone.fromCalendarTransport = model.fromCalendar;
                respone.untilCalendarTransport = model.untilCalendar;

                //Definierung Variablen zur Berechnung der Auslastung
                double TagDauer = 930;
                Int32 monat = 1;
                Int32 jahr = 2016;
                //Variable aus Übergabe Formular definieren          
                int Zeitraum = Int32.Parse(Days);
                respone.DayAmountValue = Zeitraum;

                //werden später noch benötigt für aktuellen Monat
                int remdayslastmonth = 0;
                int day = 0;
                int remdays = 0;

                //Vergangenheit + aktueller Monat
                if (Zeitraum >= 0)
                {
                    //Bedingung zur Bestimmung des Monatsendes
                    if (Zeitraum == 31)
                    {
                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        if (DateTime.Today.Month == 1)
                        {
                            monat = 12;
                            jahr = DateTime.Today.Year - 1;
                        }
                        else
                        {
                            monat = DateTime.Today.Month - 1;
                            jahr = DateTime.Today.Year;
                        }
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);
                    }



                    //aktueller Monat (1.- Ende)
                    //Deklarierungen oben!

                    if (Zeitraum == 311)
                    {
                        // Berechnung remdays (verbleibende Tage diesen Monat)
                        monat = DateTime.Today.Month;
                        jahr = DateTime.Today.Year;
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);

                        DateTime d = DateTime.Today;
                        day = d.Day;
                        remdays = Zeitraum - day + 1;

                        //notwendig?:
                        //DateTime firstdayDummy = DateTime.Today.AddDays(-day);
                        //int firstday = firstdayDummy.Day;

                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        if (DateTime.Today.Month == 1)
                        {
                            monat = 12;
                            jahr = DateTime.Today.Year - 1;
                        }
                        else
                        {
                            monat = DateTime.Today.Month - 1;
                            jahr = DateTime.Today.Year;
                        }
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);

                        //Verbleibende Tage des Tages vor genau einem Monat bis zum ersten Tag des aktuellen Monats
                        DateTime e = DateTime.Today;
                        day = e.Day;
                        remdayslastmonth = Zeitraum - day + 1;
                    };

                    //Zeitraum = Zeitraum - 1;

                    for (int i = 0; i <= Zeitraum; i++)
                    {
                        int AnzahlBelegungen = 0;
                        int Gesamtdauer = 0;

                        //langfristiges Ziel: from, until variabel gestalten. 
                        //Aber zuerst aktuellen Monat berechnen
                        //aktueller Monat
                        var thisMonth = DateTime.Today.Month;

                        //Start und Endtag des Monats berechnen
                        //daraus muss der Zeitraum angepasst werden



                        //Hardgecodet ein Zeitraum von einer Woche, der Betrachtet wird
                        var from = DateTime.Today.AddDays(-Zeitraum + i + remdayslastmonth);
                        var until = DateTime.Today.AddDays(-Zeitraum + i + 1 + remdays);

                        // die Kennzahlen für jeden Tag berechnen
                        // Alle Belegnungen im Zeitraum
                        var datesToday = room.Dates.Where(d => d.Begin >= from && d.End <= until).ToList();

                        AnzahlBelegungen += datesToday.Count;

                        foreach (var date in datesToday)
                        {
                            Gesamtdauer += (int)(date.End - date.Begin).TotalMinutes;
                        }

                        var dayStatistics = new RoomStatisticsModel();

                        dayStatistics.Day = from.Date;
                        dayStatistics.Date = from.ToShortDateString();
                        dayStatistics.TotalMinutes = Gesamtdauer;
                        dayStatistics.DateCount = AnzahlBelegungen;

                        //Den Wochentag bestimmen
                        dayStatistics.Wochentag = from.ToString("dddd");
                        dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauer;

                        if ((int)from.DayOfWeek < 6 & (int)from.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                        {
                            dayStatistics.Werktag = true;
                        }

                        respone.Statistics.Add(dayStatistics);

                    }
                    // die Kennzahlen für jeden Tag berechnen
                    return View(respone);
                }
                else
                {
                    //Bedingung zur Bestimmung des Monatsendes
                    if (Zeitraum == -31)
                    {
                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        monat = DateTime.Today.Month;
                        jahr = DateTime.Today.Year;
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = -DateTime.DaysInMonth(jahr, monat);
                    }

                    for (int i = 0; i >= Zeitraum; i--)
                    {
                        int AnzahlBelegungen = 0;
                        int Gesamtdauer = 0;

                        //Hardgecodet ein Zeitraum von einer Woche, der Betrachtet wird
                        var from = DateTime.Today.AddDays(-Zeitraum + i);
                        var until = DateTime.Today.AddDays(-Zeitraum + i + 1);
                        // die Kennzahlen für jeden Tag berechnen

                        // Alle Belegnungen im Zeitraum
                        var datesToday = room.Dates.Where(d => d.Begin >= from && d.End <= until).ToList();

                        AnzahlBelegungen += datesToday.Count;

                        foreach (var date in datesToday)
                        {
                            Gesamtdauer += (int)(date.End - date.Begin).TotalMinutes;
                        }

                        var dayStatistics = new RoomStatisticsModel();

                        dayStatistics.Day = from.Date;
                        dayStatistics.Date = from.ToShortDateString();
                        dayStatistics.TotalMinutes = Gesamtdauer;
                        dayStatistics.DateCount = AnzahlBelegungen;
                        //Den Wochentag bestimmen
                        dayStatistics.Wochentag = from.ToString("dddd");
                        dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauer;

                        if ((int)from.DayOfWeek < 6 & (int)from.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                        {
                            dayStatistics.Werktag = true;
                        }

                        respone.Statistics.Add(dayStatistics);

                    }
                    // die Kennzahlen für jeden Tag berechnen
                    return View(respone);
                }

            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="DaysValue"></param>
        /// <param name="fromCalendar"></param>
        /// <param name="untilCalendar"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTestData(string roomId, int DaysValue, string fromCalendar, string untilCalendar)
        {
            var data = new FlotSeriesModel();

            // Code von oben kopieren, um die Werte neu zu erstellen
            var room = Db.Rooms.SingleOrDefault(r => r.Number.Equals(roomId));

            //BERECHNUNG DER RAUMDATEN

            //Wenn Daten in beide Kalender eingegeben worden sind, hat dies Vorrang, wenn nur in eins oder keins etwas eingetragen wurde, passiert hier nichts
            if (fromCalendar != "" && untilCalendar != "")
            {
                DateTime fromC = Convert.ToDateTime(fromCalendar);
                DateTime untilC = Convert.ToDateTime(untilCalendar);
                DateTime untilC2 = Convert.ToDateTime(fromCalendar);

                int Zeitraum2 = (untilC - fromC).Days;
                if (Zeitraum2 < 0)
                {
                    Zeitraum2 = Zeitraum2 * (-1);
                }

                var responeC = new RoomReportResponseModel();
                responeC.Room = room;

                for (int n = 0; n <= Zeitraum2; n++)
                {
                    untilC = fromC.AddDays(n);
                    untilC2 = fromC.AddDays(n + 1);

                    double TagDauerC = 930;
                    int AnzahlBelegungenC = 0;
                    int GesamtdauerC = 0;
                    var datesTodayC = room.Dates.Where(d => d.Begin >= untilC && d.End <= untilC2).ToList();

                    AnzahlBelegungenC += datesTodayC.Count;

                    foreach (var date in datesTodayC)
                    {
                        GesamtdauerC += (int)(date.End - date.Begin).TotalMinutes;
                    }



                    var dayStatistics = new RoomStatisticsModel();
                    dayStatistics.Date = untilC.ToShortDateString();
                    dayStatistics.TotalMinutes = GesamtdauerC;
                    dayStatistics.DateCount = AnzahlBelegungenC;

                    dayStatistics.Wochentag = untilC.ToString("dddd");
                    dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauerC;

                    if ((int)fromC.DayOfWeek < 6 & (int)fromC.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                    {
                        dayStatistics.Werktag = true;
                    }

                    responeC.Statistics.Add(dayStatistics);

                }
                data.label = "Hallo";

                data.data = new List<List<string>>();
                var p = new List<string>();

                foreach (var value in responeC.Statistics)
                {
                    p = new List<string>();
                    p.Add(value.Date);
                    p.Add(value.TotalMinutes.ToString());
                    data.data.Add(p);
                }

                return Json(data);
            }

            else
            {

                var respone = new RoomReportResponseModel();
                respone.Room = room;


                //Definierung Variablen zur Berechnung der Auslastung
                double TagDauer = 930;
                Int32 monat = 1;
                Int32 jahr = 2016;
                //Variable aus Übergabe Formular definieren          
                int Zeitraum = DaysValue;

                //werden später noch benötigt für aktuellen Monat
                int remdayslastmonth = 0;
                int day = 0;
                int remdays = 0;

                //Vergangenheit + aktueller Monat
                if (Zeitraum >= 0)
                {
                    //Bedingung zur Bestimmung des Monatsendes
                    if (Zeitraum == 31)
                    {
                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        if (DateTime.Today.Month == 1)
                        {
                            monat = 12;
                            jahr = DateTime.Today.Year - 1;
                        }
                        else
                        {
                            monat = DateTime.Today.Month - 1;
                            jahr = DateTime.Today.Year;
                        }
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);
                    }



                    //aktueller Monat (1.- Ende)
                    //Deklarierungen oben!

                    if (Zeitraum == 311)
                    {
                        // Berechnung remdays (verbleibende Tage diesen Monat)
                        monat = DateTime.Today.Month;
                        jahr = DateTime.Today.Year;
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);

                        DateTime d = DateTime.Today;
                        day = d.Day;
                        remdays = Zeitraum - day + 1;

                        //notwendig?:
                        //DateTime firstdayDummy = DateTime.Today.AddDays(-day);
                        //int firstday = firstdayDummy.Day;

                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        if (DateTime.Today.Month == 1)
                        {
                            monat = 12;
                            jahr = DateTime.Today.Year - 1;
                        }
                        else
                        {
                            monat = DateTime.Today.Month - 1;
                            jahr = DateTime.Today.Year;
                        }
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = DateTime.DaysInMonth(jahr, monat);

                        //Verbleibende Tage des Tages vor genau einem Monat bis zum ersten Tag des aktuellen Monats
                        DateTime e = DateTime.Today;
                        day = e.Day;
                        remdayslastmonth = Zeitraum - day + 1;
                    };

                    //Zeitraum = Zeitraum - 1;
                    for (int i = 0; i <= Zeitraum; i++)
                    {
                        int AnzahlBelegungen = 0;
                        int Gesamtdauer = 0;

                        //langfristiges Ziel: from, until variabel gestalten. 
                        //Aber zuerst aktuellen Monat berechnen
                        //aktueller Monat
                        var thisMonth = DateTime.Today.Month;

                        //Start und Endtag des Monats berechnen
                        //daraus muss der Zeitraum angepasst werden



                        //Hardgecodet ein Zeitraum von einer Woche, der Betrachtet wird
                        var from = DateTime.Today.AddDays(-Zeitraum + i + remdayslastmonth);
                        var until = DateTime.Today.AddDays(-Zeitraum + i + 1 + remdays);

                        // die Kennzahlen für jeden Tag berechnen
                        // Alle Belegnungen im Zeitraum
                        var datesToday = room.Dates.Where(d => d.Begin >= from && d.End <= until).ToList();

                        AnzahlBelegungen += datesToday.Count;

                        foreach (var date in datesToday)
                        {
                            Gesamtdauer += (int)(date.End - date.Begin).TotalMinutes;
                        }

                        var dayStatistics = new RoomStatisticsModel();

                        dayStatistics.Day = from.Date;
                        dayStatistics.Date = from.ToShortDateString();
                        dayStatistics.TotalMinutes = Gesamtdauer;
                        dayStatistics.DateCount = AnzahlBelegungen;

                        //Den Wochentag bestimmen
                        dayStatistics.Wochentag = from.ToString("dddd");
                        dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauer;

                        if ((int)from.DayOfWeek < 6 & (int)from.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                        {
                            dayStatistics.Werktag = true;
                        }

                        respone.Statistics.Add(dayStatistics);

                    }
                    // die Kennzahlen für jeden Tag berechnen
                    // OH20170105: Hier fehlt ein return
                    // Das wäre intuitiv, erzeugt aber einen Fehler: return (respone);
                    // welche Daten wollen Sie denn zurückgeben?
                    // Workaround: nix (null) zurückgeben
                    //return (null);
                }
                else
                {
                    //Bedingung zur Bestimmung des Monatsendes
                    if (Zeitraum == -31)
                    {
                        //Bestimmung der Eingabewerte für die Monatslänge, Abziehen jeweils einer Einheit, um die Länge des letzten Monats zu erhalten, es wird ja der zurückliegende betrachtet
                        monat = DateTime.Today.Month;
                        jahr = DateTime.Today.Year;
                        //Zeitraum als verbleibende Länge des Monats festlegen, so dass nur bis zum gleichen Datum gegangen wird
                        Zeitraum = -DateTime.DaysInMonth(jahr, monat);
                    }

                    for (int i = 0; i >= Zeitraum; i--)
                    {
                        int AnzahlBelegungen = 0;
                        int Gesamtdauer = 0;

                        //Hardgecodet ein Zeitraum von einer Woche, der Betrachtet wird
                        var from = DateTime.Today.AddDays(-Zeitraum + i);
                        var until = DateTime.Today.AddDays(-Zeitraum + i + 1);
                        // die Kennzahlen für jeden Tag berechnen

                        // Alle Belegnungen im Zeitraum
                        var datesToday = room.Dates.Where(d => d.Begin >= from && d.End <= until).ToList();

                        AnzahlBelegungen += datesToday.Count;

                        foreach (var date in datesToday)
                        {
                            Gesamtdauer += (int)(date.End - date.Begin).TotalMinutes;
                        }

                        var dayStatistics = new RoomStatisticsModel();

                        dayStatistics.Day = from.Date;
                        dayStatistics.Date = from.ToShortDateString();
                        dayStatistics.TotalMinutes = Gesamtdauer;
                        dayStatistics.DateCount = AnzahlBelegungen;
                        //Den Wochentag bestimmen
                        dayStatistics.Wochentag = from.ToString("dddd");
                        dayStatistics.Auslastung = dayStatistics.TotalMinutes / TagDauer;

                        if ((int)from.DayOfWeek < 6 & (int)from.DayOfWeek != 0 || dayStatistics.TotalMinutes > 0)
                        {
                            dayStatistics.Werktag = true;
                        }

                        respone.Statistics.Add(dayStatistics);

                    }

                    data.label = "Hallo";

                    data.data = new List<List<string>>();
                    var p = new List<string>();

                    foreach (var value in respone.Statistics)
                    {
                        p = new List<string>();
                        p.Add(value.Date);
                        p.Add(value.TotalMinutes.ToString());
                        data.data.Add(p);
                    }
                    //return Json(data);
                    // OH20170105: Dieses return bezieht sich nur auf den else Fall
                }

                return Json(data);

                // OH20150105: hier fehlt ein return
                // da der else Ast ein return hat gibt fehlt offenbar eines im if ast
            }
        }
    }
}

