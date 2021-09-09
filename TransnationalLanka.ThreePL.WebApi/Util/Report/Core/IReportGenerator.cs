using System.Collections.Specialized;
using DevExpress.XtraReports.UI;

namespace TransnationalLanka.ThreePL.WebApi.Util.Report.Core
{
    public interface IReportGenerator
    {
        public bool IsMatch(string key);
        public XtraReport GenerateReport(NameValueCollection parameters);
    }
}
