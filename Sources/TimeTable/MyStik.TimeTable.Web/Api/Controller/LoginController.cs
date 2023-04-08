using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    // http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
    [RoutePrefix("api/v2/account")]

    public class LoginController : ApiBaseController
    {

        /// <summary>
        /// 
        /// </summary>
        [Route("login")]
        public AccountDto Login([FromBody] LoginModel model)
        {
            string username = model.username;
            string password = model.password;

            ApplicationUser user = null;
            //Überprüfen ob Mail
            if (username.Contains("@"))
            {
                //Suche ob Mail vorhanden
                var tempUser = UserManager.FindByEmail(username);

                if (tempUser != null)
                {
                    //wenn was gefunden wurde, Überprüfen ob PW stimmt
                    user = UserManager.Find(tempUser.UserName, password);
                }
            }
            //Übergebener string ist evtl Loginname
            else
            {
                //Überprüfen ob vorhanden und PW stimmt
                user = UserManager.Find(username, password);
            }

            var result = new AccountDto();

            if (user != null)
            {
                result.User = new UserDto();
                result.User.Id = user.Id;
                result.User.FirstName = user.FirstName;
                result.User.LastName = user.LastName;

                var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).FirstOrDefault();
                if (student != null)
                {
                    result.Curriculum = new CurriculumDto();
                    result.Curriculum.Name = student.Curriculum.Name;
                    result.Curriculum.ShortName = student.Curriculum.ShortName;

                    result.Curriculum.Organiser = new OrganiserDto();
                    result.Curriculum.Organiser.Name = student.Curriculum.Organiser.Name;
                    result.Curriculum.Organiser.ShortName = student.Curriculum.Organiser.ShortName;
                    result.Curriculum.Organiser.Color = student.Curriculum.Organiser.HtmlColor;
                }
            }


            return result;
        }
    }
}
