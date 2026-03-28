using System.Numerics;
using BlockChainTools.DataTransferObjects;
using BlockChainTools.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using NSubstitute;
using NUnit.Framework;

namespace BlockChainTools.Tests;

[TestFixture]
public class TransactionStatusServiceTests
{
    private TransactionStatusService _service = null!;

    [SetUp]
    public void SetUp() => _service = new TransactionStatusService();

    /// <summary>
    /// Builds a <see cref="Web3"/> whose underlying <see cref="IClient"/> is stubbed so that
    /// each Ethereum JSON-RPC method returns the supplied value (or <c>null</c> when omitted).
    /// </summary>
    private static Web3 BuildWeb3(
        TransactionReceipt? receipt = null,
        Transaction? tx = null,
        HexBigInteger? transactionCount = null)
    {
        var client = Substitute.For<IClient>();

        client.SendRequestAsync<TransactionReceipt>(
                Arg.Is<RpcRequest>(r => r.Method == "eth_getTransactionReceipt"),
                Arg.Any<string>())
            .Returns(receipt!);

        client.SendRequestAsync<Transaction>(
                Arg.Is<RpcRequest>(r => r.Method == "eth_getTransactionByHash"),
                Arg.Any<string>())
            .Returns(tx!);

        client.SendRequestAsync<HexBigInteger>(
                Arg.Is<RpcRequest>(r => r.Method == "eth_getTransactionCount"),
                Arg.Any<string>())
            .Returns(transactionCount!);

        return new Web3(client);
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedSuccess_ReturnsCorrectState()
    {
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(1),
            BlockNumber = new HexBigInteger(100)
        };
        var tx = new Transaction
        {
            Nonce = new HexBigInteger(7),
            Value = new HexBigInteger(1_000_000_000_000_000_000L),
            Gas = new HexBigInteger(21000)
        };
        var web3 = BuildWeb3(receipt: receipt, tx: tx);

        var result = await _service.GetTransactionStateAsync(web3, "0xabc");

        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedSuccess));
        Assert.That(result.TransactionHash, Is.EqualTo("0xabc"));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.Receipt!.Status?.Value, Is.EqualTo(BigInteger.One));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.One));
        Assert.That(result.BlockNumber, Is.EqualTo(new BigInteger(100)));
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(7)));
        Assert.That(result.Value, Is.EqualTo(new BigInteger(1_000_000_000_000_000_000L)));
        Assert.That(result.Gas, Is.EqualTo(new BigInteger(21000)));
        Assert.That(result.ConfirmedNonce, Is.Null);
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedReverted_ReturnsCorrectState()
    {
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(0),
            BlockNumber = new HexBigInteger(200)
        };
        var tx = new Transaction
        {
            Nonce = new HexBigInteger(3),
            Value = new HexBigInteger(0),
            Gas = new HexBigInteger(21000)
        };
        var web3 = BuildWeb3(receipt: receipt, tx: tx);

        var result = await _service.GetTransactionStateAsync(web3, "0xdef");

        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedReverted));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.Receipt!.Status?.Value, Is.EqualTo(BigInteger.Zero));
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.Zero));
        Assert.That(result.BlockNumber, Is.EqualTo(new BigInteger(200)));
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(3)));
        Assert.That(result.Value, Is.EqualTo(BigInteger.Zero));
        Assert.That(result.Gas, Is.EqualTo(new BigInteger(21000)));
        Assert.That(result.ConfirmedNonce, Is.Null);
    }

    [Test]
    public async Task GetTransactionStateAsync_Pending_ReturnsCorrectState()
    {
        var tx = new Transaction
        {
            From = "0xsender",
            Nonce = new HexBigInteger(5),
            Value = new HexBigInteger(500_000_000_000_000_000L),
            Gas = new HexBigInteger(21000)
        };
        // Confirmed nonce equals tx nonce — not yet replaced, still pending
        var web3 = BuildWeb3(tx: tx, transactionCount: new HexBigInteger(5));

        var result = await _service.GetTransactionStateAsync(web3, "0xpending");

        Assert.That(result.State, Is.EqualTo(TransactionState.Pending));
        Assert.That(result.Receipt, Is.Null);
        Assert.That(result.ReceiptStatus, Is.Null);
        Assert.That(result.BlockNumber, Is.Null);
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(5)));
        Assert.That(result.Value, Is.EqualTo(new BigInteger(500_000_000_000_000_000L)));
        Assert.That(result.Gas, Is.EqualTo(new BigInteger(21000)));
        Assert.That(result.ConfirmedNonce, Is.Null);
    }

    [Test]
    public async Task GetTransactionStateAsync_Replaced_ReturnsCorrectState()
    {
        var tx = new Transaction
        {
            From = "0xsender",
            Nonce = new HexBigInteger(3),
            Value = new HexBigInteger(250_000_000_000_000_000L),
            Gas = new HexBigInteger(21000)
        };
        // Confirmed nonce (5) > tx nonce (3) — the tx was replaced by another
        var web3 = BuildWeb3(tx: tx, transactionCount: new HexBigInteger(5));

        var result = await _service.GetTransactionStateAsync(web3, "0xreplaced");

        Assert.That(result.State, Is.EqualTo(TransactionState.Replaced));
        Assert.That(result.Nonce, Is.EqualTo(new BigInteger(3)));
        Assert.That(result.ConfirmedNonce, Is.EqualTo(new BigInteger(5)));
        Assert.That(result.ConfirmedNonce!.Value, Is.GreaterThan(result.Nonce!.Value));
        Assert.That(result.Value, Is.EqualTo(new BigInteger(250_000_000_000_000_000L)));
        Assert.That(result.Gas, Is.EqualTo(new BigInteger(21000)));
    }

    [Test]
    public async Task GetTransactionStateAsync_StalePending_ReturnsCorrectState()
    {
        // Node returns null for both receipt and transaction — likely dropped from mempool
        var web3 = BuildWeb3();

        var result = await _service.GetTransactionStateAsync(web3, "0xstale");

        Assert.That(result.State, Is.EqualTo(TransactionState.StalePending));
        Assert.That(result.TransactionHash, Is.EqualTo("0xstale"));
        Assert.That(result.Receipt, Is.Null);
        Assert.That(result.ReceiptStatus, Is.Null);
        Assert.That(result.BlockNumber, Is.Null);
        Assert.That(result.Nonce, Is.Null);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Gas, Is.Null);
        Assert.That(result.ConfirmedNonce, Is.Null);
    }

    [Test]
    public async Task GetTransactionStateAsync_ConfirmedSuccess_TransactionDataUnavailable_NullableFieldsAreNull()
    {
        // Receipt found but the node has pruned the transaction object.
        // Nonce, Value, and Gas fall back to null gracefully; Receipt and ReceiptStatus are still populated.
        var receipt = new TransactionReceipt
        {
            Status = new HexBigInteger(1),
            BlockNumber = new HexBigInteger(50)
        };
        var web3 = BuildWeb3(receipt: receipt); // tx = null (pruned)

        var result = await _service.GetTransactionStateAsync(web3, "0xpruned");

        Assert.That(result.State, Is.EqualTo(TransactionState.ConfirmedSuccess));
        Assert.That(result.Receipt, Is.Not.Null);
        Assert.That(result.ReceiptStatus, Is.EqualTo(BigInteger.One));
        Assert.That(result.BlockNumber, Is.EqualTo(new BigInteger(50)));
        Assert.That(result.Nonce, Is.Null);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Gas, Is.Null);
    }

    [Test]
    public void GetTransactionStateAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var web3 = BuildWeb3();

        Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.GetTransactionStateAsync(web3, "0xany", cts.Token));
    }
}
