using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ScriptOrdersController : BaseController
    {
        // GET: ScriptOrders
        public ActionResult Index(Guid? id)
        {
            var org = GetMyOrganisation();
            var sem = SemesterService.GetSemester(id);


            var model = new OrganiserViewModel();

            model.Organiser = org;
            model.Semester = sem;
            model.PreviousSemester = SemesterService.GetPreviousSemester(sem);
            model.NextSemester = SemesterService.GetNextSemester(sem);


            model.OrderPeriods = Db.OrderPeriods.Where(x => x.Organiser.Id == org.Id && x.Semester.Id == sem.Id)
                .OrderBy(x => x.Begin).ToList();

            return View(model);
        }

        public ActionResult Create(Guid semId)
        {
            var sem = SemesterService.GetSemester(semId);


            var model = new OrderPeriodCreatetModel();

            model.Semester = sem;
            model.SemesterId = sem.Id;
            model.Title = $"Skriptbestellung für {sem.Name}";
            model.Description = String.Empty;
            model.Begin = DateTime.Today.AddDays(1).ToShortDateString();
            model.End = DateTime.Today.AddDays(15).ToShortDateString();


            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        [HttpPost]
        public ActionResult Create(OrderPeriodCreatetModel model)
        {
            var sem = SemesterService.GetSemester(model.SemesterId);
            var org = GetMyOrganisation();


            var begin = DateTime.Parse(model.Begin);
            var end = DateTime.Parse(model.End);


            var period = new OrderPeriod
            {
                Begin = begin,
                End = end,
                Semester = sem,
                Organiser = org,
                Title = model.Title,
                Description = model.Description
            };

            Db.OrderPeriods.Add(period);
            Db.SaveChanges();

            return RedirectToAction("Index", new {id = sem.Id});
        }



        public ActionResult Order(Guid id)
        {
            var model = CreateOrderModel(id);

            return View(model);
        }


        public ActionResult ExecuteOrder(Guid id)
        {
            var period = Db.OrderPeriods.SingleOrDefault(x => x.Id == id);

            var allBaskets = period.Baskets.ToList();

            // Liste nach "voll" und "leer" trennen
            var emptyBaskets = allBaskets.Where(x => !x.Orders.Any()).ToList();
            var filledBaskets = allBaskets.Where(x => x.Orders.Any()).ToList();


            // Vergabe der Bestellnummern nur für gefüllte Körbe
            filledBaskets.Shuffle();

            var i = 0;
            foreach (var basket in filledBaskets)
            {
                i++;
                basket.OrderNumber = i;
            }

            period.LastProcessed = DateTime.Now;

            // Bestellnummern speichern
            Db.SaveChanges();

            return RedirectToAction("Order", new {id = id});
        }

        public ActionResult SendMails(Guid id)
        {
            var model = CreateOrderModel(id);

            var deliveryModel = new MailDeliverSummaryReportModel();
            var errorCount = 0;

            var user = GetCurrentUser();

            foreach (var person in model.Persons)
            {
                try
                {
                    if (person.User != null)
                    {

                        var orderMailModel = new ScriptOrderMailModel
                        {
                            User = person.User,
                            Basket = person.Basket,
                        };



                        if (person.Basket.Orders.Any())
                        {
                            // Abholzettel
                            new MailController().ScriptOrderTicket(orderMailModel).Deliver();

                            deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                            {
                                User = person.User,
                                DeliverySuccessful = true,
                            });
                        }
                        else
                        {
                            // Mitteilung
                            new MailController().ScriptOrderNotification(orderMailModel).Deliver();

                            deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                            {
                                User = person.User,
                                DeliverySuccessful = true,
                            });
                        }
                    }
                    else
                    {
                        // nix machen, auch kein Fehler
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    var strError = string.Format("Fehler bei Versand. Grund: {0}. Mailadresse wird auf ungültig gesetzt.", ex.Message);

                    deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                    {
                        User = person.User,
                        DeliverySuccessful = false,
                        ErrorMessage = strError
                    });
                }

            }


            // Sendebericht
            // Das Mail-Model aufbauen
            var mailModel = new UserMailModel
            {
                CustomSubject = "Skriptendruck Versandbericht",
                User = user,
                SenderUser = user,
                CustomBody = "Liste der Besteller:innen, welche den Abholzettel bekommen haben.",
                IsImportant = true,
                ListName = String.Empty,
                IsDistributionList = false
            };


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Name;Vorname;E-Mail;Versand;Bemerkung");

            writer.Write(Environment.NewLine);

            foreach (var delivery in deliveryModel.Deliveries)
            {
                if (delivery.DeliverySuccessful)
                {
                    writer.Write("{0};{1};;{2};",
                        delivery.User.LastName, delivery.User.FirstName,
                        delivery.DeliverySuccessful ? "OK" : "FEHLER");
                }
                else
                {
                    writer.Write("{0};{1};{2};{3};{4}",
                        delivery.User.LastName, delivery.User.FirstName, delivery.User.Email,
                        delivery.DeliverySuccessful ? "OK" : "FEHLER",
                        delivery.ErrorMessage);

                }
                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Versandbericht");
            sb.Append(".csv");

            mailModel.Attachments.Add(new CustomMailAttachtmentModel
            {
                FileName = sb.ToString(),
                Bytes = ms.GetBuffer()
            });

            new MailController().GenericMessageMail(mailModel).Deliver();


            return RedirectToAction("Order", new { id = id });
        }


        public FileResult OrderDocument(Guid id)
        {
            var model = CreateOrderModel(id);

            var html = this.RenderViewToString("_ScriptOrderPrintOut", model);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);

            var stream = new MemoryStream();
            pdf.Save(stream, false);

            // Stream zurücksetzen
            stream.Position = 0;

            return File(stream.GetBuffer(), "application/pdf", "SkriptBestellung.pdf");
        }


        public FileResult OrderQuantity(Guid id)
        {
            var model = CreateOrderModel(id);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Titel;Version;Anzahl;URL");

            writer.Write(Environment.NewLine);

            

            foreach (var doc in model.Documents)
            {
                    
                var url = new Uri(
                        this.HttpContext.Request.Url, 
                        Url.Action("GetDocument", "Storage", new {id = doc.Document.Storage.Id})).ToString();


                writer.Write("{0};{1};{2};{3}",
                    doc.Document.Title, doc.Document.Version,
                    doc.Orderers.Count,
                    url);
                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Bestellungen");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        private ScriptOrderDetailsModel CreateOrderModel(Guid id)
        {
            var period = Db.OrderPeriods.SingleOrDefault(x => x.Id == id);


            var studentService = new StudentService(Db);

            var model = new ScriptOrderDetailsModel();
            model.Period = period;

            // Alle Baskets durchgehen
            foreach (var basket in period.Baskets)
            {
                var user = GetUser(basket.UserId);

                var student = studentService.GetCurrentStudent(basket.UserId);

                var personModel = new ScriptOrderPersonModel();
                personModel.User = user;
                personModel.Student = student;
                personModel.Basket = basket;

                model.Persons.Add(personModel);


                // Nach Dokumenten aufteilen
                foreach (var scriptOrder in basket.Orders)
                {
                    var doc = model.Documents.SingleOrDefault(x => x.Document.Id == scriptOrder.ScriptDocument.Id);

                    if (doc == null)
                    {
                        doc = new ScriptOrderDocumentModel();
                        doc.Document = scriptOrder.ScriptDocument;

                        model.Documents.Add(doc);
                    }

                    doc.Orderers.Add(personModel);
                }
            }

            return model;

        }

    }
}
