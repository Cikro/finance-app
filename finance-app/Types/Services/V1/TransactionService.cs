using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using finance_app.Types.Repositories.Account;
using finance_app.Types.Services.V1.Interfaces;
using finance_app.Types.DataContracts.V1.Responses;
using finance_app.Types.DataContracts.V1.Dtos;
using finance_app.Types.Models.ResourceIdentifiers;
using AutoMapper;
using System;
using finance_app.Types.Repositories.Transaction;

namespace finance_app.Types.Services.V1
{
    public class TransactionsService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _context;
        

        public TransactionsService(IMapper mapper, ITransactionRepository transactionRepository,
                             IAccountRepository accountRepository,
                             IAuthorizationService authorizationService,
                             IHttpContextAccessor context) {

            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _context = context;
        }

        /// <inheritdoc cref="ITransactionService.GetRecentTransactions"/>
        public async Task<ApiResponse<ListResponse<TransactionDto>>> GetRecentTransactions(AccountResourceIdentifier accountId, PaginationInfo pageInfo, bool includeJournals) {
            if (accountId == null) { throw new ArgumentNullException(nameof(AccountResourceIdentifier)); }
            if (!(pageInfo?.PageNumber != null) || pageInfo?.PageNumber <= 0) { return new ApiResponse<ListResponse<TransactionDto>>(ApiResponseCodesEnum.BadRequest, "Invalid Page Number."); }
            if (!(pageInfo?.ItemsPerPage!= null) || pageInfo?.ItemsPerPage <= 0) { return new ApiResponse<ListResponse<TransactionDto>>(ApiResponseCodesEnum.BadRequest, "Invalid Items Per Page.");; }

            var account = _accountRepository.GetAccountByAccountId(accountId.Id);
            if (!(await _authorizationService.AuthorizeAsync(_context.HttpContext.User, account, "CanAccessResourcePolicy" )).Succeeded) 
            {
                return new ApiResponse<ListResponse<TransactionDto>>(ApiResponseCodesEnum.Unauthorized, "Unauthorized");
            }

            int offset = (int)pageInfo.PageNumber - 1;
            var transactions = includeJournals ?
                await _transactionRepository.GetRecentTransactionsWithJournalByAccountId(accountId.Id, (int)pageInfo.ItemsPerPage, (int)offset) :
                await _transactionRepository.GetRecentTransactionsByAccountId(accountId.Id, (int)pageInfo.ItemsPerPage, (int)offset);

            var ret = new ListResponse<TransactionDto>(_mapper.Map<List<TransactionDto>>(transactions));

            return new ApiResponse<ListResponse<TransactionDto>>(ret, ApiResponseCodesEnum.Success, "Success");
        }

        /// <inheritdoc cref="ITransactionService.UpdateTransaction"/>   
        public async Task<ApiResponse<TransactionDto>> UpdateTransaction(Transaction transaction) {
            // Fetch Transaction to update
            var transactionToUpdate = await _transactionRepository.GetTransaction(transaction.Id);
            if (transactionToUpdate == null) 
            {
                var message = $"Error updating transaction. Transaction with id '{transaction.Id}' does not exist.";
                return new ApiResponse<TransactionDto>(ApiResponseCodesEnum.ResourceNotFound, message);
            }

            // Verify that the use can access the transaction
            var account = _accountRepository.GetAccountByAccountId(transactionToUpdate.AccountId);
            if (!(await _authorizationService.AuthorizeAsync(_context.HttpContext.User, account, "CanAccessResourcePolicy" )).Succeeded) 
            {
                return new ApiResponse<TransactionDto>(ApiResponseCodesEnum.Unauthorized, "Unauthorized");
            }

            // Currently can only update the notes on a transaction
            transactionToUpdate.Notes = transaction.Notes;

            var updatedTransaction = _transactionRepository.UpdateTransaction(transactionToUpdate);

            return new ApiResponse<TransactionDto>( _mapper.Map<TransactionDto>(updatedTransaction));

        }
    }
}