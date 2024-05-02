using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.ReportProvider.Api.Interceptors;

public class ValidationInterceptor(
    IServiceProvider serviceProvider
) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is null)
            return await continuation(request, context);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    $"{string.Join(
                        ';',
                        validationResult.Errors.Select(x => x.ErrorMessage))}"
                )
            );

        return await continuation(request, context);
    }
}