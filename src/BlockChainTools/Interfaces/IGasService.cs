using Nethereum.Web3;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace BlockChainTools.Interfaces;

public interface IGasService
{
    /// <summary>
    /// Get gas price from alchemy rpc url
    /// </summary>
    /// <param name="rpcUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> GetGasPriceAsync(string rpcUrl, CancellationToken cancellationToken);

    /// <summary>
    /// Estimate gas price of transfering main token of a chain (like matic in polygon)
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="toAddress"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> EstimateTransferGasAsync(Web3 web3, string toAddress, decimal value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimate Erc721 transfer gas
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="erc721ContractAddress"></param>
    /// <param name="toAddress"></param>
    /// <param name="tokenId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> EstimateErc721TransferGasAsync(Web3 web3, string erc721ContractAddress, string toAddress, int tokenId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Get balance of an ERC20 contract address like USDC
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="erc20ContractAddress"></param>
    /// <param name = "toAddress" ></ param >
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BigInteger> EstimateErc20TransferGasAsync(Web3 web3, string erc20ContractAddress, string toAddress, BigInteger value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggest Fee To Transfer Whole Balance In Ether
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="fromAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Nethereum.RPC.Fee1559Suggestions.Fee1559> SuggestFeeToTransferWholeBalanceAsync(Web3 web3, string fromAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate Total Amount To Transfer Whole Balance In Ether
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="fromAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<decimal> CalculateTotalAmountToTransferWholeBalanceAsync(Web3 web3, string fromAddress, CancellationToken cancellationToken = default);
}