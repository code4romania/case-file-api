using AutoMapper;
using CaseFile.Api.Business.Commands;
using CaseFile.Api.Business.Models;
using CaseFile.Api.Business.Queries;
using CaseFile.Api.Core;
using CaseFile.Entities;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaseFile.Api.Business.Handlers
{
    public class StatisticsRequestsHandler :
        IRequestHandler<NewReportCommand, int>,
        IRequestHandler<GetReport, Result<ReportModel>>,
        IRequestHandler<ReportListCommand, ApiListResponse<ReportInfoModel>>,
        IRequestHandler<GetGeneralStatistics, Result<StatisticsModel>>
    {
        private readonly CaseFileContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public StatisticsRequestsHandler(CaseFileContext context, ILogger<UserRequestsHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(NewReportCommand message, CancellationToken token)
        {
            try
            {
                var report = _mapper.Map<Report>(message);
                report.UserId = message.CurrentUserId;
                report.Title = message.ReportType == ReportType.Monthly ? "Fișă de monitorizare lunară - " + message.Period 
                                                    : "Fișă de monitorizare trimestrială - " + message.Period;
                
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                return report.ReportId;
            }
            catch (Exception ex)
            {
                _logger.LogError("NewReportCommand: ", ex);
                return -1;
            }

        }

        public async Task<Result<ReportModel>> Handle(GetReport request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(r => r.ReportId == request.ReportId, cancellationToken);
                if (report == null)
                {
                    return Result.Failure<ReportModel>($"Could not find user with id = {request.ReportId}");
                }

                var reportModel = _mapper.Map<ReportModel>(report);
                
                return Result.Ok(reportModel);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Unable to load report {request.ReportId}", e);
                return Result.Failure<ReportModel>($"Unable to load report {request.ReportId}");
            }
        }

        public async Task<ApiListResponse<ReportInfoModel>> Handle(ReportListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Reports with the following filters (Title): {request.Title}");

            var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);

            IQueryable<Entities.Report> reports = _context.Reports
                .Include(r => r.User)
                .Where(r => r.User.NgoId == user.NgoId);

            
            if (!string.IsNullOrEmpty(request.Title))
            {
                reports = reports.Where(r => r.Title.Contains(request.Title));
            }

            var count = await reports.CountAsync(cancellationToken);

            var requestedPageReports = GetPagedQuery(reports.OrderByDescending(r => r.CreationDate), request.Page, request.PageSize)
                .ToList()
                .Select(_mapper.Map<ReportInfoModel>);


            return new ApiListResponse<ReportInfoModel>
            {
                TotalItems = count,
                Data = requestedPageReports.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<Result<StatisticsModel>> Handle(GetGeneralStatistics request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                var currentYear = DateTime.Now.Year;
                var firstdayCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var statisticsModel = new StatisticsModel();
                statisticsModel.ActiveCasesCurrentMonth = _context.Beneficiaries.Count(b => b.User.NgoId == user.NgoId);
                statisticsModel.ActiveCasesChildren = _context.Beneficiaries.Count(b => b.User.NgoId == user.NgoId && currentYear - b.BirthDate.Year <= 18);
                statisticsModel.ActiveCasesPreviousMonth = _context.Beneficiaries.Count(b => b.User.NgoId == user.NgoId && b.RegistrationDate < firstdayCurrentMonth);

                return Result.Ok(statisticsModel);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Unable to load general statistics for user {request.UserId}", e);
                return Result.Failure<StatisticsModel>($"Unable to load general statistics for user {request.UserId}");
            }
        }

        private static IQueryable<Entities.Report> GetPagedQuery(IQueryable<Entities.Report> reports, int page, int pageSize)
        {
            if (pageSize > 0)
            {
                return reports
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return reports;
        }

    }
}
