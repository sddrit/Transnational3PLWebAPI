using System.Threading.Tasks;

namespace TransnationalLanka.ThreePL.Services.Setting
{
    public interface ISettingService
    {
        Task<string> GetValue(string name);
        Task CreateOrUpdateValue(string name, string value);
    }
}