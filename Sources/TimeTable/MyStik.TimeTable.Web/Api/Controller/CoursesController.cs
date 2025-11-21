using Ical.Net.DataTypes;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Migrations;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;
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
        public IQueryable<CourseApiModel> GetCourses(string organiser = "", string curriculum = "", string semester = "")
        {
            var list = new List<CourseApiModel>();

            Semester sem = null;
            if (string.IsNullOrEmpty(semester))
            {
                sem = new SemesterService().GetSemester(DateTime.Today);
            }
            else
            {
                sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            }
            if (sem == null)
                return list.AsQueryable();

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var courses = new List<Course>();

            if (string.IsNullOrEmpty(organiser) && string.IsNullOrEmpty(curriculum))
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                    x.Semester != null &&    
                    x.Semester.Id == sem.Id).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(organiser) && string.IsNullOrEmpty(curriculum))
                {
                    var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser));
                    if (org == null)
                        return list.AsQueryable();
                    courses = Db.Activities.OfType<Course>().Where(x =>
                        x.Semester != null &&
                        x.Organiser != null &&
                        x.Semester.Id == sem.Id &&
                        x.Organiser.Id == org.Id).ToList();
                }
                if (!string.IsNullOrEmpty(curriculum))
                {
                    var curr = Db.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum));
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
        [Route("{id}")]
        [ResponseType(typeof(CourseApiModel))]
        public async Task<IHttpActionResult> GetCourse(Guid id)
        {
            if (!(await Db.Activities.FindAsync(id) is Course course))
            {
                return NotFound();
            }

            bool isAuth = true;
            var converter = new CourseConverter(Db, UserManager, isAuth);
            var dto = converter.Convert_new(course, true);

            return Ok(dto);
        }

        /*
                /// <summary>
                /// 
                /// </summary>
                [Route("subscribe")]
                public IQueryable<SubscriptionDto> Subscribe([FromBody] SubscriptionBasketModel model)
                {
                    var list = new List<SubscriptionDto>();

                    var user = GetUser(model.User.Id);

                    if (user == null)
                    {
                        var subModel = new SubscriptionDto
                        {
                            CourseId = Guid.Empty,
                            IsValid = false,
                            Message = $"Invaild user with id {model.User.Id}"
                        };
                        list.Add(subModel);
                        return list.AsQueryable();
                    }


                    foreach (var courseModel in model.Courses)
                    {
                        var subModel = new SubscriptionDto();
                        subModel.CourseId = courseModel.Id;
                        subModel.IsValid = true;

                        var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseModel.Id);

                        if (course != null)
                        {
                            var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(model.User.Id));

                            if (subscription == null)
                            {
                                subscription = new OccurrenceSubscription();
                                subscription.UserId = model.User.Id;
                                subscription.TimeStamp = DateTime.Now;
                                subscription.Occurrence = course.Occurrence;

                                // TODO: Status "Warteliste" dafür dann den Service bauen
                                // einfach: wenn LV zu CIE gehört, dann Warteliste

                                bool isCie = course.SemesterGroups.Any(x =>
                                    x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.StartsWith("CIE"));

                                if (isCie)
                                    subscription.OnWaitingList = true;

                                subscription.SubscriberRemark = "Über API eingeschrieben";

                                subscription.Priority = 1;


                                Db.Subscriptions.Add(subscription);
                            }

                        }
                        else
                        {
                            subModel.IsValid = false;
                            subModel.Message = $"Inavlid Course with Id {courseModel.Id}";
                        }

                        list.Add(subModel);
                    }

                    Db.SaveChanges();

                    return list.AsQueryable();
                }

                /// <summary>
                /// 
                /// </summary>
                [Route("unsubscribe")]
                public IQueryable<SubscriptionDto> Unsubscribe([FromBody] SubscriptionBasketModel model)
                {
                    var subService = new SubscriptionService(Db);

                    var user = GetUser(model.User.Id);

                    foreach (var courseModel in model.Courses)
                    {
                        var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseModel.Id);

                        if (course != null && user != null)
                        {
                            var subscription =
                                course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(model.User.Id));

                            if (subscription != null)
                            {
                                subService.DeleteSubscription(subscription);
                            }
                        }
                    }

                    return new List<SubscriptionDto>().AsQueryable();
                }

                /// <summary>
                /// 
                /// </summary>
                [Route("subscriptions")]
                public IQueryable<SubscriptionDto> Subscriptions([FromBody] SubscriptionBasketModel model)
                {
                    var list = new List<SubscriptionDto>();

                    var user = GetUser(model.User.Id);

                    if (user != null)
                    {
                        var courses = Db.Activities.OfType<Course>().Where(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                        foreach (var course in courses)
                        {
                            var subscription =
                                course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(model.User.Id));

                            var subModel = new SubscriptionDto();

                            subModel.CourseId = course.Id;
                            subModel.IsValid = true;
                            subModel.OnWaitingList = subscription.OnWaitingList;

                            list.Add(subModel);
                        }
                    }

                    return list.AsQueryable();
                }
        */
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(CourseApiResponseModel))]
        public async Task<IHttpActionResult> CreateCourse(string api_key, [FromBody] CourseApiModel request)
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
                ExternalId = request.externalId,
                Occurrence = occ
            };
            Db.Occurrences.Add(occ);
            Db.Activities.Add(course);

            await Db.SaveChangesAsync();

            var response = new CourseApiResponseModel
            {
                id = Guid.NewGuid(), // course.Id,
                externalId = request.externalId,
                message = "Course created successfully"
            };  

            return Ok(response);
        }


        [HttpPost]
        [Route("{id}/dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> CreateCourseDate(string api_key, Guid id, [FromBody] CourseDateApiModel request)
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

        [HttpPatch]
        [Route("dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> UpdateCourseDate(string api_key, [FromBody] CourseDateApiModel request)
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
        [Route("dates")]
        [ResponseType(typeof(CourseDateApiModel))]
        public async Task<IHttpActionResult> DeleteCourseDate(string api_key, [FromBody] CourseDateApiModel request)
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
        [Route("{id}/subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> CreateCourseSubscription(string api_key, Guid id, [FromBody] CourseSubscriberApiModel request)
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

        [HttpPatch]
        [Route("subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> UpdateCourseSubscription(string api_key, [FromBody] CourseSubscriberApiModel request)
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
        [Route("subscriptions")]
        [ResponseType(typeof(CourseSubscriberApiModel))]
        public async Task<IHttpActionResult> DeleteCourseSubscription(string api_key, [FromBody] CourseSubscriberApiModel request)
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
    }
}