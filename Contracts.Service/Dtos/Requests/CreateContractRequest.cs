using System;
using System.Collections.Generic;

namespace Cmas.Services.Contracts.Dtos.Requests
{
    public class CreateContractRequest
    {
        public string Name;

        public string Number;

        public DateTime? StartDate;

        public DateTime? FinishDate;

        public string ContractorName;

        public string Currency;
        
        public bool VatIncluded;

        public string ConstructionObjectName;

        public string ConstructionObjectTitleName;

        public string ConstructionObjectTitleCode;

        public string Description;

        public IList<AmountRequest> Amounts;
    }
}
