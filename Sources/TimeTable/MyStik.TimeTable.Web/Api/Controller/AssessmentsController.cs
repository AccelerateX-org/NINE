using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [System.Web.Http.RoutePrefix("api/v2/assessments")]

    public class AssessmentsController : ApiBaseController
    {
        [System.Web.Http.Route("")]
        public IQueryable<AssessmentDto> GetIndex()
        {
            var rooms = Db.Assessments.ToList();

            var result = new List<AssessmentDto>();

            foreach (var room in rooms)
            {
                var r = new AssessmentDto();

                r.Id = room.Id;
                r.Name = room.Name;

                result.Add(r);
            }

            return result.AsQueryable();
        }

        [System.Web.Http.Route("{id}/candidatures")]
        public IQueryable<CandidatureDto> GetCandidatures(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var userService = new UserInfoService();


            var result = new List<CandidatureDto>();

            foreach (var candidature in assessment.Candidatures)
            {
                var user = userService.GetUser(candidature.UserId);

                var c = new CandidatureDto
                {
                    Id = candidature.Id,
                    FirstName = user != null ? user.FirstName : "",
                    LastName = user != null ? user.LastName : "Kein Benutzerkonto",
                    Motivation = candidature.Motivation,
                    Characteristics = candidature.Characteristics,
                    ProfileImgId = user != null && user.BinaryData != null ? candidature.Id : Guid.Empty,
                    Stages = new List<StageDto>()
                };


                // die stages
                foreach (var stage in assessment.Stages.OrderBy(x => x.OpeningDateTime))
                {
                    // es kann auch sein, dass jemand mehrere Stufen angelegt hat
                    var candStages = candidature.Stages.Where(x => x.AssessmentStage.Id == stage.Id).ToList();

                    foreach (var candStage in candStages)
                    {
                        var cs = new StageDto
                        {
                            Name = stage.Name,
                            Material = new List<MaterialDto>()
                        };


                        foreach (var material in candStage.Material)
                        {
                            var cm = new MaterialDto();

                            cm.Id = material.Id;

                            cs.Material.Add(cm);
                        }

                        c.Stages.Add(cs);
                    }
                }

                result.Add(c);

            }

            return result.AsQueryable();
        }


        [System.Web.Http.Route("candidatures/{id}/ProfileImage")]
        public HttpResponseMessage GetProfileImage(Guid id)
        {
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);
            var user = GetUser(candidature.UserId);
            var response = Request.CreateResponse(HttpStatusCode.OK);

            if (user?.BinaryData == null) return response;

            var stream = new MemoryStream(user.BinaryData) {Position = 0};

            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(user.FileType);

            return response;
        }

        [System.Web.Http.Route("material/{id}")]
        public HttpResponseMessage GetMaterial(Guid id)
        {
            var material = Db.CandidatureStageMaterial.SingleOrDefault(x => x.Id == id);
            var response = Request.CreateResponse(HttpStatusCode.OK);

            if (material.Storage != null && material.Storage.BinaryData != null)
            {
                var stream = new MemoryStream(material.Storage.BinaryData) {Position = 0};

                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(material.Storage.FileType);
            }

            return response;
        }

        [System.Web.Http.Route("material/{id}/description")]
        public MaterialDto GetMaterialDesc(Guid id)
        {
            var material = Db.CandidatureStageMaterial.SingleOrDefault(x => x.Id == id);

            var cm = new MaterialDto();

            if (material.Storage != null)
            {
                cm.Description = material.Storage.Description;
                cm.Title = material.Storage.Name;
                cm.Created = material.Storage.Created?.ToString("s");
            }
            else
            {
                cm.Title = "Kein Bild vorhanden";
            }

            return cm;
        }

    }

    public class AssessmentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }


    public class CandidatureDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Characteristics { get; set; }

        public string Motivation { get; set; }

        public Guid ProfileImgId { get; set; }

        public List<StageDto> Stages { get; set; }
    }

    public class StageDto
    {
        public string Name { get; set; }

        public List<MaterialDto> Material { get; set; }
    }

    public class MaterialDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Created { get; set; }
    }

}
