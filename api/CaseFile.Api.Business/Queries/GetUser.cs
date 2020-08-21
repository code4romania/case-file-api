using MediatR;
using CaseFile.Api.Business.Models;
using CSharpFunctionalExtensions;

namespace CaseFile.Api.Business.Queries
{
    public class GetUser : IRequest<Result<UserModel>>
    {
        public GetUser(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }
    }
}
