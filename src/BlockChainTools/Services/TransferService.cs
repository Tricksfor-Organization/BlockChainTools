using System.Threading.Tasks;
using BlockChainTools.Interfaces;
using Nethereum.Web3;

namespace BlockChainTools.Services;

public class TransferService : ITransferService
{
    /// <summary>
    /// Transfer main token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> TransferAsync(Web3 web3, Nethereum.RPC.Eth.DTOs.TransactionInput request)
    {
        return await web3.Eth.TransactionManager.SendTransactionAsync(request);
    }

    /// <summary>
    /// Transfer ERC20 token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> TransferAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction request)
    {
        var handler = web3.Eth.ERC20.GetContractService(contractAddress);

        return await handler.TransferRequestAsync(request);
    }

    /// <summary>
    /// Transfer ERC721 token
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<string> TransferAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC721.ContractDefinition.TransferFromFunction request)
    {
        var handler = web3.Eth.ERC721.GetContractService(contractAddress);

        return await handler.TransferFromRequestAsync(request);
    }

    /// <summary>
    /// Transfer main token and wait for receipt
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, Nethereum.RPC.Eth.DTOs.TransactionInput request)
    {
        return await web3.Eth.TransactionManager.SendTransactionAndWaitForReceiptAsync(request);
    }

    /// <summary>
    /// Transfer ERC20 token and wait for receipt
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction request)
    {
        var handler = web3.Eth.ERC20.GetContractService(contractAddress);

        return await handler.TransferRequestAndWaitForReceiptAsync(request);
    }

    /// <summary>
    /// Transfer ERC20 token and wait for receipt
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="contractAddress"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> TransferAndWaitForReceiptAsync(Web3 web3, string contractAddress,
        Nethereum.Contracts.Standards.ERC721.ContractDefinition.TransferFromFunction request)
    {
        var handler = web3.Eth.ERC721.GetContractService(contractAddress);

        return await handler.TransferFromRequestAndWaitForReceiptAsync(request);
    }
}