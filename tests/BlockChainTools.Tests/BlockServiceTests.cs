using System.Numerics;
using BlockChainTools.Interfaces;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using NUnit.Framework;
using NSubstitute;

namespace BlockChainTools.Tests;

[TestFixture]
public class BlockServiceTests
{
    private IBlockService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IBlockService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task GetLastBlockAsync_ReturnsExpectedBlockNumber()
    {
        var expected = new BigInteger(999);
        _service!.GetLastBlockAsync(_web3!, Arg.Any<CancellationToken>()).Returns(expected);
        var result = await _service.GetLastBlockAsync(_web3, CancellationToken.None);
        Assert.AreEqual(expected, result);
    }
}
