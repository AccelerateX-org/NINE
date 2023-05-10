using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MyStik.TimeTable.Data
{
    public class TimeTableDbContext : DbContext
    {
        public DbSet<ChangeLog> ChangeLogs { get; set; }

        public DbSet<BinaryStorage> Storages { get; set; }
        
        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomEquipment> RoomEquipments { get; set; }

        public DbSet<Occurrence> Occurrences { get; set; }

        public DbSet<OccurrenceGroup> OccurrenceGroups { get; set; }
        
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<ActivityOrganiser> Organisers { get; set; }
        
        public DbSet<OrganiserMember> Members { get; set; }

        public DbSet<MemberExport> MemberExports { get; set; }

        public DbSet<MemberResponsibility> MemberResponsibilities { get; set; }

        public DbSet<MemberSkill> MemberSkills { get; set; }

        public DbSet<RoomAssignment> RoomAssignments { get; set; }

        public DbSet<RoomBooking> RoomBookings { get; set; }

        public DbSet<BookingConfirmation> BookingConfirmations { get; set; }
        
        public DbSet<Activity> Activities { get; set; }
        
        public DbSet<ActivityDate> ActivityDates { get; set; }
        
        public DbSet<ActivitySlot> ActivitySlots { get; set; }

        public DbSet<Semester> Semesters { get; set; }
        
        public DbSet<SemesterDate> SemesterDates { get; set; }

        public DbSet<ContentChannel> ContentChannels { get; set; }


        public DbSet<Curriculum> Curricula { get; set; }
        
        public DbSet<CurriculumGroup> CurriculumGroups { get; set; }

        public DbSet<CapacityGroup> CapacityGroups { get; set; }

        public DbSet<CurriculumModule> CurriculumModules { get; set; }

        public DbSet<ModuleApplicability> ModuleApplicabilities { get; set; }
       

        public DbSet<ModuleSubject> ModuleCourses { get; set; }


        public  DbSet<CurriculumChapter> CurriculumChapters { get; set; }

        public DbSet<CurriculumTopic> CurriculumTopics { get; set; }

        public DbSet<SemesterTopic> SemesterTopics { get; set; }

        public DbSet<CurriculumArea> CurriculumAreas { get; set; }

        public DbSet<AreaOption> AreaOptions { get; set; }


        public DbSet<SemesterGroup> SemesterGroups { get; set; }

        public DbSet<CurriculumOpportunity> CurriculumOpportunities { get; set; }
        
        public  DbSet<Degree> Degrees { get; set; }

        public DbSet<GroupAlias> GroupAliases { get; set; }

        public DbSet<Alumnus> Alumnae { get; set; }

        public DbSet<ActivityOwner> ActivityOwners { get; set; }

        public DbSet<ActivityDateChange> DateChanges { get; set; }
        
        public DbSet<RoomAllocationChange> RoomChanges { get; set; }

        public DbSet<NotificationState> NotificationStates { get; set; }

        public DbSet<Infoscreen> Infoscreens { get; set; }
        
        public DbSet<InfoAnnouncement> Announcements { get; set; }
        
        public DbSet<InfoText> InfoTexts { get; set; }

        public DbSet<Level> Levels { get; set; }

        public DbSet<LotteryBundle> LotteryBundles { get; set; }


        public DbSet<Lottery> Lotteries { get; set; }

        public DbSet<LotteryBudget> LotteriyBudgets { get; set; }

        public DbSet<LotteryBet> LotteriyBets { get; set; }

        public DbSet<LotteryGame> LotteryGames { get; set; }

        public DbSet<LotteryDrawing> LotteryDrawings { get; set; }

        public DbSet<OccurrenceDrawing> OccurrenceDrawings { get; set; }

        public DbSet<SubscriptionDrawing> SubscriptionDrawings { get; set; }


        public DbSet<CoursePlan> CoursePlans { get; set; }

        public DbSet<ModuleMapping> ModuleMappings { get; set; }

        public DbSet<ModuleTrial> ModuleTrials { get; set; }

        public DbSet<Thesis> Theses { get; set;  }

        public DbSet<Institution> Institutions { get; set; }

        public DbSet<Building> Buildings { get; set; }

        public DbSet<CurriculumCriteria> Criterias { get; set; }

        public DbSet<ModuleAccreditation> Accreditations { get; set; }

        public DbSet<ExaminationDescription> ExaminationDescriptions { get; set; }

        public DbSet<Examiner> Examiners { get; set; }


        public DbSet<TeachingDescription> TeachingDescriptions { get; set; }

        public DbSet<ThesisAnnouncement> ThesisAnnouncements { get; set; }

        public DbSet<ThesisProvider> ThesisProvider { get; set; }

        public DbSet<ThesisWorkflow> ThesisWorkflows { get; set; }

        public DbSet<ThesisFeedback> ThesisFeedbacks { get; set; }

        public DbSet<Student> Students { get; set; }
        //public DbSet<StudentExam> StudentExams { get; set; }
        //public DbSet<ExamPaper> ExamPapers { get; set; }

        public DbSet<CorporateContact> CorporateContacts { get; set; }

        public DbSet<PersonalContact> PersonalContacts { get; set; }

        public DbSet<Advertisement> Advertisements { get; set; }

        public DbSet<AdvertisementRole> AdvertisementRoles { get; set; }

        public DbSet<AdvertisementInfo> AdvertisementInfos { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ExaminationOption> ExaminationOptions { get; set; }
        public DbSet<ExaminationFraction> ExaminationFractions { get; set; }

        public DbSet<ExaminationForm> ExaminationForms { get; set; }
        //public DbSet<ExaminationUnit> ExaminationUnits { get; set; }
        //public DbSet<ExaminationAid> ExaminationAids { get; set; }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }

        public DbSet<Internship> Internships { get; set; }

        public DbSet<VirtualRoom> VirtualRooms { get; set; }

        public DbSet<VirtualRoomAccess> VirtualRoomAccesses { get; set; }


        public DbSet<Assessment> Assessments { get; set; }

        public DbSet<AssessmentStage> AssessmentStages { get; set; }

        public DbSet<AssessmentStageMaterial> AssessmentStageMaterial { get; set; }

        public DbSet<Candidature> Candidatures { get; set; }

        public DbSet<CandidatureStage> CandidatureStages { get; set; }

        public DbSet<CandidatureStageMaterial> CandidatureStageMaterial { get; set; }

        public DbSet<Autonomy> Autonomy { get; set; }


        public DbSet<Committee> Committees { get; set; }

        public DbSet<CommitteeMember> CommitteeMember { get; set; }


        public DbSet<ScriptDocument> ScriptDocuments { get; set; }

        public DbSet<OrderPeriod> OrderPeriods { get; set; }

        public DbSet<OrderBasket> OrderBaskets { get; set; }

        public DbSet<ScriptOrder> ScriptOrders { get; set; }

        public DbSet<ScriptPublishing> ScriptPublishings { get; set; }


        public DbSet<CurriculumSection> CurriculumSections { get; set; }
        public DbSet<CurriculumSlot> CurriculumSlots { get; set; }

        public DbSet<ModuleResponsibility> ModuleResponsibilities { get; set; }

        

        public DbSet<ItemLabel> ItemLabels { get; set; }
        public DbSet<ItemLabelSet> ItemLabelSets { get; set; }


        public DbSet<StudentChannel> StudentChannels { get; set; }

        public DbSet<CurriculumModuleCatalog> CurriculumModuleCatalogs { get; set; }

        public DbSet<CatalogResponsibility> CatalogResponsibilities { get; set; }

        public DbSet<SubjectOpportunity> SubjectOpportunities { get; set; }

        public DbSet<TeachingFormat> TeachingFormats { get; set; }

        public DbSet<ModuleDescription> ModuleDescriptions { get; set; }


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
