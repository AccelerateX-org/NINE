using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Controllers
{
    public class AnalyseController : Controller
    {
        // GET: Quest/Analyse
        public ActionResult Index()
        {
            // 1. Verbindung zur Datenbank
            var db = new QuestDbContext();

            // 2. Daten abfragen
            var model = db.Users.ToList();


            return View(model);
        }


        public ActionResult DownloadLogData()
        {
            var db = new QuestDbContext();

            // 2. Daten abfragen
            var model = db.Users.ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Geschlecht;Alter;Hochschulzugang");

            var firstUser = model.FirstOrDefault();
            if (firstUser != null)
            {
                foreach (var log in firstUser.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    writer.Write(";{0}_{1}", log.Question.Category.Reihenfolge, log.Question.Reihenfolge);
                }
            }

            writer.Write(Environment.NewLine);

            foreach (var user in model)
            {
                writer.Write("{0};{1};{2}",
                    user.Geschlecht, user.Altersgruppe, user.Hochschulzugangsberechtigung);


                foreach (var log in user.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    if (log.Answer != null)
                    {
                        // richtig = 1
                        // falsch = -1
                        writer.Write(";{0}", log.Answer.IsCorrect ? 1 : -1);
                    }
                    else
                    {
                        // unbeantwirtet = 0
                        writer.Write(";0");
                    }
                }


                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("WiQuestLog_");
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
            
        }
    }
}