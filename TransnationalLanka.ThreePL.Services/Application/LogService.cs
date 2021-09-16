using System.Linq;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Application
{
    public class LogService : ILogService
    {
        private readonly ThreePlDbContext _context;

        public LogService(ThreePlDbContext context)
        {
            _context = context;
        }

        public IQueryable<Event> GetEvents()
        {
            return _context.Events;
        }

        public IQueryable<Dal.Entities.Audit> GetAuditLogs()
        {
            return _context.AuditLogs;
        }
    }
}
