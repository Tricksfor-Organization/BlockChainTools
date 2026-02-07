using BlockChainTools.Interfaces;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace BlockChainTools.Services;

public class HdWalletService : IHdWalletService
{
    public Nethereum.HdWallet.Wallet GenerateWallet(string seedPassword, WordCount wordCount)
    {
        var mnemonic = new Mnemonic(Wordlist.English, wordCount);
        return new Nethereum.HdWallet.Wallet(mnemonic.WordList, wordCount, seedPassword: seedPassword);
    }

    public Nethereum.HdWallet.Wallet RestoreWallet(List<string> words, string seedPassword)
    {
        return new Nethereum.HdWallet.Wallet(string.Join(' ', words), seedPassword);
    }

    public Account GetAccount(Nethereum.HdWallet.Wallet wallet, int index)
    {
        var account = wallet.GetAccount(index);
        return account;
    }

    public Account GetAccount(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index)
    {
        var account = wallet.GetAccount(index, (int)chain);
        return account;
    }

    public static Account GetAccount(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, string address)
    {
        var account = wallet.GetAccount(address, (int)chain);
        return account;
    }

    public Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index, string rpcUrl)
    {
        var account = GetAccount(wallet, chain, index: index);

        return new Web3(account, rpcUrl);
    }

    public Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, string address, string rpcUrl)
    {
        var account = GetAccount(wallet, chain, address: address);
        return new Web3(account, rpcUrl);
    }
}