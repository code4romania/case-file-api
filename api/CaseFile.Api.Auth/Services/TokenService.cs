using CaseFile.Entities;
using System;
using System.Linq;

namespace CaseFile.Api.Auth.Services
{
    public interface ITokenService
    {
        string GetTemporaryToken(int userId);
    }

    public class TokenService : ITokenService
    {
        private readonly CaseFileContext _context;
        public TokenService(CaseFileContext context)
        {
            _context = context;
        }

        public string GetTemporaryToken(int userId)
        {
            try
            {
                var temporaryToken = _context.Users.FirstOrDefault(u => u.UserId == userId).TemporaryToken;

                return temporaryToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
