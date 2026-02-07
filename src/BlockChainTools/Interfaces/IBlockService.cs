using System.Numerics;
using Nethereum.Web3;

namespace BlockChainTools.Interfaces;

public interface IBlockService
{
    /// <summary>
    /// Get the latest block number from the blockchain.
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> GetLastBlockAsync(Web3 web3, CancellationToken cancellationToken);
}