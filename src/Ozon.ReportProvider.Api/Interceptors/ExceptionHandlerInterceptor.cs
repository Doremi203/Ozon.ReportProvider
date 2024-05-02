using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Ozon.ReportProvider.Api.Interceptors;

public class ExceptionHandlerInterceptor(
    ILogger<ExceptionHandlerInterceptor> logger
) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            logger.LogError(e.StackTrace);
            
            throw new RpcException(
                new Status(StatusCode.Internal, "Internal server error")
            );
        }
    }
}