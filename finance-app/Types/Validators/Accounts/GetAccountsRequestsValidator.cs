﻿using FluentValidation;
using finance_app.Types.DataContracts.V1.Requests.Accounts;

namespace finance_app.Types.Validators.Accounts
{
    public class GetAccountsRequestValidator : AbstractValidator<GetAccountsRequest>
    {
        public GetAccountsRequestValidator()
        {
            RuleFor(request => request.PageInfo).SetValidator(new PaginationInfoValidator()).When(request => request.PageInfo != null);
        }
    }
}
