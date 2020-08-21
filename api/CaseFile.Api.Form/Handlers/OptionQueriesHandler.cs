using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaseFile.Api.Form.Models;
using CaseFile.Api.Form.Queries;
using CaseFile.Entities;

namespace CaseFile.Api.Form.Handlers
{
    public class OptionQueriesHandler : IRequestHandler<FetchAllOptionsCommand, List<OptionDto>>,
                                        IRequestHandler<GetOptionByIdCommand, OptionDto>,
                                        IRequestHandler<AddOptionCommand, OptionDto>,
                                        IRequestHandler<UpdateOptionCommand, int>

    {
        private readonly CaseFileContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public OptionQueriesHandler(CaseFileContext context, IMapper mapper, ILogger<OptionQueriesHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<OptionDto>> Handle(FetchAllOptionsCommand request, CancellationToken cancellationToken)
        {
            return await _context.Options.Select(x => _mapper.Map<OptionDto>(x)).ToListAsync(cancellationToken);
        }

        public Task<OptionDto> Handle(GetOptionByIdCommand request, CancellationToken cancellationToken)
        {
            var option = _context.Options.FirstOrDefault(c => c.OptionId == request.OptionId);

            var optionDto = _mapper.Map<OptionDto>(option);

            return Task.FromResult(optionDto);
        }

        public async Task<OptionDto> Handle(AddOptionCommand request, CancellationToken cancellationToken)
        {
            var optionEntity = _mapper.Map<Option>(request.Option);

            _context.Options.Add(optionEntity);
            await _context.SaveChangesAsync(cancellationToken);


            return _mapper.Map<OptionDto>(optionEntity);
        }

        public async Task<int> Handle(UpdateOptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var option = await _context.Options
                    .FirstOrDefaultAsync(a => a.OptionId == request.Option.Id, cancellationToken);

                if (option == null)
                {
                    throw new ArgumentException($"Can't find this option by id = {request.Option.Id}");
                }

                _mapper.Map(request.Option, option);
                _context.Update(option);

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}