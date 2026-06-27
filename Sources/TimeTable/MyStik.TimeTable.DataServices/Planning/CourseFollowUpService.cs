using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Planning
{
    public class CourseFollowUpService
    {
        private TimeTableDbContext _dbContext;
        public CourseFollowUpService(TimeTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Course CopyCourse(Guid courseId, Guid targetSegmentId)
        {
            var course = _dbContext.Activities.OfType<Course>().Include(activity => activity.Organiser).Include(course1 => course1.SubjectTeachings.Select(subjectTeaching => subjectTeaching.Subject)).Include(activity => activity.LabelSet.ItemLabels).Include(activity1 => activity1.Owners.Select(activityOwner => activityOwner.Member)).Include(activity => activity.Occurrence.SeatQuotas.Select(seatQuota => seatQuota.Curriculum)).Include(activity1 => activity1.Occurrence.SeatQuotas.Select(seatQuota1 => seatQuota1.ItemLabelSet.ItemLabels)).Include(activity => activity.Occurrence.SeatQuotas.Select(seatQuota2 => seatQuota2.Fractions.Select(seatQuotaFraction => seatQuotaFraction.Curriculum))).Include(activity1 => activity1.Occurrence.SeatQuotas.Select(seatQuota3 => seatQuota3.Fractions.Select(seatQuotaFraction1 =>
                seatQuotaFraction1.ItemLabelSet.ItemLabels))).FirstOrDefault(c => c.Id == courseId);
            if (course?.Organiser == null)
            {
                return null;
            }

            var targetSegment = _dbContext.SemesterDates.Include(semesterDate => semesterDate.Organiser)
                .Include(semesterDate1 => semesterDate1.Semester).FirstOrDefault(s => s.Id == targetSegmentId);
            if (targetSegment?.Organiser == null || targetSegment.Organiser.Id != course.Organiser.Id)
            {
                return null;
            }

            var copyDates = true;


            try
            {
                var planCourse = new Course
                {
                    ShortName = course.ShortName,
                    Name = course.Name,
                    Semester = targetSegment.Semester,
                    Segment = targetSegment,
                    Organiser = course.Organiser,
                    UrlMoodleCourse = course.UrlMoodleCourse,
                    IsInternal = true,
                    IsProjected = true,
                    ExternalSource = "FollowUp",
                    Occurrence = new Occurrence()
                    {
                        Capacity = course.Occurrence.Capacity,
                        IsAvailable = false,
                        FromIsRestricted = course.Occurrence.FromIsRestricted,
                        UntilIsRestricted = course.Occurrence.UntilIsRestricted,
                        IsCanceled = course.Occurrence.IsCanceled,
                        IsMoved = false,
                        UseGroups = course.Occurrence.UseGroups,
                    }
                };

                var itemLabelSet = new ItemLabelSet();
                planCourse.LabelSet = itemLabelSet;
                _dbContext.ItemLabelSets.Add(itemLabelSet);

                // Module
                foreach (var teaching in course.SubjectTeachings)
                {
                    var planTeaching = new SubjectTeaching()
                    {
                        Course = planCourse,
                        Subject = teaching.Subject,
                    };

                    _dbContext.SubjectTeachings.Add(planTeaching);
                }

                // Kohorten
                if (course.LabelSet != null)
                {
                    foreach (var label in course.LabelSet.ItemLabels.ToList())
                    {
                        planCourse.LabelSet.ItemLabels.Add(label);
                    }
                }

                // Platzbeschränkungen
                foreach (var quota in course.Occurrence.SeatQuotas)
                {
                    var planQuota = new SeatQuota()
                    {
                        Occurrence = planCourse.Occurrence,
                        Name = quota.Name,
                        Description = quota.Description,
                        MinCapacity = quota.MinCapacity,
                        MaxCapacity = quota.MaxCapacity,
                        Curriculum = quota.Curriculum,
                        ItemLabelSet = new ItemLabelSet(),
                        Fractions = new List<SeatQuotaFraction>()
                    };

                    if (quota.ItemLabelSet != null)
                    {
                        foreach (var label in quota.ItemLabelSet.ItemLabels.ToList())
                        {
                            planQuota.ItemLabelSet.ItemLabels.Add(label);
                        }
                    }


                    foreach (var fraction in quota.Fractions)
                    {
                        var planFraction = new SeatQuotaFraction()
                        {
                            Curriculum = fraction.Curriculum,
                            ItemLabelSet = new ItemLabelSet(),
                            Percentage = fraction.Percentage,
                            Quota = planQuota,
                            Weight = fraction.Weight
                        };

                        foreach (var label in fraction.ItemLabelSet.ItemLabels.ToList())
                        {
                            planFraction.ItemLabelSet.ItemLabels.Add(label);
                        }

                        _dbContext.SeatQuotaFractions.Add(planFraction);
                        _dbContext.ItemLabelSets.Add(planFraction.ItemLabelSet);
                    }


                    _dbContext.SeatQuotas.Add(planQuota);
                    _dbContext.ItemLabelSets.Add(planQuota.ItemLabelSet);
                }


                // Owner sind alle Dozenten sowie der Anleger
                /*
                var member = MemberService.GetMember(user.Id, course.Organiser.Id);
                ActivityOwner owner = null;
                if (member != null)
                {
                    owner = course.Owners.FirstOrDefault(x => x.Member.Id == member.Id);
                }

                // sollte der aktuelle User kein Owner sein => dazufügen
                   if (owner == null && member != null)
                   {
                       var planOwner = new ActivityOwner()
                       {
                           Activity = planCourse,
                           Member = member
                       };
                       planCourse.Owners.Add(planOwner);
                       _dbContext.ActivityOwners.Add(planOwner);
                   }

                 */
                foreach (var courseOwner in course.Owners)
                {
                    var planOwner = new ActivityOwner()
                    {
                        Activity = planCourse,
                        Member = courseOwner.Member
                    };
                    planCourse.Owners.Add(planOwner);
                    _dbContext.ActivityOwners.Add(planOwner);
                }


                if (copyDates)
                {
                    //var courseService = new CourseInfoService(_dbContext);

                    //var summary = courseService.GetCourseSummary(course);

                    //var dates = courseService.GetPlanningDates(summary, segment);

                    var dates = new List<ActivityDate>();

                    foreach (var date in dates)
                    {
                        date.Occurrence = new Occurrence
                        {
                            Capacity = -1,
                            IsAvailable = true,
                            FromIsRestricted = false,
                            UntilIsRestricted = false,
                            IsCanceled = false,
                            IsMoved = false,
                            UseGroups = false,
                        };
                        date.Activity = planCourse;

                        planCourse.Dates.Add(date);
                    }
                }


                _dbContext.Activities.Add(planCourse);
                _dbContext.SaveChanges();

                return planCourse;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



    }
}
