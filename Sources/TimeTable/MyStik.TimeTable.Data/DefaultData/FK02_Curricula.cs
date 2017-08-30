using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK02_InitCurriculumBAU(ActivityOrganiser fk02)
        {
            var bau = GetCurriculum(fk02, "BAU");

            if (bau != null)
                return;

            // Studienprogramme
            bau = new Curriculum
            {
                ShortName = "BAU",
                Name = "Bachelor Bauingenieurwesen",
                Organiser = fk02,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(bau);

            AddCapacityGroup(bau, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(bau, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(bau, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(bau, "1", "D", true, true, new string[] { "1D" });

            AddCapacityGroup(bau, "2", "W", true, true, new string[] { "2W" });


            AddCapacityGroup(bau, "3", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(bau, "3", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(bau, "4", "A", true, true, new string[] { "4A" });

            AddCapacityGroup(bau, "5", "AP", true, true, new string[] { "5AP" });
            AddCapacityGroup(bau, "5", "BP", true, true, new string[] { "5BP" });

            AddCapacityGroup(bau, "6", "IB", true, true, new string[] { "6IB" });
            AddCapacityGroup(bau, "6", "IK", true, true, new string[] { "6IK" });

            AddCapacityGroup(bau, "7", "IB", true, true, new string[] { "7IB" });
            AddCapacityGroup(bau, "7", "IK", true, true, new string[] { "7IK" });
            AddCapacityGroup(bau, "7", "S-bau", true, true, new string[] { "7S-bau" });

            AddCapacityGroup(bau, "WPM", "", true, true, new string[] { "WPM bau" });

            _db.SaveChanges();
        }

        public void FK02_InitCurriculumBAUDUAL(ActivityOrganiser fk02)
        {
            var baudual = GetCurriculum(fk02, "BAUDUAL");

            if (baudual != null)
                return;


            baudual = new Curriculum
            {

                ShortName = "BAUDUAL",
                Name = "Bachelor Bauingenieurwesen-Dual",
                Organiser = fk02,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "8", IsSubscribable = true},
                    new CurriculumGroup { Name = "9", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(baudual);

            AddCapacityGroup(baudual, "1", "DU", true, true, new string[] { "1DU" });

            AddCapacityGroup(baudual, "2", "DU", true, true, new string[] { "2DU" });

            AddCapacityGroup(baudual, "3", "DU", true, true, new string[] { "3DU" });

            AddCapacityGroup(baudual, "4", "DU", true, true, new string[] { "4DU" });

            AddCapacityGroup(baudual, "5", "DU", true, true, new string[] { "5DU" });

            AddCapacityGroup(baudual, "6", "DU", true, true, new string[] { "6DU" });

            AddCapacityGroup(baudual, "7", "DU", true, true, new string[] { "7DU" });

            AddCapacityGroup(baudual, "8", "DU", true, true, new string[] { "8DU" });

            AddCapacityGroup(baudual, "9", "DU", true, true, new string[] { "9DU" });


            _db.SaveChanges();
        }

        public void FK02_InitCurriculumMAG(ActivityOrganiser fk02)
        {
            var mag = GetCurriculum(fk02, "MAG");

            if (mag != null)
                return;

            // Studienprogramme
            mag = new Curriculum
            {
                ShortName = "MAG",
                Name = "Master Stahlbau und Gestaltungstechnik ",
                Organiser = fk02,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},

                 },
            };

            _db.Curricula.Add(mag);

            AddCapacityGroup(mag, "1", "MS", true, true, new string[] { "MAG-1MS" });


            AddCapacityGroup(mag, "3", "A", true, true, new string[] { "MAG-3MS" });


            _db.SaveChanges();
        }

            public void FK02_InitCurriculumMAI(ActivityOrganiser fk02)
        {
            var mai = GetCurriculum(fk02, "MAI");

            if (mai != null)
                return;

            // Studienprogramme
            mai = new Curriculum
            {
                ShortName = "MAI",
                Name = "Master Allgemeiner Ingenieurbau ",
                Organiser = fk02,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},

                 },
            };

            _db.Curricula.Add(mai);

            AddCapacityGroup(mai, "1", "MS", true, true, new string[] { "MAI-1AI" });


            AddCapacityGroup(mai, "3", "A", true, true, new string[] { "MAI-3AI" });


            _db.SaveChanges();

        }
    }
}
      