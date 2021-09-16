using System.Linq;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Application
{
    public interface ILogService
    {
        IQueryable<Event> GetEvents();
        IQueryable<Dal.Entities.Audit> GetAuditLogs();
    }
}