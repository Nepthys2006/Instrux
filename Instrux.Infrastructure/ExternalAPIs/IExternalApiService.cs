namespace Instrux.Infrastructure.ExternalAPIs;

public interface IExternalApiService
{
    Task<string?> CallAsync(string endpoint, CancellationToken ct = default);
}
