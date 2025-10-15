using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Booking
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
                var isAvailable = false;
                var remark = string.Empty;
                foreach (var fraction in quota.Fractions)
                {
                    var checkLabelSet = CheckLabelSet(fraction.ItemLabelSet, labels);
                    if (fraction.Curriculum != null)
                    {
                        if (curr != null)
                        {
                            var checkCurr = fraction.Curriculum.Id == curr.Id;
                            if (checkCurr)
                            {
                                if (checkLabelSet.Success)
                                {
                                    isAvailable = true;
                                }
                            }
                            else
                            {
                                remark = $"Kein Platzkontingent für Studienangebot {curr.ShortName} vorhanden";
                            }
                        }
                        else
                        {
                            remark = $"Angabe des Studienangebots fehlt oder Studienangebot existiert nicht";
                        }
                    }
                    else
                    {
                        if (checkLabelSet.Success)
                        {
                            isAvailable = true;
                        }
                        remark = checkLabelSet.Remark;
                    }
                }

                if (isAvailable)
                {
                    result.Success = true;
                    result.Remark = string.Empty;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Remark = remark;
                    return result;
                }
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
                            checkLabelSet.Remark = $"Kein Platzkontingent für Studienangebot {curr.ShortName} vorhanden";
                            return checkLabelSet;
                        }
                    }
                    else
                    {
                        checkLabelSet.Success = false;
                        checkLabelSet.Remark = $"Angabe des Studienangebots fehlt oder Studienangebot existiert nicht";
                        return checkLabelSet;
                    }
                }
                else
                {
                    // Kein Studiengang, dann nur die Labels
                    return checkLabelSet;
                }
            }
        }

        private QuotaCheckResponse CheckLabelSet(ItemLabelSet labelSet, ICollection<ItemLabel> labels)
        {
            var resp = new QuotaCheckResponse() { Success = true };

            if (labelSet != null && labelSet.ItemLabels.Any())
            {
                if (labels == null || !labels.Any())
                {
                    // Quota fordert Labels, Student hat kein Label => kein Zugang
                    resp.Success = false;
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
                        resp.Success = false;
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