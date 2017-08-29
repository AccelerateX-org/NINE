using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Curriculum
{
    public class AccreditationService
    {
        public void ImportModuleCatalog(string orgName, string currName)
        {
            TimeTableDbContext db = new TimeTableDbContext();

            var org = db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));
            if (org == null)
                return;

            var curriculum =
                db.Curricula.SingleOrDefault(x => x.Organiser.ShortName.Equals(orgName) && x.ShortName.Equals(currName));
            if (curriculum == null)
                return;

            // Den Modulkatalog holen
            var mcs = new ModuleCatalogService();
            var mc = mcs.GetCatalog(orgName, currName);
            if (mc == null)
                return;

            // WICHTIG:
            // Noch stehen in den Modulen noch Informationen drin
            // die für die Akkreditierung verwendet werden können
            // Entweder später eigene Datenklassen verwenden
            // oder weitere Interfaces ICriteriaCatalog / IAccreditationCatalog
            // verwenden und mit fachlichen Schlüsseln arbeiten


            // Für jedes Moodul
            // Kriterium suchen und ggf. anlegen, z.B. aus der Studiengruppe heraus
            // Akkreditierung suchen und ggf. anlegen
            // Ein Modul kann in einem Studiengang nur eine Akkreditierung haben
            // Schlüssel ist das Kriterium!
            var allModules = mc.GetModules(string.Empty);
            foreach (var module in allModules)
            {
                // MV suchen und ggf. anlegen
                var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(module.MV.ShortName));
                if (member == null)
                {
                    member = new OrganiserMember
                    {
                        ShortName = module.MV.ShortName,
                        Name = module.MV.ShortName,
                        Role = "Dozent",
                        Organiser = org
                    };
                    db.Members.Add(member);
                    db.SaveChanges();
                }

                // Das echte Modul suchen und ggf. anlegen
                // Hypothes:
                // - Kurzname
                // - Name MV
                // - ModulId
                // bilden einen eindeutigen fachlichen Schlüssel!
                // So was wie Wechsel MV geht dann hier so nicht
                // First ist hier nur der Sicherheit geschuldet
                var realModule = db.CurriculumModules.FirstOrDefault(x =>
                    x.ShortName.Equals(module.ShortName) &&
                    x.ModuleId != null && x.ModuleId.Equals(module.ModuleId) &&
                    x.MV != null && x.MV.ShortName.Equals(module.MV.ShortName)
                );

                if (realModule == null)
                {
                    // in den globalen Modulkatalog aufnehmen
                    realModule = new CurriculumModule
                    {
                        ShortName = module.ShortName,
                        Name = module.Name,
                        ModuleId = module.ModuleId,
                        Description = module.Description,
                        ECTS = module.ECTS,
                        MV = member,
                    };

                    foreach (var moduleCourse in module.ModuleCourses)
                    {
                        var course = new ModuleCourse
                        {
                            Name = moduleCourse.Name,
                            CourseType = moduleCourse.CourseType,
                            ExternalId = moduleCourse.ExternalId,
                            SWS = moduleCourse.SWS,
                            Module = realModule
                        };

                        realModule.ModuleCourses.Add(course);
                        db.ModuleCourses.Add(course);
                    }

                    // TODO: Prüfungen ergänzen

                    db.CurriculumModules.Add(realModule);
                    db.SaveChanges();
                }


                
                // Studiengruppe suchen und ggf. anlegen
                // es reicht die Studiengruppe
                // Kapazität kommt hier nicht vor => wir sind hier auch auf der Ebene des Studiengangs
                // und noch nicht auf der Ebene der Semester
                var groupList = new List<CurriculumGroup>();
                foreach (var curriculumGroup in module.Groups)
                {
                    var group = curriculum.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(curriculumGroup.Name));
                    if (group == null)
                    {
                        // sollte nicht sein  noch nicht
                    }

                    if (group != null)
                    {
                        groupList.Add(group);
                    }
                }

                // Kriterium suchen und ggf. anlegen
                var words = module.ModuleId.Split('!');
                var critName = words[0];

                var criteria = curriculum.Criterias.SingleOrDefault(x => x.ShortName.Equals(critName));
                if (criteria == null)
                {
                    criteria = new CurriculumCriteria
                    {
                        Curriculum = curriculum,
                        ShortName = critName,
                        Name = critName,
                        MinECTS = -1,           // keine Beschränkung
                        MaxECTS = -1,           // keine Beschränkung
                        Option = -1             // alle Module müssen belegt werden
                    };

                    db.Criterias.Add(criteria);
                    db.SaveChanges();
                }


                // Akkreditierung suchen ggf. Akkreditierung anlegen
                var accredit = criteria.Accreditations.SingleOrDefault(x => x.Module.Id == realModule.Id);
                if (accredit == null)
                {
                    accredit = new ModuleAccreditation
                    {
                        Module = realModule,
                        Criteria = criteria,
                        Groups = groupList
                    };

                    db.Accreditations.Add(accredit);
                    db.SaveChanges();
                }

                // TODO: ggf. prüfen, ob es eine neue Gruppe gibt
            }
        }

        /// <summary>
        /// pro Modul ein Kriterium
        /// WPMs werden in einem zusammengefasst
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="currName"></param>
        public void ImportModuleCatalogSingle(string orgName, string currName)
        {
            TimeTableDbContext db = new TimeTableDbContext();

            var org = db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));
            if (org == null)
                return;

            var curriculum =
                db.Curricula.SingleOrDefault(x => x.Organiser.ShortName.Equals(orgName) && x.ShortName.Equals(currName));
            if (curriculum == null)
                return;

            // Den Modulkatalog holen
            var mcs = new ModuleCatalogService();
            var mc = mcs.GetCatalog(orgName, currName);
            if (mc == null)
                return;

            // WICHTIG:
            // Noch stehen in den Modulen noch Informationen drin
            // die für die Akkreditierung verwendet werden können
            // Entweder später eigene Datenklassen verwenden
            // oder weitere Interfaces ICriteriaCatalog / IAccreditationCatalog
            // verwenden und mit fachlichen Schlüsseln arbeiten


            // Für jedes Moodul wird ein eigenes Kriterium angelegt
            var allModules = mc.GetModules(string.Empty);
            foreach (var module in allModules)
            {
                // MV suchen und ggf. anlegen
                var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(module.MV.ShortName));
                if (member == null)
                {
                    member = new OrganiserMember
                    {
                        ShortName = module.MV.ShortName,
                        Name = module.MV.ShortName,
                        Role = "Dozent",
                        Organiser = org
                    };
                    db.Members.Add(member);
                    db.SaveChanges();
                }

                // Das echte Modul suchen und ggf. anlegen
                // Hypothes:
                // - Kurzname
                // - Name MV
                // - ModulId
                // bilden einen eindeutigen fachlichen Schlüssel!
                // So was wie Wechsel MV geht dann hier so nicht
                // First ist hier nur der Sicherheit geschuldet
                var realModule = db.CurriculumModules.FirstOrDefault(x =>
                    x.ShortName.Equals(module.ShortName) &&
                    x.ModuleId != null && x.ModuleId.Equals(module.ModuleId) &&
                    x.MV != null && x.MV.ShortName.Equals(module.MV.ShortName)
                );

                if (realModule == null)
                {
                    // in den globalen Modulkatalog aufnehmen
                    realModule = new CurriculumModule
                    {
                        ShortName = module.ShortName,
                        Name = module.Name,
                        ModuleId = module.ModuleId,
                        Description = module.Description,
                        ECTS = module.ECTS,
                        MV = member,
                    };

                    foreach (var moduleCourse in module.ModuleCourses)
                    {
                        var course = new ModuleCourse
                        {
                            Name = moduleCourse.Name,
                            CourseType = moduleCourse.CourseType,
                            ExternalId = moduleCourse.ExternalId,
                            SWS = moduleCourse.SWS,
                            Module = realModule
                        };

                        realModule.ModuleCourses.Add(course);
                        db.ModuleCourses.Add(course);

                    
                        // jetzt noch die Parallelkurse hinzufügen
                        // wenn es keine gibt, dann default
                        if (moduleCourse.CapacityCourses.Any())
                        {
                            foreach (var capacityCourse in moduleCourse.CapacityCourses)
                            {
                                var capCourse = new CapacityCourse
                                {
                                    ShortName = capacityCourse.ShortName,
                                    Course = course
                                };

                                course.CapacityCourses.Add(capCourse);
                                db.CapacityCourses.Add(capCourse);
                            }
                        }
                        else
                        {
                            var capCourse = new CapacityCourse
                            {
                                ShortName = moduleCourse.ExternalId,
                                Course = course
                            };
                            
                            course.CapacityCourses.Add(capCourse);
                            db.CapacityCourses.Add(capCourse);
                        }
                    
                    }

                    // TODO: Prüfungen ergänzen

                    db.CurriculumModules.Add(realModule);
                    db.SaveChanges();
                }



                // Studiengruppe suchen und ggf. anlegen
                // es reicht die Studiengruppe
                // Kapazität kommt hier nicht vor => wir sind hier auch auf der Ebene des Studiengangs
                // und noch nicht auf der Ebene der Semester
                var groupList = new List<CurriculumGroup>();
                foreach (var curriculumGroup in module.Groups)
                {
                    var group = curriculum.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(curriculumGroup.Name));
                    if (group == null)
                    {
                        // sollte nicht sein  noch nicht
                    }

                    if (group != null)
                    {
                        groupList.Add(group);
                    }
                }

                // Kriterium suchen und ggf. anlegen
                var criteria = curriculum.Criterias.SingleOrDefault(x => x.ShortName.Equals(module.ShortName));
                if (criteria == null)
                {
                    criteria = new CurriculumCriteria
                    {
                        Curriculum = curriculum,
                        ShortName = module.ShortName,
                        Name = module.Name,
                        MinECTS = -1,           // keine Beschränkung
                        MaxECTS = -1,           // keine Beschränkung
                        Option = -1             // alle Module müssen belegt werden
                    };

                    db.Criterias.Add(criteria);
                    db.SaveChanges();
                }


                // Akkreditierung suchen ggf. Akkreditierung anlegen
                var accredit = criteria.Accreditations.SingleOrDefault(x => x.Module.Id == realModule.Id);
                if (accredit == null)
                {
                    accredit = new ModuleAccreditation
                    {
                        Module = realModule,
                        Criteria = criteria,
                        Groups = groupList
                    };

                    db.Accreditations.Add(accredit);
                    db.SaveChanges();
                }

                // TODO: ggf. prüfen, ob es eine neue Gruppe gibt
            }
        }
    
    }
}
