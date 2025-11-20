using System.Numerics;
using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class BalanceServiceTests
{
    private IBalanceService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IBalanceService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task GetBalanceOfAsync_ReturnsExpectedBalance()
    {
        var expected = new BigInteger(12345);
        _service!.GetBalanceOfAsync(_web3!, "0xabc", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetBalanceOfAsync(_web3!, "0xabc");
        Assert.That(expected, Is.EqualTo(result));
    }

    [Test]
    public async Task GetErc721BalanceAsync_ReturnsExpectedBalance()
    {
        var expected = new BigInteger(5);
        _service!.GetErc721BalanceAsync(_web3!, "0xerc721", "0xabc", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetErc721BalanceAsync(_web3!, "0xerc721", "0xabc");
        Assert.That(expected, Is.EqualTo(result));
    }

    [Test]
    public async Task GetErc20BalanceAsync_ReturnsExpectedBalance()
    {
        var expected = new BigInteger(1000);
        _service!.GetErc20BalanceAsync(_web3!, "0xerc20", "0xabc", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetErc20BalanceAsync(_web3!, "0xerc20", "0xabc");
        Assert.That(expected, Is.EqualTo(result));
    }
}
