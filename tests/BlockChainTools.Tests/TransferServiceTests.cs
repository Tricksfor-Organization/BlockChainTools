using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class TransferServiceTests
{
    private ITransferService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<ITransferService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task TransferAsync_MainToken_ReturnsTxHash()
    {
        var expected = "0xtxhash";
        var request = Substitute.For<Nethereum.RPC.Eth.DTOs.TransactionInput>();
        _service!.TransferAsync(_web3!, request).Returns(expected);
        var result = await _service.TransferAsync(_web3, request);
        Assert.AreEqual(expected, result);
    }
}
