using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;
using DistributedLockManager;
using DistributedNonce;
using BlockChainTools.Interfaces;

namespace BlockChainTools.Tests;

[TestFixture]
public class BlockChainToolsTests
{
    private IServiceScope? scope;
    private IContainer? _redisContainer;
    private const string RedisImage = "redis:latest";
    private const int RedisPort = 6379;
    private ConnectionMultiplexer? _mux;
    private ServiceProvider? _provider;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _redisContainer = new ContainerBuilder(RedisImage)
            .WithCleanUp(true)
            .WithName($"dtm-redis-{Guid.NewGuid():N}")
            .WithPortBinding(RedisPort, true)
            .Build();

        if (_redisContainer == null) Assert.Fail("Redis container not started");
        
        await _redisContainer!.StartAsync();

        var container = _redisContainer!;
        var host = container.Hostname;
        var port = container.GetMappedPublicPort(RedisPort);
        var endpoint = $"{host}:{port}";

        // create the connection multiplexer and register it in DI (keep it alive for all tests)
        _mux = await ConnectionMultiplexer.ConnectAsync(endpoint);

        var services = new ServiceCollection();
        services.AddDistributedLockManager();
        services.AddDistributedNonce();
        services.AddSingleton<HttpClient>(_ => new HttpClient());
        services.AddSingleton<IConnectionMultiplexer>(_mux);
        // Register BlockChainTools services under test
        services.AddBlockChainTools();

        _provider = services.BuildServiceProvider();
        scope = _provider.CreateScope();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_redisContainer is not null)
            await _redisContainer.StopAsync();

        scope?.Dispose();
        if (_provider is not null)
            await _provider.DisposeAsync();
        _mux?.Dispose();
    }

    [Test, Order(1)]
    public void All_BlockChainTools_Interfaces_Are_Resolvable()
    {
        if (scope is null) Assert.Fail("Service scope not initialized");
        var sp = scope!.ServiceProvider;

        // Namespace of the interfaces folder
        var ns = typeof(IBalanceService).Namespace!; // "BlockChainTools.Interfaces"
        var assembly = typeof(IBalanceService).Assembly;

        // Exclusions if you add marker/non-DI interfaces in future
        var excluded = new HashSet<string>(StringComparer.Ordinal)
        {
            // e.g. "BlockChainTools.Interfaces.ISomeMarkerInterface"
        };

        var interfaceTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsInterface &&
                t.Namespace != null &&
                (t.Namespace == ns || t.Namespace.StartsWith(ns + ".")) &&
                !t.IsGenericTypeDefinition &&
                !excluded.Contains(t.FullName!))
            .OrderBy(t => t.Name)
            .ToArray();

        var missing = new List<string>();

        foreach (var it in interfaceTypes)
        {
            var svc = sp.GetService(it);
            if (svc is null)
            {
                missing.Add(it.FullName!);
            }
        }

        if (missing.Count > 0)
        {
            Assert.Fail("Unregistered interfaces: " + string.Join(", ", missing));
        }

        Assert.Pass($"All {interfaceTypes.Length} interfaces under '{ns}' are registered.");
    }
}
