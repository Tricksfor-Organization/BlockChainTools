using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace BlockChainTools.Interfaces;

public interface IHdWalletService
{
    /// <summary>
    /// Generates a new HD wallet
    /// </summary>
    /// <param name="seedPassword"></param>
    /// <param name="wordCount"></param>
    /// <returns></returns>
    Nethereum.HdWallet.Wallet GenerateWallet(string seedPassword, WordCount wordCount);

    /// <summary>
    /// Restores an HD wallet from mnemonic words
    /// </summary>
    /// <param name="words"></param>
    /// <param name="seedPassword"></param>
    /// <returns></returns>
    Nethereum.HdWallet.Wallet RestoreWallet(List<string> words, string seedPassword);

    /// <summary>
    /// Gets account by index from HD wallet
    /// </summary>
    /// <param name="wallet"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    Account GetAccount(Nethereum.HdWallet.Wallet wallet, int index);

    /// <summary>
    /// Gets account by index from HD wallet for specific chain
    /// </summary>
    /// <param name="wallet"></param>
    /// <param name="chain"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    Account GetAccount(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index);

    /// <summary>
    /// Gets Web3 instance from HD wallet by account index for specific chain
    /// </summary>
    /// <param name="wallet"></param>
    /// <param name="chain"></param>
    /// <param name="index"></param>
    /// <param name="rpcUrl"></param>
    /// <returns></returns>
    Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index, string rpcUrl);

    /// <summary>
    /// Gets Web3 instance from HD wallet by account address for specific chain
    /// </summary>
    /// <param name="wallet"></param>
    /// <param name="chain"></param>
    /// <param name="address"></param>
    /// <param name="rpcUrl"></param>
    /// <returns></returns>
    Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, string address, string rpcUrl);
}