using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MyStik.TimeTable.Data
{
    public class TimeTableDbContext : DbContext
    {
        public IDbSet<BinaryStorage> Storages { get; set; }
        
        public IDbSet<Room> Rooms { get; set; }

        public IDbSet<Occurrence> Occurrences { get; set; }

        public IDbSet<OccurrenceGroup> OccurrenceGroups { get; set; }
        
        public IDbSet<Subscription> Subscriptions { get; set; }

        public IDbSet<ActivityOrganiser> Organisers { get; set; }
        
        public IDbSet<OrganiserMember> Members { get; set; }

        public IDbSet<RoomAssignment> RoomAssignments { get; set; }

        public IDbSet<RoomBooking> RoomBookings { get; set; }

        public IDbSet<BookingConfirmation> BookingConfirmations { get; set; }
        
        public IDbSet<Activity> Activities { get; set; }
        
        public IDbSet<ActivityDate> ActivityDates { get; set; }
        
        public IDbSet<ActivitySlot> ActivitySlots { get; set; }

        public IDbSet<Semester> Semesters { get; set; }
        
        public IDbSet<SemesterDate> SemesterDates { get; set; }


        public IDbSet<Curriculum> Curricula { get; set; }
        
        public IDbSet<CurriculumGroup> CurriculumGroups { get; set; }

        public IDbSet<CapacityGroup> CapacityGroups { get; set; }

        public IDbSet<CurriculumModule> CurriculumModules { get; set; }

        public IDbSet<ModuleCourse> ModuleCourses { get; set; }

        public IDbSet<CapacityCourse> CapacityCourses { get; set; }

        public  IDbSet<CurriculumChapter> CurriculumChapters { get; set; }

        public IDbSet<CurriculumTopic> CurriculumTopics { get; set; }

        public IDbSet<SemesterTopic> SemesterTopics { get; set; }


        public IDbSet<ModuleExam> ModuleExams { get; set; }

        public IDbSet<SemesterGroup> SemesterGroups { get; set; }


        public IDbSet<GroupAlias> GroupAliases { get; set; }

        public IDbSet<Alumnus> Alumnae { get; set; }

        public IDbSet<ActivityOwner> ActivityOwners { get; set; }

        public IDbSet<ActivityDateChange> DateChanges { get; set; }
        
        public IDbSet<RoomAllocationChange> RoomChanges { get; set; }

        public IDbSet<NotificationState> NotificationStates { get; set; }

        public IDbSet<Infoscreen> Infoscreens { get; set; }
        
        public IDbSet<InfoAnnouncement> Announcements { get; set; }
        
        public IDbSet<InfoText> InfoTexts { get; set; }

        public IDbSet<Level> Levels { get; set; }

        public IDbSet<Lottery> Lotteries { get; set; }

        public IDbSet<LotteryDrawing> LotteryDrawings { get; set; }

        public IDbSet<OccurrenceDrawing> OccurrenceDrawings { get; set; }

        public IDbSet<SubscriptionDrawing> SubscriptionDrawings { get; set; }


        public IDbSet<CoursePlan> CoursePlans { get; set; }

        public IDbSet<ModuleMapping> ModuleMappings { get; set; }

        public IDbSet<ModuleTrial> ModuleTrials { get; set; }

        //public IDbSet<Thesis> Thesis { get; set;  }

        public IDbSet<Institution> Institutions { get; set; }

        public IDbSet<Building> Buildings { get; set; }

        public IDbSet<CurriculumCriteria> Criterias { get; set; }

        public IDbSet<ModuleAccreditation> Accreditations { get; set; }

        public IDbSet<CriteriaRule> Rules { get; set; }

        public IDbSet<ThesisAnnouncement> ThesisAnnouncements { get; set; }

        public IDbSet<ThesisProvider> ThesisProvider { get; set; }

        public IDbSet<ThesisWorkflow> ThesisWorkflows { get; set; }

        public IDbSet<ThesisFeedback> ThesisFeedbacks { get; set; }

        public IDbSet<Student> Students { get; set; }
        public IDbSet<StudentExam> StudentExams { get; set; }
        public IDbSet<ExamPaper> ExamPapers { get; set; }

        public IDbSet<CorporateContact> CorporateContacts { get; set; }

        public IDbSet<PersonalContact> PersonalContacts { get; set; }

        public IDbSet<Advertisement> Advertisements { get; set; }

        public IDbSet<AdvertisementRole> AdvertisementRoles { get; set; }


        public TimeTableDbContext() : base()
        {
        }

        public TimeTableDbContext(string nameOrConnectionString) 
            : base(nameOrConnectionString)
        { 
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
