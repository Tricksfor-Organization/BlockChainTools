using System.Numerics;

namespace BlockChainTools.DataTransferObjects;

/// <summary>
/// Contains the resolved state of a transaction along with supporting metadata.
/// </summary>
public class TransactionStateInfo
{
    /// <summary>
    /// The resolved lifecycle state of the transaction.
    /// </summary>
    public required TransactionState State { get; init; }

    /// <summary>
    /// The transaction hash that was queried.
    /// </summary>
    public required string TransactionHash { get; init; }

    /// <summary>
    /// The receipt status value (1 = success, 0 = revert) when a receipt exists; otherwise null.
    /// </summary>
    public BigInteger? ReceiptStatus { get; init; }

    /// <summary>
    /// The block number in which the transaction was mined, if available.
    /// </summary>
    public BigInteger? BlockNumber { get; init; }

    /// <summary>
    /// The nonce of the transaction, if the transaction data was available from the node.
    /// </summary>
    public BigInteger? Nonce { get; init; }

    /// <summary>
    /// The confirmed on-chain nonce for the sender address, if checked.
    /// </summary>
    public BigInteger? ConfirmedNonce { get; init; }
}
