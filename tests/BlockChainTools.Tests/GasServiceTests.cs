using System.Numerics;
using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class GasServiceTests
{
    private IGasService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IGasService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task GetGasPriceAsync_ReturnsExpectedGasPrice()
    {
        var expected = new BigInteger(100);
        _service!.GetGasPriceAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetGasPriceAsync("http://rpc", CancellationToken.None);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public async Task EstimateTransferGasAsync_ReturnsExpectedGas()
    {
        var expected = new BigInteger(21000);
        _service!.EstimateTransferGasAsync(_web3!, "0xabc", 1.0m, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.EstimateTransferGasAsync(_web3, "0xabc", 1.0m);
        Assert.AreEqual(expected, result);
    }
}
