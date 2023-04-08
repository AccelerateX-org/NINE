using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Lottery.Data;
using System;
using System.Linq;

namespace MyStik.TimeTable.DataServices.Lottery
{
    public class DrawingByPriority : IDrawingAlgorithm
    {
        private DrawingService drawingService;

        public DrawingByPriority(DrawingService ds)
        {
            drawingService = ds;
        }

        public int Execute()
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
            drawingService.AddMessage(null, null, "Starte Ziehung");
            // Zuerst nachsehen, ob Wünsche offen sind
            foreach (var game in drawingService.Games)
            {
                if (game.Seats.Count >= game.CoursesWanted)
                {
                    // alle Lose ungültig machen => hat schon alles
                    foreach (var lot in game.Lots)
                    {
                        lot.IsValid = false;
                        lot.IsTouched = true;
                        lot.AddMessage("Die Wünsche konnten erfüllt werden, dieses Los wird nicht mehr gebraucht");

                        drawingService.AddMessage(lot.Course, lot.Subscription, "Die Wünsche konnten erfüllt werden, dieses Los wird nicht mehr gebraucht");
                    }
                }
            }


            foreach (var lotPot in drawingService.LotPots.OrderByDescending(x => x.BookingRank))
            {
                // Ziehen
                ExecuteDrawing(lotPot);


                // Alle erfolgreichen Tickets ansehen => nicht mehr auf Warteliste
                foreach (var lot in lotPot.Lots.Where(x => !x.Subscription.OnWaitingList))
                {
                    // das zugehörige Spiel suchen
                    var game = drawingService.Games.SingleOrDefault(x => x.UserId.Equals(lot.Subscription.UserId));

                    // Die besten n Lose, also Prio 1,2,3 
                    var studentLots = game.Lots.OrderBy(x => x.Priority).Take(game.CoursesWanted).ToList();

                    // Student ist glücklich, wenn er alle seine Prio 1 bis n bekommen hat
                    var isHappy = !studentLots.Any(x => x.Subscription.OnWaitingList);

                    if (isHappy)
                    {
                        // jetzt können wir alle seine anderen Lose auf "invalid" setzen
                        // warum: das sind die Lose mit geringer Prio, die er nicht mehr braucht
                        // er hat schon die besten Prios erhalten
                        foreach (var drawingLot in studentLots.Where(x => x.Subscription.OnWaitingList))
                        {
                            drawingLot.IsValid = false;
                            drawingLot.IsTouched = true;
                            drawingLot.AddMessage("Die Wünsche konnten erfüllt werden, dieses Los wird nicht mehr gebraucht");

                            drawingService.AddMessage(drawingLot.Course, drawingLot.Subscription, "Die Wünsche konnten erfüllt werden, dieses Los wird nicht mehr gebraucht");

                        }
                    }
                }
            }

            // jetzt alle Plätze auflösen, die zu viel vergeben wurden
            var nReject = 0;
            foreach (var game in drawingService.Games)
            {
                var studentLots = game.Lots.OrderBy(x => x.Priority).ToList();

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
                                drawingLot.AddMessage(
                                    "Zuviel vergebenes Los. Platz wird wieder freigegeben");

                                nReject++;

                                drawingService.AddMessage(drawingLot.Course, drawingLot.Subscription, "Zuviel vergebenes Los. Platz wird wieder freigegeben");
                            }
                        }
                    }
                }

            }

            drawingService.AddMessage(null, null, $"Ziehung beendet mit {nReject} wieder zurückgegebenen Plätzen");

            return nReject;
        }

        private void ExecuteDrawing(DrawingLotPot lotPot)
        {
            // Anzahl der verfügbaren Plätze
            // Ausgangslage - aller bereits gezogener Plätze
            // OHI: 20200204: Hier werden die zugelosten Plätze doppelt berücksichtigt
            /*
            var alreadyTaken = lotPot.Lots.Count(x => !x.Subscription.OnWaitingList);

            var nCapacity = lotPot.SeatsAvailable - alreadyTaken;
            */
            // da ist der aktuelle Stand bereits berücksichtigt
            var nCapacity = lotPot.SeatsAvailable;


            drawingService.AddMessage(lotPot.Course, null, "Starte Ziehung für Lostopf");
            drawingService.AddMessage(lotPot.Course, null, $"Freie Plätze im Lostopf {nCapacity}");


            // alle gültigen Lose, die noch auf der Warteliste sind

            var debugLots = lotPot.Lots.Where(x => x.Subscription.OnWaitingList).GroupBy(x => x.Priority)
                .OrderBy(x => x.Key);
            foreach (var lotGroup in debugLots)
            {
                var lotList = lotGroup.ToList();
                drawingService.AddMessage(lotPot.Course, null, $"Lose insgesamt: {lotList.Count} in Prio {lotGroup.Key}");
            }


            var allLots = lotPot.Lots.Where(x => x.IsValid && x.Subscription.OnWaitingList).GroupBy(x => x.Priority)
                .OrderBy(x => x.Key);


            foreach (var lotGroup in allLots)
            {

                var lotList = lotGroup.ToList();

                drawingService.AddMessage(lotPot.Course, null, $"Valide Lose insgesamt: {lotList.Count} in Prio {lotGroup.Key}");

                // passt die ganze Liste rein?
                if (lotList.Count <= nCapacity)
                {

                    foreach (var lot in lotList)
                    {
                        lot.Subscription.OnWaitingList = false;
                        lot.IsTouched = true;
                        lot.AddMessage("Platz erhalten, weil noch ausreichend Kapazität vorhanden");

                        drawingService.AddMessage(lot.Course, lot.Subscription, "Platz erhalten, weil noch ausreichend Kapazität vorhanden");
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
                        lot.AddMessage("Platz durch Los erhalten");

                        drawingService.AddMessage(lot.Course, lot.Subscription, "Platz durch Los erhalten");
                    }

                    break;
                }

            }
        }

        private void TryToHelpJinx()
        {
            foreach (var game in drawingService.Games)
            {
                var studentLots = game.Lots.OrderBy(x => x.Priority).ToList();

                // Student ist Ober-Pechvogel, wenn er gar nichts erhalten hat => zuerst 1 Kurs
                // gar nichts: Keinen Platz und auch kein Los auf der Warteliste

                // alte Formulierung
                // var isJinx = !game.Seats.Any() && studentLots.All(x => x.IsValid && x.Subscription.OnWaitingList);

                var nSeats = game.Seats.Count;
                var nLots = studentLots.Count;
                var nWaitingLots = studentLots.Count(x => x.Subscription.OnWaitingList);
                var validLots = studentLots.Count(x => x.IsValid);
                var nWaitingValidLots = studentLots.Count(x => x.IsValid && x.Subscription.OnWaitingList);

                drawingService.AddMessage(game.UserId, $"Plätze: {nSeats}, Lose {nLots}, auf Warteliste {nWaitingLots}, valide {validLots}, valide und Warteliste {nWaitingValidLots}");


                bool isJinx = nSeats == 0 && nLots == nWaitingLots;


                if (isJinx)
                {
                    drawingService.AddMessage(game.UserId, $"Bisher keinen Platz erhalten");

                    // Suche einen Kurs, der noch freie Plätze hat und in dem er noch nicht drin ist
                    var availableCourses = drawingService.LotPots.Where(x =>
                        !x.Course.Occurrence.Subscriptions.Any(s => s.UserId.Equals(game.UserId)) &&
                        x.SeatsAvailable > 0).OrderByDescending(x => x.SeatsAvailable).ToList();

                    drawingService.AddMessage(null, null, $"Es sind {availableCourses.Count} LVs noch frei");

                    var availableCourse = availableCourses.FirstOrDefault();

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
                            lot.AddMessage("Platz über Pechvogelregel erhaltem");
                            lot.Subscription = subscription;
                            lot.Course = availableCourse.Course;

                            game.Lots.Add(lot);

                            availableCourse.Lots.Add(lot);

                            drawingService.AddMessage(lot.Course, lot.Subscription, "Platz über Pechvogelregel erhaltem");

                            /*
                            var lotPot = LotPots.SingleOrDefault(x => x.Course.Id == availableCourse.Course.Id);
                            lotPot.Lots.Add(lot);
                            */
                        }
                        else
                        {
                            drawingService.AddMessage(game.UserId, "Will nichts anderes. Es hätte noch einen Platz in einer anderen Lehrveranstaltung gegeben.");

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
            foreach (var game in drawingService.Games)
            {
                var studentLots = game.Lots.OrderBy(x => x.Priority).ToList();

                // Hat Student alle Wünsche erfüllt bekommen
                var isJinx = (studentLots.Count(x => !x.Subscription.OnWaitingList) + game.Seats.Count) < game.CoursesWanted;

                if (isJinx)
                {
                    // Suche einen Kurs, der noch freie Plätze hat und in dem er noch nicht drin ist
                    var availableCourse = drawingService.LotPots.Where(x =>
                        !x.Course.Occurrence.Subscriptions.Any(s => s.UserId.Equals(game.UserId)) &&
                        x.SeatsAvailable > 0).OrderByDescending(x => x.SeatsAvailable).FirstOrDefault();

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

                            // Ein neues Los
                            var lot = new DrawingLot();
                            lot.IsTouched = true;
                            lot.IsValid = true;
                            lot.AddMessage("Platz durch Auffüllen erhalten");
                            lot.Subscription = subscription;
                            lot.Course = availableCourse.Course;

                            game.Lots.Add(lot);

                            availableCourse.Lots.Add(lot);

                            /*
                            var lotPot = LotPots.SingleOrDefault(x => x.Course.Id == availableCourse.Course.Id);
                            lotPot.Lots.Add(lot);
                            */

                            drawingService.AddMessage(lot.Course, lot.Subscription, "Platz durch Auffüllen erhalten");

                            nSuccess++;
                        }
                        else
                        {
                            drawingService.AddMessage(game.UserId, "Will nichts anderes. Es hätte noch einen Platz in einer anderen Lehrveranstaltung gegeben.");

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
