namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDto User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StudentCurriculumDto Curriculum { get; set; }
    }

    public class StudentCurriculumDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        public string ShortName { get; set; }

        public OrganiserDto Organiser { get; set; }

        public string Semester { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

    }
}