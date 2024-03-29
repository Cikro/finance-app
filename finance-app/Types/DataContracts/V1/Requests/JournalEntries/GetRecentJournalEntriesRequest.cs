using Microsoft.AspNetCore.Mvc;
using finance_app.Types.DataContracts.V1.Dtos;

namespace finance_app.Types.DataContracts.V1.Requests.JournalEntries
{
    public class GetRecentJournalEntriesRequest
    {   
        public PaginationInfo PageInfo { get; set; }
        public bool IncludeTransactions {get; set;}
    }
}
