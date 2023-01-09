using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking;
using MyStik.TimeTable.DataServices.Lottery.Data;

namespace MyStik.TimeTable.DataServices.Lottery
{
    public class DrawingService
    {
        private TimeTableDbContext db;
        public TimeTable.Data.Lottery Lottery { get; }
        public List<Course> Courses { get; }

        /// <summary>
        /// Lostöpfe
        /// </summary>
        public List<DrawingLotPot> LotPots { get; }

        /// <summary>
        /// Studierenden
        /// </summary>
        public List<DrawingGame> Games { get; }

        public List<DrawingMessage> Messages { get; }


        public DrawingService(TimeTableDbContext db, Guid id)
        {
            this.db = db;
            Lottery = db.Lotteries.SingleOrDefault(l => l.Id == id);

            Courses = new List<Course>();

            if (Lottery != null)
            {
                Courses.AddRange(
                    Lottery.Occurrences.Select(
                        occurrence => db.Activities.OfType<Course>().SingleOrDefault(
                            c => c.Occurrence.Id == occurrence.Id)).Where(course => course != null));
            }

            LotPots = new List<DrawingLotPot>();

            Games = new List<DrawingGame>();

            Messages = new List<DrawingMessage>();

            AddMessage(null, null, "Ziehung angelegt");
        }

        public void InitLotPots()
        {
            AddMessage(null, null, "Beginn Initialisierung der Lostöpfe");
            AddMessage(null, null, $"Anzahl Lehrveranstaltungen: {Courses.Count}");

            var bookingService = new BookingService(db);

            // Pro Kurs die Lostöpfe anlegen
            foreach (var course in Courses)
            {
                var bookingLists = bookingService.GetBookingLists(course.Occurrence.Id);
                AddMessage(course, null, $"Anzahl Buchungslisten: {bookingLists.Count}");

                foreach (var bookingList in bookingLists)
                {
                    var lotPot = new DrawingLotPot();

                    lotPot.BookingList = bookingList;
                    lotPot.Course = course;

                    // Alle Subscriptions durchgehen
                    // Sichern des Status vor der Verlosung!

                    foreach (var booking in bookingList.Bookings)
                    {
                        var subscription = booking.Subscription;
                        var game =
                            Games.FirstOrDefault(x => x.UserId.Equals(subscription.UserId));

                        if (game == null)
                        {
                            var student = db.Students.Where(x => x.UserId.Equals(subscription.UserId))
                                .OrderByDescending(x => x.Created).FirstOrDefault();
                            var lotteryGame = db.LotteryGames.Where(x =>
                                    x.Lottery.Id == Lottery.Id && 
                                    x.UserId.Equals(subscription.UserId))
                                    .OrderByDescending(x => x.Created).FirstOrDefault();

                            // Eintragung wurde außerhalb des Wahlverfahrens gemacht => Standard aufnehmen, damit Studierende an die Wahl dann auch rankommen
                            if (lotteryGame == null)
                            {
                                lotteryGame = new LotteryGame();
                                lotteryGame.Lottery = Lottery;
                                lotteryGame.UserId = subscription.UserId;
                                lotteryGame.AcceptDefault = false;
                                lotteryGame.CoursesWanted = Lottery.MaxConfirm;
                                lotteryGame.Created = DateTime.Now;
                                lotteryGame.LastChange = DateTime.Now;  // bisher nicht angegeben

                                Lottery.Games.Add(lotteryGame);
                            }

                            game = new DrawingGame();
                            game.UserId = subscription.UserId;
                            game.Student = student;
                            game.LotteryGame = lotteryGame;
                            game.Lottery = Lottery;

                            Games.Add(game);
                        }

                        // Trennung von bereits erhaltenen Plätzen und Losen
                        if (subscription.OnWaitingList == false)
                        {
                            var seat = new DrawingSeat();
                            seat.Course = course;
                            seat.Subscription = subscription;

                            game.Seats.Add(seat);

                            AddMessage(course, subscription, "Vorhandener Platz");
                        }
                        else
                        {
                            var drawingLot = new DrawingLot();
                            drawingLot.IsValid = true; // Am Beginn ist das Ticket gültig
                            drawingLot.IsTouched = false;
                            drawingLot.Course = course;
                            drawingLot.Subscription = subscription;

                            game.Lots.Add(drawingLot);

                            lotPot.Lots.Add(drawingLot);

                            AddMessage(course, subscription, "Ein Los für die Verlosung erhalten");
                        }
                    }

                    LotPots.Add(lotPot);
                }



            }
        }

        public double OccupancyRate
        {
            get
            {
                if (!LotPots.Any())
                    return -1;

                var totalRate = 0.0;

                foreach (var lotPot in LotPots)
                {
                    totalRate += lotPot.OccupancyRate;
                }

                return totalRate / LotPots.Count;
            }
        }

        public double SuccessRate
        {
            get
            {
                if (!Games.Any())
                    return 0;

                return Games.Count(x => x.Seats.Any()) / (double)Games.Count;
            }
        }

       

        public int ExecuteDrawing()
        {
            // Alle Nachrichten auf null setzen
            foreach (var game in Games)
            {
                foreach (var drawingLot in game.Lots)
                {
                    drawingLot.InitMessage();
                }
            }


            // ermittle aus den Parametern der Lottery den zugehörigen Algorithmus
            // starte den Algorithmus

            var drawingAlgorithm = GetAlgorithm();

            var result = drawingAlgorithm.Execute();

            // alle Texte sichern
            foreach (var game in Games)
            {
                foreach (var drawingLot in game.Lots)
                {
                    drawingLot.SaveMessage();
                }
            }


            return result;
        }



        private IDrawingAlgorithm GetAlgorithm()
        {
            if (Lottery.MaxConfirm == 0 && Lottery.MaxExceptionConfirm == 0)
                return new DrawingByMaxWinners(this);

            return new DrawingByPriority(this);
        }




        public void AddMessage(Course course, OccurrenceSubscription subscription, string remark)
        {
            Messages.Add(new DrawingMessage
            {
                TimeStamp = DateTime.Now,
                Course = course,
                Subscription = subscription,
                UserId = subscription?.UserId,
                Remark = remark
            });

        }

        public void AddMessage(string userId, string remark)
        {
            Messages.Add(new DrawingMessage
            {
                TimeStamp = DateTime.Now,
                Course = null,
                Subscription = null,
                UserId = userId,
                Remark = remark
            });

        }

    }
}
