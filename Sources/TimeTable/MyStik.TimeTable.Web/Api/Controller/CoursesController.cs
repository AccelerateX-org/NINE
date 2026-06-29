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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.DataServices.IO.Contracts;
using Curriculum = MyStik.TimeTable.Data.Curriculum;

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
        [ResponseType(typeof(List<CourseApiContract>))]

        public IHttpActionResult GetCourses(string institution, string organiser = "", string code = "", string semester = "")
        {
            var list = new List<CourseApiContract>();
            Semester sem = null;
            sem = string.IsNullOrEmpty(semester) ? 
                new SemesterService().GetSemester(DateTime.Today) : 
                Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return Ok(list);

            var inst = Db.Institutions.FirstOrDefault(x => x.Tag.ToUpper().Equals(institution.ToUpper()));
            if (inst == null)
                return Ok(list);


            const bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            List<Course> courses;

            if (string.IsNullOrEmpty(organiser))
            {
                courses = Db.Activities.OfType<Course>()
                    .Where(x =>
                        x.Organiser != null && x.Organiser.Institution.Id == inst.Id &&
                        x.Semester != null && x.Semester.Id == sem.Id &&
                        (string.IsNullOrEmpty(code) || x.ShortName.Equals(code)))
                    .ToList();
            }
            else
            {
                var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser));
                if (org == null)
                    return Ok(list);

                courses = Db.Activities.OfType<Course>().Where(x =>
                    x.Semester != null && x.Organiser != null &&
                    x.Semester.Id == sem.Id && x.Organiser.Id == org.Id &&
                    (string.IsNullOrEmpty(code) || x.ShortName.Equals(code))).ToList();
            }

            list.AddRange(courses.Select(course => converter.Convert_new(course)));
            
            return Ok(list);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CourseApiContract))]
        public async Task<IHttpActionResult> GetCourse(Guid id)
        {
            if (!(await Db.Activities.FindAsync(id) is Course course))
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
        public async Task<IHttpActionResult> CreateCourse(string apiKey, [FromBody] CourseApiContract request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return BadRequest("unknown user");
            }

            if (member.IsCourseAdmin == false)
            {
                return BadRequest("no access rights");
            }

            if (string.IsNullOrEmpty(request.InstitutionId))
            {
                return BadRequest("Missing institution");
            }

            if (string.IsNullOrEmpty(request.OrganiserId))
            {
                return BadRequest("Missing organiser");
            }

            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest("Missing course code");
            }

            if (string.IsNullOrEmpty(request.SemesterId))
            {
                return BadRequest("Missing semester");
            }

            var semesterName = request.SemesterId;
            var semesterSegment = string.Empty;

            if (request.SemesterId.Contains(":"))
            {
                var words = request.SemesterId.Split(':');
                if (words.Length != 2)
                {
                    return BadRequest("Invalid semester format");
                }
                semesterName = words[0];
                semesterSegment = words[1];
            }

            var semester = Db.Semesters.Include(semester1 => semester1.Dates).SingleOrDefault(x => x.Name.Equals(semesterName));
            // Semester muss angegeben sein
            if (semester == null)
            {
                return BadRequest("Missing or invalid semester");
            }

            var segment = string.IsNullOrEmpty(semesterSegment) ? null :  semester.Dates.FirstOrDefault(x => x.Description.Equals(semesterSegment));

            var inst = Db.Institutions.FirstOrDefault(x => x.Tag.Equals(request.InstitutionId));
            if (inst == null)
            {
                return BadRequest("missing institution");
            }

            var org = Db.Organisers.Include(activityOrganiser => activityOrganiser.Members).FirstOrDefault(x =>
                x.Institution.Id == inst.Id && x.ShortName.Equals(request.OrganiserId));

            if (org == null)
            {
                return BadRequest("missing organisation in institution");
            }

            // Angabe Einrichtung optional muss aber zu api_key passen
            if (member.Organiser.Id != org.Id)
            {
                return BadRequest("mismatch apiKey and organiser membership");
            }

            if (segment != null)
            {
                var courseCheck = Db.Activities.OfType<Course>().FirstOrDefault(x =>
                    x.Segment.Id != null && x.Segment.Id == segment.Id &&
                    x.Semester.Id != null && x.Semester.Id == semester.Id &&
                    x.Organiser != null && x.Organiser.Id == org.Id &&
                    x.ShortName.Equals(request.Code));

                if (courseCheck != null)
                {
                    return BadRequest("a course with this code exists");
                }
            }
            else
            {
                var courseCheck = Db.Activities.OfType<Course>().FirstOrDefault(x =>
                    x.Semester.Id != null && x.Semester.Id == semester.Id &&
                    x.Organiser != null && x.Organiser.Id == org.Id &&
                    x.ShortName.Equals(request.Code));

                if (courseCheck != null)
                {
                    return BadRequest("a course with this code exists");
                }
            }

            Course course = null;
            var warnings = new List<string>();

            try
            {

                // Die Occurrence des Kurses
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
                    Semester = semester,                // ist redundant, da sich das Semester auch über die Segmentzuordnung ergibt, aber es erleichtert die Suche nach Kursen in einem Semester
                    Segment = segment,                  // das kann auch null sein
                    Organiser = org,                    // erforderlich, da die Zugehörigkeit zu einer Einrichtung definiert werden muss
                    Name = request.Title,               // optional
                    ShortName = request.Code,           // erforderlich
                    Description = request.Description,  // optional
                    ExternalId = request.ExternalId,    // optional
                    ExternalSource = "API",             // Hard coded gesetzt
                    Occurrence = occ,
                };

                // Owner ist der Member, der den Call macht
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


                // Kohorten: Fehlende Kohorten werden angelegt
                if (request.Cohorts != null)
                {
                    foreach (var cohort in request.Cohorts)
                    {
                        var currLabel = GetLabel(cohort);

                        if (currLabel?.Label != null)
                        {
                            course.LabelSet.ItemLabels.Add(currLabel.Label);
                        }
                        else
                        {
                            warnings.Add($"invalid cohort {cohort}");
                        }
                    }
                }

                // Kapazitäten
                if (request.Quotas != null)
                {
                    foreach (var quota in request.Quotas)
                    {
                        var seatQuota = new SeatQuota
                        {
                            Occurrence = course.Occurrence,
                            MinCapacity = 0,
                            MaxCapacity = quota.Capacity,
                            Fractions = new List<SeatQuotaFraction>(),
                        };

                        if (quota.Cohorts != null)
                        {
                            foreach (var cohort in quota.Cohorts)
                            {
                                var quotaFraction = new SeatQuotaFraction
                                {
                                    Quota = seatQuota,
                                    ItemLabelSet = new ItemLabelSet()
                                };

                                // Labels suchen
                                var currLabel = GetLabel(cohort);
                                if (currLabel == null || currLabel.Label == null)
                                {
                                    warnings.Add($"invalid cohort {cohort}");
                                }
                                else
                                {
                                    quotaFraction.ItemLabelSet.ItemLabels.Add(currLabel.Label);
                                    quotaFraction.Curriculum = currLabel.Curriculum;
                                    seatQuota.Fractions.Add(quotaFraction);

                                    Db.ItemLabelSets.Add(quotaFraction.ItemLabelSet);
                                    Db.SeatQuotaFractions.Add(quotaFraction);
                                }
                            }
                        }
                        Db.SeatQuotas.Add(seatQuota);
                    }
                }

                // Termine => Einzeltermine auswerten
                var semesterService = new SemesterService(Db);


                if (request.Dates != null)
                {
                    foreach (var apiDateContract in request.Dates)
                    {
                        var begin = apiDateContract.Begin;
                        var end = apiDateContract.End;

                        var sem = semesterService.GetSemester(begin);
                        if (sem == null)
                        {
                            return BadRequest("invalid date");
                        }
                        // nur an Vorlesungstagen
                        if (!semesterService.IsLectureDay(sem, org, begin))
                        {
                            continue;
                        }

                        var courseDate = new Data.ActivityDate
                        {
                            Activity = course,
                            Title = apiDateContract.Title,
                            Description = apiDateContract.Description,
                            Begin = begin,
                            End = end,
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
                        if (apiDateContract.Rooms != null)
                        {
                            foreach (var rrr in apiDateContract.Rooms)
                            {
                                var roomNumber = rrr.Trim();
                                var room = Db.Rooms.Include(room1 =>
                                    room1.Assignments.Select(roomAssignment => roomAssignment.Organiser)).SingleOrDefault(x => x.Number.Equals(roomNumber));
                                if (room == null)
                                {
                                    warnings.Add($"unknown room: {roomNumber}");
                                }
                                else
                                {
                                    var assignment = room.Assignments.FirstOrDefault(x => x.Organiser.Id == org.Id);

                                    if (assignment != null)
                                    {
                                        // Raumbuchung ergänzen!
                                        courseDate.Rooms.Add(room);
                                    }
                                    else
                                    {
                                        warnings.Add($"Room {roomNumber} is not assigned to organiser: {org.ShortName}");
                                    }
                                }
                            }
                        }

                        // Hosts
                        if (apiDateContract.Hosts != null)
                        {
                            foreach (var hhh in apiDateContract.Hosts)
                            {
                                var host = hhh.Trim();
                                var lecturer = org.Members
                                    .FirstOrDefault(x => x.FullName.Equals(host) ||
                                                         x.ShortName.Equals(host));
                                /*
                                if (lecturer == null)
                                    return BadRequest($"lecturer: {host} not available for organiser: {org.ShortName}");

                                courseDate.Hosts.Add(lecturer);
                                */
                                if (lecturer != null)
                                {
                                    courseDate.Hosts.Add(lecturer);
                                }
                                else
                                {
                                    warnings.Add($"lecturer: {host} not available for organiser: {org.ShortName}");
                                }
                            }
                        }

                        Db.ActivityDates.Add(courseDate);
                        Db.Occurrences.Add(courseDate.Occurrence);
                    }
                }


                // Termine => Sequenzen auswerten
                if (request.Sequences != null) 
                {

                    foreach (var sequence in request.Sequences)
                    {
                        var begin = sequence.FirstBegin.Date;
                        var end = sequence.LastEnd.Date;

                        var from = sequence.FirstBegin.TimeOfDay;
                        var until = sequence.LastEnd.TimeOfDay;

                        var date = begin;
                        var frequency = sequence.Frequency > 0 ? sequence.Frequency : 7; // default: wöchentlich

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
                                date = date.AddDays(frequency);
                                continue;
                            }

                            var courseDate = new Data.ActivityDate
                            {
                                Activity = course,
                                Title = sequence.Title,
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
                            if (sequence.RoomIds != null)
                            {
                                foreach (var rrr in sequence.RoomIds)
                                {
                                    var roomNumber = rrr.Trim();
                                    var room = Db.Rooms.SingleOrDefault(x => x.Number.Equals(roomNumber));
                                    if (room == null)
                                    {
                                        warnings.Add($"unknown room: {roomNumber}");
                                    }
                                    else
                                    {
                                        var assignment = room.Assignments.FirstOrDefault(x => x.Organiser.Id == org.Id);

                                        if (assignment != null)
                                        {
                                            // Raumbuchung ergänzen!
                                            courseDate.Rooms.Add(room);
                                        }
                                        else
                                        {
                                            warnings.Add($"Room {roomNumber} is not assigned to organiser: {org.ShortName}");
                                        }
                                    }
                                }
                            }

                            // Hosts
                            if (sequence.LecturerIds != null)
                            {
                                foreach (var hhh in sequence.LecturerIds)
                                {
                                    var host = hhh.Trim();
                                    var lecturer = org.Members
                                        .FirstOrDefault(x => x.ShortName.Equals(host) ||
                                                             x.Name.Equals(host));
                                    if (lecturer != null)
                                    {
                                        courseDate.Hosts.Add(lecturer);
                                    }
                                    else
                                    {
                                        warnings.Add($"lecturer: {host} not available for organiser: {org.ShortName}");
                                        /*
                                        // warum Lehrende anlegen? Es könnte sich um einen Gastdozenten handeln, der nicht in der Einrichtung verankert ist.
                                        // In diesem Fall könnte man den Lehrenden anlegen und mit einem speziellen Flag versehen,
                                        // damit er später identifiziert und ggf. gelöscht werden kann.
                                        lecturer = new OrganiserMember()
                                        {
                                            Name = host,
                                            ShortName = host,
                                            Organiser = course.Organiser,
                                        };
                                        org.Members.Add(lecturer);
                                        Db.Members.Add(lecturer);

                                        // return BadRequest($"Invalid lecturer: {host}");
                                        */
                                    }
                                }
                            }


                            Db.ActivityDates.Add(courseDate);
                            Db.Occurrences.Add(courseDate.Occurrence);

                            date = date.AddDays(frequency);
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
                        CourseId = course.Id,
                        Message = "Course created successfully",
                        Warnings = warnings.ToList()
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
        [Route("{courseId}")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public IHttpActionResult DeleteCourse(string apiKey, Guid courseId)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
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

            service.DeleteCourse(courseId);

            var response = new CourseApiResponseModel
            {
                CourseId = courseId,
                Message = "Course deleted successfully"
            };

            return Ok(response);
        }
        #endregion

        #region Dates
        [HttpGet]
        [Route("{courseId}/dates")]
        [ResponseType(typeof(CourseApiContract))]
        public async Task<IHttpActionResult> GetCourseDates(string apiKey, Guid courseId)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            if (!(await Db.Activities.FindAsync(courseId) is Course course))
            {
                return NotFound();
            }

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var dto = converter.Convert_new(course, true, (member != null));

            return Ok(dto);
        }



        [HttpPost]
        [Route("{courseId}/dates")]
        [ResponseType(typeof(CourseDateApiResponseModel))]
        public async Task<IHttpActionResult> CreateCourseDate(string apiKey, Guid courseId, [FromBody] CourseApiDateContract request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = Db.Activities.OfType<Course>().Include(activity => activity.Organiser.Members).SingleOrDefault(x => x.Id == courseId);
            if (course == null)
                return NotFound();

            if (course.Organiser.Id != member.Organiser.Id)
            {
                return BadRequest("mismatch apiKey and organiser membership");
            }

            var org = course.Organiser;

            var date = Db.ActivityDates.SingleOrDefault(x => x.Activity.Id == courseId && x.Begin == request.Begin && x.End == request.End);
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
                Title = request.Title,
                Description = request.Description,
                Begin = request.Begin,
                End = request.End,
                Occurrence = occ
            };

            // Räume
            if (request.Rooms != null)
            {
                foreach (var rrr in request.Rooms)
                {
                    var roomNumber = rrr.Trim();
                    var room = Db.Rooms.Include(room1 =>
                        room1.Assignments.Select(roomAssignment => roomAssignment.Organiser)).SingleOrDefault(x => x.Number.Equals(roomNumber));
                    if (room == null)
                        return BadRequest($"unknown room: {roomNumber}");

                    var assignment = room.Assignments.FirstOrDefault(x => x.Organiser.Id == org.Id);

                    if (assignment == null)
                        return BadRequest($"room: {roomNumber} not available for organiser: {org.ShortName}");

                    // Raumbuchung ergänzen!
                    courseDate.Rooms.Add(room);
                }
            }

            // Hosts
            if (request.Hosts != null)
            {
                foreach (var hhh in request.Hosts)
                {
                    var host = hhh.Trim();
                    var lecturer = org.Members
                        .FirstOrDefault(x => x.FullName.Equals(host) ||
                                             x.ShortName.Equals(host));
                    if (lecturer == null)
                        return BadRequest($"lecturer: {host} not available for organiser: {org.ShortName}");

                    courseDate.Hosts.Add(lecturer);
                }
            }


            Db.Occurrences.Add(occ);
            Db.ActivityDates.Add(courseDate);

            await Db.SaveChangesAsync();

            var response = new CourseDateApiResponseModel
            {
                CourseId = courseId,
                DateId = courseDate.Id,
                Message = "Course date created successfully"
            };

            return Ok(response);
        }

        [HttpPatch]
        [Route("{courseId}/dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> UpdateCourseDate(string apiKey, Guid courseId, [FromBody] CourseDateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
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

            request.Id = Guid.NewGuid(); // course.Id;

            return Ok(request);
        }


        [HttpDelete]
        [Route("{courseId}/dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> DeleteCourseDate(string apiKey, Guid courseId, [FromBody] CourseDateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
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

            request.Id = Guid.NewGuid(); // course.Id;

            return Ok(request);
        }
        #endregion

        #region Subscriptions
        [HttpGet]
        [Route("{courseId}/subscriptions")]
        [ResponseType(typeof(List<CourseApiSubscriptionContract>))]
        public async Task<IHttpActionResult> GetCourseSubscriptions(string apiKey, Guid courseId)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == courseId);
            if (course == null)
            {
                return NotFound();
            }

            var studentService = new StudentService(Db);

            var subscribers = new List<CourseApiSubscriptionContract>();

            var subscriptions = Db.Subscriptions.OfType<OccurrenceSubscription>().Where(x => x.Occurrence.Id == course.Occurrence.Id).ToList();
            foreach (var subscription in subscriptions)
            {
                var user = await UserManager.FindByIdAsync(subscription.UserId);
                if (user != null)
                {
                    var student = studentService.GetCurrentStudent(user.Id).FirstOrDefault();

                    var subscriber = new CourseApiSubscriptionContract
                    {
                        UserId = user.Claims.FirstOrDefault(c => c.ClaimType == "eduPersonPrincipalName")?.ClaimValue,
                        MatriculationNumber = student != null ? student.Number : string.Empty,
                        SubscriptionDate = subscription.TimeStamp,
                        OnWaitingList = subscription.OnWaitingList,
                    };
                    subscribers.Add(subscriber);
                }
            }

            return Ok(subscribers);
        }


        [HttpPost]
        [Route("{courseId}/subscriptions")]
        [ResponseType(typeof(CourseSubscriptionApiModel))]
        public async Task<IHttpActionResult> CreateCourseSubscription(string apiKey, Guid courseId, [FromBody]CourseSubscriptionCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == courseId);
            if (course == null)
            {
                return NotFound();
            }

            ApplicationUser user = null;

            // Die user_id schlägt die matriculation_number
            if (!string.IsNullOrEmpty(request.MatriculationNumber) && string.IsNullOrEmpty(request.UserId))
            {
                var students = Db.Students.Where(x => !string.IsNullOrEmpty(x.Number) && x.Number.Equals(request.MatriculationNumber)).ToList();
                if (students.Count != 1)
                {
                    return BadRequest("unambiguous matriculation number");
                }
                user = await UserManager.FindByIdAsync(students[0].UserId);
            }

            if (!string.IsNullOrEmpty(request.UserId))
            {
                var userDb = new ApplicationDbContext();
                var users = userDb.Users.Where(x => x.Claims.Any(c => c.ClaimType == "eduPersonPrincipalName" && c.ClaimValue.Equals(request.UserId))).ToList();

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
                CourseId = course.Id,
                UserId = request.UserId,
                MatriculationNumber = request.MatriculationNumber,
                SubscriptionDate = subscription.TimeStamp,
            };


            return Ok(response);
        }

        /*
        [HttpPatch]
        [Route("{courseId}/subscriptions")]
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
        /// <param name="apiKey">9bd6e09da20f4eb794740c430fa3bc3c</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{courseId}/subscriptions")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public async Task<IHttpActionResult> DeleteCourseSubscription(string apiKey, Guid courseId, [FromBody] CourseSubscriptionCreateApiModel request)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(apiKey);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = await Db.Activities.OfType<Course>().SingleOrDefaultAsync(x => x.Id == courseId);
            if (course == null)
            {
                return NotFound();
            }

            ApplicationUser user = null;

            // Die user_id schlägt die matriculation_number
            if (!string.IsNullOrEmpty(request.MatriculationNumber) && string.IsNullOrEmpty(request.UserId))
            {
                var students = Db.Students.Where(x => !string.IsNullOrEmpty(x.Number) && x.Number.Equals(request.MatriculationNumber)).ToList();
                if (students.Count != 1)
                {
                    return BadRequest("unambiguous matriculation number");
                }
                user = await UserManager.FindByIdAsync(students[0].UserId);
            }

            if (!string.IsNullOrEmpty(request.UserId))
            {
                var userDb = new ApplicationDbContext();
                var users = userDb.Users.Where(x => x.Claims.Any(c => c.ClaimType == "eduPersonPrincipalName" && c.ClaimValue.Equals(request.UserId))).ToList();

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
                CourseId = courseId,
                Message = "Subscription deleted successfully"
            };

            return Ok(response);
        }
        #endregion

        #region Helpers
        private CurriculumLabel GetLabel(CourseApiCohortContract cohort)
        {
            // Varianten
            // Inst:Label | gehört zur Institution
            // Inst:Org:Label | gehört zur Einrichtung
            // Inst:Org:Curr:Label | dann darf es nur eine Version geben
            // Inst:Org:Curr:Version:Label | es muss diese Version geben
            var currLabel = new CurriculumLabel();

            if (string.IsNullOrEmpty(cohort.Label))
                return currLabel;

            var inst = Db.Institutions.Include(institution => institution.LabelSet.ItemLabels).Include(institution1 => institution1.Organisers.Select(activityOrganiser =>
                activityOrganiser.LabelSet.ItemLabels)).Include(institution => institution.Organisers.Select(activityOrganiser1 =>
                activityOrganiser1.Curricula.Select(curriculum => curriculum.LabelSet.ItemLabels))).FirstOrDefault(x => x.Tag.Equals(cohort.InstitutionId));
            if (inst == null)
                return currLabel; // "Invalid Institution";

            if (string.IsNullOrEmpty(cohort.OrganiserId))
            {
                // es wird aktuell kein neues Label angelegt
                currLabel.Label = inst.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohort.Label));
                return currLabel;
            }

            var targetOrg = inst.Organisers.FirstOrDefault(x => x.ShortName.Equals(cohort.OrganiserId));
            if (targetOrg == null)
                return currLabel;

            // zuerst den Alias prüfen
            Curriculum curr = null;
            if (!string.IsNullOrEmpty(cohort.CurriculumAlias))
            {
                curr = targetOrg.Curricula.FirstOrDefault(x => x.ShortName.Equals(cohort.CurriculumAlias));
                currLabel.Curriculum = curr;
                currLabel.Label = curr?.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohort.Label));
                return currLabel;
            }

            // jetzt die SPO
            if (string.IsNullOrEmpty(cohort.CurriculumId))
                return null;

            if (cohort.CurriculumDate.HasValue)
            {
                curr = targetOrg.Curricula.FirstOrDefault(x =>
                    x.Tag.Equals(cohort.CurriculumId) &&
                    x.StatuteTakeEffect.HasValue && x.StatuteTakeEffect.Value == cohort.CurriculumDate.Value);
            }
            else
            {
                curr = targetOrg.Curricula.FirstOrDefault(x =>
                    x.Tag.Equals(cohort.CurriculumId));
            }

            currLabel.Curriculum = curr;
            currLabel.Label = curr?.LabelSet.ItemLabels.SingleOrDefault(x => x.Name.Equals(cohort.Label));
            return currLabel;
        }
        #endregion
    }

    public class CurriculumLabel
    {
        public Curriculum Curriculum { get; set; }
        public ItemLabel Label { get; set; }
    }
}