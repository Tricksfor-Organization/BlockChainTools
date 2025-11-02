using System.Threading.Tasks;
using Nethereum.Web3;

namespace BlockChainTools.Interfaces;

public interface ITransferService
{
    /// <summary>
    /// Transfer main token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<string> TransferAsync(Web3 web3, Nethereum.RPC.Eth.DTOs.TransactionInput request);

    /// <summary>
    /// Transfer ERC20 token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<string> TransferAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction request);

    /// <summary>
    /// Transfer ERC721 token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<string> TransferAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC721.ContractDefinition.TransferFromFunction request);

    /// <summary>
    /// Transfer ERC721 token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, Nethereum.RPC.Eth.DTOs.TransactionInput request);

    /// <summary>
    /// Transfer ERC20 token and wait for receipt
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction request);

    /// <summary>
    /// Transfer ERC20 token and wait for receipt
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC721.ContractDefinition.TransferFromFunction request);
}
