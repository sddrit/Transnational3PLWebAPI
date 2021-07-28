using FluentValidation;
using TransnationalLanka.ThreePL.Services.Common.Validation;

namespace TransnationalLanka.ThreePL.Services.ApiCredential.Core
{
    public class ApiCredential : AbstractValidator<Dal.Entities.ApiCredential>
    {
        public ApiCredential()
        {
            RuleFor(p => p.Name).NotEmpty();


        }
    }


}
