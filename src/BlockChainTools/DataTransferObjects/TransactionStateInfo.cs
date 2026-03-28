using System.Numerics;
using Nethereum.RPC.Eth.DTOs;

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
    /// The full transaction receipt when a receipt exists (ConfirmedSuccess or ConfirmedReverted states); otherwise null.
    /// Contains canonical on-chain data such as gas used, logs, contract address, and effective gas price.
    /// </summary>
    public TransactionReceipt? Receipt { get; init; }

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
    /// The nonce is not included in the transaction receipt and is sourced from the transaction object.
    /// </summary>
    public BigInteger? Nonce { get; init; }

    /// <summary>
    /// The confirmed on-chain nonce for the sender address at the time the replacement was detected.
    /// Only populated when <see cref="State"/> is <see cref="TransactionState.Replaced"/>;
    /// null for all other states.
    /// </summary>
    public BigInteger? ConfirmedNonce { get; init; }

    /// <summary>
    /// The amount of native currency (in wei) sent with the transaction, if the transaction data was available.
    /// This property is not included in the transaction receipt and is sourced from the transaction object.
    /// </summary>
    public BigInteger? Value { get; init; }

    /// <summary>
    /// The gas limit provided by the sender, if the transaction data was available.
    /// This property is not included in the transaction receipt and is sourced from the transaction object.
    /// </summary>
    public BigInteger? Gas { get; init; }
}
