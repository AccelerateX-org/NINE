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
        public IQueryable<CourseDto> GetCourses()
        {
            var list = new List<CourseDto>();

            var sem = new SemesterService().GetSemester(DateTime.Today);
            if (sem == null)
                return list.AsQueryable();

            var allCourses = Db.Activities.OfType<Course>().Where(x =>
                x.Semester.Id == sem.Id).ToList();

            var converter = new CourseConverter(Db);
            foreach (var course in allCourses)
            {
                list.Add(converter.Convert(course));
            }

            return list.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(CourseDto))]
        public async Task<IHttpActionResult> GetCourse(Guid id)
        {
            if (!(await Db.Activities.FindAsync(id) is Course course))
            {
                return NotFound();
            }

            var converter = new CourseConverter(Db);
            var dto = converter.Convert(id);

            return Ok(dto);
        }



        /// <summary>
        /// 
        /// </summary>
        [Route("{organiser}/{curriculum}/{semester}")]
        public IQueryable<CourseDto> GetCourses(string organiser, string curriculum, string semester)
        {
            var list = new List<CourseDto>();

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser));
            if (org == null)
                return list.AsQueryable();

            var curr = org.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum));
            if (curr == null)
                return list.AsQueryable();

            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return list.AsQueryable();

            var allCourses = Db.Activities.OfType<Course>().Where(x =>
                x.Semester.Id == sem.Id &&
                x.Organiser.Id == org.Id).ToList();

            var converter = new CourseConverter(Db);
            foreach (var course in allCourses)
            {
                list.Add(converter.Convert(course));
            }

            return list.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        [Route("{organiser}/{curriculum}/{semester}/summary")]
        public IQueryable<CourseSummaryDto> GetCourseSummaries(string organiser, string curriculum, string semester)
        {
            var list = new List<CourseSummaryDto>();

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser));
            if (org == null)
                return list.AsQueryable();

            var curr = org.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum));
            if (curr == null)
                return list.AsQueryable();

            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return list.AsQueryable();

            var allCourses = Db.Activities.OfType<Course>().Where(x =>
                x.Semester.Id == sem.Id &&
                x.Organiser.Id == org.Id).ToList();

            var converter = new CourseConverter(Db);
            foreach (var course in allCourses)
            {
                list.Add(converter.ConvertSummary(course));
            }

            return list.AsQueryable();
        }


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

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(CourseCreateResponse))]
        public async Task<IHttpActionResult> CreateCourse([FromBody] CourseCreateRequest request)
        {
            // Verhindert Redirects bei Unauthorized (401) Antworten => funktioniert so nicht
            // dann erst mal raus
            // HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

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

            var apiKeyService = new ApiKeyService(Db);
            var member = apiKeyService.IsValidApiKey(authHeader);
            if (member == null)
            {
                return Unauthorized();
            }



            var response = new CourseCreateResponse();



            await Db.SaveChangesAsync();

            return Ok(response);
        }
    }
}