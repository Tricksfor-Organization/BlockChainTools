using BlockChainTools.DataTransferObjects;
using Nethereum.Web3;

namespace BlockChainTools.Interfaces;

public interface ITransactionStatusService
{
    /// <summary>
    /// Determines the current lifecycle state of a transaction by checking its receipt and mempool presence.
    /// Uses receipt status as the canonical success/failure signal when a receipt exists.
    /// When no receipt is found, checks whether the transaction is still known to the node
    /// and whether the sender's on-chain nonce has advanced past the transaction nonce (indicating replacement).
    /// </summary>
    /// <param name="web3">The Web3 instance to query against.</param>
    /// <param name="transactionHash">The hash of the transaction to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="TransactionStateInfo"/> describing the current state and metadata of the transaction.</returns>
    Task<TransactionStateInfo> GetTransactionStateAsync(Web3 web3, string transactionHash, CancellationToken cancellationToken = default);
}
