using BlockChainTools.Interfaces;
using NBitcoin;
using Nethereum.HdWallet;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class HdWalletServiceTests
{
    private IHdWalletService? _service;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IHdWalletService>();
    }

    [Test]
    public void GenerateWallet_ReturnsWallet()
    {
        var expected = new Wallet(Wordlist.English, WordCount.Twelve);
        _service!.GenerateWallet("seed", WordCount.Twelve).Returns(expected);
        var result = _service.GenerateWallet("seed", WordCount.Twelve);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void RestoreWallet_ReturnsWallet()
    {
        var expected = new Wallet(Wordlist.English, WordCount.Twelve);
        var words = new List<string> { "word1", "word2" };
        _service!.RestoreWallet(words, "seed").Returns(expected);
        var result = _service.RestoreWallet(words, "seed");
        Assert.AreEqual(expected, result);
    }
}
