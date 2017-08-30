using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogAOB_GS(ActivityOrganiser fk06, Curriculum AOB)
        {
            var brillenoptik = new CurriculumModule()
            {
                ShortName = "Brillenoptik I",
                Name = "Brillenoptik I",
                ModuleId = "AOB100",
                Description = "Grundstruktur des optischen Systems",
                ECTS = 5,
                MV = GetHost(fk06, "BAE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Brillenoptik I Vorl"
                    },
                }

            };

            brillenoptik.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(brillenoptik);
            _db.CurriculumModules.Add(brillenoptik);

            var mathematik = new CurriculumModule()
            {
                ShortName = "Mathematik I",
                Name = "Mathematik I",
                ModuleId = "AOB110",
                Description = "Grundkenntnisse einzelner für die Augenoptik relevanter mathematischer Begriffe und Methoden",
                ECTS = 4,
                MV = GetHost(fk06, "HIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathematik I"
                    },
                }
            };

            mathematik.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(mathematik);
            _db.CurriculumModules.Add(mathematik);

            var physik = new CurriculumModule()
            {
                ShortName = "Physik I",
                Name = "Physik I",
                ModuleId = "AOB120",
                Description = "Grundlagen der Natur-und Ingenieurwissenschaften",
                ECTS = 4,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik I"
                    },
                }
            };

            physik.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(physik);
            _db.CurriculumModules.Add(physik);


            var CWT = new CurriculumModule()
            {
                ShortName = "Chemie/Werkstoffe",
                Name = "Grundlagen der Chemie, Werkstofftechnik, optische Werkstoffe",
                ModuleId = "AOB130",
                Description = "Organische und anorganische Chemie, Eigenschaften und Aufbau von Metallen und Kunststoffen",
                ECTS = 7,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie/Werkstoffe"
                    },
                }

            };

            CWT.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(CWT);
            _db.CurriculumModules.Add(CWT);

            var tap = new CurriculumModule()
            {
                ShortName = "Termi/Ana/Physio",
                Name = "Medizinische Terminologie, Allgemeine Anatomie und Physiologie",
                ModuleId = "AOB140",
                Description = "Grundkenntnisse und Verständnis der medizinischen Fachsprache,  Erwerb grundlegender Kenntnisse der Anatomie, Verständnis grundlegender funktioneller Vorgänge des menschlichen Körpers",
                ECTS = 8,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Termi/Ana/Physio"
                    },
                }

            };

            tap.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(tap);
            _db.CurriculumModules.Add(tap);

            var brillenoptik2 = new CurriculumModule()
            {
                ShortName = "Brillenoptik2",
                Name = "Brillenoptik II",
                ModuleId = "AOB200",
                Description = "Optik und Technik der Brille",
                ECTS = 5,
                MV = GetHost(fk06, "Bae"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Brillenoptik2"
                    },
                }

            };

            brillenoptik2.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(brillenoptik2);
            _db.CurriculumModules.Add(brillenoptik2);

            var m2 = new CurriculumModule()
            {
                ShortName = "Mathe2",
                Name = "Mathematik II",
                ModuleId = "AOB210",
                Description = "Polynome; Partialbruchzerlegung; Integralrechnung; Folgen und Reihen; Potenzreihen; Taylorreihe; Funktionen mehrerer Veränderlichen; partielle Ableitungen; Richtungsableitung;Tangentialebene; Fehlerrechnung etc.; Matrix-und Determinantenrechnung; lineare Gleichungssysteme; Ausgleichsrechnung; Differentialgleichung",
                ECTS = 5,
                MV = GetHost(fk06, "HIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Mathe2"
                    },
                }
            };

            m2.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(m2);
            _db.CurriculumModules.Add(m2);

            var phy2 = new CurriculumModule()
            {
                ShortName = "Physik II",
                Name = "Physik II",
                ModuleId = "AOB220",
                Description = "Teilchen und Kräfte",
                ECTS = 5,
                MV = GetHost(fk06, "HUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Physik II"
                    },
                }

            };

            phy2.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(phy2);
            _db.CurriculumModules.Add(phy2);

            var to1 = new CurriculumModule()
            {
                ShortName = "TO1",
                Name = "Technische Optik I",
                ModuleId = "AOB230",
                Description = "Berechnung der Ausbreitung eines Strahls in optischen Bauteilen mit ebenen Oberflächen",
                ECTS = 4,
                MV = GetHost(fk06, "FIK"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TO1"
                    },
                }
            };

            to1.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(to1);
            _db.CurriculumModules.Add(to1);

            var aaps = new CurriculumModule()
            {
                ShortName = "AAPS",
                Name = "Anatomie des Auges / Physiologie des Sehvorgangs",
                ModuleId = "AOB250",
                Description = "Einführung in die Genese des visuellen Systems",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AAPS"
                    },
                }

            };

            aaps.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(aaps);
            _db.CurriculumModules.Add(aaps);

            var tg = new CurriculumModule()
            {
                ShortName = "TG",
                Name = "Technologische Grundlagen",
                ModuleId = "AOB240",
                Description = "Fertigungsverfahren von Brillengläsern, Brillenfassungen und Kontaktlinsen",
                ECTS = 4,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Technologische Grundlagen"
                    },
                }
            };

            tg.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(tg);
            _db.CurriculumModules.Add(tg);

            // Fortsetzung 9.1.17

            var patho = new CurriculumModule()
            {
                ShortName = "Allgemeine Pathologie",
                Name = "Allgemeine Pathologie / Pathophysiologie ",
                ModuleId = "AOB350",
                Description = "Grundlegendes Verständnis mikro-und makropathologischer Erscheinungsbilder sowie pathophysiologischer Funktionsabläufe",
                ECTS = 6,
                MV = GetHost(fk06, "GIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeine Pathologie / Pathophysiologie"
                    },
                }

            };

            patho.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(patho);
            _db.CurriculumModules.Add(patho);

            var bwl = new CurriculumModule()
            {
                ShortName = "Grundlagen der BWL ",
                Name = "Grundlagen der BWL ",
                ModuleId = "AOB360",
                Description = "Dieses Modul vermittelt grundlegende fachübergreifende, insbesondere betriebswirtschaftliche Kenntnisse, die Fähigkeit, technische Produkte und betriebliche Prozesse nach wirtschaftlichen Kriterien zu analysieren, zu bewerten und zu gestalten sowie die Kompetenz, fach-und disziplinübergreifend im Team zu arbeiten.",
                ECTS = 4,
                MV = GetHost(fk06, "ZAN"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Grundlagen der BWL"
                    },
                }
            };

            bwl.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(bwl);
            _db.CurriculumModules.Add(bwl);

            var info = new CurriculumModule()
            {
                ShortName = "Informatik ",
                Name = "Informatik ",
                ModuleId = "AOB340",
                Description = "Den Aufbau und die Funktion moderner Computer und Betriebssysteme verstehen",
                ECTS = 5,
                MV = GetHost(fk06, "BRA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Informatik"
                    },
                }
            };

            info.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(info);
            _db.CurriculumModules.Add(info);


            var optmess = new CurriculumModule()
            {
                ShortName = "Optische Messtechnik ",
                Name = "Optische Messtechnik",
                ModuleId = "AOB320",
                Description = "Die Studierenden verstehen die Grundprinzipien der unterschiedlichen Messtechniken, deren Unterschiede, Anwendungsbereiche sowie deren Vor-und Nachteile",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optische Messtechnik"
                    },
                }

            };

            optmess.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(optmess);
            _db.CurriculumModules.Add(optmess);

            var opt1 = new CurriculumModule()
            {
                ShortName = "Optometrie I",
                Name = "Optometrie I, Grundlagen der Optometrie",
                ModuleId = "AOB310",
                Description = "Die Studierenden haben adäquate Kenntnisse in der physiologischen Optik, um optische Abbildungen im menschlichen Auge zu verstehen.",
                ECTS = 6,
                MV = GetHost(fk06, "BAE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optometrie I"
                    },
                }

            };

            opt1.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(opt1);
            _db.CurriculumModules.Add(opt1);

            var top2 = new CurriculumModule()
            {
                ShortName = "Technische Optik II ",
                Name = "Technische Optik II ",
                ModuleId = "AOB330",
                Description = "Es wird ein fundamentales ingenieurwissenschaftliches Grundwissen der Lichttechnik, des optischen Gerätebaus und der optischen Messtechnik für Augenoptiker angelegt.",
                ECTS = 4,
                MV = GetHost(fk06, "FIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Technische Optik II"
                    },
                }

            };

            top2.Groups.Add(GetGroup(AOB, "3"));
            AOB.Modules.Add(top2);
            _db.CurriculumModules.Add(top2);

            var kon1 = new CurriculumModule()
            {
                ShortName = "Kontaktlinsen I",
                Name = "Kontaktlinsen I",
                ModuleId = "AOB420",
                Description = "Die Studierenden beraten Fehlsichtige über die Vor-und Nachteile der Korrektion ihrer Fehlsichtigkeit mit Kontaktlinsen gegenüber der Brille.",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kontaktlinsen I"
                    },
                }
            };

            kon1.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(kon1);
            _db.CurriculumModules.Add(kon1);

            var control = new CurriculumModule()
            {
                ShortName = "Controlling/Marketing und Arbeitssicherheit ",
                Name = "Controlling/Marketing und Arbeitssicherheit ",
                ModuleId = "AOB630",
                Description = "Die Studierende haben adäquate Kenntnisse im Controlling und Marketing um Managementaufgaben im Augenoptischen Betrieb wahrnehmen zu können. ",
                ECTS = 5,
                MV = GetHost(fk06, "DRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Controlling/Marketing und Arbeitssicherheit"
                    },
                }

            };

            control.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(control);
            _db.CurriculumModules.Add(control);

            var wpm = new CurriculumModule()
            {
                ShortName = "Wahlpflichtmodul ",
                Name = "Wahlpflichtmodul ",
                ModuleId = "AOB460",
                Description = "Wahlpflichtmodul ",
                ECTS = 5,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Wahlpflichtmodul"
                    },
                }
            };

            wpm.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(wpm);
            _db.CurriculumModules.Add(wpm);

            var medqm = new CurriculumModule()
            {
                ShortName = "Medizin. Qualitätsmanagement / Gerätesicherheit ",
                Name = "Medizin. Qualitätsmanagement / Gerätesicherheit ",
                ModuleId = "AOB450",
                Description = "Kenntnisse über strategisches Qualitätsmanagement",
                ECTS = 5,
                MV = GetHost(fk06, "FUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Medizin. Qualitätsmanagement / Gerätesicherheit"
                    },
                }

            };

            medqm.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(medqm);
            _db.CurriculumModules.Add(medqm);

            var oku = new CurriculumModule()
            {
                ShortName = "Okuläre Anomalien ",
                Name = "Okuläre Anomalien ",
                ModuleId = "AOB435",
                Description = "grundlegendes Verständnis okulärer Anomalien",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Okuläre Anomalien"
                    },
                }
            };

            oku.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(oku);
            _db.CurriculumModules.Add(oku);


            var opto2 = new CurriculumModule()
            {
                ShortName = "Optometrie II, Augenglasbestimmung ",
                Name = "Optometrie II, Augenglasbestimmung ",
                ModuleId = "AOB410",
                Description = "Die Studierenden haben fundierte Kenntnis über die Grundlagen des Binokularsehens und dessen Störungen.",
                ECTS = 5,
                MV = GetHost(fk06, "BAE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optometrie II, Augenglasbestimmung"
                    },
                }

            };

            opto2.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(opto2);
            _db.CurriculumModules.Add(opto2);

            var kindopt = new CurriculumModule()
            {
                ShortName = "Kinderoptometrie",
                Name = "Kinderoptometrie",
                ModuleId = "AOB460",
                Description = "•kennen die rechtlichen Grundlagen der optometrischen Versorgung von Kindern",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kinderoptometrie"
                    },
                }
            };

            kindopt.Groups.Add(GetGroup(AOB, "4"));
            AOB.Modules.Add(kindopt);
            _db.CurriculumModules.Add(kindopt);

            var kont2 = new CurriculumModule()
            {
                ShortName = "Kontaktlinsen II, Kontaktlinsenanpassung ",
                Name = "Kontaktlinsen II, Kontaktlinsenanpassung ",
                ModuleId = "AOB520",
                Description = "Die Studierenden sind in der Lage formstabile Kontaktlinsen anzupassen",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kontaktlinsen II, Kontaktlinsenanpassung"
                    },
                }
            };

            kont2.Groups.Add(GetGroup(AOB, "5"));
            AOB.Modules.Add(kont2);
            _db.CurriculumModules.Add(kont2);


            var lowv = new CurriculumModule()
            {
                ShortName = "Low Vision ",
                Name = "Low Vision ",
                ModuleId = "AOB561",
                Description = "sehbehindertengerechte Bestimmung der Sehleistung durchführen und daraus den erforderlichen Versorgungsbedarf ableiten ",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Low Vision"
                    },
                }

            };

            lowv.Groups.Add(GetGroup(AOB, "5"));
            AOB.Modules.Add(lowv);
            _db.CurriculumModules.Add(lowv);

            var opto3 = new CurriculumModule()
            {
                ShortName = "Optometrie III: Verordnung von Sehhilfen ",
                Name = "Optometrie III: Verordnung von Sehhilfen ",
                ModuleId = "AOB510",
                Description = "Die Studierenden haben adäquate Kenntnisse über Entwicklung und altersbedingte Veränderungen des visuellen Systems.",
                ECTS = 5,
                MV = GetHost(fk06, "BAE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optometrie III: Verordnung von Sehhilfen"
                    },
                }

            };

            opto3.Groups.Add(GetGroup(AOB, "5"));
            AOB.Modules.Add(opto3);
            _db.CurriculumModules.Add(opto3);

            var optscreen = new CurriculumModule()
            {
                ShortName = "Optometrische Screeningverfahren ",
                Name = "Optometrische Screeningverfahren ",
                ModuleId = "AOB530",
                Description = "Verständnis gurndlegender Prinzipien der Früherkennung von Augenkrankheiten; Fähigkeit, die entsprechenden Verfahren anwenden und kritisch bewerten zu können.",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optometrische Screeningverfahren"
                    },
                }

            };

            optscreen.Groups.Add(GetGroup(AOB, "5"));
            AOB.Modules.Add(optscreen);
            _db.CurriculumModules.Add(optscreen);

            var pharma = new CurriculumModule()
            {
                ShortName = "Pharmakologie",
                Name = "Pharmakologie",
                ModuleId = "AOB540",
                Description = "Kenntniss der Grundlagen der Pharmakologie",
                ECTS = 5,
                MV = GetHost(fk06, "DIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Pharmakologie"
                    },
                }
            };

            pharma.Groups.Add(GetGroup(AOB, "5"));
            AOB.Modules.Add(pharma);
            _db.CurriculumModules.Add(pharma);

            var paeda = new CurriculumModule()
            {
                ShortName = "Pädagogik / Personalmanagement ",
                Name = "Pädagogik / Personalmanagement ",
                ModuleId = "AOB370",
                Description = "Betriebliche Ausbildungen planen und vorbereiten",
                ECTS = 4,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Pädagogik / Personalmanagement"
                    },
                }

            };

            paeda.Groups.Add(GetGroup(AOB, "6"));
            AOB.Modules.Add(paeda);
            _db.CurriculumModules.Add(paeda);

            var rheto = new CurriculumModule()
            {
                ShortName = "Praxisseminar / Rhetorik ",
                Name = "Praxisseminar / Rhetorik ",
                ModuleId = "AOB620",
                Description = "Fähigkeit Ergebnisse und Erfahrungen sicher und prägnant zu präsentieren",
                ECTS = 2,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Praxisseminar / Rhetorik"
                    },
                }
            };

            rheto.Groups.Add(GetGroup(AOB, "6"));
            AOB.Modules.Add(rheto);
            _db.CurriculumModules.Add(rheto);

            var kont3 = new CurriculumModule()
            {
                ShortName = "Kontaktlinsen III, klientenorientierte Kontaktlinsenanpassun",
                Name = "Kontaktlinsen III, klientenorientierte Kontaktlinsenanpassun",
                ModuleId = "AOB720",
                Description = "Die Studierenden erkennen mögliche Komplikationen, die beim Tragen von Kontaktlinsen auftreten können. Sie sind in der Lage, Ursachen zu benennen und das vorliegende Risiko einzuschätzen.",
                ECTS = 5,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Kontaktlinsen III"
                    },
                }

            };

            kont3.Groups.Add(GetGroup(AOB, "7"));
            AOB.Modules.Add(kont3);
            _db.CurriculumModules.Add(kont3);

            var ophthal = new CurriculumModule()
            {
                ShortName = "Ophthalmologische Therapie, Grundlagen",
                Name = "Ophthalmologische Therapie, Grundlagen",
                ModuleId = "AOB730",
                Description = "Gründliche Kenntnisse therapeutischer Möglichkeiten bei anlagebedingten Sehbehinderungen und Krankheiten des visuellen Systems einschließlich der Möglichkeiten individueller Hilfsmittel und sozialer Eingliederungshilfen",
                ECTS = 2,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ophthalmologische Therapie, Grundlagen"
                    },
                }
            };

            ophthal.Groups.Add(GetGroup(AOB, "7"));
            AOB.Modules.Add(ophthal);
            _db.CurriculumModules.Add(ophthal);


            var optover = new CurriculumModule()
            {
                ShortName = "Optometrische Versorgung  ",
                Name = "Optometrische Versorgung  ",
                ModuleId = "AOB760",
                Description = "selbständig eine optometrische Untersuchung planen und durchführen",
                ECTS = 8,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Optometrische Versorgung"
                    },
                }

            };

            optover.Groups.Add(GetGroup(AOB, "7"));
            AOB.Modules.Add(optover);
            _db.CurriculumModules.Add(optover);

            var bach = new CurriculumModule()
            {
                ShortName = "Bachelorseminar ",
                Name = "Bachelorseminar ",
                ModuleId = "AOB770",
                Description = "Das Modul befähigt die Studierenden praxisbezogene Probleme im Fachgebiet Augenoptik/Optometrie selbständig nach wissenschaftlichen Methoden zu bearbeiten. ",
                ECTS = 3,
                MV = GetHost(fk06, "EIS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorseminar"
                    },
                }
            };

            bach.Groups.Add(GetGroup(AOB, "7"));
            AOB.Modules.Add(bach);
            _db.CurriculumModules.Add(bach);

            var aw = new CurriculumModule()
            {
                ShortName = "Modul Allgemeinwissenschaften ",
                Name = "Modul Allgemeinwissenschaften ",
                ModuleId = "AOB260.1",
                Description = "Gemäß den Ankündigungen der FK Allgemeinwissenschaften",
                ECTS = 2,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Modul Allgemeinwissenschaften"
                    },
                }
            };

            aw.Groups.Add(GetGroup(AOB, "1"));
            AOB.Modules.Add(aw);
            _db.CurriculumModules.Add(aw);

            var aw2 = new CurriculumModule()
            {
                ShortName = "Modul Allgemeinwissenschaften ",
                Name = "Modul Allgemeinwissenschaften ",
                ModuleId = "AOB260.1",
                Description = "Gemäß den Ankündigungen der FK Allgemeinwissenschaften",
                ECTS = 2,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Modul Allgemeinwissenschaften"
                    },
                }
            };

            aw2.Groups.Add(GetGroup(AOB, "2"));
            AOB.Modules.Add(aw2);
            _db.CurriculumModules.Add(aw2);


            var bacharb = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "AOB750",
                Description = "Bachelorarbeit",
                ECTS = 12,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Bachelorarbeit"
                    },
                }

            };

            bacharb.Groups.Add(GetGroup(AOB, "7"));
            AOB.Modules.Add(bacharb);
            _db.CurriculumModules.Add(bacharb);

            var indprak = new CurriculumModule()
            {
                ShortName = "Industriepraktikum ",
                Name = "Industriepraktikum ",
                ModuleId = "AOB610",
                Description = "Industriepraktikum ",
                ECTS = 24,
                MV = GetHost(fk06, "N.N."),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Industriepraktikum "
                    },
                }

            };

            indprak.Groups.Add(GetGroup(AOB, "6"));
            AOB.Modules.Add(indprak);
            _db.CurriculumModules.Add(indprak);



            _db.SaveChanges();
        }

    }
}