using System;
using System.Collections.Generic;
using System.Text;

namespace Cmas.Services.Contracts.Dtos.Responses
{
    
    public class SimpleContractResponse
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public string Id;

        /// <summary>
        /// Название договора
        /// </summary>
        public string Name;

        /// <summary>
        /// Номер договора
        /// </summary>
        public string Number;

        /// <summary>
        /// Дата заключения
        /// </summary>
        public DateTime? StartDate;

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? FinishDate;

        /// <summary>
        /// Имя подрядчика
        /// </summary>
        public string ContractorName;

        /// <summary>
        /// Стоимости договора
        /// </summary>
        public IList<AmountResponse> Amounts = new List<AmountResponse>();

        /// <summary>
        /// Название объекта строительства
        /// </summary>
        public string ConstructionObjectName;
        
        /// <summary>
        /// Системное имя шаблона для НЗ и TS
        /// </summary>
        public string TemplateSysName;

        /// <summary>
        /// Валюты договора
        /// </summary>
        public IEnumerable<string> Currencies;
    }
}
