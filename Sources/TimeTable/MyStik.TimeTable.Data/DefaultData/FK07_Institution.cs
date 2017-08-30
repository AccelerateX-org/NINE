namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK07()
        {
            var fk07 = GetOrganiser("FK 07");

            if (fk07 != null) return;

            fk07 = new ActivityOrganiser
            {
                Name = "Fakultät 07",
                ShortName = "FK 07",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk07);
            _db.SaveChanges();
        }

        public void InitMemberFK07()
        {
            var fk07 = GetOrganiser("FK 07");
            AddMember(fk07, "BOE", "Böttcher");
            AddMember(fk07, "BRA", "Braun");
            AddMember(fk07, "ESO", "Eich-Soellner");
            AddMember(fk07, "FIS", "Fischer");
            AddMember(fk07, "FSC", "Fischer");
            AddMember(fk07, "FSR", "Fischer");
            AddMember(fk07, "GRL", "Gerling");
            AddMember(fk07, "GRN", "Greiner");
            AddMember(fk07, "GNZ", "Guenzel");
            AddMember(fk07, "HAF", "Hafner");
            AddMember(fk07, "HAM", "Hammerschall");
            AddMember(fk07, "HEI", "Heigert");
            AddMember(fk07, "HER", "Hertle");
            AddMember(fk07, "HOB", "Hobelsberger");
            AddMember(fk07, "HOE", "Hörwick");
            AddMember(fk07, "KAH", "Kahl");
            AddMember(fk07, "KOE", "Köster");
            AddMember(fk07, "LEI", "Leitner");
            AddMember(fk07, "LIN", "Lindermeier");
            AddMember(fk07, "MAN", "Mandl");
            AddMember(fk07, "MOE", "Möbert");
            AddMember(fk07, "MON", "Möncke");
            AddMember(fk07, "NIS", "Nischwitz");
            AddMember(fk07, "ORE", "Orehek");
            AddMember(fk07, "PET", "Peters");
            AddMember(fk07, "PER", "Petri");
            AddMember(fk07, "PLE", "Pleier");
            AddMember(fk07, "REC", "Recknagel");
            AddMember(fk07, "RUK", "Ruckert");
            AddMember(fk07, "SIE", "Schiedermeier");
            AddMember(fk07, "SLU", "Schlüchtermann");
            AddMember(fk07, "SMI", "Schmidt");
            AddMember(fk07, "SNO", "Schnörr");
            AddMember(fk07, "SWE", "Schwenkert");
            AddMember(fk07, "SOC", "Socher");
            AddMember(fk07, "STA", "Staudt");
            AddMember(fk07, "STU", "Stützle");
            AddMember(fk07, "THU", "Thurner");
            AddMember(fk07, "TOR", "Tornow");
            AddMember(fk07, "VOG", "Vogt");
            AddMember(fk07, "WIS", "Wischhof");
            AddMember(fk07, "ZIE", "Zielke");
            AddMember(fk07, "ZIM", "Zimmer");
            AddMember(fk07, "ZUG", "Zugenmaier", true);

        }

        public void InitCurriculaFK07()
        {
            var fk07 = GetOrganiser("FK 07");

            FK07_InitCurriculumInfoMath(fk07);
            FK07_InitCurriculumWirtschaftsinformatik(fk07);
            FK07_InitCurriculumKART(fk07);
            FK07_InitCurriculumMWINF(fk07);
            FK07_InitCurriculumMINFO(fk07);
            FK07_InitCurriculumANMATH(fk07);
        }

        public void InitModulesFK07()
        {
            var fk07 = GetOrganiser("FK 07");

        }


    }

}