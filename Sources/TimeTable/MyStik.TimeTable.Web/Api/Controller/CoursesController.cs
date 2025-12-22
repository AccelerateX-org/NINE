//using Ical.Net.DataTypes;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Migrations;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System;
using System.Collections.Generic;
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
    public class SubscriptionBasketModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDto User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<SubscriptionCourseModel> Courses { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionCourseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/courses")]
    public class CoursesController : ApiBaseController
    {

        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        public IQueryable<CourseApiModel> GetCourses(string organiser_id = "", string curriculum_id = "", string semester_id = "")
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
                return list.AsQueryable();

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
                        return list.AsQueryable();
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
                        return list.AsQueryable();
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
            return list.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        [Route("{course_id}")]
        [ResponseType(typeof(CourseApiModel))]
        public async Task<IHttpActionResult> GetCourse(Guid course_id, string api_key = "")
        {
            if (!(await Db.Activities.FindAsync(course_id) is Course course))
            {
                return NotFound();
            }

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var dto = converter.Convert_new(course, true, (member != null));

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

            var semester = Db.Semesters.SingleOrDefault(x => x.Name.Equals(request.semester));
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(request.organiser));

            if (semester == null || org == null)
            {
                return BadRequest("Invalid organiser or semester");
            }

            if (org.Id != member.Organiser.Id)
            {
                return Unauthorized();
            }

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

            var course = new Course
            {
                Semester = semester,
                Organiser = org,
                Name = request.title,
                ShortName = request.code,
                Description = request.description,
                ExternalId = request.external_id,
                Occurrence = occ
            };
            Db.Occurrences.Add(occ);
            Db.Activities.Add(course);

            // Kohorten

            // Kapazitäten

            // Termine


            await Db.SaveChangesAsync();

            var response = new CourseApiResponseModel
            {
                course_id = Guid.NewGuid(), // course.Id,
                external_id = request.external_id,
                message = "Course created successfully"
            };  

            return Ok(response);
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
            // Verhindert Redirects bei Unauthorized (401) Antworten => funktioniert so nicht
            // dann erst mal raus
            // HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            /*
            Request.Headers.TryGetValues("Authorization", out var headers);
            if (headers == null)
            {
                return Unauthorized();
            }

            var authHeader = headers.FirstOrDefault();
            if (authHeader == null)
            {
                return Unauthorized();
            }
            */

            var authHeader = api_key;

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(authHeader);
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
            // Verhindert Redirects bei Unauthorized (401) Antworten => funktioniert so nicht
            // dann erst mal raus
            // HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            /*
            Request.Headers.TryGetValues("Authorization", out var headers);
            if (headers == null)
            {
                return Unauthorized();
            }

            var authHeader = headers.FirstOrDefault();
            if (authHeader == null)
            {
                return Unauthorized();
            }
            */

            var authHeader = api_key;

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(authHeader);
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


        [HttpPost]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriptionApiModel))]
        public async Task<IHttpActionResult> CreateCourseSubscription(string api_key, Guid course_id, string student_id)
        {
            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(api_key);
            if (member == null)
            {
                return Unauthorized();
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == course_id);
            if (course == null)
            {
                return NotFound();
            }

            var students = Db.Students.Where(x => !string.IsNullOrEmpty(x.Number) && x.Number.Equals(student_id)).ToList();
            if (students.Count > 1)
            {
                return NotFound();
            }

            Student student = null;
            if (students.Count == 0)
            {
                var userDb = new ApplicationDbContext();

                var users = userDb.Users.Where(x => x.Claims.Any(c => c.ClaimType == "eduPersonPrincipalName" && c.ClaimValue.Equals(student_id))).ToList();

                if (users.Count != 1)
                {
                    return NotFound();
                }
                
                student = Db.Students.FirstOrDefault(x => x.UserId.Equals(users[0].Id));
            }
            else
            {
                student = students.FirstOrDefault();
            }

            if (student == null)
            {
                return NotFound();
            }

            var subscription = new OccurrenceSubscription
            {
                Occurrence = course.Occurrence,
                UserId = student.UserId,
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
                subscribed = subscription.TimeStamp,
                onWaitingList = subscription.OnWaitingList
            };


            return Ok(response);
        }

        [HttpPatch]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> UpdateCourseSubscription(string api_key, Guid course_id, [FromBody] CourseSubscriberApiModel request)
        {
            // Verhindert Redirects bei Unauthorized (401) Antworten => funktioniert so nicht
            // dann erst mal raus
            // HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            /*
            Request.Headers.TryGetValues("Authorization", out var headers);
            if (headers == null)
            {
                return Unauthorized();
            }

            var authHeader = headers.FirstOrDefault();
            if (authHeader == null)
            {
                return Unauthorized();
            }
            */

            var authHeader = api_key;

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(authHeader);
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

            request.id = Guid.NewGuid().ToString(); // course.Id;

            return Ok(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api_key">9bd6e09da20f4eb794740c430fa3bc3c</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{course_id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> DeleteCourseSubscription(string api_key, Guid course_id, [FromBody] CourseSubscriberApiModel request)
        {
            // Verhindert Redirects bei Unauthorized (401) Antworten => funktioniert so nicht
            // dann erst mal raus
            // HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
            /*
            Request.Headers.TryGetValues("Authorization", out var headers);
            if (headers == null)
            {
                return Unauthorized();
            }

            var authHeader = headers.FirstOrDefault();
            if (authHeader == null)
            {
                return Unauthorized();
            }
            */

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

            request.id = Guid.NewGuid().ToString(); // course.Id;

            return Ok(request);
        }
    }
}