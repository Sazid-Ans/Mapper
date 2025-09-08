using AuthServer.Dto.ResponseDto;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.PipeLineExt
{
    public static class ModelValidationResponseExt
    {
        public static IServiceCollection AddCustomModelValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new ErrorDetail
                        {
                            Code = x.Key, //field name
                            Message = x.Value?.Errors.First().ErrorMessage
                        })
                        .ToList();

                    var response = BaseResponse<object>.Failure(errors);

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
