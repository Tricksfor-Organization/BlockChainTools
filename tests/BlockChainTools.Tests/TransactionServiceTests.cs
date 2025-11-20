using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class TransactionServiceTests
{
    private ITransactionService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<ITransactionService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task GetTransactionByHashAsync_ReturnsTransaction()
    {
        var expected = Substitute.For<Nethereum.RPC.Eth.DTOs.Transaction>();
        _service!.GetTransactionByHashAsync(_web3!, "0xhash", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionByHashAsync(_web3!, "0xhash", CancellationToken.None);
        Assert.That(expected, Is.EqualTo(result));
    }
}
