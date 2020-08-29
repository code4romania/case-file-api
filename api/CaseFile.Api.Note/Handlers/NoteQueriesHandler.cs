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
using Microsoft.Extensions.Logging;

namespace CaseFile.Api.Note.Handlers
{
    public class NoteQueriesHandler :
        IRequestHandler<NoteQuery, List<NoteModel>>,
        IRequestHandler<AddNoteCommand, int>
    {

        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public NoteQueriesHandler(CaseFileContext context, IMapper mapper, ILogger<NoteQueriesHandler> logger)
		{
			_context = context;
			_mapper = mapper;
            _logger = logger;
        }
		public async Task<List<NoteModel>> Handle(NoteQuery message, CancellationToken token)
		{
            // check if beneficiary exists and that the current user is allowed to view the beneficiary notes 
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == message.UserId);

            var beneficiary = _context.Beneficiaries.Include(b => b.User).FirstOrDefault(b => b.BeneficiaryId == message.BeneficiaryId);
            if (beneficiary == null || (currentUser.NgoId != beneficiary.User.NgoId))
                throw new UnauthorizedAccessException();

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
                // check if beneficiary exists and that the current user is allowed to add notes for the beneficiary
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);

                var beneficiary = _context.Beneficiaries.Include(b => b.User).FirstOrDefault(b => b.BeneficiaryId == request.BeneficiaryId);
                if (beneficiary == null || (currentUser.NgoId != beneficiary.User.NgoId))
                    return -1;

                var noteEntity = _mapper.Map<Entities.Note>(request);

                _context.Notes.Add(noteEntity);

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Add note error", ex);
                throw ex;
            }
        }
    }
}
