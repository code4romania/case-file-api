using System.Collections.Generic;

namespace CaseFile.Api.Core.Services
{
    public interface IFirebaseService
    {
        int SendAsync(string from, string title, string message, List<string> recipients);
    }
}
