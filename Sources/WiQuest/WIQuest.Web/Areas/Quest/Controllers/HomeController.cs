using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using WIQuest.Web.Areas.Quest.Services;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // GET: Quest/Home
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Information_Page()
        {
            return View();
        }

        public ActionResult Personal_Data()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Personal_Data(User user)
        {
            // Der Benutzer soll in der Datenbank gespeichert werden
            // 1. Verbindung zur Datenbank
            var db = new QuestDbContext();

            if (ModelState.IsValid)
            {
                // das ist ein neuer User => gib ihm eine Id
                user.Id = Guid.NewGuid();

                // den neuen Benutzer zur Datenbank hinzufügen
                db.Users.Add(user);

                // Änderungen jetzt wirklich in der Datenbank speichern
                db.SaveChanges();

                // jetzt das Quiz starten
                return RedirectToAction("Index", "Quiz", new {id = user.Id});
            }


            // wenn es nicht geklappt hat, dann hat der Benutzer wohl was falsches eingegeben
            // zeige den Dialog noch einmal
            return View();
        }

         public ActionResult Evaluation(Guid id)
        {
            // Alle Logeinträge des Benutzers
            var service = new QuestionService();
            var user = service.GetUser(id);

            var db = new QuestDbContext();

            var logs = db.QuestLogs.Where(l => l.User.Id == user.Id).ToList();
            
            // Endergebnis für den ganzen Test
            var AnzahlFragen = logs.Count();

            var IsCorrect = logs.Count(a => a.Answer != null && a.Answer.IsCorrect);

            var Result = (int)(IsCorrect / (double)AnzahlFragen * 100);

            ViewBag.AnzahlFragen = AnzahlFragen;
            ViewBag.IsCorrect= IsCorrect;
            ViewBag.Result = Result;

  
             // Ergebnis für Mathematik
            var AnzahlMatheFragen = logs.Count(m => m.Question.Category.Name.Equals("Mathematik"));

            var IsCorrectMathe = logs.Count(a => a.Answer != null && a.Answer.IsCorrect && a.Question.Category.Name.Equals("Mathematik") );

            var ResultMathe = (int)(IsCorrectMathe / (double)AnzahlMatheFragen * 100);


            ViewBag.ResultMathe = ResultMathe;

 
             // Ergebnis für Technik
             var AnzahlTechFragen = logs.Count(m => m.Question.Category.Name.Equals("Technik"));

            var IsCorrectTech = logs.Count(a => a.Answer != null && a.Answer.IsCorrect && a.Question.Category.Name.Equals("Technik") );

            var ResultTech = (int)(IsCorrectTech / (double)AnzahlTechFragen * 100);


            ViewBag.ResultTech = ResultTech;

            // Ergebnis für Wirtschaft
             var AnzahlWirtFragen = logs.Count(m => m.Question.Category.Name.Equals("Wirtschaft"));

            var IsCorrectWirt = logs.Count(a => a.Answer != null && a.Answer.IsCorrect && a.Question.Category.Name.Equals("Wirtschaft") );

            var ResultWirt = (int)(IsCorrectWirt / (double)AnzahlWirtFragen * 100);

             ViewBag.ResultWirt = ResultWirt;

            // Ergebnis für Naturwissenschaften
             var AnzahlNaturFragen = logs.Count(m => m.Question.Category.Name.Equals("Naturwissenschaften"));

            var IsCorrectNatur = logs.Count(a => a.Answer != null && a.Answer.IsCorrect && a.Question.Category.Name.Equals("Naturwissenschaften") );

            var ResultNatur = (int)(IsCorrectNatur / (double)AnzahlNaturFragen * 100);


            ViewBag.ResultNatur = ResultNatur;
            return View(logs);
        }

        public FileResult ShowChart(Guid id)
        {
            Chart chart = new Chart();
            chart.Width = 700;
            chart.Height = 300;

            chart.BackColor = Color.FromArgb(211, 223, 240);

            Series series = new Series("Default");
            chart.Series.Add(series);

            chart.ChartAreas.Add("ChartArea1");


            var service = new QuestionService();
            var user = service.GetUser(id);

            var db = new QuestDbContext();

            var logs = db.QuestLogs.Where(l => l.User.Id == user.Id).ToList();
            var categories = logs.Select(d => d.Question.Category).Distinct().ToList();

            double[] yValues = new double[categories.Count()];
            string[] xValues = new string[categories.Count()];

            var i = 0;
            foreach (var category in categories)
            {
                var AnzahlFragen = logs.Count(m => m.Question.Category.Id == category.Id);

                var IsCorrect = logs.Count(a => a.Answer != null && a.Answer.IsCorrect && a.Question.Category.Id == category.Id);

                var Result = IsCorrect / (double)AnzahlFragen * 100;
                
                yValues[i] = Result;
                xValues[i] = category.Name;
                i++;
            }



        // Populate series data
        //double[]    yValues = {65.62, 75.54, 60.45, 34.73, 85.42, 55.9, 63.6, 55.2, 77.1};
        //string[]    xValues = {"France", "Canada", "Germany", "USA", "Italy", "Spain", "Russia", "Sweden", "Japan"};
        chart.Series["Default"].Points.DataBindXY(xValues, yValues);

        // Set radar chart type
        chart.Series["Default"].ChartType = SeriesChartType.Radar;

        // Set radar chart style (Area, Line or Marker)
        chart.Series["Default"]["RadarDrawingStyle"] = "Area";

        // Set circular area drawing style (Circle or Polygon)
        chart.Series["Default"]["AreaDrawingStyle"] = "Polygon";

        // Set labels style (Auto, Horizontal, Circular or Radial)
        chart.Series["Default"]["CircularLabelsStyle"] = "Horizontal";

        // Show as 3D
        chart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;



            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
 }
}