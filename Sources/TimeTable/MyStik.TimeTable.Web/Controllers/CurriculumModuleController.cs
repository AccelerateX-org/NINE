using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Hubs;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CurriculumModuleController : BaseController
    {
        /// <summary>
        /// Liste aller Module des aktuellen Benutzers
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var model = Db.CurriculumModules.Where(x =>
                x.ModuleResponsibilities.Any(m =>
                    !string.IsNullOrEmpty(m.Member.UserId) && m.Member.UserId.Equals(user.Id))).ToList();

            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);

            ViewBag.Semester = semester;
            ViewBag.NextSemester = nextSemester;

            return View(model);
        }

        public ActionResult Admin(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(model.Catalog.Organiser);

            return View(model);
        }


        public ActionResult Create(Guid catalogId)
        {
            var org = GetMyOrganisation();


            var model = new CurriculumModuleCreateModel();
            model.catalogId = catalogId;
            model.SWS = 4;


            ViewBag.TeachingFormats = Db.TeachingFormats.OrderBy(x => x.Tag).Select(c => new SelectListItem
            {
                Text = c.Tag,
                Value = c.Id.ToString(),
            });


            ViewBag.ExamFormats = Db.ExaminationForms.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CurriculumModuleCreateModel model)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == model.catalogId);

            var member = GetMyMembership(catalog.Organiser.Id);

            var isDuplicate = catalog.Modules.Any(x => x.Tag.Equals(model.Tag.Trim()));

            if (!isDuplicate)
            {
                var teachingFormat = Db.TeachingFormats.SingleOrDefault(x => x.Id == model.TeachingFormatId);
                var examFormat = Db.ExaminationForms.SingleOrDefault(x => x.Id == model.ExamFormatId);

                var module = new CurriculumModule
                {
                    Name = model.Name,
                    NameEn = model.NameEn,
                    Tag = model.Tag,
                    Applicableness = model.Applicableness,
                    Prerequisites = model.Prequisites,
                    Catalog = catalog,
                    ModuleSubjects = new List<ModuleSubject>(),
                    ExaminationOptions = new List<ExaminationOption>()
                };

                var subject = new ModuleSubject
                {
                    Name = "Lehrveranstaltung",
                    Tag = "LV",
                    TeachingFormat = teachingFormat,
                    SWS = model.SWS,
                    Module = module
                };

                module.ModuleSubjects.Add(subject);
                Db.ModuleCourses.Add(subject);

                var examOption = new ExaminationOption
                {
                    Name = "Option A",
                    Module = module,
                    Fractions = new List<ExaminationFraction>()
                };

                var examFraction = new ExaminationFraction
                {
                    ExaminationOption = examOption,
                    Form = examFormat,
                    Weight = 1.0,
                };

                examOption.Fractions.Add(examFraction);
                Db.ExaminationFractions.Add(examFraction);

                module.ExaminationOptions.Add(examOption);
                Db.ExaminationOptions.Add(examOption);

                var resp = new ModuleResponsibility { Member = member, Module = module };

                module.ModuleResponsibilities.Add(resp);

                Db.ModuleResponsibilities.Add(resp);
                Db.CurriculumModules.Add(module);
                Db.SaveChanges();
            }

            return RedirectToAction("Details", "Catalogs",new {id = model.catalogId});
        }

        public ActionResult EditGeneral(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumModuleCreateModel();
            model.moduleId = module.Id;
            model.catalogId = module.Catalog.Id;
            model.Name = module.Name;
            model.NameEn = module.NameEn;
            model.Tag = module.Tag;
            model.Prequisites = module.Prerequisites;
            model.Applicableness = module.Applicableness;


            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);

            return View(model);
        }


        [HttpPost]
        public ActionResult EditGeneral(CurriculumModuleCreateModel model)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.moduleId);

            var catalog = module.Catalog;
            var isDuplicate = catalog.Modules.Any(x => x.Tag.Equals(model.Tag) && x.Id != model.moduleId);

            if (!isDuplicate)
            {
                module.Tag = model.Tag;
                module.Name = model.Name;
                module.NameEn = model.NameEn;
                module.Applicableness = model.Applicableness;
                module.Prerequisites = model.Prequisites;

                Db.SaveChanges();
            }

            return RedirectToAction("Details", "ModuleDescription", new { id = model.moduleId });
        }

        public ActionResult EditResponsibilities(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            return View(module);
        }

        [HttpPost]
        public ActionResult SaveResponsibilities(Guid moduleId, ICollection<Guid> DozIds)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

            var resp2delete = new List<ModuleResponsibility>();
            foreach (var responsibility in module.ModuleResponsibilities)
            {
                if (!DozIds.Contains(responsibility.Member.Id))
                {
                    resp2delete.Add(responsibility);
                }
            }

            foreach (var responsibility in resp2delete)
            {
                module.ModuleResponsibilities.Remove(responsibility);
                Db.ModuleResponsibilities.Remove(responsibility);
            }

            var doz2create = new List<Guid>();
            foreach (var dozId in DozIds)
            {
                var isHere = module.ModuleResponsibilities.Any(x => x.Member.Id == dozId);

                if (!isHere)
                {
                    doz2create.Add(dozId);
                }
            }

            foreach (var dozId in doz2create)
            {
                var member = Db.Members.SingleOrDefault(x => x.Id == dozId);
                var resp = new ModuleResponsibility
                {
                    Module = module,
                    Member = member
                };
                Db.ModuleResponsibilities.Add(resp);
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult Delete(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            return View(model);
        }


        public ActionResult DeleteConfirmed(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            var catalog = model.Catalog;


            // so lange es nich Historie gibt zuerst diese löschen
            foreach (var description in model.Descriptions.ToList())
            {
                if (description.ChangeLog != null)
                    Db.ChangeLogs.Remove(description.ChangeLog);
                Db.ModuleDescriptions.Remove(description);
            }

            foreach (var option in model.ExaminationOptions.ToList())
            {
                foreach (var examinationDescription in option.ExaminationDescriptions.ToList())
                {
                    if (examinationDescription.ChangeLog != null)
                        Db.ChangeLogs.Remove(examinationDescription.ChangeLog);

                    foreach (var examiner in examinationDescription.Examiners.ToList())
                    {
                        Db.Examiners.Remove(examiner);
                    }


                    Db.ExaminationDescriptions.Remove(examinationDescription);
                }


                foreach (var fraction in option.Fractions.ToList())
                {
                    Db.ExaminationFractions.Remove(fraction);
                }

                Db.ExaminationOptions.Remove(option);
            }

            foreach (var moduleResponsibility in model.ModuleResponsibilities.ToList())
            {
                Db.ModuleResponsibilities.Remove(moduleResponsibility);
            }


            foreach (var subject in model.ModuleSubjects.ToList())
            {
                foreach (var subjectAccreditation in subject.SubjectAccreditations.ToList())
                {
                    Db.SubjectAccreditations.Remove(subjectAccreditation);
                }

                foreach (var teaching in subject.SubjectTeachings.ToList())
                {
                    Db.SubjectTeachings.Remove(teaching);
                }

                Db.ModuleCourses.Remove(subject);
            }


            var mappings = Db.ModuleMappings.Where(x => x.Module.Id == model.Id).ToList();
            foreach (var mapping in mappings)
            {
                mapping.Module = null;
            }

            Db.CurriculumModules.Remove(model);
            Db.SaveChanges();

            if (catalog != null)
            {
                return RedirectToAction("Details", "Catalogs", new {id=catalog.Id});
            }

            return RedirectToAction("Index", "Catalogs");
        }

        public ActionResult Copy(Guid moduleId, Guid sourceSemId, Guid destSemId, Guid backSemId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var sourceSemester = SemesterService.GetSemester(sourceSemId);
            var destSemester = SemesterService.GetSemester(destSemId);
            var backSemester = SemesterService.GetSemester(backSemId);

            var model = new ModuleCopyModel()
            {
                Module = module,
                SourceSemester = sourceSemester,
                DestSemester = destSemester,
                BackSemester = backSemester
            };

            return View(model);
        }

        [HttpPost]
        public PartialViewResult CopyCourse(Guid courseId, Guid subjectId, Guid sourceSemId, Guid destSemId, bool copyDates)
        {
            var user = GetCurrentUser();

            var course = Db.Activities.OfType<Course>().Include(activity => activity.Segment).Include(activity1 => activity1.Organiser).Include(activity2 => activity2.Dates).Include(course1 => course1.SubjectTeachings.Select(subjectTeaching => subjectTeaching.Subject)).Include(activity => activity.LabelSet.ItemLabels).Include(activity1 => activity1.Owners.Select(activityOwner => activityOwner.Member)).Include(activity => activity.Occurrence.SeatQuotas.Select(seatQuota => seatQuota.Curriculum)).Include(activity1 => activity1.Occurrence.SeatQuotas.Select(seatQuota1 => seatQuota1.ItemLabelSet.ItemLabels)).Include(activity => activity.Occurrence.SeatQuotas.Select(seatQuota2 => seatQuota2.Fractions.Select(seatQuotaFraction => seatQuotaFraction.Curriculum))).Include(activity1 => activity1.Occurrence.SeatQuotas.Select(seatQuota3 => seatQuota3.Fractions.Select(seatQuotaFraction1 =>
                seatQuotaFraction1.ItemLabelSet.ItemLabels))).SingleOrDefault(x => x.Id == courseId);
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);
            var sourceSemester = SemesterService.GetSemester(sourceSemId);
            var destSemester = SemesterService.GetSemester(destSemId);

            var courseService = new CourseService();


            var summary = courseService.GetCourseSummary(course);

            try
            {

                var planCourse = new Course
                {
                    ShortName = course.ShortName,
                    Name = course.Name,
                    Semester = destSemester,
                    Segment = course.Segment,
                    Organiser = course.Organiser,
                    UrlMoodleCourse = course.UrlMoodleCourse,
                    IsInternal = true,
                    IsProjected = true,
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
                Db.ItemLabelSets.Add(itemLabelSet);

                // Module
                foreach (var teaching in course.SubjectTeachings)
                {
                    var planTeaching = new SubjectTeaching()
                    {
                        Course = planCourse,
                        Subject = teaching.Subject,
                    };

                    Db.SubjectTeachings.Add(planTeaching);
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

                        Db.SeatQuotaFractions.Add(planFraction);
                        Db.ItemLabelSets.Add(planFraction.ItemLabelSet);
                    }


                    Db.SeatQuotas.Add(planQuota);
                    Db.ItemLabelSets.Add(planQuota.ItemLabelSet);
                }


                List<Room> favRooms = new List<Room>();
                List<OrganiserMember> favHosts = new List<OrganiserMember>();

                // Owner sind alle Dozenten sowie der Anleger
                var member = MemberService.GetMember(user.Id, course.Organiser.Id);
                OrganiserMember owner = null;
                if (member != null)
                {
                    owner = course.Owners.Where(x => x.Member.Id == member.Id).Select(x => x.Member).FirstOrDefault();
                }

                foreach (var courseOwner in course.Owners)
                {
                    var planOwner = new ActivityOwner()
                    {
                        Activity = planCourse,
                        Member = courseOwner.Member
                    };

                }

                // sollte der aktuelle User kein Owner sein => dazufügen
                if (owner != null && planCourse.Owners.All(x => x.Member.Id != owner.Id))
                {
                    var planOwner = new ActivityOwner()
                    {
                        Activity = planCourse,
                        Member = owner
                    };
                }


                if (copyDates)
                {
                    if (summary.IsPureRegular())
                    {
                        var ordereDates = course.Dates.OrderBy(x => x.Begin).ToList();
                        var segment = sourceSemester.Dates.FirstOrDefault(x => x.HasCourses &&
                                                                               (x.Organiser != null &&
                                                                                   x.Organiser.Id ==
                                                                                   course.Organiser.Id) &&
                                                                               x.From.Date <= ordereDates.First().Begin
                                                                                   .Date &&
                                                                               ordereDates.Last().Begin.Date <=
                                                                               x.To.Date);

                        if (segment != null)
                        {
                            var planSegment = destSemester.Dates.FirstOrDefault(x =>
                                x.HasCourses &&
                                (x.Organiser != null && x.Organiser.Id == course.Organiser.Id) &&
                                x.Description.Equals(segment.Description));

                            if (planSegment != null)
                            {
                                var room = summary.GetFavoriteRoom();
                                var host = summary.GetFavoriteHost();

                                // auf den richtigen EntityTracker holen
                                if (room != null)
                                {
                                    var r = Db.Rooms.SingleOrDefault(x => x.Id == room.Id);
                                    favRooms.Add(r);
                                }

                                if (host != null)
                                {
                                    var h = Db.Members.SingleOrDefault(x => x.Id == host.Id);
                                    favHosts.Add(h);
                                }


                                var refDate = course.Dates.First();
                                var semesterStartTag = (int)planSegment.From.DayOfWeek;
                                var day = (int)refDate.Begin.DayOfWeek;
                                int nDays = day - semesterStartTag;
                                if (nDays < 0)
                                {
                                    nDays += 7;
                                }

                                var occDate = planSegment.From.AddDays(nDays);


                                //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
                                var numOcc = 0;
                                while (occDate <= planSegment.To)
                                {
                                    var isVorlesung = true;
                                    foreach (var sd in destSemester.Dates)
                                    {
                                        // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                                        if (sd.From.Date <= occDate.Date &&
                                            occDate.Date <= sd.To.Date &&
                                            sd.HasCourses == false)
                                        {
                                            isVorlesung = false;
                                        }
                                    }

                                    if (isVorlesung)
                                    {
                                        var ocStart = new DateTime(occDate.Year, occDate.Month, occDate.Day,
                                            refDate.Begin.Hour,
                                            refDate.Begin.Minute, refDate.Begin.Second);
                                        var ocEnd = new DateTime(occDate.Year, occDate.Month, occDate.Day,
                                            refDate.End.Hour,
                                            refDate.End.Minute, refDate.End.Second);

                                        var occ = new ActivityDate
                                        {
                                            Activity = planCourse,
                                            Begin = ocStart,
                                            End = ocEnd,
                                            Occurrence = new Occurrence
                                            {
                                                Capacity = -1,
                                                IsAvailable = true,
                                                FromIsRestricted = false,
                                                UntilIsRestricted = false,
                                                IsCanceled = false,
                                                IsMoved = false,
                                                UseGroups = false,
                                            },
                                        };


                                        foreach (var favRoom in favRooms)
                                        {
                                            occ.Rooms.Add(favRoom);
                                        }

                                        foreach (var favHost in favHosts)
                                        {
                                            occ.Hosts.Add(favHost);
                                        }

                                        planCourse.Dates.Add(occ);
                                        Db.ActivityDates.Add(occ);
                                        numOcc++;
                                    }

                                    occDate = occDate.AddDays(7);
                                }

                            }
                            else
                            {
                                // es werden keine Termine angelegt, wenn es kein Segment gibt
                            }
                        }
                    }
                }

                Db.Activities.Add(planCourse);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.CallStack = ex.StackTrace;

                return PartialView("_Error");
            }


            var model = new CopyCourseModel()
            {
                Subject = subject,
                OriginCourse = course,
                SourceSemester = sourceSemester,
                DestSemester = destSemester,
                WithDates = copyDates
            };

            return PartialView("_CourseCopied", model);
        }

        public ActionResult CreateCourse(Guid subjectId, Guid destSemId)
        {
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);
            var destSemester = SemesterService.GetSemester(destSemId);

            var model = new CreateSubjectCourseModel()
            {
                Subject = subject,
                DestSemester = destSemester
            };

            return View(model);
        }

    }
}