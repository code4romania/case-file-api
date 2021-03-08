using AutoMapper;
using CaseFile.Api.Auth.Models;
using CaseFile.Api.Core.Services;
using CaseFile.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CaseFile.Api.Auth.Services
{    
    public interface IAccountService
    {            
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        bool SaveTemporaryToken(int userId, string token);
        User GetUser(int userId);
    }

    public class AccountService : IAccountService
    {
        private readonly CaseFileContext _context;
        private readonly IEmailService _emailService;
        private IHashService _hashService;

        public AccountService(CaseFileContext context, IEmailService emailService, IHashService hashService)
        {
            _context = context;
            _emailService = emailService;
            _hashService = hashService;
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (user == null) return;

            // create reset token that expires after 1 day
            user.ResetToken = RandomTokenString();
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(24);

            _context.Users.Update(user);
            _context.SaveChanges();

            // send email
            SendPasswordResetEmail(user, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Users.FirstOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new Exception("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var user = _context.Users.FirstOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
                throw new Exception("Invalid token");

            // update password and remove reset token
            user.Password = _hashService.GetHash(model.Password);
            user.PasswordReset = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public bool SaveTemporaryToken(int userId, string token)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

                user.TemporaryToken = token;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public User GetUser(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendPasswordResetEmail(User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/reset-password/{user.ResetToken}";
                message = $@"<p>Vă rugăm să folosiți link-ul de mai jos pentru a vă seta o parolă nouă. Link-ul va fi valid pentru 1 zi:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/reset-password</code> api route:</p>
                             <p><code>{user.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: user.Email,
                subject: "Resetare Parolă",
                body: $@"<h4>Resetare Parolă</h4>
                         {message}"
            );
        }
    }    
}
