using Nethereum.Web3;
using System.Numerics;

namespace BlockChainTools.Interfaces;

public interface IBalanceService
{
    /// <summary>
    /// Get main token balance of a chain (like matic in polygon)
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="address"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> GetBalanceOfAsync(Web3 web3, string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get balance of an ERC721 contract address like NFT
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="erc721ContractAddress"></param>
    /// <param name="address"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> GetErc721BalanceAsync(Web3 web3, string erc721ContractAddress, string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get balance of an ERC20 contract address like USDC
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="erc20ContractAddress"></param>
    /// <param name="address"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> GetErc20BalanceAsync(Web3 web3, string erc20ContractAddress, string address, CancellationToken cancellationToken = default);
}