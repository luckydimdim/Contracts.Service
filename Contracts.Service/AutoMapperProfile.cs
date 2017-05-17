using AutoMapper;
using Cmas.Services.Contracts.Dtos.Requests;
using Cmas.BusinessLayers.Contracts.Entities;

namespace Cmas.Services.Contracts
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateContractRequest, Contract>();
            CreateMap<AmountRequest, Amount>();
        }
    }
}
