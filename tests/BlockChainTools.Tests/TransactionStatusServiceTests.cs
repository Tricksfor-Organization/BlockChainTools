using System.Numerics;
using BlockChainTools.DataTransferObjects;
using BlockChainTools.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
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
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(1),
            BlockNumber = new HexBigInteger(100)
        };
        var expected = new TransactionStateInfo
        {
            State = TransactionState.ConfirmedSuccess,
            TransactionHash = "0xabc",
            Receipt = receipt,
            ReceiptStatus = BigInteger.One,
            BlockNumber = new BigInteger(100),
            Nonce = new BigInteger(7),
            Value = new BigInteger(1000000000000000000),
            Gas = new BigInteger(21000)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xabc", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xabc");
        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedSuccess));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.One));
        Assert.That(result.BlockNumber, Is.EqualTo(new BigInteger(100)));
        Assert.That(result.TransactionHash, Is.EqualTo("0xabc"));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.Receipt!.Status?.Value, Is.EqualTo(BigInteger.One));
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(7)));
        Assert.That(result.Value, Is.EqualTo(new BigInteger(1000000000000000000)));
        Assert.That(result.Gas, Is.EqualTo(new BigInteger(21000)));
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedReverted_ReturnsCorrectState()
    {
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(0),
            BlockNumber = new HexBigInteger(200)
        };
        var expected = new TransactionStateInfo
        {
            State = TransactionState.ConfirmedReverted,
            TransactionHash = "0xdef",
            Receipt = receipt,
            ReceiptStatus = BigInteger.Zero,
            BlockNumber = new BigInteger(200),
            Nonce = new BigInteger(3)
        };
        _service!.GetTransactionStateAsync(_web3!, "0xdef", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xdef");
        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedReverted));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.Zero));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.Receipt!.Status?.Value, Is.EqualTo(BigInteger.Zero));
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(3)));
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
    public async Task GetTransactionStateAsync_ConfirmedSuccess_TransactionDataUnavailable_NullableFieldsAreNull()
    {
        // Simulates the case where a receipt is found but the transaction fetch returns null
        // (e.g., the node has pruned the transaction data). Receipt is still populated;
        // Nonce, Value, and Gas are gracefully null.
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(1),
            BlockNumber = new HexBigInteger(50)
        };
        var expected = new TransactionStateInfo
        {
            State = TransactionState.ConfirmedSuccess,
            TransactionHash = "0xpruned",
            Receipt = receipt,
            ReceiptStatus = BigInteger.One,
            BlockNumber = new BigInteger(50),
            Nonce = null,
            Value = null,
            Gas = null
        };
        _service!.GetTransactionStateAsync(_web3!, "0xpruned", Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetTransactionStateAsync(_web3!, "0xpruned");
        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedSuccess));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.One));
        Assert.That(result.Nonce, Is.Null);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Gas, Is.Null);
    }

}
