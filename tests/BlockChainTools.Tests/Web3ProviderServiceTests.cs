using BlockChainTools.Interfaces;
using Nethereum.Signer;
using Nethereum.Web3;
using NUnit.Framework;
using NSubstitute;
using Nethereum.JsonRpc.Client;

namespace BlockChainTools.Tests;

[TestFixture]
public class Web3ProviderServiceTests
{
    private IWeb3ProviderService? _service;
    private IClient? _mockClient;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _service = Substitute.For<IWeb3ProviderService>();
        _mockClient = Substitute.For<IClient>();
    }

    [Test]
    public void CreateWeb3_WithRpcUrl_ReturnsWeb3()
    {
        var expected = new Web3(_mockClient);
        _service!.CreateWeb3("http://rpc").Returns(expected);
        var result = _service.CreateWeb3("http://rpc");
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void CreateWeb3_WithPrivateKey_ReturnsWeb3()
    {
        var expected = new Web3(_mockClient);
        _service!.CreateWeb3("privkey", Chain.MainNet, "http://rpc").Returns(expected);
        var result = _service.CreateWeb3("privkey", Chain.MainNet, "http://rpc");
        Assert.AreEqual(expected, result);
    }
}
