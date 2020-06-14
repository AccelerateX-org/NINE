using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HtmlAgilityPack;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{

    [RoutePrefix("api/v2/apps/mvg")]
    public class MvgController : ApiBaseController
    {

        [Route("schedule/{station}")]
        public IQueryable<MvgScheduleDto> GetSchedule(string station)
        {
            var mvgStation = "Hochschule+M%fcnchen+(Lothstra%dfe)";

            if (station.Equals("Karlstraße"))
            {
                mvgStation = "Ottostra%dfe";
            }
            else if (station.Equals("Pasing"))
            {
                mvgStation = "Avenariusplatz";
            }


            var model = new List<MvgScheduleDto>();

            var url =
                $"http://www.mvg-live.de/ims/dfiStaticAnzeige.svc?haltestelle={mvgStation}&ubahn=checked&bus=checked&tram=checked&sbahn=checked";


            var req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "GET";
            var res = (HttpWebResponse) req.GetResponse();

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
                    var mvv = new MvgScheduleDto
                    {
                        Number = tdNodes[0].InnerText,
                        Destination = ziel,
                        Departure = int.Parse(tdNodes[2].InnerText)
                    };
                    // Das Objekt zur Liste hinzufügen
                    model.Add(mvv);
                }
            }

            return model.AsQueryable();
        }
    }
}