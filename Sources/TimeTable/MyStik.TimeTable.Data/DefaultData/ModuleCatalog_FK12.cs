using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogDE_GS(ActivityOrganiser fk12, Curriculum de)
        {
            var gestaltungstheorie = new CurriculumModule()
            {
                ShortName = "Gestaltungstheorie",
                Name = "Gestaltungstheorie",
                ModuleId = "M101",
                Description = "Gestaltungslehre, Design- und Kommunikationstheorie",
                ECTS = 6,
                MV = GetHost(fk12, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GT1"
                    },
                }
            };

            gestaltungstheorie.Groups.Add(GetGroup(de, "1"));
            de.Modules.Add(gestaltungstheorie);
            _db.CurriculumModules.Add(gestaltungstheorie);

            var gestaltungsgrundlagen1 = new CurriculumModule()
            {
                ShortName = "Gest. Grundl. 1",
                Name = "Gestaltungsgrundlagen 1",
                ModuleId = "M102",
                Description = "Synthese der Bereiche - Gestaltung, Wissenschaft und Technologie",
                ECTS = 6,
                MV = GetHost(fk12, "DAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GG1"
                    },
                }
            };

            gestaltungsgrundlagen1.Groups.Add(GetGroup(de, "1"));
            de.Modules.Add(gestaltungsgrundlagen1);
            _db.CurriculumModules.Add(gestaltungsgrundlagen1);

            var zeichnengrundl = new CurriculumModule()
            {
                ShortName = "Zeichnengrundl",
                Name = "Zeichnen Grundlagen",
                ModuleId = "M103",
                Description = "Zeichnerischen Grundtechniken",
                ECTS = 6,
                MV = GetHost(fk12, "GUE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ZG"
                    },
                }
            };

            zeichnengrundl.Groups.Add(GetGroup(de, "1"));
            de.Modules.Add(zeichnengrundl);
            _db.CurriculumModules.Add(zeichnengrundl);

            var fotografietypografie = new CurriculumModule()
            {
                ShortName = "FotoTypoGrundlagen",
                Name = "Fotografie und Typografie Grundlagen",
                ModuleId = "M104",
                Description = "Vermittlung fotografischer und typografischer Grundkentnisse",
                ECTS = 6,
                MV = GetHost(fk12, "DAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FTG"
                    },
                }

            };

            fotografietypografie.Groups.Add(GetGroup(de, "1"));
            de.Modules.Add(fotografietypografie);
            _db.CurriculumModules.Add(fotografietypografie);

            var digitaleGestaltung = new CurriculumModule()
            {
                ShortName = "DigitaleGestaltungGrundl",
                Name = "Grundlagen digitaler Gestaltung",
                ModuleId = "M105",
                Description = "Vermittlung konzeptioneller und technischer Grundkentnisse",
                ECTS = 6,
                MV = GetHost(fk12, "AMR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GDG"
                    },
                }

            };

            digitaleGestaltung.Groups.Add(GetGroup(de, "1"));
            de.Modules.Add(digitaleGestaltung);
            _db.CurriculumModules.Add(digitaleGestaltung);

            // Bis hier Semester 1
            // Ab hier Semester 2
            var designkultur = new CurriculumModule()
            {
                ShortName = "Designkultur",
                Name = "Designkultur",
                ModuleId = "M201",
                Description = "Grundkent. ästhetischen und historischen Wurzeln",
                ECTS = 6,
                MV = GetHost(fk12, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Dk"
                    },
                }

            };

            designkultur.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(designkultur);
            _db.CurriculumModules.Add(designkultur);

            var gestaltungsgrundlagen2 = new CurriculumModule()
            {
                ShortName = "Gest. Grundl. 2",
                Name = "Gestaltungsgrundlagen 2",
                ModuleId = "M202",
                Description = "Vermittlung Syntax der Gestaltungstheorie und -soziologie",
                ECTS = 6,
                MV = GetHost(fk12, "DAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GG2"
                    },
                }

            };

            gestaltungsgrundlagen2.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(gestaltungsgrundlagen2);
            _db.CurriculumModules.Add(gestaltungsgrundlagen2);

            var kreativität = new CurriculumModule()
            {
                ShortName = "Kreativität",
                Name = "Kreativität",
                ModuleId = "M203",
                Description = "Kentnisse, Fähigkeiten und Fertigkeiten zum Entwickeln von Ideen",
                ECTS = 6,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "KR"
                    },
                }

            };

            kreativität.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(kreativität);
            _db.CurriculumModules.Add(kreativität);

            var produktfoto = new CurriculumModule()
            {
                ShortName = "Produktfoto",
                Name = "Produktfotografie",
                ModuleId = "M204",
                Description = "Fähigkeit, dreidimensinoale Gegenstände zu fotografieren",
                ECTS = 6,
                MV = GetHost(fk12, "GRS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PF"
                    },
                }

            };

            produktfoto.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(produktfoto);
            _db.CurriculumModules.Add(produktfoto);

            var modefoto = new CurriculumModule()
            {
                ShortName = "Modefoto",
                Name = "Modefotografie",
                ModuleId = "M205",
                Description = "Theoretische und praktische Grundlagen der Modefotografie",
                ECTS = 6,
                MV = GetHost(fk12, "DEU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MF"
                    },
                }

            };

            modefoto.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(modefoto);
            _db.CurriculumModules.Add(modefoto);

            var mod1 = new CurriculumModule()
            {
                ShortName = "Modelling1",
                Name = "Modelling 1",
                ModuleId = "M206",
                Description = "Vermittlung Fähigkeiten zum sicheren Umgang mit den Maschinen der Werkstätten",
                ECTS = 6,
                MV = GetHost(fk12, "PET"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "M1"
                    },
                }

            };

            mod1.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(mod1);
            _db.CurriculumModules.Add(mod1);

            var darstellung = new CurriculumModule()
            {
                ShortName = "Darstellung",
                Name = "Darstellung 1",
                ModuleId = "M207",
                Description = "Theoretische und praktische Grundlagen der gängigen Zeichnentechniken",
                ECTS = 6,
                MV = GetHost(fk12, "NAU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DA"
                    },
                }

            };

            darstellung.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(darstellung);
            _db.CurriculumModules.Add(darstellung);

            var zeichnen = new CurriculumModule()
            {
                ShortName = "Zeichnen",
                Name = "Zeichnen 1",
                ModuleId = "M208",
                Description = "Freie, experimentelle und systematische Erstellung von Zeichnungen",
                ECTS = 6,
                MV = GetHost(fk12, "GUE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ZE"
                    },
                }

            };

            zeichnen.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(zeichnen);
            _db.CurriculumModules.Add(zeichnen);

            var typo = new CurriculumModule()
            {
                ShortName = "Typo",
                Name = "Typografie",
                ModuleId = "M209",
                Description = "Praktische und typografische Kenntnisse über Entwicklungen",
                ECTS = 6,
                MV = GetHost(fk12, "DAM"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TY"
                    },
                }

            };

            typo.Groups.Add(GetGroup(de, "2"));
            de.Modules.Add(typo);
            _db.CurriculumModules.Add(typo);

            var destr = new CurriculumModule()
            {
                ShortName = "destr",
                Name = "Designstragie",
                ModuleId = "M301",
                Description = "Fähigkeiten zur ganzheitlichen Problembetrachtung",
                ECTS = 6,
                MV = GetHost(fk12, "KIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DS"
                    },
                }

            };

            destr.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(destr);
            _db.CurriculumModules.Add(destr);

            var prWMP1 = new CurriculumModule()
            {
                ShortName = "ProjektWPM1",
                Name = "Projekt-Wahlpflichtmodul 1",
                ModuleId = "M302",
                Description = "Fähigkeiten im prozessorientierten Arbeiten",
                ECTS = 12,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PR1"
                    },
                }

            };

            prWMP1.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(prWMP1);
            _db.CurriculumModules.Add(prWMP1);

            var arf = new CurriculumModule()
            {
                ShortName = "ArchFoto",
                Name = "Architekturfotografie",
                ModuleId = "M303",
                Description = "Wettbewerbsbefähigung für Einstieg in Betätigungsumfeld",
                ECTS = 6,
                MV = GetHost(fk12, "OST"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ARF"
                    },
                }

            };

            arf.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(arf);
            _db.CurriculumModules.Add(arf);

            var bildjourn = new CurriculumModule()
            {
                ShortName = "Bildjourn",
                Name = "Bildjournalismus",
                ModuleId = "M304",
                Description = "Theoretische und praktische Grundlagen des Bildjournalismus",
                ECTS = 6,
                MV = GetHost(fk12, "NIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BJ"
                    },
                }

            };

            bildjourn.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(bildjourn);
            _db.CurriculumModules.Add(bildjourn);

            var mod2 = new CurriculumModule()
            {
                ShortName = "Modell2",
                Name = "Modelling 2",
                ModuleId = "M305",
                Description = "Modellbauerfahrung und dreidimensinoales 'Skizzieren'",
                ECTS = 6,
                MV = GetHost(fk12, "PET"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "M2"
                    },
                }

            };

            mod2.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(mod2);
            _db.CurriculumModules.Add(mod2);

            var ergo = new CurriculumModule()
            {
                ShortName = "Ergo",
                Name = "Ergonomie",
                ModuleId = "M306",
                Description = "Theoretische und praktische Grundlagen der Ergonomie",
                ECTS = 6,
                MV = GetHost(fk12, "KIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ergo"
                    },
                }

            };

            ergo.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(ergo);
            _db.CurriculumModules.Add(ergo);

            var foto = new CurriculumModule()
            {
                ShortName = "Foto",
                Name = "Fotografie",
                ModuleId = "M307",
                Description = "Kenntnisse, Fähigkeiten und Fertigkeiten zum Erstellen von Fotografien",
                ECTS = 6,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FO"
                    },
                }

            };

            foto.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(foto);
            _db.CurriculumModules.Add(foto);

            
            var digmed = new CurriculumModule()
            {
                ShortName = "Digmed",
                Name = "Digitale Medien",
                ModuleId = "M308",
                Description = "Qualifizierte Verwendng digitaler Technologien",
                ECTS = 6,
                MV = GetHost(fk12, "AMR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DM"
                    },
                }

            };

            digmed.Groups.Add(GetGroup(de, "3"));
            de.Modules.Add(digmed);
            _db.CurriculumModules.Add(digmed);

            var gesellgrundldesign = new CurriculumModule()
            {
                ShortName = "GesellGrundlDesigns",
                Name = "Gesellschaftliche Grundlagen des Designs",
                ModuleId = "M401",
                Description = "Grundlagenkenntnisse designrelevanter Rechte und Rechtsgebite",
                ECTS = 6,
                MV = GetHost(fk12, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GGD"
                    },
                }

            };

            gesellgrundldesign.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(gesellgrundldesign);
            _db.CurriculumModules.Add(gesellgrundldesign);

            var prWMP2 = new CurriculumModule()
            {
                ShortName = "ProjektWPM1",
                Name = "Projekt-Wahlpflichtmodul 2",
                ModuleId = "M402",
                Description = "Ausbau Fähigkeiten im prozessorienteirten Arbeiten",
                ECTS = 12,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PR2"
                    },
                }

            };

            prWMP2.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(prWMP2);
            _db.CurriculumModules.Add(prWMP2);

            var grndlwpm = new CurriculumModule()
            {
                ShortName = "Grundl-WPM",
                Name = "Grundlagen-Wahlpflichtmodul",
                ModuleId = "M403",
                Description = "Erweiterung und Vertiefung der fachlichen Kompetenzen",
                ECTS = 6,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GRWPM"
                    },
                }

            };

            grndlwpm.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(grndlwpm);
            _db.CurriculumModules.Add(grndlwpm);

            var videofilm = new CurriculumModule()
            {
                ShortName = "VideoFilm",
                Name = "Video/Film",
                ModuleId = "M404",
                Description = "Theoretische und praktische Grundlagen des Filmemachers",
                ECTS = 6,
                MV = GetHost(fk12, "BIR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "VF"
                    },
                }

            };

            videofilm.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(videofilm);
            _db.CurriculumModules.Add(videofilm);

            var mod3 = new CurriculumModule()
            {
                ShortName = "Modellin3",
                Name = "Modelling 3",
                ModuleId = "M405",
                Description = "Kenntnisse zu Konstruktion, Fertigungstechnik und Materialkunde",
                ECTS = 6,
                MV = GetHost(fk12, "PET"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "M3"
                    },
                }

            };

            mod3.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(mod3);
            _db.CurriculumModules.Add(mod3);

            var drucktec = new CurriculumModule()
            {
                ShortName = "Drucktechnik",
                Name = "Drucktechnik",
                ModuleId = "M406",
                Description = "Ewerb Fähigkeiten und Fertigkeiten zum handwerklichen Erstellen von Bildern, Kenntnisse diversen Druckvorstufen",
                ECTS = 6,
                MV = GetHost(fk12, "KEL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DT"
                    },
                }

            };

            drucktec.Groups.Add(GetGroup(de, "4"));
            de.Modules.Add(drucktec);
            _db.CurriculumModules.Add(drucktec);

            var praktsem = new CurriculumModule()
            {
                ShortName = "Praktsem",
                Name = "Praktikumsseminare",
                ModuleId = "M501",
                Description = "Reflektion und Einblick in das Berufsleben des Designers",
                ECTS = 5,
                MV = GetHost(fk12, "DEU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PS"
                    },
                }

            };

            praktsem.Groups.Add(GetGroup(de, "5"));
            de.Modules.Add(praktsem);
            _db.CurriculumModules.Add(praktsem);

            var betrprak = new CurriculumModule()
            {
                ShortName = "Betriebl. Praktikum",
                Name = "Betriebliches Praktikum",
                ModuleId = "M502",
                Description = "Anschlussfähigkeit an das Berufsleben bezüglich fachlicher Kompetenz",
                ECTS = 24,
                MV = GetHost(fk12, "DEU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PRAK"
                    },
                }

            };

            betrprak.Groups.Add(GetGroup(de, "5"));
            de.Modules.Add(betrprak);
            _db.CurriculumModules.Add(betrprak);

            var desmanagement = new CurriculumModule()
            {
                ShortName = "Designmanagement",
                Name = "Designmanagement",
                ModuleId = "M601",
                Description = "Beherrschung und Umsetzung von Designmanagement in die Praxis",
                ECTS = 6,
                MV = GetHost(fk12, "KIE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DEM"
                    },
                }

            };

            desmanagement.Groups.Add(GetGroup(de, "6"));
            de.Modules.Add(desmanagement);
            _db.CurriculumModules.Add(desmanagement);

            var projwpm3 = new CurriculumModule()
            {
                ShortName = "Projekt-WPM3",
                Name = "Projekt-Wahlpflichtmodul 3",
                ModuleId = "M602",
                Description = "Fähigkeiten im prozessorientierten Arbeiten mit dem Ziel einer problemorientierten Projektarbeit",
                ECTS = 12,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PWMP3"
                    },
                }

            };

            projwpm3.Groups.Add(GetGroup(de, "6"));
            de.Modules.Add(projwpm3);
            _db.CurriculumModules.Add(projwpm3);

            var projwpm4 = new CurriculumModule()
            {
                ShortName = "Projekt-WPM4",
                Name = "Projekt-Wahlpflichtmodul 4",
                ModuleId = "M603",
                Description = "Fähigkeiten im prozessorientierten Arbeiten mit dem Ziel einer problemorientierten Projektarbeit auf hohem Niveau",
                ECTS = 12,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "PWMP4"
                    },
                }

            };

            projwpm4.Groups.Add(GetGroup(de, "6"));
            de.Modules.Add(projwpm4);
            _db.CurriculumModules.Add(projwpm4);

            var existgründ = new CurriculumModule()
            {
                ShortName = "Existenzgründung",
                Name = "Projekt-Existenzgründung 4",
                ModuleId = "M701",
                Description = "Umfang und Vorraussetzungen einer Erfolg versprechenden Unternehmensgründung richtig einschätzen",
                ECTS = 6,
                MV = GetHost(fk12, "DEU"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EG"
                    },
                }

            };

            existgründ.Groups.Add(GetGroup(de, "7"));
            de.Modules.Add(existgründ);
            _db.CurriculumModules.Add(existgründ);

            var wissenschaftlarbeiten = new CurriculumModule()
            {
                ShortName = "WissenschaftlichesArbeiten",
                Name = "Wissenschaftliches Arbeiten",
                ModuleId = "M702",
                Description = "Erlernen und Anwenden der grundlegen Kenntnisse und Techniken des Wissenschaftlichen Arbeitens",
                ECTS = 6,
                MV = GetHost(fk12, "FRE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WA"
                    },
                }

            };

            wissenschaftlarbeiten.Groups.Add(GetGroup(de, "7"));
            de.Modules.Add(wissenschaftlarbeiten);
            _db.CurriculumModules.Add(wissenschaftlarbeiten);

            var BAarbeit = new CurriculumModule()
            {
                ShortName = "Bachelorarbeit",
                Name = "Bachelorarbeit",
                ModuleId = "M703",
                Description = "Anwendung gestalterischer und konzeptioneller Fach-, Kern- und Schlüsselkompetenzen in Entwurf und Ausführung einer eigenen Abschlussarbeit",
                ECTS = 14,
                MV = GetHost(fk12, "BUC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BA"
                    },
                }

            };

            BAarbeit.Groups.Add(GetGroup(de, "7"));
            de.Modules.Add(BAarbeit);
            _db.CurriculumModules.Add(BAarbeit);

            var algwissenwpm = new CurriculumModule()
            {
                ShortName = "AllgemeinwissWPM",
                Name = "Allgemeinwissenschaftliches Wahlpflichmodul",
                ModuleId = "M704",
                Description = "Individuelle Ergänzung des Kompetenzspektrums unter besonderer Berücksichtigung der Schlüsselkompetenzen",
                ECTS = 4,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "AWPM"
                    },
                }

            };

            algwissenwpm.Groups.Add(GetGroup(de, "7"));
            de.Modules.Add(algwissenwpm);
            _db.CurriculumModules.Add(algwissenwpm);

            var designtheorie = new CurriculumModule()
            {
                ShortName = "Design-Theorie",
                Name = "Design-Theorie",
                ModuleId = "101.1",
                Description = "Design-Theorie",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DT"
                    },
                }

            };

            designtheorie.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(designtheorie);
            _db.CurriculumModules.Add(designtheorie);

            var globalundinterkultur = new CurriculumModule()
            {
                ShortName = "GlobalisierungInterkult",
                Name = "Globalisierung und Interkulturalität",
                ModuleId = "101.2",
                Description = "Globalisierung und Interkulturalität",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GI"
                    },
                }

            };

            globalundinterkultur.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(globalundinterkultur);
            _db.CurriculumModules.Add(globalundinterkultur);

            var WMPinterdisz = new CurriculumModule()
            {
                ShortName = "WPM Interdisziplinarität",
                Name = "Wahlpflichtmodul Interdisziplinarität ",
                ModuleId = "102",
                Description = "Wahlpflichtmodul Interdisziplinarität",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "WPMI"
                    },
                }

            };

            WMPinterdisz.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(WMPinterdisz);
            _db.CurriculumModules.Add(WMPinterdisz);

            var projektrechercheanalyse1 = new CurriculumModule()
            {
                ShortName = "Projekt-Seminar Recherche/Analyse",
                Name = "Projekt-Seminar Recherche und Analyse",
                ModuleId = "103.1",
                Description = "Projekt 1",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P1"
                    },
                }

            };

            projektrechercheanalyse1.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(projektrechercheanalyse1);
            _db.CurriculumModules.Add(projektrechercheanalyse1);

            var projektexperminententwurf1 = new CurriculumModule()
            {
                ShortName = "Projekt-Atelier Experiment/Entwurf",
                Name = "Projekt-Atelier Experiment und Entwurf",
                ModuleId = "103.2",
                Description = "Projekt 1",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P1"
                    },
                }

            };

            projektexperminententwurf1.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(projektexperminententwurf1);
            _db.CurriculumModules.Add(projektexperminententwurf1);

            var projektfdokumentationpräsenation1 = new CurriculumModule()
            {
                ShortName = "Projekt-Labor Dokumentation/Präsenation",
                Name = "Projekt-Labor Dokumentation und Präsenation",
                ModuleId = "103.3",
                Description = "Projekt 1",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P1"
                    },
                }

            };

            projektfdokumentationpräsenation1.Groups.Add(GetGroup(de, "8"));
            de.Modules.Add(projektfdokumentationpräsenation1);
            _db.CurriculumModules.Add(projektfdokumentationpräsenation1);

            var designkritik = new CurriculumModule()
            {
                ShortName = "Design-Kritik",
                Name = "Design-Kritik",
                ModuleId = "201.1",
                Description = "Theoriemodul2",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "T2"
                    },
                }

            };

            designkritik.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(designkritik);
            _db.CurriculumModules.Add(designkritik);

            var ökonachhaltig = new CurriculumModule()
            {
                ShortName = "Ökologie/Nachhaltigkeit",
                Name = "Ökologie und Nachhaltigkeit",
                ModuleId = "201.2",
                Description = "Theoriemodul2",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "T2"
                    },
                }

            };

            ökonachhaltig.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(ökonachhaltig);
            _db.CurriculumModules.Add(ökonachhaltig);

            var exposémodul = new CurriculumModule()
            {
                ShortName = "Exposémodul",
                Name = "Exposémodul",
                ModuleId = "202",
                Description = "Exposémodul",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "EM"
                    },
                }

            };

            exposémodul.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(exposémodul);
            _db.CurriculumModules.Add(exposémodul);

            var projektrechercheanalyse2 = new CurriculumModule()
            {
                ShortName = "Projekt-Seminar Recherche/Analyse2",
                Name = "Projekt-Seminar Recherche und Analyse2",
                ModuleId = "203.1",
                Description = "Projekt 2",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P2"
                    },
                }

            };

            projektrechercheanalyse2.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(projektrechercheanalyse2);
            _db.CurriculumModules.Add(projektrechercheanalyse2);

            var projektexperminententwurf2 = new CurriculumModule()
            {
                ShortName = "Projekt-Atelier Experiment/Entwurf2",
                Name = "Projekt-Atelier Experiment und Entwurf2",
                ModuleId = "203.2",
                Description = "Projekt 2",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P2"
                    },
                }

            };

            projektexperminententwurf2.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(projektexperminententwurf2);
            _db.CurriculumModules.Add(projektexperminententwurf2);

            var projektfdokumentationpräsenation2 = new CurriculumModule()
            {
                ShortName = "Projekt-Labor Dokumentation/Präsenation2",
                Name = "Projekt-Labor Dokumentation und Präsenation2",
                ModuleId = "203.3",
                Description = "Projekt 2",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "P2"
                    },
                }

            };

            projektfdokumentationpräsenation2.Groups.Add(GetGroup(de, "9"));
            de.Modules.Add(projektfdokumentationpräsenation2);
            _db.CurriculumModules.Add(projektfdokumentationpräsenation2);

            var designverm = new CurriculumModule()
            {
                ShortName = "Design-Vermittlung",
                Name = "Design-Vermittlung",
                ModuleId = "301.1",
                Description = "Design-Vermittlung",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "DV"
                    },
                }

            };

            designverm.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(designverm);
            _db.CurriculumModules.Add(designverm);

            var zukunfsstrategien = new CurriculumModule()
            {
                ShortName = "Zukunfsstrategien",
                Name = "Zukunfsstrategien",
                ModuleId = "301.2",
                Description = "Zukunfsstrategien",
                ECTS = 3,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ZSTR"
                    },
                }

            };

            zukunfsstrategien.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(zukunfsstrategien);
            _db.CurriculumModules.Add(zukunfsstrategien);

            var masterexperimententwurf1 = new CurriculumModule()
            {
                ShortName = "Master-Atelier1 Exp/Entwurf",
                Name = "Master-Atelier1 Experiment und Entwurf",
                ModuleId = "302.1",
                Description = "Masterarbeit",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MA"
                    },
                }

            };

            masterexperimententwurf1.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(masterexperimententwurf1);
            _db.CurriculumModules.Add(masterexperimententwurf1);

            var masterexperimententwurf2 = new CurriculumModule()
            {
                ShortName = "Master-Atelier2 Exp/Entwurf",
                Name = "Master-Atelier2 Experiment und Entwurf",
                ModuleId = "302.2",
                Description = "Masterarbeit",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MA"
                    },
                }

            };

            masterexperimententwurf2.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(masterexperimententwurf2);
            _db.CurriculumModules.Add(masterexperimententwurf2);

            var masterexperimententwurf3 = new CurriculumModule()
            {
                ShortName = "Master-Atelier3 Exp/Entwurf",
                Name = "Master-Atelier3 Experiment und Entwurf",
                ModuleId = "302.3",
                Description = "Masterarbeit",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MA"
                    },
                }

            };

            masterexperimententwurf3.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(masterexperimententwurf3);
            _db.CurriculumModules.Add(masterexperimententwurf3);

            var masterdokumentpräs = new CurriculumModule()
            {
                ShortName = "Master-LaborDok/Präs",
                Name = "Master-Labor Dokumentation und Präsentation",
                ModuleId = "302.4",
                Description = "Masterarbeit",
                ECTS = 6,
                MV = GetHost(fk12, "-"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "MA"
                    },
                }

            };

            masterdokumentpräs.Groups.Add(GetGroup(de, "10"));
            de.Modules.Add(masterdokumentpräs);
            _db.CurriculumModules.Add(masterdokumentpräs);
        }
    }
}
    






