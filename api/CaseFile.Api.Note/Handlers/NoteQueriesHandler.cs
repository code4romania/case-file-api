using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Note.Commands;
using CaseFile.Api.Note.Models;
using CaseFile.Api.Note.Queries;
using CaseFile.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseFile.Api.Note.Handlers
{
    public class NoteQueriesHandler :
        IRequestHandler<NoteQuery, List<NoteModel>>,
        IRequestHandler<AddNoteCommand, int>
    {

        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;

		public NoteQueriesHandler(CaseFileContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<List<NoteModel>> Handle(NoteQuery message, CancellationToken token)
		{
            if (message.FormId > 0)
			    return await _context.Notes
				    .Where(n => n.UserId == message.UserId && n.BeneficiaryId == message.BeneficiaryId && n.Question.FormSection.Form.FormId == message.FormId)
                    .OrderBy(n => n.LastModified)
				    .Select(n => new NoteModel
				    {
					    AttachmentPath = n.AttachementPath,
					    Text = n.Text,                        
                        QuestionId = n.QuestionId,
                        LastModified = n.LastModified
                    })
				    .ToListAsync(cancellationToken: token);
            else
                return await _context.Notes
                    .Where(n => n.UserId == message.UserId && n.BeneficiaryId == message.BeneficiaryId && n.QuestionId == null)
                    .OrderBy(n => n.LastModified)
                    .Select(n => new NoteModel
                    {
                        AttachmentPath = n.AttachementPath,
                        Text = n.Text,
                        LastModified = n.LastModified
                    })
                    .ToListAsync(cancellationToken: token);
        }

        public async Task<int> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var beneficiary = _context.Beneficiaries.FirstOrDefault(b => b.BeneficiaryId == request.BeneficiaryId);
                if (beneficiary == null)
                    return -1;

                var noteEntity = _mapper.Map<Entities.Note>(request);

                _context.Notes.Add(noteEntity);

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
