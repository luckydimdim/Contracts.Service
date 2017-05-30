using System;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Infrastructure.Security;
using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cmas.Infrastructure.ErrorHandler;
using Cmas.Services.Contracts.Dtos.Requests;
using Cmas.Services.Contracts.Dtos.Responses;
using Nancy.Validation;

namespace Cmas.Services.Contracts
{
    public class ContractsModule : NancyModule
    {
        private IServiceProvider _serviceProvider;

        private ContractsService contractsService;

        private ContractsService _contractsService
        {
            get
            {
                if (contractsService == null)
                    contractsService = new ContractsService(_serviceProvider, Context);

                return contractsService;
            }
        }


        public ContractsModule(IServiceProvider serviceProvider) : base("/contracts")
        {
            this.RequiresRoles(new[] { Role.Contractor, Role.Customer });
            _serviceProvider = serviceProvider;


            /// <summary>
            /// /contracts- получить договоры
            /// </summary>
            Get<IEnumerable<SimpleContractResponse>>("/", GetContractsHandlerAsync);

            /// <summary>
            /// /contracts/{id} - получить договор по ID
            /// </summary>
            Get<Contract>("/{id}", GetContractHandlerAsync);

            /// <summary>
            /// Создать договор
            /// </summary>
            Post<string>("/", CreateContractHandlerAsync);

            /// <summary>
            /// Обновить договор
            /// </summary>
            Put<string>("/{id}", UpdateContractHandlerAsync);

            /// <summary>
            /// Удалить
            /// </summary>
            Delete<string>("/{id}", DeleteContractHandlerAsync);
        }

        #region Обработчики

        private async Task<IEnumerable<SimpleContractResponse>> GetContractsHandlerAsync(dynamic args,  CancellationToken ct)
        {
            return await _contractsService.GetContractsAsync();
        }

        private async Task<Contract> GetContractHandlerAsync(dynamic args, CancellationToken ct)
        {
            return await _contractsService.GetContractAsync(args.id);
        }

        private async Task<string> CreateContractHandlerAsync(dynamic args, CancellationToken ct)
        {
            this.RequiresRoles(new[] { Role.Customer });
            
            return  await _contractsService.CreateContractAsync();
        }

        private async Task<string> UpdateContractHandlerAsync(dynamic args, CancellationToken ct)
        {
            this.RequiresRoles(new[] { Role.Customer });

            var request = this.Bind<UpdateContractRequest>();

            var validationResult = this.Validate(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationErrorException(validationResult.FormattedErrors);
            }

            return await _contractsService.UpdateContractAsync(args.Id, request);
        }

        private async Task<string> DeleteContractHandlerAsync(dynamic args, CancellationToken ct)
        {
            this.RequiresRoles(new[] { Role.Customer });

            return await _contractsService.DeleteContractAsync(args.id);
        }

        #endregion

    }
}
