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
using Cmas.BusinessLayers.TimeSheets;
using Cmas.Infrastructure.Security;

namespace Cmas.Services.Contracts
{
    public class ContractsService
    {
        private readonly ContractsBusinessLayer _contractsBusinessLayer;
        private readonly RequestsBusinessLayer _requestsBusinessLayer;
        private readonly CallOffOrdersBusinessLayer _callOffOrdersBusinessLayer;
        private readonly TimeSheetsBusinessLayer _timeSheetsBusinessLayer;
        private readonly IMapper _autoMapper;
        private ILogger _logger;
        private NancyContext context;

        public ContractsService(IServiceProvider serviceProvider, NancyContext ctx)
        {
            var _loggerFactory = (ILoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));

            _autoMapper = (IMapper) serviceProvider.GetService(typeof(IMapper));

            _contractsBusinessLayer = new ContractsBusinessLayer(serviceProvider, ctx.CurrentUser);
            _requestsBusinessLayer = new RequestsBusinessLayer(serviceProvider, ctx.CurrentUser);
            _callOffOrdersBusinessLayer = new CallOffOrdersBusinessLayer(serviceProvider, ctx.CurrentUser);
            _timeSheetsBusinessLayer = new TimeSheetsBusinessLayer(serviceProvider, ctx.CurrentUser);
            context = ctx;
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

            bool isAdmin = context.CurrentUser.HasRole(Role.Administrator);

            if (contract == null)
            {
                throw new NotFoundErrorException();
            }

            var result = _autoMapper.Map<DetailedContractResponse>(contract);
             
            var callOffsCount = await _callOffOrdersBusinessLayer.GetCallOffOrdersCount(contractID);

            result.readOnly = callOffsCount > 0;

            result.canDelete = true;

            if (isAdmin)
                result.canDelete = true;
            else if (!isAdmin && result.readOnly)
                result.canDelete = false;

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

            bool isAdmin = context.CurrentUser.HasRole(Role.Administrator);

            if (requests.Any() && !isAdmin)
                throw new GeneralServiceErrorException("Can not delete a contract that has a requests");
            
            
            foreach (var request in requests)
            {
                IEnumerable<string> timeSheetIds =
                    await _timeSheetsBusinessLayer.GetTimeSheetsIdsByRequestId(request.Id);

                _logger.LogInformation(
                    $"deleting time-sheets before request: {string.Join(",", timeSheetIds)} ...");
                     
                foreach (var timeSheetId in timeSheetIds)
                {
                    await _timeSheetsBusinessLayer.DeleteTimeSheet(timeSheetId);
                }

                _logger.LogInformation("Deleting request...");

                await _requestsBusinessLayer.DeleteRequest(request.Id);

                _logger.LogInformation("request deleted");
            }
            

            // удаление НЗ
            var callOffs = await _callOffOrdersBusinessLayer.GetCallOffOrders(contractId);
            foreach (var callOff in callOffs)
            {
                _logger.LogInformation($"deleting call off {callOff.Id}...");
                await _callOffOrdersBusinessLayer.DeleteCallOffOrder(callOff.Id);
                _logger.LogInformation("call off  deleted");
            }

            _logger.LogInformation("Deleting contract...");
            return await _contractsBusinessLayer.DeleteContract(contractId);
        }
    }
}