using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class MaterialExportHub : Hub
    {
        /// <summary>
        /// Löscht alle Kurse aus dem angegebenen Semester, die aus gpUntis importiert wurden
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        public void DeleteMaterial(Guid assessmentId)
        {
            var db = new TimeTableDbContext();
            var userService = new UserInfoService();


            var msg = "Sammle Daten";
            var perc1 = 0;

            Clients.Caller.updateProgresCandidature(msg, perc1);

            var assessment = db.Assessments.SingleOrDefault(x => x.Id == assessmentId);


            try
            {
                msg = $"Prüfe {assessment.Name} mit {assessment.Candidatures.Count} Kandidaturen";
                perc1 = 0;
                Clients.Caller.updateProgresCandidature(msg, perc1);

                var n = assessment.Candidatures.Count;
                var i = 0;
                foreach (var candidature in assessment.Candidatures)
                {
                    perc1 = (i * 100) / n;

                    var user = userService.GetUser(candidature.UserId);


                    if (user != null)
                    {
                        msg = $"Lösche Material von {user.FullName}";
                        Clients.Caller.updateProgressCandidature(msg, perc1);


                        // Profilbild

                        if (user.BinaryData != null)
                        {
                            user.BinaryData = null;
                        }
                    }


                    var j = 0;
                    foreach (var stage in candidature.Stages.ToList())
                    {
                        var perc2 = (j*100) / assessment.Stages.Count;


                        var perc3 = 0;
                        var k = 0;


                        msg = $"Lösche Material für Stufe {stage.AssessmentStage.Name} mit {stage.Material.Count} Einträgen";
                        Clients.Caller.updateProgressStage(msg, perc2);


                        foreach (var material in stage.Material)
                        {
                            if (material.Storage != null)
                            {
                                db.Storages.Remove(material.Storage);
                                material.Storage = null;

                                db.SaveChanges();
                            }
                            k++;
                        }

                        j++;
                    }


                    i++;
                }

                msg = "Alle Materiaklien gelöscht";
                perc1 = 100;
                Clients.Caller.updateProgressCandidature(msg, perc1);
            }
            catch (Exception e)
            {
                msg = $"FEHLER: {e.Message}";
                Clients.Caller.updateError(msg);
            }
        }

   }
}