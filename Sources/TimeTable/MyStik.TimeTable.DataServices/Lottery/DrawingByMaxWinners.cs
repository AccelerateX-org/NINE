using MyStik.TimeTable.DataServices.Lottery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.Lottery
{
    class DrawingByMaxWinners : IDrawingAlgorithm
    {
        private DrawingService drawingService;

        public DrawingByMaxWinners(DrawingService ds)
        {
            drawingService = ds;
        }

        public int Execute()
        {
            DrawAllPots();


            return 0;
        }

        private int DrawAllPots()
        {
            drawingService.AddMessage(null, null, "Starte Ziehung");
            // Zuerst nachsehen, ob Wünsche offen sind
            /*
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
            */

            foreach (var lotPot in drawingService.LotPots.OrderByDescending(x => x.BookingRank))
            {
                // Ziehen
                ExecuteDrawing(lotPot);


                // Alle erfolgreichen Tickets ansehen => nicht mehr auf Warteliste
                foreach (var lot in lotPot.Lots.Where(x => !x.Subscription.OnWaitingList))
                {
                    // das zugehörige Spiel suchen
                    var game = drawingService.Games.SingleOrDefault(x => x.UserId.Equals(lot.Subscription.UserId));

                    // Alle Lose, die noch auf Warteliste sind 
                    var studentLots = game.Lots.Where(x => x.Subscription.OnWaitingList).ToList();

                    // jetzt können wir alle seine anderen Lose eine priorität schlechter setzen
                    // warum: er hat ja schon einen Platz bekommen
                    foreach (var drawingLot in studentLots)
                    {
                        drawingLot.Subscription.Priority++;

                        drawingLot.AddMessage($"Priorität des Loses auf {drawingLot.Subscription.Priority} gesetzt.");

                        drawingService.AddMessage(drawingLot.Course, drawingLot.Subscription, $"Priorität des Loses auf {drawingLot.Subscription.Priority} gesetzt.");

                    }

                }
            }

            return 0;
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

    }
}
