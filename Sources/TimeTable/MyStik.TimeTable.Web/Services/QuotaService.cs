using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using MyStik.TimeTable.Data;
using PdfSharp;

namespace MyStik.TimeTable.Web.Services
{
    public class QuotaCheckResponse
    {
        public bool Success { get; set; }
        public string Remark { get; set; }
    }


    public class QuotaService
    {
        public QuotaCheckResponse IsAvailable(SeatQuota quota, Curriculum curr, ICollection<ItemLabel> labels)
        {
            var result = new QuotaCheckResponse() { Success = false };

            if (quota.Fractions.Any())
            {

            }
            else
            {
                var checkLabelSet = CheckLabelSet(quota.ItemLabelSet, labels);
                if (quota.Curriculum != null)
                {
                    if (curr != null)
                    {
                        var checkCurr = quota.Curriculum.Id == curr.Id;
                        if (checkCurr)
                        {
                            return checkLabelSet;
                        }
                        else
                        {
                            checkLabelSet.Success = false;
                            checkLabelSet.Remark = $"Nicht für Studiengang {quota.Curriculum.ShortName} zugänglich";
                            return checkLabelSet;
                        }
                    }
                    else
                    {
                        checkLabelSet.Success = false;
                        checkLabelSet.Remark = $"Angabe des Studiengangs fehlt oder Studiengang existiert nicht";
                        return checkLabelSet;
                    }
                }
                else
                {
                    // Kein Studiengang, dann nur die Labels
                    return checkLabelSet;
                }
            }

            result.Remark = "Fehlerhaftes Kontingent";
            return result;
        }

        private QuotaCheckResponse CheckLabelSet(ItemLabelSet labelSet, ICollection<ItemLabel> labels)
        {
            var resp = new QuotaCheckResponse() { Success = true };

            if (labelSet != null && labelSet.ItemLabels.Any())
            {
                if (labels == null || !labels.Any())
                {
                    // Quota fordert Labels, Student hat kein Label => kein Zugang
                    resp.Remark = "Erforderliche Kohorten fehlen";
                    return resp;
                }

                // Es muss jedes geforderte Label dabei sein
                foreach (var quotaLabel in labelSet.ItemLabels)
                {
                    var exist = labels.SingleOrDefault(x => x.Id == quotaLabel.Id);
                    // sobald ein Label nicht dabei ist, sofort aufhören
                    if (exist == null)
                    {
                        resp.Remark = $"Erforderliche Kohorte {quotaLabel.Name} fehlt";
                        return resp;
                    }
                }
            }

            
            // keine Labels, kein Studiengang => offen für alle
            resp.Remark = "Offen für alle Kohorten";
            return resp;
        }
    }
}