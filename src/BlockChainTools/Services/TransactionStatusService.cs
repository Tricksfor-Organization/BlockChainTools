using BlockChainTools.DataTransferObjects;
using BlockChainTools.Interfaces;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace BlockChainTools.Services;

public class TransactionStatusService : ITransactionStatusService
{
    public async Task<TransactionStateInfo> GetTransactionStateAsync(Web3 web3, string transactionHash, CancellationToken cancellationToken = default)
    {
        // 1. Try to get the receipt — this is the canonical success/failure signal
        TransactionReceipt? receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

        if (receipt is not null)
        {
            var succeeded = receipt.Status?.Value == 1;

            // Fetch the transaction to populate fields not included in the receipt (Nonce, Value, Gas)
            var confirmedTx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

            return new TransactionStateInfo
            {
                State = succeeded ? TransactionState.ConfirmedSuccess : TransactionState.ConfirmedReverted,
                TransactionHash = transactionHash,
                Receipt = receipt,
                ReceiptStatus = receipt.Status?.Value,
                BlockNumber = receipt.BlockNumber?.Value,
                Nonce = confirmedTx?.Nonce?.Value,
                Value = confirmedTx?.Value?.Value,
                Gas = confirmedTx?.Gas?.Value
            };
        }

        // 2. No receipt — check if the node still knows about the transaction
        var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

        if (transaction is not null)
        {
            // Transaction exists in the node but is not yet mined — check for replacement
            var senderAddress = transaction.From;
            var txNonce = transaction.Nonce?.Value;

            if (senderAddress is not null && txNonce is not null)
            {
                // Get the confirmed (mined) nonce for the sender
                var confirmedNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(
                    senderAddress, BlockParameter.CreateLatest());

                if (confirmedNonce?.Value > txNonce)
                {
                    // On-chain nonce has advanced past this tx's nonce — it was replaced
                    return new TransactionStateInfo
                    {
                        State = TransactionState.Replaced,
                        TransactionHash = transactionHash,
                        Nonce = txNonce,
                        Value = transaction.Value?.Value,
                        Gas = transaction.Gas?.Value,
                        ConfirmedNonce = confirmedNonce.Value
                    };
                }
            }

            return new TransactionStateInfo
            {
                State = TransactionState.Pending,
                TransactionHash = transactionHash,
                Nonce = txNonce,
                Value = transaction.Value?.Value,
                Gas = transaction.Gas?.Value
            };
        }

        // 3. Node does not know the transaction and no receipt — likely dropped
        return new TransactionStateInfo
        {
            State = TransactionState.StalePending,
            TransactionHash = transactionHash
        };
    }
}
