using System;
using System.Collections.Generic;
using System.Linq;
using BlockChainTools.Interfaces;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace BlockChainTools.Services;

public class HdWalletService : IHdWalletService
{
    public Nethereum.HdWallet.Wallet GenerateWallet(string seedPassword, WordCount wordCount)
    {
        try
        {
            var mnemonic = new Mnemonic(Wordlist.English, wordCount);
            return new Nethereum.HdWallet.Wallet(mnemonic.WordList, wordCount, seedPassword: seedPassword);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot generate HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public Nethereum.HdWallet.Wallet RestoreWallet(List<string> words, string seedPassword)
    {
        try
        {
            return new Nethereum.HdWallet.Wallet(string.Join(' ', words.ToList()), seedPassword);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot restore HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public Account GetAccount(Nethereum.HdWallet.Wallet wallet, int index)
    {
        try
        {
            var account = wallet.GetAccount(index);
            return account;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot get account by index from HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public Account GetAccount(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index)
    {
        try
        {
            var account = wallet.GetAccount(index, (int)chain);
            return account;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot get account by index from HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public static Account GetAccount(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, string address)
    {
        try
        {
            var account = wallet.GetAccount(address, (int)chain);
            return account;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot get account by address from HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, int index, string rpcUrl)
    {
        try
        {
            var account = GetAccount(wallet, chain, index: index);

            return new Web3(account, rpcUrl);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot get web3 by index from HD wallet with result: {e.Message}.", e.InnerException);
        }
    }

    public Web3 GetWeb3(Nethereum.HdWallet.Wallet wallet, Nethereum.Signer.Chain chain, string address, string rpcUrl)
    {
        try
        {
            var account = GetAccount(wallet, chain, address: address);
            return new Web3(account, rpcUrl);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot get web3 by address from HD wallet with result: {e.Message}.", e.InnerException);
        }
    }
}