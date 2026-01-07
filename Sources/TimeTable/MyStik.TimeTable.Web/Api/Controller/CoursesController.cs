//using Ical.Net.DataTypes;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Migrations;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.CRUD;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/courses")]
    public class CoursesController : ApiBaseController
    {
        #region Course
        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        [ResponseType(typeof(List<CourseApiModel>))]

        public IHttpActionResult GetCourses(string organiser_id = "", string curriculum_id = "", string semester_id = "")
        {
            var list = new List<CourseApiModel>();

            Semester sem = null;
            if (string.IsNullOrEmpty(semester_id))
            {
                sem = new SemesterService().GetSemester(DateTime.Today);
            }
            else
            {
                sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester_id));
            }
            if (sem == null)
                return Ok(list);

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var courses = new List<Course>();

            if (string.IsNullOrEmpty(organiser_id) && string.IsNullOrEmpty(curriculum_id))
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                    x.Semester != null &&    
                    x.Semester.Id == sem.Id).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(organiser_id) && string.IsNullOrEmpty(curriculum_id))
                {
                    var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser_id));
                    if (org == null)
                        return Ok(list);
                    courses = Db.Activities.OfType<Course>().Where(x =>
                        x.Semester != null &&
                        x.Organiser != null &&
                        x.Semester.Id == sem.Id &&
                        x.Organiser.Id == org.Id).ToList();
                }
                if (!string.IsNullOrEmpty(curriculum_id))
                {
                    var curr = Db.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum_id));
                    if (curr == null)
                        return Ok(list);
                    foreach (var label in curr.LabelSet.ItemLabels.ToList())
                    {
                        var labeledCourses = Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester != null &&
                                x.Semester.Id == sem.Id &&
                                x.LabelSet != null &&
                                x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                            .ToList();
                        courses.AddRange(labeledCourses);
                    }
                    courses = courses.Distinct().ToList();
                }
            }

            foreach (var course in courses)
            {
                list.Add(converter.Convert_new(course));
            }
            return Ok(list);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [Route("{course_id}")]
        [ResponseType(typeof(CourseApiModel))]
        public async Task<IHttpActionResult> GetCourse(Guid course_id)
        {
            if (!(await Db.Activities.FindAsync(course_id) is Course course))
            {
                return NotFound();
            }

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var dto = converter.Convert_new(course, true, false);

            return Ok(dto);
        }


        [HttpPost]
        [Route("")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public async Task<IHttpActionResult> CreateCourse(string api_key, [FromBody] CourseCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            if (member.IsCourseAdmin == false)
            {
                return Unauthorized();
            }

            Course course = null;

            try
            {
                var semester = Db.Semesters.SingleOrDefault(x => x.Name.Equals(request.semester_id));

                // Semester muss angegeben sein
                if (semester == null)
                {
                    return BadRequest("Invalid semester");
                }

                // Angabe Einrichtung optional, muss aber zu api_key passen
                var org = string.IsNullOrEmpty(request.organiser_id) ? member.Organiser : Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(request.organiser_id));

                if (org.Id != member.Organiser.Id)
                {
                    return Unauthorized();
                }

                // Die Occurrnce des Kurses
                var occ = new Data.Occurrence
                {
                    Capacity = -1,
                    IsAvailable = false,
                    IsCanceled = false,
                    IsMoved = false,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    UseGroups = false,
                };

                course = new Course
                {
                    Semester = semester,
                    Organiser = org,
                    Name = request.title,
                    ShortName = request.code,
                    Description = request.description,
                    ExternalId = request.external_id,
                    ExternalSource = "API",
                    Occurrence = occ,
                };

                // Owner
                var owner = new ActivityOwner
                {
                    Activity = course,
                    Member = member,
                };

                course.Owners.Add(owner);

                // Labelset
                var labelSet = new ItemLabelSet();
                course.LabelSet = labelSet;

                // Entities in Tabellen ablegen
                Db.ItemLabelSets.Add(labelSet);
                Db.ActivityOwners.Add(owner);
                Db.Occurrences.Add(occ);
                Db.Activities.Add(course);

                // Kohorten
                if (request.cohortes != null)
                {
                    foreach (var cohorte in request.cohortes)
                    {
                        var labels = new List<ItemLabel>();
                        var msg = GetLabels(cohorte, labels);

                        if (!string.IsNullOrEmpty(msg))
                        {
                            return BadRequest(msg);
                        }


                        // aktuell müssen Label existieren
                        // neue Labels nur für eigene Einrichtung erlauben
                        foreach (var label in labels)
                        {
                            course.LabelSet.ItemLabels.Add(label);
                        }
                    }
                }

                // Kapazitäten
                if (request.quotas != null)
                {
                    foreach (var quota in request.quotas)
                    {
                        var seatQuota = new SeatQuota
                        {
                            Occurrence = course.Occurrence,
                            MinCapacity = 0,
                            MaxCapacity = quota.amount,
                            Fractions = new List<SeatQuotaFraction>(),
                        };

                        if (quota.cohortes != null)
                        {
                            foreach (var cohorte in quota.cohortes)
                            {
                                var quotaFraction = new SeatQuotaFraction
                                {
                                    Quota = seatQuota,
                                    ItemLabelSet = new ItemLabelSet()
                                };

                                // Labels suchen
                                var labels = new List<ItemLabel>();
                                var msg = GetLabels(cohorte, labels);

                                if (!string.IsNullOrEmpty(msg))
                                {
                                    return BadRequest(msg);
                                }

                                foreach (var label in labels)
                                {
                                    quotaFraction.ItemLabelSet.ItemLabels.Add(label);
                                }

                                seatQuota.Fractions.Add(quotaFraction);
                                Db.SeatQuotaFractions.Add(quotaFraction);
                            }
                        }

                        Db.SeatQuotas.Add(seatQuota);
                    }
                }




                // Termine => Sequenzen auswerten
                if (request.sequences != null) 
                {
                var semesterService = new SemesterService(Db);

                    foreach (var sequence in request.sequences)
                    {
                        var begin = sequence.first_begin.Date;
                        var end = sequence.last_end.Date;

                        var from = sequence.first_begin.TimeOfDay;
                        var until = sequence.last_end.TimeOfDay;

                        var date = begin;

                        while (date <= end)
                        {
                            // Check ob Vorlesungsfrei
                            var sem = semesterService.GetSemester(date);
                            if (sem == null)
                            {
                                return BadRequest("invalid date");
                            }

                            // nur an Vorlesungstagen
                            if (!semesterService.IsLectureDay(sem, org, date))
                            {
                                date = date.AddDays(sequence.frequency);
                                continue;
                            }

                            var courseDate = new Data.ActivityDate
                            {
                                Activity = course,
                                Title = sequence.title,
                                Description = "",
                                Begin = date.Add(from),
                                End = date.Add(until),
                                Occurrence = new Data.Occurrence
                                {
                                    Capacity = -1,
                                    IsAvailable = false,
                                    IsCanceled = false,
                                    IsMoved = false,
                                    FromIsRestricted = false,
                                    UntilIsRestricted = false,
                                    UseGroups = false,
                                }
                            };

                            // Räume
                            if (sequence.room_ids != null)
                            {
                                foreach (var roomNumber in sequence.room_ids)
                                {
                                    var room = Db.Rooms.SingleOrDefault(x => x.Number.Equals(roomNumber));
                                    if (room != null)
                                    {
                                        courseDate.Rooms.Add(room);
                                        // Raumbuchungen ergänzen!
                                    }
                                    else
                                    {
                                        return BadRequest($"Invalid room: {roomNumber}");
                                    }
                                }
                            }

                            // Hosts
                            if (sequence.lecturer_ids != null)
                            {
                                foreach (var host in sequence.lecturer_ids)
                                {
                                    var lecturer = course.Organiser.Members.FirstOrDefault(x => x.ShortName.Equals(host));
                                    if (lecturer != null)
                                    {
                                        courseDate.Hosts.Add(lecturer);
                                    }
                                    else
                                    {
                                        return BadRequest($"Invalid lecturer: {host}");
                                    }
                                }
                            }


                            Db.ActivityDates.Add(courseDate);
                            Db.Occurrences.Add(courseDate.Occurrence);

                            date = date.AddDays(sequence.frequency);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



            try
            {
                await Db.SaveChangesAsync();

                if (course != null)
                {
                    var response = new CourseApiResponseModel
                    {
                        course_id = course.Id,
                        message = "Course created successfully"
                    };

                    return Ok(response);
                }

                return BadRequest("unexpected error");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{course_id}")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public IHttpActionResult DeleteCourse(string api_key, Guid course_id)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == course_id);
            if (course == null)
                return NotFound();

            if (course.Organiser.Id != member.Organiser.Id)
            {
                return Unauthorized();
            }

            if (member.IsCourseAdmin == false)
            {
                return Unauthorized();
            }

            var service = new CourseDeleteService(Db);

            service.DeleteCourse(course_id);

            var response = new CourseApiResponseModel
            {
                course_id = course_id,
                message = "Course deleted successfully"
            };

            return Ok(response);
        }
        #endregion

        #region Dates
        [HttpGet]
        [Route("{course_id}/dates")]
        [ResponseType(typeof(CourseApiModel))]
        public async Task<IHttpActionResult> GetCourseDates(string api_key, Guid course_id)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            if (!(await Db.Activities.FindAsync(course_id) is Course course))
            {
                return NotFound();
            }

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var dto = converter.Convert_new(course, true, (member != null));

            return Ok(dto);
        }



        [HttpPost]
        [Route("{course_id}/dates")]
        [ResponseType(typeof(CourseDateApiResponseModel))]
        public async Task<IHttpActionResult> CreateCourseDate(string api_key, Guid course_id, [FromBody] CourseDateCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == course_id);
            if (course == null)
                return NotFound();

            if (course.Organiser.Id != member.Organiser.Id)
            {
                return Unauthorized();
            }


            var date = Db.ActivityDates.SingleOrDefault(x => x.Activity.Id == course_id && x.Begin == request.begin && x.End == request.end);
            if (date != null)
            {
                return BadRequest("Course date already exists");
            }

            // Eine Prüfung auf Vorlesungszeiten findet hier nicht statt
            // Das wird nur bei Sequenzen gemacht
            // Grund: es soll der spezifische Termin angelegt werden!

            var occ = new Data.Occurrence
            {
                Capacity = -1,
                IsAvailable = false,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
                UseGroups = false,
            };

            var courseDate = new Data.ActivityDate
            {
                Activity = course,
                Title = request.title,
                Description = request.description,
                Begin = request.begin,
                End = request.end,
                Occurrence = occ
            };

            // Räume
            if (request.room_ids != null)
            {
                foreach (var roomNumber in request.room_ids)
                {
                    var room = Db.Rooms.SingleOrDefault(x => x.Number.Equals(roomNumber));
                    if (room != null)
                    {
                        courseDate.Rooms.Add(room);
                        // Raumbuchungen ergänzen!
                    }
                    else
                    {
                        return BadRequest($"Invalid room: {roomNumber}");
                    }
                }
            }

            // Hosts
            if (request.lecturer_ids != null)
            {
                foreach (var host in request.lecturer_ids)
                {
                    var lecturer = course.Organiser.Members.FirstOrDefault(x => x.ShortName.Equals(host));
                    if (lecturer != null)
                    {
                        courseDate.Hosts.Add(lecturer);
                    }
                    else
                    {
                        return BadRequest($"Invalid lecturer: {host}");
                    }
                }
            }


            Db.Occurrences.Add(occ);
            Db.ActivityDates.Add(courseDate);

            await Db.SaveChangesAsync();

            var response = new CourseDateApiResponseModel
            {
                course_id = course_id,
                date_id = courseDate.Id,
                message = "Course date created successfully"
            };

            return Ok(response);
        }

        [HttpPatch]
        [Route("{course_id}/dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> UpdateCourseDate(string api_key, Guid course_id, [FromBody] CourseDateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }


            /*
            var course = new Course
            {
                Title = request.title,
                Description = request.description,
                ExternalId = request.externalId,
            };


            Db.Activities.Add(course);
            */
            await Db.SaveChangesAsync();

            request.id = Guid.NewGuid(); // course.Id;

            return Ok(request);
        }


        [HttpDelete]
        [Route("{course_id}/dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> DeleteCourseDate(string api_key, Guid course_id, [FromBody] CourseDateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }


            /*
            var course = new Course
            {
                Title = request.title,
                Description = request.description,
                ExternalId = request.externalId,
            };


            Db.Activities.Add(course);
            */
            await Db.SaveChangesAsync();

            request.id = Guid.NewGuid(); // course.Id;

            return Ok(request);
        }
        #endregion

        #region Subscriptions
        [HttpGet]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(List<CourseSubscriberApiModel>))]
        public async Task<IHttpActionResult> GetCourseSubscriptions(string api_key, Guid course_id)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == course_id);
            if (course == null)
            {
                return NotFound();
            }

            var studentService = new StudentService(Db);

            var subscribers = new List<CourseSubscriberApiModel>();

            var subscriptions = Db.Subscriptions.OfType<OccurrenceSubscription>().Where(x => x.Occurrence.Id == course.Occurrence.Id).ToList();
            foreach (var subscription in subscriptions)
            {
                var user = await UserManager.FindByIdAsync(subscription.UserId);
                if (user != null)
                {
                    var student = studentService.GetCurrentStudent(user.Id).FirstOrDefault();

                    var subscriber = new CourseSubscriberApiModel
                    {
                        user_id = user.Claims.FirstOrDefault(c => c.ClaimType == "eduPersonPrincipalName")?.ClaimValue,
                        matriculation_number = student != null ? student.Number : string.Empty,
                        subscrition_date = subscription.TimeStamp,
                        on_waiting_list = subscription.OnWaitingList,
                    };
                    subscribers.Add(subscriber);
                }
            }

            return Ok(subscribers);
        }


        [HttpPost]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriptionApiModel))]
        public async Task<IHttpActionResult> CreateCourseSubscription(string api_key, Guid course_id, [FromBody]CourseSubscriptionCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == course_id);
            if (course == null)
            {
                return NotFound();
            }

            ApplicationUser user = null;

            // Die user_id schlägt die matriculation_number
            if (!string.IsNullOrEmpty(request.matriculation_number) && string.IsNullOrEmpty(request.user_id))
            {
                var students = Db.Students.Where(x => !string.IsNullOrEmpty(x.Number) && x.Number.Equals(request.matriculation_number)).ToList();
                if (students.Count != 1)
                {
                    return BadRequest("unambiguous matriculation number");
                }
                user = await UserManager.FindByIdAsync(students[0].UserId);
            }

            if (!string.IsNullOrEmpty(request.user_id))
            {
                var userDb = new ApplicationDbContext();
                var users = userDb.Users.Where(x => x.Claims.Any(c => c.ClaimType == "eduPersonPrincipalName" && c.ClaimValue.Equals(request.user_id))).ToList();

                if (users.Count != 1)
                {
                    return BadRequest("unambiguous user_id");
                }
                user = users[0];
            }

            if (user == null)
            {
                return NotFound();
            }

            var subscription = new OccurrenceSubscription
            {
                Occurrence = course.Occurrence,
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                Priority = 1,
                OnWaitingList = false,
                SubscriberRemark = $"Über api_key von {member.FullName} eingeschrieben"
            };
            Db.Subscriptions.Add(subscription);

            await Db.SaveChangesAsync();


            var response = new CourseSubscriptionApiModel
            {
                course_id = course.Id,
                user_id = request.user_id,
                matriculation_number = request.matriculation_number,
                subscription_date = subscription.TimeStamp,
            };


            return Ok(response);
        }

        /*
        [HttpPatch]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> UpdateCourseSubscription(string api_key, Guid course_id, [FromBody] CourseSubscriberApiModel request)
        {
            var authHeader = api_key;

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(authHeader);
            if (member == null)
            {
                return Unauthorized();
            }


            await Db.SaveChangesAsync();

            request.user_id = Guid.NewGuid().ToString(); // course.Id;

            return Ok(request);
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api_key">9bd6e09da20f4eb794740c430fa3bc3c</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public async Task<IHttpActionResult> DeleteCourseSubscription(string api_key, Guid course_id, [FromBody] CourseSubscriptionCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == course_id);
            if (course == null)
            {
                return NotFound();
            }

            ApplicationUser user = null;

            // Die user_id schlägt die matriculation_number
            if (!string.IsNullOrEmpty(request.matriculation_number) && string.IsNullOrEmpty(request.user_id))
            {
                var students = Db.Students.Where(x => !string.IsNullOrEmpty(x.Number) && x.Number.Equals(request.matriculation_number)).ToList();
                if (students.Count != 1)
                {
                    return BadRequest("unambiguous matriculation number");
                }
                user = await UserManager.FindByIdAsync(students[0].UserId);
            }

            if (!string.IsNullOrEmpty(request.user_id))
            {
                var userDb = new ApplicationDbContext();
                var users = userDb.Users.Where(x => x.Claims.Any(c => c.ClaimType == "eduPersonPrincipalName" && c.ClaimValue.Equals(request.user_id))).ToList();

                if (users.Count != 1)
                {
                    return BadRequest("unambiguous user_id");
                }
                user = users[0];
            }

            if (user == null)
            {
                return NotFound();
            }

            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Occurrence.Id == course.Occurrence.Id && x.UserId.Equals(user.Id));
            if (subscription == null)
            {
                return NotFound();
            }

            var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
            foreach (var drawing in allDrawings)
            {
                Db.SubscriptionDrawings.Remove(drawing);
            }

            course.Occurrence.Subscriptions.Remove(subscription);
            Db.Subscriptions.Remove(subscription);


            await Db.SaveChangesAsync();

            var response = new CourseApiResponseModel
            {
                course_id = course_id,
                message = "Subscription deleted successfully"
            };

            return Ok(response);
        }
        #endregion

        #region Helpers
        private string GetLabels(CourseCohorte cohorte, List<ItemLabel> labels)
        {
            if (string.IsNullOrEmpty(cohorte.label))
                return "Cohorte label is required";

            var inst = Db.Institutions.FirstOrDefault(x => x.Tag.Equals(cohorte.institution_id));
            if (inst == null)
                return "Invalid Institution";


            var targetOrg = inst.Organisers.FirstOrDefault(x => x.ShortName.Equals(cohorte.organiser_id));
            if (targetOrg == null)
            {
                // Label auf Ebene Institution
                var label = inst.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohorte.label));
                if (label != null)
                {
                    labels.Add(label);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(cohorte.curriculum_id) && string.IsNullOrEmpty(cohorte.curriculum_alias))
                {
                    // Label auf Ebene Einrichtung
                    var label = targetOrg.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohorte.label));
                    if (label != null)
                    {
                        labels.Add(label);
                    }
                }
                else
                {
                    // Label auf Ebene Studiengang bzw. Studiengänge, wenn mehrere SPOs gemeint durch Angabe des Alias
                    if (string.IsNullOrEmpty(cohorte.curriculum_id))
                    {
                        var currs = Db.Curricula.Where(x => x.Organiser.Id == targetOrg.Id && x.ShortName.Equals(cohorte.curriculum_alias)).ToList();
                        foreach (var curr in currs)
                        {
                            var label = curr.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohorte.label));
                            if (label != null)
                            {
                                labels.Add(label);
                            }
                        }
                    }
                    else
                    {
                        var curr = Db.Curricula.SingleOrDefault(x => x.Organiser.Id == targetOrg.Id && x.ID.Equals(cohorte.curriculum_id));
                        if (curr == null)
                            return "Invalid Curriculum for Cohorte";
                        var label = curr.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohorte.label));
                        if (label != null)
                        {
                            labels.Add(label);
                        }
                    }
                }
            }
            
            return string.Empty;
        }
        #endregion
    }
}