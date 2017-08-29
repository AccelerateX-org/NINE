using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitCatalogMBB_GS(ActivityOrganiser fk03, Curriculum MBB)
        {
            var Ingenieurmathematik1 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik1",
                Name = "Ingenieurmathematik1",
                ModuleId = "MBB1",
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

            Ingenieurmathematik1.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(Ingenieurmathematik1);
            _db.CurriculumModules.Add(Ingenieurmathematik1);

            var TechnischeMechanik1 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik1",
                Name = "TechnischeMechanik1",
                ModuleId = "MBB2",
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

            TechnischeMechanik1.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(TechnischeMechanik1);
            _db.CurriculumModules.Add(TechnischeMechanik1);

            var Produktentwicklung1 = new CurriculumModule()
            {
                ShortName = "Produktentwicklung1",
                Name = "Produktentwicklung1",
                ModuleId = "MBB3",
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

            Produktentwicklung1.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(Produktentwicklung1);
            _db.CurriculumModules.Add(Produktentwicklung1);


            var Elektrotechnik = new CurriculumModule()
            {
                ShortName = "Elektrotechnik",
                Name = "Elektrotechnik",
                ModuleId = "MBB4",
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

            Elektrotechnik.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(Elektrotechnik);
            _db.CurriculumModules.Add(Elektrotechnik);

            var Werkstofftechnik = new CurriculumModule()
            {
                ShortName = "Werkstofftechnik",
                Name = "Werkstofftechnik",
                ModuleId = "MBB5",
                Description = "Kenntnis und Verständnis für die in der Anwendung der Informatik erforderlichen Begriffe und Methoden; Fähigkeit, den Einsatz von Informatikmitteln zu analysieren, zu planen und kritisch zu beurteilen.",
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

            Werkstofftechnik.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(Werkstofftechnik);
            _db.CurriculumModules.Add(Werkstofftechnik);

            var Allgemeinwissenschaften1 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften1",
                Name = "Allgemeinwissenschaften1",
                ModuleId = "MBB6",
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

            Allgemeinwissenschaften1.Groups.Add(GetGroup(MBB, "1"));
            MBB.Modules.Add(Allgemeinwissenschaften1);
            _db.CurriculumModules.Add(Allgemeinwissenschaften1);

            var Allgemeinwissenschaften2 = new CurriculumModule()
            {
                ShortName = "Allgemeinwissenschaften2",
                Name = "Allgemeinwissenschaften2",
                ModuleId = "MBB7",
                Description = "",
                ECTS = 2,
                MV = GetHost(fk03, ""),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Chemie2"
                    },
                }
            };

            Allgemeinwissenschaften2.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(Allgemeinwissenschaften2);
            _db.CurriculumModules.Add(Allgemeinwissenschaften2);

            var Ingenieurmathematik2 = new CurriculumModule()
            {
                ShortName = "Ingenieurmathematik2",
                Name = "Ingenieurmathematik2",
                ModuleId = "MBB8",
                Description = "Kurven in der Ebene, Funktionen von mehreren Variablen, Gewöhnliche Differenzialrechnungen",
                ECTS = 4,
                MV = GetHost(fk03, "WAR"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Ingenieurmathematik2"
                    },
                }

            };

            Ingenieurmathematik2.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(Ingenieurmathematik2);
            _db.CurriculumModules.Add(Ingenieurmathematik2);

            var TechnischeMechanik2 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik2",
                Name = "TechnischeMechanik2",
                ModuleId = "MBB9",
                Description = "Elastostatik (Beanspruchungen und Verformungen elastischer Körper): Elastostatische Grundlagen (Spannungszustand, Verzerrungszustand, Elastizitätsgesetz, Festigkeitshypothesen, Kerbwirkung), Kräfte und Verformungen in Stäben, Balkenbiegung (Flächenträgheitsmomente, einachsige und zweiachsige Biegung, Integration der Biegedifferentialgleichung, Superposition), Torsion (kreiszylindrische Querschnitte, dünnwandig geschlossene und dünnwandig offene Profile), zusammengesetzte Beanspruchungen bei Balken und Rahmen (Biegung, Zug/Druck, Torsion), Knicken von Stäben.",
                ECTS = 6,
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

            TechnischeMechanik2.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(TechnischeMechanik2);
            _db.CurriculumModules.Add(TechnischeMechanik2);

            var Maschinenelemente1 = new CurriculumModule()
            {
                ShortName = "Maschinenelemente1",
                Name = "Maschinenelemente1",
                ModuleId = "MBB10",
                Description = "Festigkeitslehre auf Basis der FKM-Richtlinie mit folgenden Gliederungspunkten:a) Kräfte, Momente und Spannungen, b) Statische Festigkeitslehre, c) Dynamische Festigkeitslehre. Grundlegendes Dimensionieren von Schraubenverbindungen. Grundlegendes Dimensionieren von Kleb-, Löt-, Niet- und Schweißverbindungen. Grundlegendes Dimensionieren von Bolzen- und Stiftverbindungen",
                ECTS = 5,
                MV = GetHost(fk03, "LOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Maschinenelemente1"
                    },
                }

            };

            Maschinenelemente1.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(Maschinenelemente1);
            _db.CurriculumModules.Add(Maschinenelemente1);

            var Produkentwicklung2 = new CurriculumModule()
            {
                ShortName = "Produkentwicklung2",
                Name = "Produkentwicklung2",
                ModuleId = "MBB11",
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

            Produkentwicklung2.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(Produkentwicklung2);
            _db.CurriculumModules.Add(Produkentwicklung2);

            var SpanloseFertigung = new CurriculumModule()
            {
                ShortName = "SpanloseFertigung",
                Name = "SpanloseFertigung",
                ModuleId = "MBB12",
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

            SpanloseFertigung.Groups.Add(GetGroup(MBB, "2"));
            MBB.Modules.Add(SpanloseFertigung);
            _db.CurriculumModules.Add(SpanloseFertigung);


      
            var Ingenieurinformatik = new CurriculumModule()
            {
                ShortName = "Ingenieurinformatik",
                Name = "Ingenieurinformatik",
                ModuleId = "MBB13",
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

            Ingenieurinformatik.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(Ingenieurinformatik);
            _db.CurriculumModules.Add(Ingenieurinformatik);

            var GrundlagenBWLWirtschaftsrecht = new CurriculumModule()
            {
                ShortName = "GrundlagenBWLWirtschaftsrecht",
                Name = "GrundlagenBWLWirtschaftsrecht",
                ModuleId = "MBB14",
                Description = "Betriebswirtschaftslehre und Wirtschaftsrecht",
                ECTS = 4,
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

            GrundlagenBWLWirtschaftsrecht.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(GrundlagenBWLWirtschaftsrecht);
            _db.CurriculumModules.Add(GrundlagenBWLWirtschaftsrecht);

            var ChemieKunststofftechnik = new CurriculumModule()
            {
                ShortName = "ChemieKunststofftechnik",
                Name = "ChemieKunststofftechnik",
                ModuleId = "MBB15",
                Description = "Chemie(M2022) und Kunststofftechnik(M2021)",
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

            ChemieKunststofftechnik.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(ChemieKunststofftechnik);
            _db.CurriculumModules.Add(ChemieKunststofftechnik);

            var TechnischeMechanik3 = new CurriculumModule()
            {
                ShortName = "TechnischeMechanik3",
                Name = "TechnischeMechanik3",
                ModuleId = "CHB16",
                Description = "Kinetik: Kinematik des Massepunktes sowie des starren Körpers, Kinetik des Massenpunktes sowie des starren Körpers. Der Anwendungsfall bleibt auf die Ebene beschränkt. (Schwerpunktsatz, Drallsatz, Massenträgheitsmomente, Arbeitssatz und Energiesatz, Impulssatz und Stoß).",
                ECTS = 4,
                MV = GetHost(fk03, "MID"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Werkstofftechnik2"
                    },
                }
            };

            TechnischeMechanik3.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(TechnischeMechanik3);
            _db.CurriculumModules.Add(TechnischeMechanik3);

            var Maschinenelemente2 = new CurriculumModule()
            {
                ShortName = "Maschinenelemente2",
                Name = "Maschinenelemente2",
                ModuleId = "MBB17",
                Description = "Die Studierenden sind in der Lage, Maschinenelemente unter Beachtung von Normen und Auslegungsvorschriften zu berechnen",
                ECTS = 6,
                MV = GetHost(fk03, "KNA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Maschinenelemente2"
                    },
                }
            };

            Maschinenelemente2.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(Maschinenelemente2);
            _db.CurriculumModules.Add(Maschinenelemente2);

            var Produkentwicklung3 = new CurriculumModule()
            {
                ShortName = "Produkentwicklung3",
                Name = "Produkentwicklung3",
                ModuleId = "MBB18",
                Description = "Gestaltung von Wälzlagerungen, Wellen, Welle-Nabe-Verbindungen und Federn unter Berücksichtigung der Herstellbarkeit, Zeichnerische Darstellung von Maschinen und bewegten Baugruppen, Berechnung und Dimensionierung von Wälzlagerungen, Wellen, Welle-Nabe-Verbindungen und Federn, Einzelteildarstellung mit fertigungsgerechter Bemaßung, Funktionsgerechte Darstellung von Maschinen und bewegten Baugruppen",
                ECTS = 4,
                MV = GetHost(fk03, "LOE"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produkentwicklung3"
                    },
                }
            };

            Produkentwicklung3.Groups.Add(GetGroup(MBB, "3"));
            MBB.Modules.Add(Produkentwicklung3);
            _db.CurriculumModules.Add(Produkentwicklung3);


            var TechnischeStrömungsmechanik = new CurriculumModule()
            {
                ShortName = "TechnischeStrömungsmechanik",
                Name = "TechnischeStrömungsmechanik",
                ModuleId = "MBB19",
                Description = "Die Studierenden kennen die Terminologie und Modellbildungen der technischen Strömungslehre (inklusive Hydro- und Aerostatik), sind mit den elementaren Grundgesetzen und den Grenzen ihrer Gültigkeit vertraut, können die theoretischen Grundlagen zur Lösung konkreter Aufgaben anwenden, und sind in der Lage, technische Strömungsprozesse und -aufgabenstellungen zu analysieren und mit angemessenen Methoden zu berechnen.",
                ECTS = 5,
                MV = GetHost(fk03, "SCH"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "TechnischeStrömungsmechanik"
                    },
                }
            };

            TechnischeStrömungsmechanik.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(TechnischeStrömungsmechanik);
            _db.CurriculumModules.Add(TechnischeStrömungsmechanik);

            var ThermodynamikIWärmeübertragung = new CurriculumModule()
            {
                ShortName = "ThermodynamikIWärmeübertragung",
                Name = "ThermodynamikIWärmeübertragung",
                ModuleId = "MBB20",
                Description = "Dieses Modul vermittelt die methodischen und fachlichen Qualifikationen zur thermodynamischen Analyse technischer Systeme",
                ECTS = 6,
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

            ThermodynamikIWärmeübertragung.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(ThermodynamikIWärmeübertragung);
            _db.CurriculumModules.Add(ThermodynamikIWärmeübertragung);

            var TechnischeDynamik = new CurriculumModule()
            {
                ShortName = "TechnischeDynamik",
                Name = "TechnischeDynamik",
                ModuleId = "MBB21",
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

            TechnischeDynamik.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(TechnischeDynamik);
            _db.CurriculumModules.Add(TechnischeDynamik);

            var SpannendeFertigungBetriebsorganisation = new CurriculumModule()
            {
                ShortName = "SpannendeFertigungBetriebsorganisation",
                Name = "SpannendeFertigungBetriebsorganisation",
                ModuleId = "MBB22",
                Description = "",
                ECTS = 5,
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

            SpannendeFertigungBetriebsorganisation.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(SpannendeFertigungBetriebsorganisation);
            _db.CurriculumModules.Add(SpannendeFertigungBetriebsorganisation);

            var RegelunsMesstechnik = new CurriculumModule()
            {
                ShortName = "RegelunsMesstechnik",
                Name = "RegelunsMesstechnik",
                ModuleId = "MBB23",
                Description = "",
                ECTS = 6,
                MV = GetHost(fk03, "THI"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "RegelunsMesstechnik"
                    },
                }
            };

            RegelunsMesstechnik.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(RegelunsMesstechnik);
            _db.CurriculumModules.Add(RegelunsMesstechnik);

            var Produktentwicklung4 = new CurriculumModule()
            {
                ShortName = "Produktentwicklung4",
                Name = "Produktentwicklung4",
                ModuleId = "MBB24",
                Description = "",
                ECTS = 4,
                MV = GetHost(fk03, "KNA"),
                ModuleCourses = new List<ModuleCourse>
                {
                    new ModuleCourse
                    {
                        CourseType = CourseType.Seminar,
                        ExternalId = "Produktentwicklung4"
                    },
                }
            };

            Produktentwicklung4.Groups.Add(GetGroup(MBB, "4"));
            MBB.Modules.Add(Produktentwicklung4);
            _db.CurriculumModules.Add(Produktentwicklung4);



        }

        public void InitCatalogMBB_Wahl(ActivityOrganiser fk03, Curriculum wi)
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
