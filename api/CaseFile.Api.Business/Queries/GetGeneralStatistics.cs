using CaseFile.Api.Business.Models;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaseFile.Api.Business.Queries
{
    public class GetGeneralStatistics : IRequest<Result<StatisticsModel>>
    {
        public GetGeneralStatistics(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }
    }
}
