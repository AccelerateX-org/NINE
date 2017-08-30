namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK10()
        {
            var fk10 = GetOrganiser("FK 10");

            if (fk10 != null) return;

            fk10 = new ActivityOrganiser
            {
                Name = "Fakultät 10",
                ShortName = "FK 10",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk10);
            _db.SaveChanges();
        }

        public void InitMemberFK10()
        {
            var fk10 = GetOrganiser("FK 10");
        }

        public void InitCurriculaFK10()
        {
            var fk10 = GetOrganiser("FK 10");

            FK10_InitCurriculumBW(fk10);
            FK10_InitCurriculumWIF(fk10);
            FK10_InitCurriculumMisc(fk10);
        }

        public void InitModulesFK10()
        {
            var fk10 = GetOrganiser("FK 10");

        }

    }

}