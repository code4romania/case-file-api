using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Form.Models;
using CaseFile.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace CaseFile.Api.Form.Queries
{
    public class AddFormQueryHandler :
        IRequestHandler<AddFormQuery, FormDTO>
    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddFormQueryHandler(CaseFileContext context, IMapper mapper, ILogger<AddFormQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FormDTO> Handle(AddFormQuery message, CancellationToken cancellationToken)
        {
            try
            {
                var newForm = new Entities.Form
                {
                    Code = string.Empty,
                    CurrentVersion = 1,
                    Description = message.Form.Description,
                    FormSections = new List<FormSection>(),
                    Draft = message.Form.Draft,
                    Order = 0,
                    Type = message.Form.Type,
                    CreatedByUserId = message.UserId,
                    Date = DateTime.UtcNow
                };

                foreach (var fs in message.Form.FormSections)
                {
                    var formSection = new FormSection
                    {
                        Code = fs.Title,
                        Description = fs.Description,
                        Questions = new List<Question>()
                    };
                    foreach (var q in fs.Questions)
                    {
                        var question = new Question { QuestionType = q.QuestionType, Hint = q.Hint, Text = q.Text };
                        var optionsForQuestion = new List<OptionToQuestion>();
                        foreach (var o in q.OptionsToQuestions)
                        {
                            if (o.IdOption > 0)
                            {
                                var existingOption = _context.Options.FirstOrDefault(option => option.OptionId == o.IdOption);
                                optionsForQuestion.Add(new OptionToQuestion { Option = existingOption });
                            }
                            else
                            {
                                optionsForQuestion.Add(_mapper.Map<OptionToQuestion>(o));
                            }
                        }

                        question.OptionsToQuestions = optionsForQuestion;
                        formSection.Questions.Add(question);
                    }
                    newForm.FormSections.Add(formSection);
                }

                _context.Forms.Add(newForm);

                await _context.SaveChangesAsync();
                message.Form.Id = newForm.FormId;
                return message.Form;
            } 
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}