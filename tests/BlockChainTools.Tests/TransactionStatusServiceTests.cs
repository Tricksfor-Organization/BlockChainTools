using System.Numerics;
using BlockChainTools.DataTransferObjects;
using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class TransactionStatusServiceTests
{
    private ITransactionStatusService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<ITransactionStatusService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedSuccess_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.ConfirmedSuccess,
            TransactionHash = "0xabc",
            ReceiptStatus = BigInteger.One,
            BlockNumber = new BigInteger(100)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xabc", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xabc");
        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedSuccess));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.One));
        Assert.That(result.BlockNumber, Is.EqualTo(new BigInteger(100)));
        Assert.That(result.TransactionHash, Is.EqualTo("0xabc"));
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedReverted_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.ConfirmedReverted,
            TransactionHash = "0xdef",
            ReceiptStatus = BigInteger.Zero,
            BlockNumber = new BigInteger(200)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xdef", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xdef");
        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedReverted));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.Zero));
    }

    [Test]
    public async Task GetTransactionStateAsync_Pending_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.Pending,
            TransactionHash = "0xpending",
            Nonce = new BigInteger(5)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xpending", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xpending");
        Assert.That(result.State, Is.EqualTo(TransactionState.Pending));
        Assert.That(result.ReceiptStatus, Is.Null);
        Assert.That(result.BlockNumber, Is.Null);
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(5)));
    }

    [Test]
    public async Task GetTransactionStateAsync_Replaced_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.Replaced,
            TransactionHash = "0xreplaced",
            Nonce = new BigInteger(3),
            ConfirmedNonce = new BigInteger(5)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xreplaced", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xreplaced");
        Assert.That(result.State, Is.EqualTo(TransactionState.Replaced));
        Assert.That(result.Nonce, Is.Not.Null);
        Assert.That(result.ConfirmedNonce, Is.Not.Null);
        Assert.That(result.ConfirmedNonce!.Value, Is.GreaterThan(result.Nonce!.Value));
    }

    [Test]
    public async Task GetTransactionStateAsync_StalePending_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.StalePending,
            TransactionHash = "0xstale"
        };
        _service!.GetTransactionStateAsync(_web3!, "0xstale", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xstale");
        Assert.That(result.State, Is.EqualTo(TransactionState.StalePending));
        Assert.That(result.ReceiptStatus, Is.Null);
        Assert.That(result.BlockNumber, Is.Null);
        Assert.That(result.Nonce, Is.Null);
        Assert.That(result.ConfirmedNonce, Is.Null);
    }

    [Test]
    public async Task GetTransactionStateAsync_Broadcasted_ReturnsCorrectState()
    {
        var expected = new TransactionStateInfo
        {
            State = TransactionState.Broadcasted,
            TransactionHash = "0xbroadcast"
        };
        _service!.GetTransactionStateAsync(_web3!, "0xbroadcast", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xbroadcast");
        Assert.That(result.State, Is.EqualTo(TransactionState.Broadcasted));
    }
}
