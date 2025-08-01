
public interface IUnitOfWork : IDisposable
{
    IApiKeyRepository ApiKeys { get; }
    IEncryptionKeyRepository EncryptionKeys { get; }
    IAuditLogRepository AuditLogs { get; }
    ISystemConfigurationRepository SystemConfigurations { get; }
    IClientAppRepository ClientApps { get; }
    IRoleRepository Roles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Transaction methods simplified for TransactionBehavior
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default); // Không cần tham số transaction
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default); // Không cần tham số transaction

    #region Utility

    IExecutionStrategy CreateExecutionStrategy();
    Task RefreshEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task DetachEntityAsync<T>(T entity) where T : class;
    string GetEntityDebugInfo<T>(T entity) where T : class;

    #endregion
}

public class UnitOfWork : IUnitOfWork
{
    private readonly CPGDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private IDbContextTransaction _currentTransaction; // Quản lý transaction nội bộ
    private bool _disposed;

    public UnitOfWork(
        CPGDbContext context,
        IApiKeyRepository apiKeys,
        IEncryptionKeyRepository encryptionKeys,
        IAuditLogRepository auditLogs,
        ISystemConfigurationRepository systemConfigurations,
        IClientAppRepository clientApps,
        IRoleRepository roles,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ApiKeys = apiKeys;
        EncryptionKeys = encryptionKeys;
        AuditLogs = auditLogs;
        SystemConfigurations = systemConfigurations;
        ClientApps = clientApps;
        Roles = roles;

        _retryPolicy = Policy
            .Handle<DbUpdateException>()
            .Or<DbUpdateConcurrencyException>()
            .OrInner<Exception>(IsTransientError)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, pollyContext) =>
                {
                    _logger.LogWarning(exception, "Error during database operation. Retrying {RetryCount}/{MaxRetries} in {Delay}s...",
                        retryCount, 3, timeSpan.TotalSeconds);
                });
    }

    public IApiKeyRepository ApiKeys { get; }
    public IEncryptionKeyRepository EncryptionKeys { get; }
    public IAuditLogRepository AuditLogs { get; }
    public ISystemConfigurationRepository SystemConfigurations { get; }
    public IClientAppRepository ClientApps { get; }
    public IRoleRepository Roles { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Log entities về to be saved for debugging
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            _logger.LogDebug("Saving {Count} entities", entries.Count);

            foreach (var entry in entries)
            {
                _logger.LogDebug("Entity {EntityType} - State: {State}",
                    entry.Entity.GetType().Name, entry.State);
            }

            //var result = await _context.SaveChangesAsync(cancellationToken);
            var result = await _retryPolicy.ExecuteAsync(ct => _context.SaveChangesAsync(ct), cancellationToken);
            _logger.LogDebug("Successfully saved {Count} entities", result);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency exception occurred during save");

            // Log chi tiết về entities gây conflict
            foreach (var entry in ex.Entries)
            {
                _logger.LogWarning("Conflict in entity {EntityType} with key {Key}",
                    entry.Entity.GetType().Name,
                    string.Join(",", entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue)));
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception occurred");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogDebug("TransactionBehavior: Attempted to begin a new transaction while one is already in progress. Using existing transaction: {TransactionId}", _currentTransaction.TransactionId);
            return; // TransactionBehavior sẽ gọi lại, không cần throw lỗi
        }
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("TransactionBehavior: Transaction began with ID: {TransactionId}", _currentTransaction.TransactionId);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("TransactionBehavior: CommitTransactionAsync called but no active transaction found.");
            // Có thể xảy ra nếu BeginTransactionAsync chưa được gọi hoặc đã được commit/rollback
            // Trong ngữ cảnh TransactionBehavior, điều này không nên xảy ra nếu Begin được gọi đúng.
            return;
        }

        try
        {
            // SaveChangesAsync nên được gọi bởi CommandHandler *TRƯỚC KHI* next() trong Behavior trả về.
            // Behavior chỉ chịu trách nhiệm commit transaction đã có các thay đổi.
            // Tuy nhiên, để an toàn, có thể gọi lại ở đây, nhưng nó có thể là thừa nếu handler đã gọi.
            // await SaveChangesAsync(cancellationToken); 

            await _currentTransaction.CommitAsync(cancellationToken);
            _logger.LogInformation("TransactionBehavior: Transaction committed with ID: {TransactionId}", _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TransactionBehavior: Error committing transaction with ID: {TransactionId}", _currentTransaction?.TransactionId);
            // Rollback sẽ được gọi bởi catch block của TransactionBehavior
            throw;
        }
        finally
        {
            // Quan trọng: Dispose và reset _currentTransaction sau khi commit thành công
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("TransactionBehavior: RollbackTransactionAsync called but no active transaction found.");
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("TransactionBehavior: Transaction rolled back with ID: {TransactionId}", _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TransactionBehavior: Error rolling back transaction with ID: {TransactionId}", _currentTransaction?.TransactionId);
            // Ném lại lỗi để Behavior có thể xử lý hoặc log thêm
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    private bool IsTransientError(Exception ex)
    {
        if (_context.Database.IsOracle())
        {
            var message = ex.Message.ToLowerInvariant();
            return message.Contains("ora-12152") || message.Contains("ora-12170") ||
                   message.Contains("ora-12535") || message.Contains("ora-24550") ||
                   message.Contains("timeout") || message.Contains("connection reset");
        }
        if (_context.Database.IsNpgsql())
        {
            var message = ex.Message.ToLowerInvariant();
            return message.Contains("timeout") || message.Contains("connection reset") ||
                   message.Contains("connection refused") || message.Contains("too many clients");
        }
        return ex is TimeoutException || ex.Message.ToLowerInvariant().Contains("timeout");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose(); // Đảm bảo transaction được dispose nếu UnitOfWork bị dispose khi transaction còn active
                _context.Dispose();
                _logger.LogDebug("UnitOfWork disposed.");
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #region Utility

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _context.Database.CreateExecutionStrategy();
    }

    public async Task RefreshEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : class
    {
        var entry = _context.Entry(entity);
        if (entry.State != EntityState.Detached)
        {
            await entry.ReloadAsync(cancellationToken);
        }
    }

    public async Task DetachEntityAsync<T>(T entity) where T : class
    {
        var entry = _context.Entry(entity);
        entry.State = EntityState.Detached;
    }

    public string GetEntityDebugInfo<T>(T entity) where T : class
    {
        var entry = _context.Entry(entity);
        var sb = new StringBuilder();
        sb.AppendLine($"Entity: {typeof(T).Name}");
        sb.AppendLine($"State: {entry.State}");

        if (entry.State != EntityState.Detached)
        {
            sb.AppendLine("Modified Properties:");
            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                sb.AppendLine($"  {property.Metadata.Name}: {property.OriginalValue} -> {property.CurrentValue}");
            }
        }

        return sb.ToString();
    }

    #endregion
}
