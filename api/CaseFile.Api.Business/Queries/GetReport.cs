using CaseFile.Api.Business.Models;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Queries
{
    public class GetReport : IRequest<Result<ReportModel>>
    {
        public GetReport(int reportId)
        {
            ReportId = reportId;
        }

        public int ReportId { get; }
    }
}
