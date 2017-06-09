using System;
using Cmas.Services.Contracts.Dtos.Requests;
using FluentValidation;

namespace Cmas.Services.Contracts.Validation
{
    public class UpdateContractRequestValidator : AbstractValidator<UpdateContractRequest>
    {
        public UpdateContractRequestValidator()
        {
            RuleFor(request => request.Number).NotEmpty();
            RuleFor(request => request.ConstructionObjectName).NotEmpty();
            RuleFor(request => request.ConstructionObjectTitleCode).NotEmpty();
            RuleFor(request => request.ConstructionObjectTitleName).NotEmpty();
            RuleFor(request => request.ContractorName).NotEmpty();
            RuleFor(request => request.Name).NotEmpty();
            RuleFor(request => request.StartDate).Must(d => d.Kind == DateTimeKind.Utc);
            RuleFor(request => request.FinishDate).Must(d => d.Kind == DateTimeKind.Utc);
            RuleFor(request => request).Must(r => r.FinishDate > r.StartDate);
        }
    }
}