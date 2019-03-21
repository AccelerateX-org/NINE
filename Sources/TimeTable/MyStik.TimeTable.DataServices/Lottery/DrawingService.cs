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
        }

        public void InitLotPots()
        {
            var bookingService = new BookingService(db);

            // Pro Kurs die Lostöpfe anlegen
            foreach (var course in Courses)
            {
                var bookingLists = bookingService.GetBookingLists(course.Occurrence.Id);

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
            var round = 0;

            while (DrawAllPots() > 0)
            {
                round++;
            }

            // jetzt noch die, die bisher komplett leer ausgegangen sind
            TryToHelpJinx();
            round++;

            // zum Schluss bestmöglich auffüllem
            while (TryToFillUp() > 0)
            {
                round++;
            }


            return round;
        }


        private int DrawAllPots()
        {
            // Zuerst nachsehen, ob Wünsche offen sind
            foreach (var game in Games)
            {
                if (game.Seats.Count >= game.CoursesWanted)
                {
                    // alle Lose ungültig machen => hat schon alles
                    foreach (var lot in game.Lots)
                    {
                        lot.IsValid = false;
                        lot.IsTouched = true;
                        lot.Message = "Die Wünsche konnten erfüllt werden, Los wird nicht mehr gebraucht";
                    }
                }
            }


            foreach (var lotPot in LotPots.OrderByDescending(x => x.BookingRank))
            {
                // Ziehen
                ExecuteDrawing(lotPot);


                // Alle erfolgreichen Tickets ansehen => nicht mehr auf Warteliste
                foreach (var lot in lotPot.Lots.Where(x => !x.Subscription.OnWaitingList))
                {
                    // das zugehörige Spiel suchen
                    var game = Games.SingleOrDefault(x => x.UserId.Equals(lot.Subscription.UserId));
                    var studentLots = game.Lots.OrderBy(x => x.Priority).Take(game.CoursesWanted).ToList();

                    // Student ist glücklich, wenn er seine Prio 1 bis n bekommen hat
                    var isHappy = !studentLots.Any(x => x.Subscription.OnWaitingList);

                    if (isHappy)
                    {
                        // jetzt können wir alle seine anderen Lose auf "invalid" setzen
                        foreach (var drawingLot in studentLots.Where(x => x.Subscription.OnWaitingList))
                        {
                            drawingLot.IsValid = false;
                            drawingLot.IsTouched = true;
                            drawingLot.Message = "Die Wünsche konnten erfüllt werden, Los wird nicht mehr gebraucht";
                        }
                    }
                }
            }

            // jetzt alle Plätze auflösen, die zu viel vergeben wurden
            var nReject = 0;
            foreach (var game in Games)
            {
                var studentLots =game.Lots.OrderBy(x => x.Priority).ToList();

                // Student ist glücklich, wenn er die Anzahl der gewünschten Plätze erhalten hat
                var isHappy = studentLots.Count(x => !x.Subscription.OnWaitingList) >= game.CoursesWanted;

                if (isHappy)
                {
                    var i = 0;
                    // jetzt können wir alle seine anderen Lose auf "invalid" setzen
                    foreach (var drawingLot in studentLots)
                    {
                        if (!drawingLot.Subscription.OnWaitingList)
                        {
                            i++;
                            if (i > game.CoursesWanted)
                            {
                                drawingLot.IsValid = false;
                                drawingLot.IsTouched = true;
                                drawingLot.Subscription.OnWaitingList = true; // Zurück auf die Warteliste
                                drawingLot.Message =
                                    "Die Wünsche konnten erfüllt werden, Los wird nicht mehr gebraucht";

                                nReject++;
                            }
                        }
                    }
                }

            }
            return nReject;
        }

        private void ExecuteDrawing(DrawingLotPot lotPot)
        {
            // Anzahl der verfügbaren Plätze
            // Ausgangslage - aller bereits gezogener Plätze
            var nCapacity = lotPot.SeatsAvailable - lotPot.Lots.Count(x => !x.Subscription.OnWaitingList);

            // alle gültigen Lose, die noch auf der Warteliste sind
            var allLots = lotPot.Lots.Where(x => x.IsValid && x.Subscription.OnWaitingList).GroupBy(x => x.Priority)
                .OrderBy(x => x.Key);

            foreach (var lotGroup in allLots)
            {
                var lotList = lotGroup.ToList();

                // passt die ganze Liste rein?
                if (lotList.Count <= nCapacity)
                {

                    foreach (var lot in lotList)
                    {
                        lot.Subscription.OnWaitingList = false;
                        lot.IsTouched = true;
                        lot.Message = "Erfolg";
                    }

                    nCapacity = nCapacity - lotList.Count;

                }
                else
                {
                    lotList.Shuffle();

                    var winner = lotList.Take(nCapacity);
                    foreach (var lot in winner)
                    {
                        lot.Subscription.OnWaitingList = false;
                        lot.IsTouched = true;
                        lot.Message = "Erfolg";
                    }

                    break;
                }

            }
        }

        private void TryToHelpJinx()
        {
            foreach (var game in Games)
            {
                var studentLots = game.Lots.OrderBy(x => x.Priority).ToList();

                // Student ist Ober-Pechvogel, wenn er gar nichts erhalten hat => zuerst 1 Kurs
                // gar nichts: Keinen Platz und auch kein Los auf der Warteliste
                var isJinx = !game.Seats.Any() && studentLots.All(x => x.IsValid && x.Subscription.OnWaitingList);

                if (isJinx)
                {
                    // Suche einen Kurs, der noch freie Plätze hat und in dem er noch nicht drin ist
                    var availableCourse = LotPots.Where(x =>
                        !x.Course.Occurrence.Subscriptions.Any(s => s.UserId.Equals(game.UserId)) &&
                        x.RemainingSeats > 0).OrderByDescending(x => x.RemainingSeats).FirstOrDefault();

                    if (availableCourse == null)
                    {
                        game.Message = "Im gesamten Wahlverfahren stehen keine freien Plätze mehr zur Verfügung.";
                    }
                    else
                    {
                        if (game.AcceptDefault)
                        {

                            // eine neue Eintragung
                            var subscription = new OccurrenceSubscription();
                            subscription.TimeStamp = DateTime.Now;
                            subscription.UserId = game.UserId;
                            subscription.OnWaitingList = false;
                            subscription.Priority = 0;
                            subscription.Occurrence = availableCourse.Course.Occurrence;

                            // Die Subscription hinzufügen
                            availableCourse.Course.Occurrence.Subscriptions.Add(subscription);

                            // Die Bookinglist aktualisieren
                            availableCourse.BookingList.Bookings.Add(new Booking.Data.Booking
                            {
                                Student = game.Student,
                                Subscription = subscription
                            });

                            var lot = new DrawingLot();
                            lot.IsTouched = true;
                            lot.IsValid = true;
                            lot.Message = "Mindestens mal eine Lehrveranstaltung für den Pechvogel";
                            lot.Subscription = subscription;
                            lot.Course = availableCourse.Course;

                            game.Lots.Add(lot);

                            availableCourse.Lots.Add(lot);

                            /*
                            var lotPot = LotPots.SingleOrDefault(x => x.Course.Id == availableCourse.Course.Id);
                            lotPot.Lots.Add(lot);
                            */
                        }
                        else
                        {
                            game.Message =
                                "Es hätte noch einen Platz in einer anderen Lehrveranstaltung gegeben.";
                        }
                    }
                }
            }
        }

        private int TryToFillUp()
        {
            var nSuccess = 0;
            foreach (var game in Games)
            {
                var studentLots = game.Lots.OrderBy(x => x.Priority).ToList();

                // Hat Student alle Wünsche erfüllt bekommen
                var isJinx = (studentLots.Count(x => !x.Subscription.OnWaitingList) + game.Seats.Count) < game.CoursesWanted;

                if (isJinx)
                {
                    // Suche einen Kurs, der noch freie Plätze hat und in dem er noch nicht drin ist
                    var availableCourse = LotPots.Where(x =>
                        !x.Course.Occurrence.Subscriptions.Any(s => s.UserId.Equals(game.UserId)) &&
                        x.RemainingSeats > 0).OrderByDescending(x => x.RemainingSeats).FirstOrDefault();

                    if (availableCourse == null)
                    {
                        game.Message = "Im gesamten Wahlverfahren stehen keine freien Plätze mehr zur Verfügung.";
                    }
                    else
                    {
                        if (game.AcceptDefault)
                        {
                            // eine neue Eintragung
                            var subscription = new OccurrenceSubscription();
                            subscription.TimeStamp = DateTime.Now;
                            subscription.UserId = game.UserId;
                            subscription.OnWaitingList = false;
                            subscription.Priority = 0;
                            subscription.Occurrence = availableCourse.Course.Occurrence;

                            // Die Subscription hinzufügen
                            availableCourse.Course.Occurrence.Subscriptions.Add(subscription);

                            // Die Bookinglist aktualisieren
                            availableCourse.BookingList.Bookings.Add(new Booking.Data.Booking
                            {
                                Student = game.Student,
                                Subscription = subscription
                            });

                            var lot = new DrawingLot();
                            lot.IsTouched = true;
                            lot.IsValid = true;
                            lot.Message = "Trägt zur Erfüllung der Wünsche bei";
                            lot.Subscription = subscription;
                            lot.Course = availableCourse.Course;

                            game.Lots.Add(lot);

                            availableCourse.Lots.Add(lot);

                            /*
                            var lotPot = LotPots.SingleOrDefault(x => x.Course.Id == availableCourse.Course.Id);
                            lotPot.Lots.Add(lot);
                            */

                            nSuccess++;
                        }
                        else
                        {
                            game.Message =
                                "Es hätte noch einen Platz in einer anderen Lehrveranstaltung gegeben.";
                        }
                    }
                }
            }

            return nSuccess;
        }
    }
}
