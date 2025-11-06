namespace BuslyCLI.Console.Tests.EndToEnd;

/// <summary>
///     Generic test fixture base class that ensures only one container is created across all derived test classes.
///     Provides thread-safe singleton pattern with proper cleanup.
/// </summary>
/// <typeparam name="TContainer">The type of container to manage</typeparam>
public abstract class SingletonTestFixtureBase<TContainer> : IDisposable where TContainer : class, IAsyncDisposable
{
    private static readonly object _lock = new();
    private static TContainer _container;
    private static int _referenceCount;

    protected TContainer Container
    {
        get
        {
            if (_container == null)
                throw new InvalidOperationException(
                    $"Container has not been initialized. Call {nameof(InitializeContainer)}() first.");
            return _container;
        }
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        await InitializeContainer();
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await DecrementReferenceCount();
    }

    private async Task InitializeContainer()
    {
        var shouldStart = false;
        lock (_lock)
        {
            if (_container == null)
            {
                _container = CreateContainer();
                shouldStart = true;
            }

            _referenceCount++;
        }

        if (shouldStart && _container != null) await StartContainerAsync(_container);
    }

    private async Task DecrementReferenceCount()
    {
        var shouldDispose = false;
        TContainer containerToDispose = null;
        lock (_lock)
        {
            _referenceCount--;
            if (_referenceCount <= 0 && _container != null)
            {
                shouldDispose = true;
                containerToDispose = _container;
                _container = null;
            }
        }

        if (shouldDispose && containerToDispose != null)
        {
            await containerToDispose.DisposeAsync();
        }
    }

    /// <summary>
    ///     Creates a new container instance. Must be implemented by derived classes.
    /// </summary>
    /// <returns>The created container instance</returns>
    protected abstract TContainer CreateContainer();

    /// <summary>
    ///     Starts the container asynchronously. Can be overridden by derived classes if needed.
    /// </summary>
    /// <param name="container">The container to start</param>
    protected virtual async Task StartContainerAsync(TContainer container)
    {
        // Default implementation - derived classes can override if needed
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DecrementReferenceCount();
    }
}