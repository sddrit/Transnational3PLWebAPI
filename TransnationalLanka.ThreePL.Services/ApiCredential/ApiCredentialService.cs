using System;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Core.Constants;
using TransnationalLanka.ThreePL.Core.Exceptions;
using TransnationalLanka.ThreePL.Dal;
using TransnationalLanka.ThreePL.Integration.Tracker;
using TransnationalLanka.ThreePL.Integration.Tracker.Model;
using TransnationalLanka.ThreePL.Services.ApiCredential;
using TransnationalLanka.ThreePL.Services.Common.Mapper;


namespace TransnationalLanka.ThreePL.Services.ApiCredential
{
    public class ApiCredentialService : IApiCredentialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TrackerApiService _trackerApiService;

        public ApiCredentialService(IUnitOfWork unitOfWork, TrackerApiService trackerApiService)
        {
            _unitOfWork = unitOfWork;
            _trackerApiService = trackerApiService;
        }

        public Task<Dal.Entities.ApiCredential> AddApiCredentail(Dal.Entities.ApiCredential apiCredential)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Dal.Entities.ApiCredential> Get()
        {
            throw new NotImplementedException();
        }
    }
}
