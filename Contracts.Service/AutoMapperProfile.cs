using AutoMapper;
using Cmas.Services.Contracts.Dtos.Requests;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Services.Contracts.Dtos.Responses;

namespace Cmas.Services.Contracts
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateContractRequest, Contract>();
            CreateMap<CreateContractRequest, Contract>();
            CreateMap<AmountRequest, Amount>();
            CreateMap<Contract, SimpleContractResponse>();
            CreateMap<Contract, DetailedContractResponse>();
            CreateMap<Amount, AmountResponse>();
        }
    }
}
