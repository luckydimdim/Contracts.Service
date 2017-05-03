using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cmas.BusinessLayers.Contracts;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Infrastructure.Domain.Commands;
using Cmas.Infrastructure.Domain.Queries;
using Cmas.Services.Contracts.Dtos.Requests;
using AutoMapper;

namespace Cmas.Services.Contracts
{
    public class ContractsService
    {
        private readonly ContractBusinessLayer _contractBusinessLayer;
        private readonly IMapper _autoMapper;

        public ContractsService(IServiceProvider serviceProvider)
        {
            var _commandBuilder = (ICommandBuilder)serviceProvider.GetService(typeof(ICommandBuilder));
            var _queryBuilder = (IQueryBuilder)serviceProvider.GetService(typeof(IQueryBuilder));

            _autoMapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            _contractBusinessLayer = new ContractBusinessLayer(_commandBuilder, _queryBuilder); 
        }

        public async Task<IEnumerable<Contract>> GetContractsAsync()
        {
            return await _contractBusinessLayer.GetContracts();
        }

        public async Task<Contract> GetContractAsync(string contractID)
        {
            return await _contractBusinessLayer.GetContract(contractID);
        }

        public async Task<string> CreateContractAsync()
        {
            return await _contractBusinessLayer.CreateContract();
        }

        public async Task<string> UpdateContractAsync(string contractId, UpdateContractRequest request)
        {
            Contract currentContract = await _contractBusinessLayer.GetContract(contractId);

            Contract newContract = _autoMapper.Map<UpdateContractRequest, Contract>(request, currentContract);

            return await _contractBusinessLayer.UpdateContract(contractId, newContract);
        }

        public async Task<string> DeleteContractAsync(string contractId)
        {
          
            return await _contractBusinessLayer.DeleteContract(contractId);
        }



    }
}
