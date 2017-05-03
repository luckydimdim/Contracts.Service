namespace Cmas.Services.Contracts.Dtos.Requests
{
    public class UpdateContractRequest
    {
        public string Name;

        public string Number;

        public string StartDate;

        public string FinishDate;

        public string ContractorName;

        public string Currency;

        public double Amount;

        public bool VatIncluded;

        public string ConstructionObjectName;

        public string ConstructionObjectTitleName;

        public string ConstructionObjectTitleCode;

        public string Description;

        /// <summary>
        /// Системное имя шаблона для НЗ и TS
        /// </summary>
        public string TemplateSysName;
    }
}
