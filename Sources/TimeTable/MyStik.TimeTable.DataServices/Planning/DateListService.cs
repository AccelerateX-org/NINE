using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Planning
{
    public class CourseSchedule
    {
        public CourseSchedule(ICollection<ActivityDate> dates, DateTime begin, DateTime end)
        {
            Dates = dates;
            Begin = begin;
            End = end;
        }

        public void Init()
        {
            var service = new CourseScheduleService();
            Blocks = service.GetBlocks(Dates, Begin, End);
            Sequences = service.GetSequences(Dates, Begin, End);
        }


        /// <summary>
        /// Großer Schalter, wie kopiert werden soll
        /// </summary>
        /// <returns></returns>
        public bool IsFrequent()
        {
            // Sequenzen mit mehr als einem Termin
            return Sequences.Count(x => x.Dates.Count > 1) > 0;
        }

        public DateTime FirstBegin { get; set; }

        public DateTime LastEnd { get; set; }

        public DateTime Begin { get; }
        public DateTime End { get; }

        public ICollection<ActivityDate> Dates { get; }

        public ICollection<CourseScheduleBlock> Blocks { get; set; }

        public ICollection<CourseScheduleSequence> Sequences { get; set; } 
    }


    public class CourseScheduleBlock
    {
        /// <summary>
        /// Der x-te Wochentag nach dem Beginn eines Referenzzeitraums (z.B. Semesterbeginn)
        /// </summary>
        public int OffsetWeekday { get; set; }

        /// <summary>
        /// Die Tage in dem Block
        /// </summary>
        public List<ActivityDate> Dates { get; set; }

        public ActivityDate GetAnchor()
        {
            return Dates.OrderBy(x => x.Begin).FirstOrDefault();
        }
    }

    public class CourseScheduleSequence
    {
        public int OffsetWeekday { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }

        public List<ActivityDate> Dates { get; set; }
    }

    public class CourseScheduleService
    {
        /// <summary>
        /// Calculates the offset weekday, representing the nth occurrence of a specific weekday after the segment start.
        /// Example: if segment starts on Monday and the date is Wednesday of the following week, offset is 2 (2nd Wednesday after segment start).
        /// </summary>
        private int CalculateOffsetWeekday(DateTime targetDate, DateTime segmentStart)
        {
            // Days difference between target and segment start
            var daysDifference = (int)(targetDate.Date - segmentStart.Date).TotalDays;
            
            // Get the target weekday
            var targetWeekday = targetDate.DayOfWeek;
            
            // Days from segment start to the first occurrence of target weekday
            var daysToFirstOccurrence = ((int)targetWeekday - (int)segmentStart.DayOfWeek + 7) % 7;
            
            // If target date is before the first occurrence of that weekday, we're at the 0th occurrence
            if (daysDifference < daysToFirstOccurrence)
                return 0;
            
            // Calculate which occurrence (1st, 2nd, 3rd, etc.)
            var daysAfterFirstOccurrence = daysDifference - daysToFirstOccurrence;
            var occurrenceNumber = (daysAfterFirstOccurrence / 7) + 1;
            
            return occurrenceNumber;
        }

        /// <summary>
        /// Analyzes a list of dates and returns a list of blocks, where each block is a continuous range of dates without gaps.
        /// </summary>
        /// <param name="dates"></param>
        /// <param name="segmentStart">The start date of the segment</param>
        /// <param name="segmentEnd">The end date of the segment</param>
        /// <returns>A list of blocks, where each block contains consecutive dates without gaps</returns>
        public ICollection<CourseScheduleBlock> GetBlocks(ICollection<ActivityDate> dates, DateTime segmentStart, DateTime segmentEnd)
        {
            if (dates == null || dates.Count == 0)
                return new List<CourseScheduleBlock>();

            var orderedDates = dates
                .OrderBy(x => x.Begin)
                .ToList();

            var blocks = new List<CourseScheduleBlock>();
            if (orderedDates.Count == 0)
                return blocks;

            var firstDate = orderedDates[0];
            var offsetWeekday = CalculateOffsetWeekday(firstDate.Begin, segmentStart);

            var currentBlock = new CourseScheduleBlock { OffsetWeekday = offsetWeekday, Dates = new List<ActivityDate> { orderedDates[0] } };
            for (var i = 1; i < orderedDates.Count; i++)
            {
                var previous = orderedDates[i - 1];
                var current = orderedDates[i];

                // Check if there's a gap between the previous and current date
                // a gap == 0 means "same day"
                // we do not check the time
                if ((current.Begin.Date - previous.Begin.Date).Days <= 1)
                {
                    // No gap, add to current block
                    currentBlock.Dates.Add(current);
                }
                else
                {
                    // Gap found, start a new block
                    blocks.Add(currentBlock);

                    offsetWeekday = CalculateOffsetWeekday(current.Begin, segmentStart);
                    currentBlock = new CourseScheduleBlock { OffsetWeekday = offsetWeekday, Dates = new List<ActivityDate> { current } };
                }
            }

            // Add the last block
            blocks.Add(currentBlock);

            return blocks;
        }

        /// <summary>
        /// Analyses a list of dates and returns a list of sequences. A sequence is defined by weekday, begin and end. 
        /// </summary>
        /// <param name="dates"></param>
        /// <param name="segmentStart">The start date of the segment</param>
        /// <param name="segmentEnd">The end date of the segment</param>
        /// <returns></returns>
        public ICollection<CourseScheduleSequence> GetSequences(ICollection<ActivityDate> dates, DateTime segmentStart, DateTime segmentEnd)
        {
            if (dates == null || dates.Count == 0)
                return new List<CourseScheduleSequence>();
            
            var orderedDates = dates
                .OrderBy(x => x.Begin)
                .ToList();
            
            var sequences = new List<CourseScheduleSequence>();
            if (orderedDates.Count == 0)
                return sequences;

            var days = (from occ in dates
                select
                    new
                    {
                        Day = occ.Begin.DayOfWeek,
                        Begin = occ.Begin.TimeOfDay,
                        End = occ.End.TimeOfDay,
                    }).Distinct();

            foreach (var day in days)
            {
                var seqDates = dates.Where(d => d.Begin.DayOfWeek == day.Day && d.Begin.TimeOfDay == day.Begin && d.End.TimeOfDay == day.End)
                    .OrderBy(x => x.Begin)
                    .ToList();

                var firstDate = seqDates[0];
                var offsetWeekday = CalculateOffsetWeekday(firstDate.Begin, segmentStart);

                sequences.Add(new CourseScheduleSequence
                {
                    OffsetWeekday = offsetWeekday,
                    Day = day.Day, 
                    Begin = day.Begin, 
                    End = day.End, 
                    Dates = seqDates
                });
            }

            return sequences;
        }

        public ICollection<ActivityDate> CopySchedule(CourseSchedule schedule, DateTime begin, DateTime end)
        {
            var isFrequent = schedule.IsFrequent();
            var dates = new List<ActivityDate>();

            if (isFrequent)
            {

            }
            else
            {
                foreach (var scheduleBlock in schedule.Blocks)
                {
                    // finde den startpunkt
                    var anchor = scheduleBlock.GetAnchor();
                    if (anchor == null)
                        continue;

                    var dayOfWeek = anchor.Begin.Date.DayOfWeek;

                    var offsetWeekdayBegin = dayOfWeek - begin.DayOfWeek;
                    if (offsetWeekdayBegin < 0)
                    {
                        offsetWeekdayBegin += 7;
                    }

                    var offsetDaysSegment = offsetWeekdayBegin + (scheduleBlock.OffsetWeekday - 1) * 7;

                    foreach (var blockDate in scheduleBlock.Dates)
                    {
                        // der offset innerhalb des Blocks
                        var offsetDaysBlock = (blockDate.Begin.Date - begin.Date).Days;

                        var date = new ActivityDate
                        {
                            Begin = begin.AddDays(offsetDaysSegment + offsetDaysBlock)
                                .Add(blockDate.Begin.TimeOfDay),
                            End = begin.AddDays(offsetDaysSegment + offsetDaysBlock)
                                .Add(blockDate.End.TimeOfDay)
                        };
                        dates.Add(date);
                    }
                }    
            }


            return dates;
        }
    }
}
