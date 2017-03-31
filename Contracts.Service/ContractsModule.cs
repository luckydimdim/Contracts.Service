using Cmas.BusinessLayers.Contracts;
using Cmas.BusinessLayers.Contracts.Entities;
using Cmas.Infrastructure.Domain.Commands;
using Cmas.Infrastructure.Domain.Queries;
using Nancy;
using Nancy.ModelBinding;

namespace Cmas.Services.Contracts
{
    public class ContractsModule : NancyModule
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ContractBusinessLayer _contractBusinessLayer;

        public ContractsModule(ICommandBuilder commandBuilder, IQueryBuilder queryBuilder) : base("/contracts")
        {
            _commandBuilder = commandBuilder;
            _queryBuilder = queryBuilder;
            _contractBusinessLayer = new ContractBusinessLayer(_commandBuilder, _queryBuilder);


            Get("/", _ =>
            {
                return _contractBusinessLayer.GetContracts();
            });


            Get("/{id}", async args => await _contractBusinessLayer.GetContract(args.id));


            Post("/", async (args, ct) =>
            {
                var form = this.Bind<Contract>();

                string result = await _contractBusinessLayer.CreateContract(form);

                return result.ToString();
            });

            Put("/", async (args, ct) =>
            {
                var form = this.Bind<Contract>();

                string result = await _contractBusinessLayer.UpdateContract(form.Id, form);

                return result.ToString();
            });

            Delete("/{id}", async args =>
            {
                return await _contractBusinessLayer.DeleteContract(args.id);
            });
        }

    }
}
