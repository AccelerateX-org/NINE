using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Lottery;
using MyStik.TimeTable.Web.Models;
using Postal;

namespace MyStik.TimeTable.Web.Services
{
    public class LotteryMailService : BaseMailService
    {

        private DrawingService Drawing { get; }

        public LotteryMailService(DrawingService drawing)
        {
            Drawing = drawing;
        }

        public void SendDrawingMails(LotteryDrawing drawing)
        {
            foreach (var game in Drawing.Games)
            {
                var nNeeded = game.CoursesWanted - game.Seats.Count;

                // Versand nur an jene, die noch Plätze brauchen
                if (nNeeded > 0)
                {
                    var email = new LotteryDrawingStudentEmail("LotteryDrawingStudent")
                    {
                        Subject = "[nine] Wahlverfahren " + Drawing.Lottery.Name,
                        Game = game,
                        User = UserService.GetUser(game.Student.UserId),
                        Drawing = drawing
                    };

                    try
                    {
                        if (email.User != null)
                        {
                            EmailService.Send(email);
                            Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                        }
                    }
                    catch (Exception exMail)
                    {
                        Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email,
                            exMail.Message);
                    }
                }
            }
        }


        public void SendLotteryResetMails(LotteryDrawing drawing, OrganiserMember member)
        {
            // alles wird rückgesetzt => jeder bekommt eine Mail
            foreach (var game in Drawing.Games)
            {
                var email = new LotteryDrawingStudentEmail("LotteryResetStudent")
                {
                    Subject = "[nine] Wahlverfahren " + Drawing.Lottery.Name,
                    Game = game,
                    User = UserService.GetUser(game.Student.UserId),
                    Drawing = drawing,
                    Member = member
                };

                try
                {
                    if (email.User != null)
                    {

                        EmailService.Send(email);
                        Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                    }
                }
                catch (Exception exMail)
                {
                    Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email,
                        exMail.Message);
                }
            }
        }

        public void SendLotteryClearedMails(LotteryDrawing drawing, OrganiserMember member)
        {
            foreach (var game in Drawing.Games)
            {
                var nNeeded = game.CoursesWanted - game.Seats.Count;

                // Versand nur an jene, die Plätze auf der Warteliste haben
                if (nNeeded > 0)
                {
                    var email = new LotteryDrawingStudentEmail("LotteryClearedStudent")
                    {
                        Subject = "[nine] Wahlverfahren " + Drawing.Lottery.Name,
                        Game = game,
                        User = UserService.GetUser(game.Student.UserId),
                        Drawing = drawing,
                        Member = member
                    };

                    try
                    {
                        if (email.User != null)
                        {

                            EmailService.Send(email);
                            Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                        }
                    }
                    catch (Exception exMail)
                    {
                        Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email,
                            exMail.Message);
                    }
                }
            }
        }

        public void SendLotteryRemoveMail(LotteryDrawing drawing, OrganiserMember member, Lottery lottery, Course course, OccurrenceSubscription subscription)
        {
            var email = new LotteryDrawingStudentEmail("LotteryRemoveStudent")
            {
                Subject = "[nine] Wahlverfahren " + Drawing.Lottery.Name,
                Game = null,
                User = UserService.GetUser(subscription.UserId),
                Lottery = lottery,
                Drawing = drawing,
                Member = member,
                Course = course,
                Subscription = subscription,
            };

            try
            {
                if (email.User != null)
                {
                    EmailService.Send(email);
                    Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                }
            }
            catch (Exception exMail)
            {
                Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email,
                    exMail.Message);
            }
        }


    }
    }