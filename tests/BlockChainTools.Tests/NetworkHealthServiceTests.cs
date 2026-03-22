using System.Numerics;
using BlockChainTools.DataTransferObjects;
using BlockChainTools.Interfaces;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class NetworkHealthServiceTests
{
    private INetworkHealthService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<INetworkHealthService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task CheckNetworkHealthAsync_Healthy_ReturnsHealthy()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = true,
            LatestBlockNumber = new BigInteger(50000),
            LatestBlockTimestamp = DateTime.UtcNow,
            IsBlockTimestampFresh = true,
            GasPrice = new BigInteger(30000000000),
            IsGasPriceAvailable = true,
            ChainId = new BigInteger(137),
            ChainIdMatches = true,
            Issues = []
        };
        _service!.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120);
        Assert.That(result.IsHealthy, Is.True);
        Assert.That(result.IsBlockTimestampFresh, Is.True);
        Assert.That(result.IsGasPriceAvailable, Is.True);
        Assert.That(result.ChainIdMatches, Is.True);
        Assert.That(result.Issues, Is.Empty);
    }

    [Test]
    public async Task CheckNetworkHealthAsync_StaleBlock_ReturnsUnhealthy()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = false,
            LatestBlockNumber = new BigInteger(50000),
            LatestBlockTimestamp = DateTime.UtcNow.AddMinutes(-10),
            IsBlockTimestampFresh = false,
            GasPrice = new BigInteger(30000000000),
            IsGasPriceAvailable = true,
            ChainId = new BigInteger(137),
            ChainIdMatches = true,
            Issues = ["Latest block timestamp is 600s old, exceeding the 120s threshold."]
        };
        _service!.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120);
        Assert.That(result.IsHealthy, Is.False);
        Assert.That(result.IsBlockTimestampFresh, Is.False);
        Assert.That(result.Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CheckNetworkHealthAsync_ChainIdMismatch_ReturnsUnhealthy()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = false,
            LatestBlockNumber = new BigInteger(50000),
            LatestBlockTimestamp = DateTime.UtcNow,
            IsBlockTimestampFresh = true,
            GasPrice = new BigInteger(30000000000),
            IsGasPriceAvailable = true,
            ChainId = new BigInteger(1),
            ChainIdMatches = false,
            Issues = ["Chain ID mismatch: expected 137, got 1."]
        };
        _service!.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120);
        Assert.That(result.IsHealthy, Is.False);
        Assert.That(result.ChainIdMatches, Is.False);
        Assert.That(result.Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CheckNetworkHealthAsync_NoGasPrice_ReturnsUnhealthy()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = false,
            LatestBlockNumber = new BigInteger(50000),
            LatestBlockTimestamp = DateTime.UtcNow,
            IsBlockTimestampFresh = true,
            GasPrice = null,
            IsGasPriceAvailable = false,
            ChainId = new BigInteger(137),
            ChainIdMatches = true,
            Issues = ["Gas price is unavailable or zero."]
        };
        _service!.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, Chain.Polygon, 120);
        Assert.That(result.Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CheckNetworkHealthAsync_WithoutExpectedChainId_SkipsChainIdCheck()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = true,
            LatestBlockNumber = new BigInteger(50000),
            LatestBlockTimestamp = DateTime.UtcNow,
            IsBlockTimestampFresh = true,
            GasPrice = new BigInteger(30000000000),
            IsGasPriceAvailable = true,
            ChainId = new BigInteger(137),
            ChainIdMatches = true,
            Issues = []
        };
        _service!.CheckNetworkHealthAsync(_web3!, null, 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, null, 120);
        Assert.That(result.IsHealthy, Is.True);
        Assert.That(result.ChainIdMatches, Is.True);
    }

    [Test]
    public async Task CheckNetworkHealthAsync_MultipleIssues_ReportsAll()
    {
        var expected = new NetworkHealthInfo
        {
            IsHealthy = false,
            LatestBlockNumber = null,
            LatestBlockTimestamp = null,
            IsBlockTimestampFresh = false,
            GasPrice = null,
            IsGasPriceAvailable = false,
            ChainId = null,
            ChainIdMatches = false,
            Issues = ["Unable to retrieve the latest block.", "Gas price is unavailable or zero.", "Failed to retrieve chain ID: timeout"]
        };
        _service!.CheckNetworkHealthAsync(_web3!, new BigInteger(137), 120, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.CheckNetworkHealthAsync(_web3!, new BigInteger(137), 120);
        Assert.That(result.IsHealthy, Is.False);
        Assert.That(result.Issues, Has.Count.EqualTo(3));
    }
}
