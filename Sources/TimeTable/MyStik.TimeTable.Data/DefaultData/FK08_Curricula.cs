using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK08_InitCurriculumGUG(ActivityOrganiser FK08)
        {
            var gug = GetCurriculum(FK08, "GUG");

            if (gug != null)
                return;

            // Studienprogramme
            gug = new Curriculum
            {
                ShortName = "GUG",
                Name = "Bachelor Angewandte Geodäsie und Geoinformatik ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(gug);

            AddCapacityGroup(gug, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(gug, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(gug, "1", "C", true, false, new string[] { "1C" });



            _db.SaveChanges();
        }
        public void FK08_InitCurriculumGUN(ActivityOrganiser FK08)
        {
            var gun = GetCurriculum(FK08, "GUN");

            if (gun != null)
                return;

            // Studienprogramme
            gun = new Curriculum
            {
                ShortName = "GUN",
                Name = "Bachelor Geotelematik und Navigation ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},

                },
            };

            _db.Curricula.Add(gun);

            AddCapacityGroup(gun, "1", "", true, true, new string[] { "1" });
            AddCapacityGroup(gun, "3", "", true, false, new string[] { "3" });
            AddCapacityGroup(gun, "5", "", true, false, new string[] { "5" });
            AddCapacityGroup(gun, "7", "", true, true, new string[] { "7" });

            _db.SaveChanges();
        }
        public void FK08_InitCurriculumGeoinformatik(ActivityOrganiser FK08)
        {
            var Geoinformatik = GetCurriculum(FK08, "Geoinformatik");

            if (Geoinformatik != null)
                return;

            // Studienprogramme
            Geoinformatik = new Curriculum
            {
                ShortName = "Geoinformatik",
                Name = "Bachelor Geoinformatik und Satellitenpositionierung ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(Geoinformatik);

            AddCapacityGroup(Geoinformatik, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(Geoinformatik, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(Geoinformatik, "3", "C", true, false, new string[] { "3C" });
            AddCapacityGroup(Geoinformatik, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(Geoinformatik, "5", "B", true, false, new string[] { "5B" });
            AddCapacityGroup(Geoinformatik, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }
        public void FK08_InitCurriculumKartographie(ActivityOrganiser FK08)
        {
            var Kartographie = GetCurriculum(FK08, "Kartographie");

            if (Kartographie != null)
                return;

            // Studienprogramme
            Kartographie = new Curriculum
            {
                ShortName = "Kartographie",
                Name = "Bachelor Kartographie ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(Kartographie);

            AddCapacityGroup(Kartographie, "1", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(Kartographie, "1", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(Kartographie, "1", "C", true, false, new string[] { "3C" });
            AddCapacityGroup(Kartographie, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(Kartographie, "5", "B", true, false, new string[] { "5B" });
            AddCapacityGroup(Kartographie, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }
        public void FK08_InitCurriculumMGEO(ActivityOrganiser FK08)
        {
            var MGEO = GetCurriculum(FK08, "MGEO");

            if (MGEO != null)
                return;

            // Studienprogramme
            MGEO = new Curriculum
            {
                ShortName = "MGEO",
                Name = "Master Geomatik ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(MGEO);

            AddCapacityGroup(MGEO, "1", "Vollzeit", true, true, new string[] { "3AVollzeit" });
            AddCapacityGroup(MGEO, "1", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MGEO, "2", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MGEO, "2", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MGEO, "3", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MGEO, "3", "Teilzeit", true, true, new string[] { "Teilzeit" });


            _db.SaveChanges();

        }
        public void FK08_InitCurriculumMKAT(ActivityOrganiser FK08)
        {
            var MKAT = GetCurriculum(FK08, "MKAT");

            if (MKAT != null)
                return;

            // Studienprogramme
            MKAT = new Curriculum
            {
                ShortName = "MKAT",
                Name = "Master Katastrophenmanagement ",
                Organiser = FK08,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(MKAT);

            AddCapacityGroup(MKAT, "1", "Vollzeit", true, true, new string[] { "3AVollzeit" });
            AddCapacityGroup(MKAT, "1", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MKAT, "2", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MKAT, "2", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MKAT, "3", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MKAT, "3", "Teilzeit", true, true, new string[] { "Teilzeit" });


            _db.SaveChanges();

        }
    }
}

 
