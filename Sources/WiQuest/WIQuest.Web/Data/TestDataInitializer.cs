using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    //public class TestDataInitializer : DropCreateDatabaseAlways<QuestDbContext>
    public class TestDataInitializer : DropCreateDatabaseIfModelChanges<QuestDbContext>
    {
        private string _baseDir;

        public TestDataInitializer(string path)
        {
            _baseDir = path;
        }

        protected override void Seed(QuestDbContext context)
        {
            InitCategories(context);

            InitMatheFragen(context);

            InitTechnikFragen(context);

            InitNaturwissenschaftenFragen(context);

            InitWirtschaftFragen(context);

            base.Seed(context);
        }




        private void InitMatheFragen(QuestDbContext context)
        {
            var catMathe = GetCategory(context, "Mathematik");

            var i = 0;
            var j = 0;

            j = 0;
            var question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Image = GetImage(context, "mathematik_flächeninhalt.jpg"),
                Title = "Mathematik",
                Text = "Bei einem Rechteck werden beide Seiten um 10% verlängert. Um wie viel Prozent vergrößert sich die Fläche des Rechtecks?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "10",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "20",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "21",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "25",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                       new QuestionAnswer
                    {
                        Text = "100",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Image = GetImage(context, "mathematik_dreieck.jpg"),
                Title = "Mathematik",
                Text = "Ein rechtwinkliges Dreieck hat die Abmessungen a=6 und c=10. Wie groß ist sein Flächeninhalt?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "12",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "24",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "36",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "16",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "13",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Text = "Welche der folgenden Umformungen ist richtig?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "(a+b)² = a² + b²",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "(a+b)² = a² + ab + b²",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "(a+b)² = a² + 2ab + b²",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Keine der vorgegebenen Umformungen trifft zu",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Image = GetImage(context, "mathematik_funktionsgraph.jpg"),
                Title = "Mathematik",
                Text = "Ordnen Sie dem Funktiongraphen die richtige Funktionsgleichung zu.",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "y = 0,5x + 2",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x + y = 1",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x - y = 1",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "y = x + 1",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "y = 1",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Text = "In welchem Punkt schneiden sich die beiden Geraden y = 2x - 1 und y = -3x + 4 ?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "(0,0)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "(-1,0)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "(0,1)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "(1,1)",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "Die beiden Geraden schneiden sich nicht",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Text = "Eine Gerade geht durch die Punkte (-1,5) und (2,-1). Wie lautet die Geradengleichung?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "2x - 3",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "3x + 6",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x - 3",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "-2x + 3",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "-x + 5",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Text = "Lösen Sie folgenden Ausdruck nach x auf 2(3x - 4) + 10 = 4x - 6",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "x = -5",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x = -4",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x = 1",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "x = 0",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "x = -1",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Text = "Leiten Sie folgende Funktionen nach x ab. f(x) = 3 + 4x + 9a²",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "4 + 18a",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "3a³",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "9",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "4x",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "4",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Mathematik",
                Image = GetImage(context, "mathematik_erdgraben.jpg"),
                Text = "Für den Bau eines Kanals muss auf einer Länge von 1km der folgende Erdgraben ausgehoben werden. Wie viele m³ Erde wurden dafür ausgehoben?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "45.000 m³",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "55.000 m³",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "80.000 m³",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "85.000 m³",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                     new QuestionAnswer
                    {
                        Text = "125.000 m³",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },

                }
            };

            context.Questions.Add(question);
            context.SaveChanges();
        }

        private void InitTechnikFragen(QuestDbContext context)
        {
            var catMathe = GetCategory(context, "Technik");

            var i = 0;
            var j = 0;

            j = 0;
            var question = new Question
            {
                Category = catMathe,
                Reihenfolge = ++i,
                Title = "Technik",
                Image = GetImage(context, "technik_falten1.jpg"),
                Text = "Welcher der folgenden vier Körper kann aus der Faltvorlage gebildet werden?",
                Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "a)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "b)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "c)",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "d)",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "kein Körper",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
            };

            context.Questions.Add(question);
            context.SaveChanges();


            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "technik_zahnräder.jpg"),
                 Title = "Technik",
                 Text = "Die abgebildeten Zahnräder greifen alle ineinander. Welche Zahnräder drehen sich in die gleiche Richtung wie das Zahnrad X?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Die Zahnräder 3 und 4",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Zahnräder 1, 3 und 5",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Zahnräder 2, 4 und 6",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Zahnräder blockieren und können sich nicht drehen",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Technik",
                Image = GetImage(context, "technik_falten2.jpg"),
                Text = "Welcher der folgenden vier Körper kann aus der Faltvorlage gebildet werden?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "a)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "b)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "c)",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "d)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "kein Körper",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Technik",
                 Text = "Ein Vater und seine Tochter sitzen auf einer Wippe. Der Vater ist doppelt so schwer wie seine Tochter. Wie müssen sich Vater und Tochter auf der Wippe platzieren, um sie waagrechten Gleichgewicht zu halten?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Der Vater sitzt doppelt so weit vom Drehpunkt entfernt wie die Tochter",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Tochter sitzt genauso weit vom Drehpunkt entfernt wie der Vater",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Tochter sitzt viermal so weit vom Drehpunkt entfernt wie der Vater",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Der Vater sitzt halb so weit vom Drehpunkt entfernt wie die Tochter",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "technik_stange_zahnräder.jpg"),
                 Title = "Technik",
                 Text = "Das abgebildete Antriebsrad ist durch die Schubstange mit der Scheibe verbunden. Wie bewegt sich die Scheibe, wenn sich das Antriebsrad in Pfeilrichtung bewegt?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Die Scheibe bewegt sich in Richtung A",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Scheibe bewegt sich in Richtung B",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Scheibe bewegt sich hin und her",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Scheibe bewegt sich gar nicht",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "technik_wippe.jpg"),
                 Title = "Technik",
                 Text = "Auf den Angriffspunkt A der Wippe wirken 2 Newton. Wieviel Kraft muss auf den Angriffspunkt D wirken wenn B mit 4 Newton und C mit einem Newton belastet ist? Die Punkte A und D sollen doppelt so weit vom Mittelpunkt entfernt liegen wie B und C.",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "2 Newton",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "4,5 Newton",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "3 Newton",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "3,5 Newton",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Technik",
                Image = GetImage(context, "technik_falten3.jpg"),
                Text = "Welcher der folgenden vier Körper kann aus der Faltvorlage gebildet werden?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "a)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "b)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "c)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "d)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "kein Körper",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Technik",
                 Image = GetImage(context, "technik_falten4.jpg"),
                Text = "Welcher der folgenden vier Körper kann aus der Faltvorlage oben gebildet werden?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "a)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "b)",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "c)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "kein Körper",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "technik_eisenstange.jpg"),
                 Title = "Technik",
                 Text = "Mit welcher Stange erfordert es am meisten Karft, das angeklebte Rechteck anzuheben?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Mit Stange A",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Mit Stange B",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Mit Stange C",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Bei allen Stangen gleich",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

        }

        private void InitNaturwissenschaftenFragen(QuestDbContext context)
        {
            var catMathe = GetCategory(context, "Naturwissenschaften");

            var i = 0; 
            var j = 0;

            j = 0; 
            var question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Wie verhält sich der Druck eines Autoreifens bei heißen Sommertemperaturen?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Der Reifendruck nimmt zu",
                        IsCorrect = true, 
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Der Reifendruck nimmt ab",
                        IsCorrect = false, 
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Der Reifendruck bleibt konstant",
                        IsCorrect = false, 
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Der Reifendruck ist unabhängig von äußeren Faktoren",
                        IsCorrect = false, 
                        Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();


            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Welche der unten genannten Verbindungen ist ein Produkt der unvollständigen Verbrennung von Kohlenwasserstoff?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "O₃",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "O₂",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "CO",
                        IsCorrect = true,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "CL₂",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "SO₂",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Sie wollen ein Bauteil mit möglichst großer Wärmeleitfähigkeit herstellen. Was würden Sie nehmen?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Aluminium",
                        IsCorrect = true, 
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Kunststoff",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Glas",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Stahl",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Holz",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Die Dichte von Stahl beträgt 7,85 Gramm pro Kubikzentimeter. Wie groß ist die Dichte in der Einheit [kg/m³]?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "0,785 kg/m³",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "78,5 kg/m³",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "780,5 kg/m³",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "7850 kg/m³",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "naturwissenschaft_geschwindigkeit1.jpg"),
                 Title = "Naturwissenschaften",
                 Text = "In der Grafik ist die Geschwindigkeit v eines Objekts über die Zeit t aufgetragen? Welche der folgenden Aussagen trifft zu?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Die Beschleunigung des Objekts in den ersten 3 Sekunden beträgt 18 m/s²",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Durchschnittsgeschwindigkeit des Objekts beträgt 6 m/s",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Verzögerung des Objekts in den letzten zwei Sekunden beträgt 6 m/s²",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die zurückgelegte Strecke entspricht der Länge des Graphen",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Wenn sich zwei elektrisch geladene Teilchen gegenseitig mit gleicher Kraft abstoßen, welche Bedingung muss dann für die Ladung gelten?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Die Ladungen haben einen unterschiedlichen Betrag",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Ladungen haben unterschiedliche Vorzeichen",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Ladungen haben die gleichen Vorzeichen",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Die Ladungen sind durch eine bestimmte Strecke voneinander getrennt",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Zwei elektrisch positiv geladene Körper mit verschwindend kleiner Masse befinden sich im Abstand von 2m zueinander. Sie sollen einander auf 1m Abstand angenähert werden. Welche Aussage Trifft zu?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Dabei muss keine Kraft aufgewendet werden",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Dabei wird Energie frei",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Dabei muss eine Kraft aufgewendet werden, die immer größer wird, je näher sich die Körper kommen",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Dabei muss eine Kraft aufgewendet werden, die immer kleiner wird, je näher sich die Körper kommen",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Image = GetImage(context, "naturwissenschaft_e_feld.jpg"),
                 Title = "Naturwissenschaften",
                 Text = "Welches Diagramm repräsentiert am besten das elektrische Feld, das von zwei entgegengesetzt geladenen Partikeln erzeugt wird?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "a)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "b)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "c)",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "d)",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Naturwissenschaften",
                 Text = "Die mittlere Leistung ist definiert als",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "Kraft mal Zeit",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Kraft pro Weg",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Arbeit pro Weg",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "Arbeit pro Zeit",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();
        }


        private void InitWirtschaftFragen(QuestDbContext context)
        {
            var catMathe = GetCategory(context, "Wirtschaft");

            var i = 0; 
            var j = 0;

            j = 0; 
            var question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Sie bezahlen als Endpreis 7140 Euro für eine Fräßmaschine. Welcher Nettobetrag errechnet sich daraus bei der Mehrwertsteuer von 19%?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "6200 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "8496,6 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "6672,9 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "4500 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "6000 Euro",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Von 40.000 Produkten weisen 2500 einen Fehler auf. Das entspricht wieviel Prozent?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "0,62 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,5 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "7,5 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "6,25 %",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,0475 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Für einen Euro müssen Sie derzeit 1,36 US-Dollar bezahlen. Wieviel Euro kostet ein Dollar?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "1,36",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "1,20",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,74",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,83",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,90",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Eine Maschine liefert 4000 Stück pro Tag. Durch eine Investition von 36.000 Euro würde diese Maschine 100 Stück pro Tag mehr produzieren. Der Gewinn pro Stück liegt bei 1,50 Euro. Nach wievielen Tagen hat sich die Investition amortisiert?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "120",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "365",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "360",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "240",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "200",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; 
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Ein Autohersteller möchte seine Produktpalette um ein weiteres Modell erweitern. Die jährlichen fixen Kosten (=Kosten die in der Höhe unabhänig von der produzierten Menge sind) betragen 1.000.000 €, pro Auto fallen zusätzlich 10.000 € an Produktionskosten und Vertriebskosten an. Der angenommene netto Verkaufspreis pro Auto beträgt 15.000 €. Wie viele Autos der neuen Modellreihe muss der Autohersteller pro Jahr mindestens verkaufen, um keinen Verlust durch die Erweiterung der Produktpalette zu machen?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "150",
                        IsCorrect = false, 
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "100",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "225",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "300",
                        IsCorrect = false,
                        Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "200",
                        IsCorrect = true, 
                        Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0;
            question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Die Rendite einer Investition ist definiert als der jährliche Gewinn im Vergleich zu dem Kapitaleinsatz. Bei einem Sägewerk werden pro Monat 3.000 Euro eingenommen. Die Anschaffungskosten betrugen 10.800.000 Euro. Was ist die Rendite dieser Kapitalanlage?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "3,33 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "7,5 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "0,33 %",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "15 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "33 %",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Die Mehrwertsteuer wurde in der BRD am 01.01.2007 von 16% auf 19% erhöht. Ein Produkt kostete nach der Erhöhung genau 5.950 Euro. Wie groß ist die Preiserhöhung durch die Änderung des Steuersatzes bei diesem Produkt gerundet?",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "200 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "150 Euro",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "180 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "110 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "50 Euro",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Das Angebot an technischen Sägewerken wird immer größer. Anzunehmen ist hierbei eine vereinfachte Form des Marktes. Das bedeutet, dass der Preis der Anlagen...",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "tendenziell steigt",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "tendenziell fällt",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "sich nicht verändert",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "kann man nicht vorhersagen",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();

            j = 0; question = new Question
             {
                 Category = catMathe,
                 Reihenfolge = ++i,
                 Title = "Wirtschaft",
                 Text = "Die Nachfrage an technischen Sägemaschinen steigt weltweit. Anzunehmen ist eine vereinfachte Form der Marktwirtschaft. Der Preis der Sägemaschine...",
                 Answers = new List<QuestionAnswer>
                {
                    new QuestionAnswer
                    {
                        Text = "sinkt aprubt ab",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "ändert sich nicht",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "steigt an",
                        IsCorrect = true, Reihenfolge = ++j
                    },
                    new QuestionAnswer
                    {
                        Text = "sinkt solange ab bis ein Minimalwert erreicht ist",
                        IsCorrect = false, Reihenfolge = ++j
                    },
                }
             };

            context.Questions.Add(question);
            context.SaveChanges();
        }




        private void InitCategories(QuestDbContext context)
        {
            var i = 0;
            var categories = new List<QuestionCategory>
            {
                new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Mathematik",
                    ShortName = "Mathematik"
                },

                new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Technik",
                    ShortName = "Technik"
                },
                
                new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Naturwissenschaften",
                    ShortName = "Naturwissenschaften"
                },
          
                new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Wirtschaft",
                    ShortName = "Wirtschaft"
                },
               /*
                    new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Allgemeiner Teil",
                    ShortName = "Allgemeiner Teil"
                },
                new QuestionCategory
                {
                    Reihenfolge = ++i,
                    Name = "Sprachen",
                    ShortName = "Sprachen"
                },
                 */
            };

            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

        }

        private QuestionCategory GetCategory(QuestDbContext context, string name)
        {
            return context.Categories.SingleOrDefault(c => c.Name.Equals(name));
        }

        private BinaryStorage GetImage(QuestDbContext context, string name)
        {
            var imageFile = Path.Combine(_baseDir, name);

            var image = new BinaryStorage
            {
                ImageFileType = "image/" + Path.GetExtension(imageFile).Remove(0, 1),
                ImageData = File.ReadAllBytes(imageFile)
            };

            context.BinaryStorages.Add(image);
            context.SaveChanges();

            return image;
        }

    }
}