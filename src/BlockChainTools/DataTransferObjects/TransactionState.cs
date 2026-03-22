namespace BlockChainTools.DataTransferObjects;

/// <summary>
/// Represents the lifecycle state of an EVM transaction.
/// </summary>
public enum TransactionState
{
    /// <summary>
    /// A transaction hash was obtained but no further information is available yet.
    /// </summary>
    Broadcasted,

    /// <summary>
    /// The transaction is known to the node (returned by eth_getTransactionByHash)
    /// but has not yet been mined (no receipt).
    /// </summary>
    Pending,

    /// <summary>
    /// A receipt exists and receipt.Status == 1 — the transaction executed successfully on-chain.
    /// </summary>
    ConfirmedSuccess,

    /// <summary>
    /// A receipt exists and receipt.Status == 0 — the transaction was mined but reverted on-chain.
    /// </summary>
    ConfirmedReverted,

    /// <summary>
    /// Another transaction with the same nonce from the same sender has been mined,
    /// meaning this transaction was replaced or cancelled.
    /// </summary>
    Replaced,

    /// <summary>
    /// No receipt exists and the node no longer returns the transaction,
    /// indicating it was likely dropped from the mempool.
    /// </summary>
    StalePending
}
