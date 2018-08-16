using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid LotteryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name ="Kurzname")]
        public string JobId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        [AllowHtml]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Mindestanzahl an Kursen, die Studierende belegen können")]
        public int MaxConfirm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Maximale Anzahl an Kursen die Studierende belegen können")]
        public int MaxConfirmException { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Mindestanzahl an Kursen, die Studierende wählen müssen")]
        public int MinSubscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Maximalzahl an Kursen, die Studierende wählen können")]
        public int MaxSubscription { get; set; }


        [Display(Name = "Verlosung mit Punktevergabe")]
        public bool IsActive { get; set; }


        [Display(Name = "Verlosung aktiv - Studierenden können das Wahlverfahren sehen")]
        public bool IsAvailable { get; set; }


        [Display(Name = "Manuelle Voreinschreibung durch Lehrende erlaubt")]
        public bool AllowManualSubscription { get; set; }

        [Display(Name = "Nachträgliches Ändern der Auswahl erlauben")]
        public bool IsFlexible { get; set; }

        [Display(Name = "Bewerbungsschreiben gewünscht")]
        public bool LoIneeded { get; set; }


        [Display(Name = "Automatische Ausführung")]
        public bool IsScheduled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Auswahl möglich von")]
        public string IsAvailableFrom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Auswahl möglich bis")]
        public string IsAvailableUntil { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum der ersten Verteilung")]
        public string FirstDrawing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum der letzten Verteilung")]
        public string LastDrawing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Zeitpunkt der täglichen Verteilung")]
        public string DrawingTime { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class LotteryLotPotModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LotteryLotPotModel()
        {
            PotElements = new List<LotteryLotPotCourseModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid LotteryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<LotteryLotPotCourseModel> PotElements { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LotteryLotPotCourseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary ActivitySummary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryModel CourseSummary { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LotterySummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalSubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalSubscriberCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double AvgSubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<IGrouping<string, OccurrenceSubscription>> Subscriptions { get; set; }

    }

    public class LotteryCourseDetailViewModel
    {
        public LotteryCourseDetailViewModel()
        {
            WaitingList = new List<LotteryCourseSubscriber>();
            Reservations = new List<LotteryCourseSubscriber>();
            Participients = new List<LotteryCourseSubscriber>();
        }

        public Lottery Lottery { get; set; }

        public Course Course { get; set; }

        public List<LotteryCourseSubscriber> Participients { get; private set; }

        public List<LotteryCourseSubscriber> Reservations { get; private set; }

        public List<LotteryCourseSubscriber> WaitingList { get; private set; }
    }

    public class LotteryCourseSubscriber
    {
        public LotteryCourseSubscriber()
        {
            Alternatives = new List<CourseSubscriptionViewModel>();
            Subscription = new CourseSubscriptionViewModel();
        }

        public ApplicationUser User { get; set; }

        public CourseSubscriptionViewModel Subscription { get; set; }

        public List<CourseSubscriptionViewModel> Alternatives { get; private set; }
    }


    public class CourseSubscriptionViewModel
    {
        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }
    }


    public class LotteryGambleViewModel
    {
        public LotteryGambleViewModel()
        {
            Courses = new List<LotteryGambleCourseViewModel>();
            BudgetStates = new List<LotteryGambleBudgetStateViewModel>();
        }

        public ApplicationUser User { get; set; }

        public Lottery Lottery { get; set; }

        /// <summary>
        /// Anzahl der bereits belegten Plätze
        /// </summary>
        public int Confirmed { get; set; }

        public int Subscribed { get; set; }

        public List<LotteryGambleCourseViewModel> Courses { get; private set; }


        public List<LotteryGambleBudgetStateViewModel> BudgetStates { get; private set; }

    }

    /// <summary>
    /// Alle Angaben zur Platzvelorsung einer einzelnen Lehrveranstaltung
    /// </summary>
    public class LotteryGambleCourseViewModel
    {
        public LotteryGambleCourseViewModel()
        {
            Subscriptions = new List<LotteryGambleSubscriptionViewModel>();
            Budgets = new List<LotteryGambleBudgetViewModel>();
        }

        #region Informationen zur Lehrveranstaltung
        /// <summary>
        /// Die Lehrveranstaltung
        /// ggf. doppelt, da auch über Summary möglich
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Der Zugriff auf die Zusammenfassung
        /// </summary>
        public CourseSummaryModel Summary { get; set; }


        public int Capacity
        {
            get
            {
                if (Course.Occurrence != null && Course.Occurrence.Capacity > 0)
                    return Course.Occurrence.Capacity;
                return 0;
            }
        }

        public int ParticipientCount
        {
            get
            {
                if (Course.Occurrence != null)
                    return Course.Occurrence.Subscriptions.Count(x => !x.OnWaitingList);
                return 0;
            }
        }

        public int SeatsAvailable => Math.Max(Capacity - SeatsTaken, 0);

        public int SeatsTaken
        {
            get
            {
                if (Course.Occurrence != null)
                    return Course.Occurrence.Subscriptions.Count(x => !x.OnWaitingList);
                return 0;
            }
        }
        #endregion


        /// <summary>
        /// Die zugehörige Lotterie
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// Die Eintragung des Teilnehmers
        /// </summary>
        //public LotteryGambleSubscriptionViewModel MySubscription { get; set; }


        /// <summary>
        /// Die Eintragung
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// Die Liste der getätigten Einsätze
        /// </summary>
        public List<LotteryGambleBudgetViewModel> Budgets { get; private set; }


        public int AvailableBudget
        {
            get
            {
                var sum = Budgets.Sum(x => x.PointsFeasible);
                return sum;
            }
        }

        /// <summary>
        /// Summe alle gesetzten Punkte für die zugehörige Eintragung
        /// </summary>
        public int PoinstForSubscription
        {
            get
            {
                if (Subscription != null)
                {
                    var n = Subscription.LapCount;
                    foreach (var bet in Subscription.Bets)
                    {
                        n += bet.Amount;
                    }
                    return n;
                }
                return 0;
            }
        }



        /// <summary>
        /// Alle Eintragungen
        /// </summary>
        public List<LotteryGambleSubscriptionViewModel> Subscriptions { get; private set; }


        /// <summary>
        /// Gewinner, sortiert nach Punkten
        /// </summary>
        public List<LotteryGambleSubscriptionViewModel> Winner
        {
            get
            {
                // Alle, die nicht auf der Warteliste sind
                var orderedSubscriptions = Subscriptions.OrderBy(x => x.Rank).ThenByDescending(x => x.Points).ToList();
                return orderedSubscriptions.Where(x => !x.Subscription.OnWaitingList).ToList();
            }
        }



        /// <summary>
        /// Der Lostopf
        /// </summary>
        public List<LotteryGambleSubscriptionViewModel> LotPot
        {
            get
            {
                // Ausgebucht
                // kein Lostopf
                if (SeatsAvailable == 0)
                {
                    return new List<LotteryGambleSubscriptionViewModel>();
                }


                var orderedSubscriptions = Subscriptions.OrderBy(x => x.Rank).ThenByDescending(x => x.Points).ToList();
                // Anzahl ist kleiner als Kapazität
                // LosTopf sind alle auf Warteliste
                if (orderedSubscriptions.Count < Capacity)
                {
                    return orderedSubscriptions.Where(x => x.Subscription.OnWaitingList).ToList();
                }

                // Punkte des letzten Platzes    
                var lastSeat = orderedSubscriptions.Take(Capacity).Last();

                // alle auf Warteliste mit dieser Punktzahl und mehr
                return orderedSubscriptions.Where(x => x.Subscription.OnWaitingList && x.Points >= lastSeat.Points).ToList();
            }
        }

        /// <summary>
        /// Die Chancenlosen
        /// </summary>
        public List<LotteryGambleSubscriptionViewModel> Looser
        {
            get
            {
                var orderedSubscriptions = Subscriptions.OrderBy(x => x.Rank).ThenByDescending(x => x.Points).ToList();
                // Anzahl ist kleiner als Kapazität
                // keine Looser
                if (Capacity > 0 && (orderedSubscriptions.Count < Capacity || !orderedSubscriptions.Any()))
                {
                    return new List<LotteryGambleSubscriptionViewModel>();
                }

                // Punkte des letzten Platzes    
                var lastSeat = orderedSubscriptions.Take(Capacity).LastOrDefault();

                if (lastSeat == null)
                {
                    return new List<LotteryGambleSubscriptionViewModel>();
                }


                // alle auf Warteliste mit weniger als dieser Punktzahl
                return orderedSubscriptions.Where(x => x.Subscription.OnWaitingList && x.Points < lastSeat.Points).ToList();
            }
        }


 
    }

    /// <summary>
    /// Der Zustand einer Eintragung in einem Kurs
    /// </summary>
    public class LotteryGambleSubscriptionViewModel
    {
        /// <summary>
        /// Die Eintragung
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }


        /// <summary>
        /// Alle Punkte, die gesetzt wurden
        /// </summary>
        public int Points
        {
            get
            {
                var n = Subscription.LapCount;
                foreach (var bet in Subscription.Bets)
                {
                    n += bet.Amount;
                }
                return n;
            }
        }

        /// <summary>
        /// Wertigkeit
        /// 0: Teilnehmer
        /// 1: Reservierung
        /// 2: Warteliste
        /// </summary>
        public int Rank
        {
            get
            {
                if (Subscription.OnWaitingList)
                    return 2;

                if (Subscription.IsConfirmed)
                    return 0;

                return 1;
            }
        }
    }

    public class LotteryGambleBudgetViewModel
    {
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Das Budget
        /// </summary>
        public LotteryBudget Budget { get; set; }

        /// <summary>
        /// Der getätigte Einsatz für den einen Kurs!
        /// </summary>
        public LotteryBet Bet { get; set; }

        /// <summary>
        /// die für Reservierungen und Plätze verbrauchten Punkte
        /// </summary>
        public int PointsUsed
        {
            get
            {
                // nicht auf Warteliste
                if (Budget != null && User != null)
                    return Budget.Bets.Where(x => !x.Subscription.OnWaitingList && x.Subscription.UserId.Equals(User.Id)).Sum(x => x.Amount);
                return 0;
            }
        }

        /// <summary>
        /// die maximal verfügbaren Punkte
        /// </summary>
        public int PointsAvailable
        {
            get
            {
                if (Budget != null && User != null)
                    return Budget.Size - PointsUsed;
                return 0;
            }
        }

        /// <summary>
        /// die eingesetzten Punkte uaf Warteliste
        /// </summary>
        public int PointsBetted
        {
            get
            {
                if (Budget != null && User != null)
                    return Budget.Bets.Where(x => x.Subscription.OnWaitingList && x.Subscription.UserId.Equals(User.Id)).Sum(x => x.Amount);
                return 0;
            }
        }

        /// <summary>
        /// die noch einsetzbaren Punkte
        /// </summary>
        public int PointsFeasible
        {
            get
            {
                if (Budget != null && User != null)
                    return Budget.Size - PointsBetted;
                return 0;
            }
        }

        public int PointsAutoBet
        {
            get
            {
                var pf = PointsFeasible;
                if (pf == 0)
                    return pf;

                var p = (int) (pf * Budget.Fraction);

                var rest = pf - p;
                if (rest <= 1)
                {
                    p++;
                }

                return p;
            }
        }



        public List<SelectListItem> PointList
        {
            get
            {
                var list = new List<SelectListItem>();

                if (Bet != null)
                {
                    var maxPoint = PointsFeasible + Bet.Amount;

                    for (var i = 0; i <= Budget.Size; i++)
                    {
                        list.Add(new SelectListItem
                        {
                            Text = i.ToString(),
                            Value = i.ToString(),
                            Disabled = i>maxPoint,
                            Selected = (i == Bet.Amount)
                        });
                    }
                }
                return list;
            }
        }
    }

    public class LotteryGambleBudgetStateViewModel
    {
        public LotteryGambleBudgetStateViewModel()
        {
            Bets = new List<LotteryBet>();
        }

        public LotteryBudget Budget { get; set; }

        public bool IsDistributed { get; set; }

        public List<LotteryBet> Bets { get; private set; }
    }

    public class LotteryCheckModel
    {
        public ApplicationUser User { get; set; }

        public Lottery Lottery { get; set; }

        public string Message { get; set; }
    }

    public class LotteryOverviewModel
    {
        public LotteryOverviewModel()
        {
            Courses = new List<LotteryOverviewCourseModel>();
            CoursesSelected = new List<LotteryOverviewCourseModel>();
        }
        public Lottery Lottery { get; set; }

        public Student Student { get; set; }

        public LotteryGame Game { get; set; }

         public List<LotteryOverviewCourseModel> Courses { get; set; }

        public List<LotteryOverviewCourseModel> CoursesSelected { get; set; }

        public int ConfirmCount { get; set; }

        public bool AcceptAny { get; set; }
    }

    public class LotteryOverviewCourseModel
    {
        public CourseSummaryModel CourseSummary { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Course Course { get { return CourseSummary.Course as Course; } }

        public int Points
        {
            get
            {
                if (Subscription.Priority.HasValue)
                    return Subscription.Priority.Value;
                return Subscription.LapCount;
            }
        }

        public bool OnWaitingList { get { return Subscription.OnWaitingList; } }

        public bool IsSelectable { get; set; }

        public string Message { get; set; }
    }

    public class CourseReferenceModel
    {
        public Lottery Lottery { get; set; }

        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        [AllowHtml]
        public string Message { get; set; }
    }

    public class LotteryTestModel
    {
        public Lottery Lottery { get; set; }

        [Display(Name = "Veränderung der Plätze pro Lostopf")]
        public int Capacity { get; set; }

        [Display(Name = "Geschlossene Gesellschaft (rot)")]
        public bool IsCoterie { get; set; }
    }
}