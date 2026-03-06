using Npgsql;
using WCM.API.ApiService.Domain.Shared;
using WCM.API.ApiService.Infrastructure.Persistence;

namespace WCM.API.ApiService.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly IApplicationDbContext Context;
    protected readonly ILogger Logger;

    protected BaseRepository(IApplicationDbContext context, ILogger logger)
    {
        Context = context;
        Logger = logger;
    }

    /// <summary>
    /// Executes a database query with centralized error handling and logging.
    /// </summary>
    protected async Task<Result<T>> ExecuteQueryAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        Action<T>? onSuccess = null,
        params object[] logParameters)
    {
        try
        {
            T result = await operation();

            onSuccess?.Invoke(result);

            return Result.Success(result);
        }
        catch (NpgsqlException ex) when (ex.InnerException is TimeoutException || ex.SqlState == "57014")
        {
            LogError(ex, "Database timeout while {OperationName}", operationName, logParameters);
            return Result.Failure<T>(DomainErrors.Database.Timeout);
        }
        catch (NpgsqlException ex)
        {
            Logger.LogError(ex,
                "Database error while {OperationName}: SqlState={SqlState}, Message={ErrorMessage}, Context={@LogParameters}",
                operationName, ex.SqlState, ex.Message, logParameters);
            return Result.Failure<T>(DomainErrors.Database.Error);
        }
        catch (Exception ex)
        {
            LogError(ex, "Unexpected error while {OperationName}", operationName, logParameters);
            return Result.Failure<T>(DomainErrors.General.Error);
        }
    }

    /// <summary>
    /// Executes a database command (insert/update/delete) with centralized error handling and logging.
    /// </summary>
    protected async Task<Result> ExecuteCommandAsync(
        Func<Task> operation,
        string operationName,
        Action? onSuccess = null,
        params object[] logParameters)
    {
        try
        {
            await operation();

            onSuccess?.Invoke();

            return Result.Success();
        }
        catch (NpgsqlException ex) when (ex.InnerException is TimeoutException || ex.SqlState == "57014")
        {
            LogError(ex, "Database timeout while {OperationName}", operationName, logParameters);
            return Result.Failure(DomainErrors.Database.Timeout);
        }
        catch (NpgsqlException ex)
        {
            Logger.LogError(ex,
                "Database error while {OperationName}: SqlState={SqlState}, Message={ErrorMessage}, Context={@LogParameters}",
                operationName, ex.SqlState, ex.Message, logParameters);
            return Result.Failure(DomainErrors.Database.Error);
        }
        catch (Exception ex)
        {
            LogError(ex, "Unexpected error while {OperationName}", operationName, logParameters);
            return Result.Failure(DomainErrors.General.Error);
        }
    }

    private void LogError(Exception ex, string messageTemplate, string operationName, object[] logParameters)
    {
        if (logParameters.Length > 0)
        {
            Logger.LogError(ex, messageTemplate + ": {@LogParameters}", operationName, logParameters);
        }
        else
        {
            Logger.LogError(ex, messageTemplate, operationName);
        }
    }
}
