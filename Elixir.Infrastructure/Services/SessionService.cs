using Elixir.Application.Common.SingleSession.Interface;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        private readonly ElixirHRDbContext _context;

        public SessionService(ElixirHRDbContext context)
        {
            _context = context;
        }

        public async Task CreateSession(int userId, string token, Guid sessionId)
        {
            var session = new ActiveSession
            {
                Id = sessionId,
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // adjust as needed
                IsActive = true
            };

            _context.ActiveSession.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateSessions(int userId)
        {
            var sessions = _context.ActiveSession
                .Where(s => s.UserId == userId && s.IsActive);

            foreach (var session in sessions)
            {
                session.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSessionActive(Guid sessionId)
        {
            return await _context.ActiveSession
                .AnyAsync(s => s.Id == sessionId && s.IsActive && s.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<bool> HadPreviousSession(int userId)
        {
            return await _context.ActiveSession
                .AnyAsync(s => s.UserId == userId && !s.IsActive);
        }
        public async Task<object?> GetUserDetailsBySessionIdAsync(Guid sessionId)
        {
            // Step 1: Fetch the session and get the userId
            var session = await _context.ActiveSession
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return null;

            int userId = session.UserId;

            // Step 2: Try to fetch user from Users table
            var user = await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
                return user;

            // Step 3: If not found, try to fetch from SuperUser table
            var superUser = await _context.Set<SuperUser>()
                .AsNoTracking()
                .FirstOrDefaultAsync(su => su.Id == userId);

            return superUser;
        }

    }

}
