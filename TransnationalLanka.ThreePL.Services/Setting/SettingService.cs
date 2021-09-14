using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;

namespace TransnationalLanka.ThreePL.Services.Setting
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetValue(string name)
        {
            var setting = await _unitOfWork.SettingRepository.GetAll()
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name);

            if (setting == null)
            {
                throw new ServiceException(new ErrorMessage[]
                {
                    new ErrorMessage()
                    {
                        Message = $"{name} setting is not found"
                    }
                });
            }

            return setting.Value;
        }

        public async Task CreateOrUpdateValue(string name, string value)
        {
            var setting = await _unitOfWork.SettingRepository.GetAll()
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());

            if (setting == null)
            {
                setting = new Dal.Entities.Setting()
                {
                    Name = name,
                    Value = value
                };
                _unitOfWork.SettingRepository.Insert(setting);
                await _unitOfWork.SaveChanges();
                return;
            }

            setting.Value = value;
            await _unitOfWork.SaveChanges();
        }
    }
}
