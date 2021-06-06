using System.Linq;
using System.Threading.Tasks;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Services.Grn
{
    public interface IGrnService
    {
        Task<GoodReceivedNote> AddGoodReceivedNote(GoodReceivedNote goodReceivedNote);
        IQueryable<GoodReceivedNote> GetAll();
        Task<GoodReceivedNote> GetById(long id);
    }
}