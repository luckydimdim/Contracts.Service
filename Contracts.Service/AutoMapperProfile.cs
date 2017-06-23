using AutoMapper;
using Cmas.Services.Contracts.Dtos.Requests;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Services.Contracts.Dtos.Responses;
using System;

namespace Cmas.Services.Contracts
{
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Округление до двух знаков после запятой
        /// Округление происходит к тому числу, которое дальше от нуля
        /// </summary>
        public double RoundDoubleTwo(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public AutoMapperProfile()
        {
            CreateMap<UpdateContractRequest, Contract>();
            CreateMap<CreateContractRequest, Contract>();

            CreateMap<AmountRequest, Amount>()
                .ForMember(item => item.Value, opt => opt.ResolveUsing(i => RoundDoubleTwo(i.Value)));

            CreateMap<Contract, SimpleContractResponse>();
            CreateMap<Contract, DetailedContractResponse>();
            CreateMap<Amount, AmountResponse>();
        }
    }
}
