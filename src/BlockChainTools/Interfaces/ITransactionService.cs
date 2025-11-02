using Nethereum.Web3;
using System.Threading;
using System.Threading.Tasks;

namespace BlockChainTools.Interfaces;

public interface ITransactionService
{
    /// <summary>
    /// Get transaction by hash
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Nethereum.RPC.Eth.DTOs.Transaction> GetTransactionByHashAsync(Web3 web3, string hash, CancellationToken cancellationToken);
}