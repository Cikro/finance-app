﻿using FluentValidation;
using finance_app.Types.DataContracts.V1.Dtos;

namespace finance_app.Types.Validators
{
    public class PaginationInfoValidator : AbstractValidator<PaginationInfo>
    {
        private const string _itemsPerPageRequired = "Items Per Page was not provided. It must be provided if {PropertyName} is provided";
        private const string _pageNumberRequired = "Page Number was not provided. It must be provided if {PropertyName} is provided";
        public PaginationInfoValidator()
        {
            RuleFor(info => info.PageNumber)
                .Cascade(CascadeMode.Stop)
                .NotNull().When(info => info.ItemsPerPage != null, ApplyConditionTo.CurrentValidator)
                .WithMessage(_itemsPerPageRequired)
                .GreaterThanOrEqualTo(1).When(info => BothProvided(info), ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} must be 1 or larger.");
            RuleFor(info => info.ItemsPerPage)
                .Cascade(CascadeMode.Stop)
                .NotNull().When(info => info.PageNumber != null, ApplyConditionTo.CurrentValidator)
                .WithMessage(_pageNumberRequired)
                .GreaterThanOrEqualTo(1).When(info => BothProvided(info), ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} must be 1 or larger.");
        }
        private bool BothProvided(PaginationInfo info)
        {
            return info?.PageNumber != null && info?.ItemsPerPage != null;
        }
    }
}
