using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [RoutePrefix("api/v2/apps/edbot/schedule")]

    public class EdBotScheduleController : ApiBaseController
    {
        [System.Web.Http.Route("{group}")]
        public IQueryable<GroupScheduleDto> GetGroupSchedule(string group)
        {
            var result = new List<GroupScheduleDto>();


            var semester = new SemesterService(Db).GetSemester(DateTime.Today);

            if (Request.IsLocal())
                semester = new SemesterService(Db).GetSemester(new DateTime(2020, 11, 1));


            var words = group.Split('-');

            var currName = words[0].Trim().ToUpper();
            var groupName = words[1].Trim().ToUpper();

            var groups = Db.SemesterGroups.Where(x =>
                x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(currName) &&
                x.Semester.Id == semester.Id).ToList();

            foreach (var semesterGroup in groups)
            {
                if (semesterGroup.CapacityGroup.GroupName.Equals(groupName))
                {
                    foreach (var activity in semesterGroup.Activities)
                    {
                        var dto = new GroupScheduleDto
                        {
                            Name = activity.Name,
                            ShortName = activity.ShortName
                        };

                        result.Add(dto);
                    }
                }
            }

            return result.AsQueryable();
        }



        [System.Web.Http.Route("Group")]
        public IQueryable<GroupScheduleDto> GroupSchedule([FromBody] GroupScheduleBasketModel model)
        {
            var result = new List<GroupScheduleDto>();

            var semester = new SemesterService(Db).GetSemester(DateTime.Today);

            if (Request.IsLocal())
                semester = new SemesterService(Db).GetSemester(new DateTime(2020, 11, 1));

            var currName = model.Curriculum.ToUpper();
            var groupName = model.Group.ToUpper();

            var groups = Db.SemesterGroups.Where(x =>
                x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(currName) &&
                x.Semester.Id == semester.Id).ToList();

            foreach (var semesterGroup in groups)
            {
                if (semesterGroup.CapacityGroup.GroupName.Equals(groupName))
                {
                    foreach (var activity in semesterGroup.Activities)
                    {
                        var dto = new GroupScheduleDto
                        {
                            Name = activity.Name,
                            ShortName = activity.ShortName
                        };

                        result.Add(dto);
                    }
                }
            }

            return result.AsQueryable();
        }


    }
}
