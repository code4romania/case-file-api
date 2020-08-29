using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Form.Models;
using CaseFile.Entities;
using CaseFile.Api.Core;

namespace CaseFile.Api.Form.Queries
{

    public class FormVersionQueryHandler : IRequestHandler<FormVersionQuery, List<FormDetailsModel>>,
        IRequestHandler<FormListCommand, ApiListResponse<FormDetailsModel>>
    {
        private readonly CaseFileContext _context;

        public FormVersionQueryHandler(CaseFileContext context)
        {
            _context = context;
        }

		public async Task<List<FormDetailsModel>> Handle(FormVersionQuery request, CancellationToken cancellationToken)
		{
            // todo: check that the request.UserId is the logged in user 

            List<Entities.Form> result;

            if (request.UserId == null)
            {
                result = await _context.Forms
                    .AsNoTracking()                    
                    .Where(x => x.Type == FormType.Public)
                    .Where(x => x.Draft == false)
                    .ToListAsync();
            }
            else
            {
                var ngoId = _context.Users.FirstOrDefault(u => u.UserId == request.UserId).NgoId;
                result = await _context.Forms.Include(f => f.CreatedByUser)
                    .AsNoTracking()
                    .Where(x => x.Type == FormType.Public || (x.Type == FormType.Private && x.CreatedByUser.NgoId == ngoId))
                    .Where(x => x.Draft == false)
                    .ToListAsync();
            }

			var sortedForms = result
					.OrderBy(x=>x.Order)
                    .Select(x=>new FormDetailsModel() { 
                       Id = x.FormId,
                       Description = x.Description,
                       Code = x.Code,
                       CurrentVersion = x.CurrentVersion,
                       UserName = x.CreatedByUser.Name,
                       Date = x.Date.ToString("dd.MM.yyyy")
                    })
                    .ToList();

			return sortedForms;
		}

        public async Task<ApiListResponse<FormDetailsModel>> Handle(FormListCommand request, CancellationToken cancellationToken)
        {            
            var ngoId = _context.Users.FirstOrDefault(u => u.UserId == request.UserId).NgoId;
            IQueryable<Entities.Form> result = _context.Forms.Include(f => f.CreatedByUser)
                .AsNoTracking()
                .Where(x => x.Type == FormType.Public || (x.Type == FormType.Private && x.CreatedByUser.NgoId == ngoId))
                .Where(x => x.Draft == false);

            if (!string.IsNullOrEmpty(request.Description))
            {
                result = result.Where(f => f.Description.Contains(request.Description));
            }

            var count = await result.CountAsync(cancellationToken);

            var requestedPageForms = GetPagedQuery(result.OrderBy(r => r.Description), request.Page, request.PageSize)
                .ToList()
                .Select(x => new FormDetailsModel()
                {
                    Id = x.FormId,
                    Description = x.Description,
                    Code = x.Code,
                    CurrentVersion = x.CurrentVersion,
                    UserName = x.CreatedByUser.Name,
                    Date = x.Date.ToString("dd.MM.yyyy")
                });


            return new ApiListResponse<FormDetailsModel>
            {
                TotalItems = count,
                Data = requestedPageForms.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        private static IQueryable<Entities.Form> GetPagedQuery(IQueryable<Entities.Form> forms, int page, int pageSize)
        {
            if (pageSize > 0)
            {
                return forms
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return forms;
        }
    }
}
