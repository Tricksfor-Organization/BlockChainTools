using System.Threading;
using System.Threading.Tasks;
using BlockChainTools.Interfaces;
using Nethereum.Web3;

namespace BlockChainTools.Services;

public class TransactionService : ITransactionService
{
    public async Task<Nethereum.RPC.Eth.DTOs.Transaction> GetTransactionByHashAsync(Web3 web3, string hash, CancellationToken cancellationToken)
    {
        return await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(hash);
    }
}