using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cmas.BusinessLayers.Contracts;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Infrastructure.Domain.Commands;
using Cmas.Infrastructure.Domain.Queries;
using Cmas.Services.Contracts.Dtos.Requests;
using AutoMapper;
using Cmas.BusinessLayers.Requests;
using Cmas.Infrastructure.ErrorHandler;
using Microsoft.Extensions.Logging;

namespace Cmas.Services.Contracts
{
    public class ContractsService
    {
        private readonly ContractsBusinessLayer _contractsBusinessLayer;
        private readonly RequestsBusinessLayer _requestsBusinessLayer;
        private readonly IMapper _autoMapper;
        private ILogger _logger;

        public ContractsService(IServiceProvider serviceProvider)
        {
            var _commandBuilder = (ICommandBuilder)serviceProvider.GetService(typeof(ICommandBuilder));
            var _queryBuilder = (IQueryBuilder)serviceProvider.GetService(typeof(IQueryBuilder));
            var _loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            _autoMapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            _contractsBusinessLayer = new ContractsBusinessLayer(_commandBuilder, _queryBuilder);
            _requestsBusinessLayer = new RequestsBusinessLayer(_commandBuilder, _queryBuilder);

            _logger = _loggerFactory.CreateLogger<ContractsService>();
        }

        public async Task<IEnumerable<Contract>> GetContractsAsync()
        {
            return await _contractsBusinessLayer.GetContracts();
        }

        public async Task<Contract> GetContractAsync(string contractID)
        {
            return await _contractsBusinessLayer.GetContract(contractID);
        }

        public async Task<string> CreateContractAsync()
        {
            return await _contractsBusinessLayer.CreateContract();
        }

        public async Task<string> UpdateContractAsync(string contractId, UpdateContractRequest request)
        {
            Contract currentContract = await _contractsBusinessLayer.GetContract(contractId);

            Contract newContract = _autoMapper.Map<UpdateContractRequest, Contract>(request, currentContract);

            return await _contractsBusinessLayer.UpdateContract(contractId, newContract);
        }

        public async Task<string> DeleteContractAsync(string contractId)
        {

            var requests = await _requestsBusinessLayer.GetRequestsByContractId(contractId);

            if (requests.Any())
            {
                _logger.LogWarning("Can not delete a contract that has a requests");
                throw new GeneralServiceErrorException("Can not delete a contract that has a requests");
            }

            return await _contractsBusinessLayer.DeleteContract(contractId);
        }



    }
}
