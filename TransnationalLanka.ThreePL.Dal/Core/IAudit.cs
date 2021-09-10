using System;

namespace TransnationalLanka.ThreePL.Dal.Core
{
    public interface IAudit
    {
        string AuditAction { get; set; }
        DateTime AuditDate { get; set; }
        string UserName { get; set; }
        string TransactionId { get; set; }
    }
}
