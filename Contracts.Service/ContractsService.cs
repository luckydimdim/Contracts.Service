using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cmas.BusinessLayers.Contracts;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Services.Contracts.Dtos.Requests;
using AutoMapper;
using Cmas.BusinessLayers.Requests;
using Cmas.Infrastructure.ErrorHandler;
using Cmas.Services.Contracts.Dtos.Responses;
using Microsoft.Extensions.Logging;
using Nancy;
using Cmas.BusinessLayers.CallOffOrders;

namespace Cmas.Services.Contracts
{
    public class ContractsService
    {
        private readonly ContractsBusinessLayer _contractsBusinessLayer;
        private readonly RequestsBusinessLayer _requestsBusinessLayer;
        private readonly CallOffOrdersBusinessLayer _callOffOrdersBusinessLayer;
        private readonly IMapper _autoMapper;
        private ILogger _logger;

        public ContractsService(IServiceProvider serviceProvider, NancyContext ctx)
        {
           
            var _loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            _autoMapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            _contractsBusinessLayer = new ContractsBusinessLayer(serviceProvider, ctx.CurrentUser);
            _requestsBusinessLayer = new RequestsBusinessLayer(serviceProvider, ctx.CurrentUser);
            _callOffOrdersBusinessLayer = new CallOffOrdersBusinessLayer(serviceProvider, ctx.CurrentUser);

            _logger = _loggerFactory.CreateLogger<ContractsService>();
        }

        /// <summary>
        /// Получить все договоры
        /// </summary>
        public async Task<IEnumerable<SimpleContractResponse>> GetContractsAsync()
        {
            var result = new List<SimpleContractResponse>();

            // FIXME: В случае, если подрядчика более одного, продумать показ договоров только для него

            var contracts = await _contractsBusinessLayer.GetContracts();

            foreach (var contract in contracts)
            {
                result.Add(_autoMapper.Map<SimpleContractResponse>(contract));
            }

            return result; 
        }

        /// <summary>
        /// Получить договор
        /// </summary>
        public async Task<DetailedContractResponse> GetContractAsync(string contractID)
        {
            var contract = await _contractsBusinessLayer.GetContract(contractID);
            
            if (contract == null)
            {
                throw new NotFoundErrorException();
            }

            var result = _autoMapper.Map<DetailedContractResponse>(contract);

            // FIXME: сделать вьюху чтобы не вытягивать все НЗ
            var callOffs = await _callOffOrdersBusinessLayer.GetCallOffOrders(contractID);

            result.readOnly = callOffs.Count() > 0;

            return result;
        }

        /// <summary>
        /// Создать договор
        /// </summary>
        public async Task<string> CreateContractAsync(CreateContractRequest request)
        {
            var contract = _autoMapper.Map<Contract>(request);

            return await _contractsBusinessLayer.CreateContract(contract);
        }

        /// <summary>
        /// Обновить договор
        /// </summary>
        public async Task<string> UpdateContractAsync(string contractId, UpdateContractRequest request)
        {
            // FIXME: Продумать что делать если по договору есть наряд заказы
            Contract currentContract = await _contractsBusinessLayer.GetContract(contractId);

            if (currentContract == null)
            {
                throw new NotFoundErrorException();
            }

            Contract newContract = _autoMapper.Map<UpdateContractRequest, Contract>(request, currentContract);

            return await _contractsBusinessLayer.UpdateContract(contractId, newContract);
        }

        /// <summary>
        /// Удалить договор
        /// </summary>
        public async Task<string> DeleteContractAsync(string contractId)
        {
            var requests = await _requestsBusinessLayer.GetRequestsByContractId(contractId);

            if (requests.Any())
            {
                throw new GeneralServiceErrorException("Can not delete a contract that has a requests");
            }

            // FIXME: Продумать что делать если по договору есть наряд заказы

            return await _contractsBusinessLayer.DeleteContract(contractId);
        }



    }
}
