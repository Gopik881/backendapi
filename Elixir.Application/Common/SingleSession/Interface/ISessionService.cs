using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Common.SingleSession.Interface
{
    public interface ISessionService
    {
        Task CreateSession(int userId, string token, Guid sessionId);
        Task InvalidateSessions(int userId);
        Task<bool> IsSessionActive(Guid sessionId);
        Task<bool> HadPreviousSession(int userId);

        Task<object?> GetUserDetailsBySessionIdAsync(Guid sessionId);
    }

}
