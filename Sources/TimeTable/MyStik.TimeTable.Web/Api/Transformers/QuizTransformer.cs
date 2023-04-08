using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.Transformers
{
    public class QuizTransformer
    {
        internal QuizDto GetEmptyQuiz()
        {
            var dto = new QuizDto();
            return dto;
        }

        internal QuizDto GetStructure(Quiz quiz, bool withContent = false)
        {
            var dto = new QuizDto();

            dto.id = quiz.Id;
            dto.title = quiz.Title;
            dto.description = quiz.Description;
            dto.sections = new List<QuizSectionDto>();

            foreach (var section in quiz.Sections)
            {
                var dto_section = new QuizSectionDto();

                dto_section.title = section.Title;
                dto_section.description = section.Description;
                dto_section.position = section.Position;
                dto_section.questions = new List<QuizQuestionDto>();

                dto.sections.Add(dto_section);

                foreach (var question in section.Questions)
                {
                    var dto_question = new QuizQuestionDto();

                    dto_question.position = question.Position;
                    dto_question.answers = new List<QuizQuestionAnswerDto>();

                    dto_section.questions.Add(dto_question);

                    if (withContent)
                    {
                        dto_question.title = question.Question.Problem;

                        foreach (var answer in question.Question.Answers)
                        {
                            var dto_answer = new QuizQuestionAnswerDto();

                            dto_answer.text = answer.Solution;
                            dto_answer.isCorrect = answer.IsCorrect;

                            dto_question.answers.Add(dto_answer);
                        }
                    }
                }
            }

            return dto;
        }
    }
}