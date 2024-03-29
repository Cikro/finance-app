using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using finance_app.Types.DataContracts.V1.Dtos;
using finance_app.Types.DataContracts.V1.Responses;
using finance_app.Types.Services.V1.ResponseMessages;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Filters;

namespace finance_app.Middleware {
    public class ValidationResponseMapperFilter : IActionFilter 
    {
        private readonly IMapper _mapper;

        public ValidationResponseMapperFilter(IMapper mapper)
        {
            _mapper = mapper;
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) { return; }

            var errors = context.ModelState
                .Where(v => v.Value.Errors.Count > 0)
                .Select(v => new ValidationError
                {
                    Key = v.Key,
                    Errors = v.Value.Errors.Select(v => v.ErrorMessage).ToList() 
                }).ToList();

            var apiResponse = new ApiResponse<List<ValidationError>>(errors, ApiResponseCodesEnum.BadRequest, new BadRequestErrorMessage());

            var mapper = (IMapper)context.HttpContext
                    .RequestServices.GetService(typeof(IMapper));

            context.Result = new JsonResult(apiResponse) {
                StatusCode = mapper.Map<int>(apiResponse.ResponseCode)
            };
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

    }
}