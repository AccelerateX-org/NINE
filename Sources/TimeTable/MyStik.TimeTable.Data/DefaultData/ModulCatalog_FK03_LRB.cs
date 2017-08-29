using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogLRB_GS(ActivityOrganiser fk03, Curriculum LRB)
        {
            var Ingenieurmathematik1 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik1",
                Name = "Ingenieurmathematik1",
                ModuleId = "LRB1",
                Description = "Folgen und Reihen, Funktionen einer Variablen, Komplexe Zahlen, Lineare Akgebra",
                ECTS = 6,
                MV = GetHost(fk03, "SCL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Lecture,
                        ExternalId = "Ingenieurmathematik1"
                    },
                }

            };

            Ingenieurmathematik1.Groups.Add(GetGroup(LRB, "1"));
            LRB.Modules.Add(Ingenieurmathematik1);
            _db.CurriculumModules.Add(Ingenieurmathematik1);

            var TechnischeMechanik1 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik1",
                Name = "TechnischeMechanik1",
                ModuleId = "LRB2",
                Description = "Statik starrer Körper: Gleichgewichtsbedingungen an zentralen und allgemeinen Kräftesystemen, Schwerpunkt, Lagerreaktionen, Fachwerke, Schnittgrößen an Balken und Rahmen, Haftung und Reibung.",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik1"
                    },
                }
            };

            TechnischeMechanik1.Groups.Add(GetGroup(LRB, "1"));
            LRB.Modules.Add(TechnischeMechanik1);
            _db.CurriculumModules.Add(TechnischeMechanik1);

            var Produktentwicklung1 = new CurriculumModule()
            {
                ShortName = "Produktentwicklung1",
                Name = "Produktentwicklung1",
                ModuleId = "LRB3",
                Description = "Projektionsarten, Zweitafelprojektion inklusive der Grundkonstruktionen, Abwicklung von Körperoberflächen und Darstellung von Schnittflächen, Abbildung von Kreisen, Erlernen der Grundlagen des normgerechten technischen Zeichnens, eindeutige Abbildung elementarer Funktionen (Passungen, Oberflächenetc.), Grundlagen Design to X, z. B. Fertigungs-, Montagetechnik",
                ECTS = 7,
                MV = GetHost(fk03, "AMF"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produktentwicklung1"
                    },
                }
            };

            Produktentwicklung1.Groups.Add(GetGroup(LRB, "1"));
            LRB.Modules.Add(Produktentwicklung1);
            _db.CurriculumModules.Add(Produktentwicklung1);


            var Elektrotechnik = new CurriculumModule()
            {
                ShortName = "Elektrotechnik",
                Name = "Elektrotechnik",
                ModuleId = "LRB4",
                Description = "Fähigkeit zum Entwurf und Dimensionierung elektrischer Schaltungen unter Nutzung fundamentaler Bauelemente (Spannungs- und Stromquellen, Widerstände, Kondensatoren, Spulen), Stromstärke, Ohmsches Gesetz, Kirchhoffsche Regeln, Zweipolersatzquellen, Energie, Leistung, Wirkungsgrad, Magnetisches Feld, Fluss und Flussdichte, magnetischer Kreis, (Selbst-)Induktion, Spule. Komplexe Wechselstromrechnung, Zeigerdiagramme, Wechselstromwiderstände, Wirk-, Blind- und Scheinleistung, Drehstrom",
                ECTS = 5,
                MV = GetHost(fk03, "PAL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Elektrotechnik"
                    },
                }

            };

            Elektrotechnik.Groups.Add(GetGroup(LRB, "1"));
            LRB.Modules.Add(Elektrotechnik);
            _db.CurriculumModules.Add(Elektrotechnik);

            var Werkstofftechnik = new CurriculumModule()
            {
                ShortName = "Werkstofftechnik",
                Name = "Werkstofftechnik",
                ModuleId = "LRB5",
                Description = "Lernziel des Moduls ist die Fähigkeit zur Auswahl, Planung und Durchführung spanloser Fertigungsverfahren unter Berücksichtigung des Zusammenwirkens von Werkstoff, Konstruktion und Fertigung. Die Studierenden sollen in der Lage sein, aus verschiedenen Verfahren die technisch und wirtschaftlich optimale Lösung zu ermitteln sowie die Auswirkungen auf die Bauteileigenschaften zu beurteilen.",
                ECTS = 5,
                MV = GetHost(fk03, "SCR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie"
                    },
                }

            };

            Werkstofftechnik.Groups.Add(GetGroup(LRB, "1"));
            LRB.Modules.Add(Werkstofftechnik);
            _db.CurriculumModules.Add(Werkstofftechnik);

            var Allgemeinwissenschaften1 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften1",
                Name = "Allgemeinwissenschaften1",
                ModuleId = "LRB6",
                Description = "",
                ECTS = 2,
                MV = GetHost(fk03, ""),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Allgemeinwissenschaften1"
                    },
                }

            };

            var GrundlagenBWLWirtschaftsrecht = new CurriculumModule()
            {
                ShortName = "GrundlagenBWLWirtschaftsrecht",
                Name = "GrundlagenBWLWirtschaftsrecht",
                ModuleId = "LRB7",
                Description = "Betriebswirtschaftslehre und Wirtschaftsrecht",
                ECTS = 6,
                MV = GetHost(fk03, "EIC"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "GrundlagenBWLWirtschaftsrecht"
                    },
                }
            };

            GrundlagenBWLWirtschaftsrecht.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(GrundlagenBWLWirtschaftsrecht);
            _db.CurriculumModules.Add(GrundlagenBWLWirtschaftsrecht);


            var Ingenieurmathematik2 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik2",
                Name = "Ingenieurmathematik2",
                ModuleId = "LRB8",
                Description = "Kurven in der Ebene, Funktionen von mehreren Variablen, Gewöhnliche Differenzialrechnungen",
                ECTS = 6,
                MV = GetHost(fk03, "WOL"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurmathematik2"
                    },
                }

            };

            Ingenieurmathematik2.Groups.Add(GetGroup(LRB, "2"));
            LRB.Modules.Add(Ingenieurmathematik2);
            _db.CurriculumModules.Add(Ingenieurmathematik2);

            var TechnischeMechanik2 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik2",
                Name = "TechnischeMechanik2",
                ModuleId = "LRB9",
                Description = "Elastostatik (Beanspruchungen und Verformungen elastischer Körper): Elastostatische Grundlagen (Spannungszustand, Verzerrungszustand, Elastizitätsgesetz, Festigkeitshypothesen, Kerbwirkung), Kräfte und Verformungen in Stäben, Balkenbiegung (Flächenträgheitsmomente, einachsige und zweiachsige Biegung, Integration der Biegedifferentialgleichung, Superposition), Torsion (kreiszylindrische Querschnitte, dünnwandig geschlossene und dünnwandig offene Profile), zusammengesetzte Beanspruchungen bei Balken und Rahmen (Biegung, Zug/Druck, Torsion), Knicken von Stäben.",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik2"
                    },
                }
            };

            TechnischeMechanik2.Groups.Add(GetGroup(LRB, "2"));
            LRB.Modules.Add(TechnischeMechanik2);
            _db.CurriculumModules.Add(TechnischeMechanik2);

            var BauelementeLuftfahrzeuge1 = new CurriculumModule()
            {
                ShortName = "BauelementeLuftfahrzeuge1",
                Name = "BauelementeLuftfahrzeuge1",
                ModuleId = "LRB10",
                Description = "Fähigkeit zur Dimensionierung und Berechnung von Bauelementen der Luftfahrzeuge unter Beachtung von Normen und Vorschriften sowie der besonderen Einsatzbedingungen im Luftfahrzeug",
                ECTS = 5,
                MV = GetHost(fk03, "SPE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Maschinenelemente1"
                    },
                }

            };

            BauelementeLuftfahrzeuge1.Groups.Add(GetGroup(LRB, "2"));
            LRB.Modules.Add(BauelementeLuftfahrzeuge1);
            _db.CurriculumModules.Add(BauelementeLuftfahrzeuge1);

            var Produkentwicklung2 = new CurriculumModule()
            {
                ShortName = "Produkentwicklung2",
                Name = "Produkentwicklung2",
                ModuleId = "LRB11",
                Description = "Lastflussanalyse und –beschreibung, Funktionsanalyse und -beschreibung, Gesamtkonzepterarbeitung, Grundlagen des CAD-Systemaufbaus oder eines neuen 3D-CAD-Systems inkl. Datenmanagement (PDM)",
                ECTS = 5,
                MV = GetHost(fk03, "SCW"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produkentwicklung2"
                    },
                }
            };

            Produkentwicklung2.Groups.Add(GetGroup(LRB, "2"));
            LRB.Modules.Add(Produkentwicklung2);
            _db.CurriculumModules.Add(Produkentwicklung2);

            var SpanloseFertigung = new CurriculumModule()
            {
                ShortName = "SpanloseFertigung",
                Name = "SpanloseFertigung",
                ModuleId = "LRB12",
                Description = "Gießen, Schweißen, Umformtechnik, Zerstörende und zerstörungsfreie Werkstoff- und Bauteilprüfung",
                ECTS = 5,
                MV = GetHost(fk03, "SCR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik"
                    },
                }
            };

            SpanloseFertigung.Groups.Add(GetGroup(LRB, "2"));
            LRB.Modules.Add(SpanloseFertigung);
            _db.CurriculumModules.Add(SpanloseFertigung);



            var Ingenieurinformatik = new CurriculumModule()
            {
                ShortName = "Ingenieurinformatik",
                Name = "Ingenieurinformatik",
                ModuleId = "LRB13",
                Description = "Datentypen und Kontrollstrukturen, Funktionen, Standardfunktionen, Vektoren und Matrizen, Zeiger, modulare Programmierung, Bibliotheken., Anwendungen aus der Analysis, lineare und nichtlineare Gleichungssysteme, numerische Lösung von Differentialgleichungen, Eigenwert- und Eigenvektorprobleme",
                ECTS = 5,
                MV = GetHost(fk03, "REI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurinformatik"
                    },
                }
            };

            Ingenieurinformatik.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(Ingenieurinformatik);
            _db.CurriculumModules.Add(Ingenieurinformatik);

            var ChemieKunststofftechnik = new CurriculumModule()
            {
                ShortName = "ChemieKunststofftechnik",
                Name = "ChemieKunststofftechnik",
                ModuleId = "LRB14",
                Description = "Chemie(L2021) und Kunststofftechnik(L2022)",
                ECTS = 6,
                MV = GetHost(fk03, "HOS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ChemieKunststofftechnik"
                    },
                }
            };

            ChemieKunststofftechnik.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(ChemieKunststofftechnik);
            _db.CurriculumModules.Add(ChemieKunststofftechnik);

            var TechnischeMechanik3 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik3",
                Name = "TechnischeMechanik3",
                ModuleId = "LRB15",
                Description = "Kinetik: Kinematik des Massepunktes sowie des starren Körpers, Kinetik des Massenpunktes sowie des starren Körpers. Der Anwendungsfall bleibt auf die Ebene beschränkt. (Schwerpunktsatz, Drallsatz, Massenträgheitsmomente, Arbeitssatz und Energiesatz, Impulssatz und Stoß).",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeMechanik3"
                    },
                }
            };

            TechnischeMechanik3.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(TechnischeMechanik3);
            _db.CurriculumModules.Add(TechnischeMechanik3);

            var Fluidmechanik = new CurriculumModule()
            {
                ShortName = "Fluidmechanik",
                Name = "Fluidmechanik",
                ModuleId = "LRB16",
                Description = "Die Studierenden kennen die wichtigsten Begriffe und Modellbildungen der technischen Strömungslehre (inklusive Hydro- und Aerostatik), sind mit den elementaren Grundgesetzen und den Grenzen ihrer Gültigkeit vertraut, haben gelernt, die theoretischen Grundlagen zur Lösung konkreter Aufgaben anzuwenden, und sind somit in der Lage, verschiedenartige technische Strömungsprozesse und -aufgabenstellungen zu analysieren und mit angemessenen Methoden zu berechnen",
                ECTS = 5,
                MV = GetHost(fk03, "HAK"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Fluidmechanik"
                    },
                }
            };

            Fluidmechanik.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(Fluidmechanik);
            _db.CurriculumModules.Add(Fluidmechanik);

            var LuftRaumfahrttechnikGerätekonstruktion1 = new CurriculumModule()
            {
                ShortName = "LuftRaumfahrttechnikGerätekonstruktion1",
                Name = "LuftRaumfahrttechnikGerätekonstruktion1",
                ModuleId = "LRB17",
                Description = "Kenntnisse der grundlegenden, konstruktiven Gestaltungsprinzipien von Luft-und Raumfahrzeugen und deren zulassungstechnischen Nachweisführung",
                ECTS = 4,
                MV = GetHost(fk03, "SPE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "LuftRaumfahrttechnikGerätekonstruktion1"
                    },
                }
            };

            LuftRaumfahrttechnikGerätekonstruktion1.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(LuftRaumfahrttechnikGerätekonstruktion1);
            _db.CurriculumModules.Add(LuftRaumfahrttechnikGerätekonstruktion1);

            var BauelementeLuftfahrzeuge2 = new CurriculumModule()
            {
                ShortName = "BauelementeLuftfahrzeuge2",
                Name = "BauelementeLuftfahrzeuge2",
                ModuleId = "LRB18",
                Description = "Fähigkeit zur Dimensionierung, Berechnung und Nachweis Führung von Bauelementen der Luftfahrzeuge unter Beachtung von Normen und Vorschriften sowie der besonderen Einsatzbedingungen im Luftfahrzeug",
                ECTS = 4,
                MV = GetHost(fk03, "SPE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "BauelementeLuftfahrzeuge2"
                    },
                }
            };

            BauelementeLuftfahrzeuge2.Groups.Add(GetGroup(LRB, "3"));
            LRB.Modules.Add(BauelementeLuftfahrzeuge2);
            _db.CurriculumModules.Add(BauelementeLuftfahrzeuge2);


            var ThermodynamikIWärmeübertragung = new CurriculumModule()
            {
                ShortName = "ThermodynamikIWärmeübertragung",
                Name = "ThermodynamikIWärmeübertragung",
                ModuleId = "LRB19",
                Description = "Dieses Modul vermittelt die methodischen und fachlichen Qualifikationen zur thermodynamischen Analyse technischer Systeme",
                ECTS = 5,
                MV = GetHost(fk03, "GUB"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "ThermodynamikIWärmeübertragung"
                    },
                }
            };

            ThermodynamikIWärmeübertragung.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(ThermodynamikIWärmeübertragung);
            _db.CurriculumModules.Add(ThermodynamikIWärmeübertragung);

            var TechnischeDynamik = new CurriculumModule()
            {
                ShortName = "TechnischeDynamik",
                Name = "TechnischeDynamik",
                ModuleId = "LRB20",
                Description = "Die Studierenden sind in der Lage, dynamische Systeme mit einem oder mehreren Freiheitsgraden mittels analytischer Methoden zu modellieren und ggf. zu linearisieren. Sie können freie und erzwungene Schwingungen dynamischer Systeme analysieren. Sie besitzen die Fähigkeit, die modale Analyse für die Untersuchung vom dynamischen Verhalten mechanischer Systeme anzuwenden. Sie können Unwucht-Phänomene beurteilen und beherrschen die wichtigsten Methoden des Wuchtens von Rotoren.",
                ECTS = 5,
                MV = GetHost(fk03, "YUA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeChemie"
                    },
                }
            };

            TechnischeDynamik.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(TechnischeDynamik);
            _db.CurriculumModules.Add(TechnischeDynamik);

            var SpannendeFertigungBetriebsorganisation = new CurriculumModule()
            {
                ShortName = "SpannendeFertigungBetriebsorganisation",
                Name = "SpannendeFertigungBetriebsorganisation",
                ModuleId = "LRB21",
                Description = "",
                ECTS = 4,
                MV = GetHost(fk03, "RAS"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Simulationstechnik"
                    },
                }
            };

            SpannendeFertigungBetriebsorganisation.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(SpannendeFertigungBetriebsorganisation);
            _db.CurriculumModules.Add(SpannendeFertigungBetriebsorganisation);

            var FlugzeugRaumfahrzeugsysteme = new CurriculumModule()
            {
                ShortName = "FlugzeugRaumfahrzeugsysteme",
                Name = "FlugzeugRaumfahrzeugsysteme",
                ModuleId = "LRB22",
                Description = "",
                ECTS = 4,
                MV = GetHost(fk03, "KNO"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "FlugzeugRaumfahrzeugsysteme"
                    },
                }
            };

            FlugzeugRaumfahrzeugsysteme.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(FlugzeugRaumfahrzeugsysteme);
            _db.CurriculumModules.Add(FlugzeugRaumfahrzeugsysteme);

            var Aerodynamik = new CurriculumModule()
            {
                ShortName = "Aerodynamik",
                Name = "Aerodynamik",
                ModuleId = "LRB23",
                Description = "Entwurf von Flugzeugen und Raumfahrzeugen",
                ECTS = 5,
                MV = GetHost(fk03, "THI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Aerodynamik"
                    },
                }
            };

            Aerodynamik.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(Aerodynamik);
            _db.CurriculumModules.Add(Aerodynamik);

            var Leichtbau = new CurriculumModule()
            {
                ShortName = "Leichtbau",
                Name = "Leichtbau",
                ModuleId = "LRB24",
                Description = "Zentrales Lernziel ist das Verständnis der Bauprinzipien, der Analyse- und Auslegungsmethoden von Leichtbaustrukturen in der Luft- und Raumfahrttechnik.",
                ECTS = 5,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Leichtbau"
                    },
                }
            };

            Leichtbau.Groups.Add(GetGroup(LRB, "4"));
            LRB.Modules.Add(Leichtbau);
            _db.CurriculumModules.Add(Leichtbau);



        }

        public void InitCatalogLRB_Wahl(ActivityOrganiser fk03, Curriculum wi)
        {

            // WPMs, AW und Abschlussarbeit
            var aw = new CurriculumModule()
            {
                ShortName = "AW",
                Name = "Allgemeinwissenschaften",
                ModuleId = "W1",
                Description = "",
                ECTS = 4,
            };

            // gehört zu allen Studiengruppen im 5. Semester
            aw.Groups.Add(GetGroup(wi, "3 BIO"));
            aw.Groups.Add(GetGroup(wi, "3 INF"));
            aw.Groups.Add(GetGroup(wi, "3 TEC"));
            wi.Modules.Add(aw);
            _db.CurriculumModules.Add(aw);


            var wpm1 = new CurriculumModule()
            {
                ShortName = "WPM1",
                Name = "Fachwissenschaftliches Wahlpflichtmodul I",
                ModuleId = "W2",
                Description = "",
                ECTS = 4,
            };

            // gehört zu allen Studiengruppen im 4. Semester
            wpm1.Groups.Add(GetGroup(wi, "4 BIO"));
            wpm1.Groups.Add(GetGroup(wi, "4 INF"));
            wpm1.Groups.Add(GetGroup(wi, "4 TEC"));
            wi.Modules.Add(wpm1);
            _db.CurriculumModules.Add(wpm1);

            var wpm2 = new CurriculumModule()
            {
                ShortName = "WPM2",
                Name = "Fachwissenschaftliches Wahlpflichtmodul II",
                ModuleId = "W3",
                Description = "",
                ECTS = 4,
            };

            // gehört zu allen Studiengruppen im 5. Semester
            wpm2.Groups.Add(GetGroup(wi, "5 BIO"));
            wpm2.Groups.Add(GetGroup(wi, "5 INF"));
            wpm2.Groups.Add(GetGroup(wi, "5 TEC"));
            wi.Modules.Add(wpm2);
            _db.CurriculumModules.Add(wpm2);

            var wpm3 = new CurriculumModule()
            {
                ShortName = "WPM3",
                Name = "Fachwissenschaftliches Wahlpflichtmodul III",
                ModuleId = "W4",
                Description = "",
                ECTS = 4,
            };

            // gehört zu allen Studiengruppen im 6. Semester
            wpm3.Groups.Add(GetGroup(wi, "6 BIO"));
            wpm3.Groups.Add(GetGroup(wi, "6 INF"));
            wpm3.Groups.Add(GetGroup(wi, "6 TEC"));
            wi.Modules.Add(wpm3);
            _db.CurriculumModules.Add(wpm3);

            var prak = new CurriculumModule()
            {
                ShortName = "PRAK",
                Name = "Industriepraktikum",
                ModuleId = "W5",
                Description = "",
                ECTS = 20,
            };

            // gehört zu allen Studiengruppen im 6. Semester
            prak.Groups.Add(GetGroup(wi, "6 BIO"));
            prak.Groups.Add(GetGroup(wi, "6 INF"));
            prak.Groups.Add(GetGroup(wi, "6 TEC"));
            wi.Modules.Add(prak);
            _db.CurriculumModules.Add(prak);



            var bac = new CurriculumModule()
            {
                ShortName = "BAC",
                Name = "Abschlussarbeit",
                ModuleId = "W6",
                Description = "",
                ECTS = 12,
            };

            // gehört zu allen Studiengruppen im 7. Semester
            bac.Groups.Add(GetGroup(wi, "7 BIO"));
            bac.Groups.Add(GetGroup(wi, "7 INF"));
            bac.Groups.Add(GetGroup(wi, "7 TEC"));
            wi.Modules.Add(bac);
            _db.CurriculumModules.Add(bac);


            _db.SaveChanges();
        }

    }
}
