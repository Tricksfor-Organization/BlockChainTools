# BlockChainTools.Tests

Unit test suite for the BlockChainTools library. This project contains comprehensive tests for all blockchain services using NUnit and NSubstitute.

## Test Framework

- **NUnit** - Testing framework
- **NSubstitute** - Mocking library (no Moq)
- **DotNet.Testcontainers** - Integration testing with Redis containers
- **Aspire.StackExchange.Redis.DistributedCaching** - Redis client for distributed nonce tests

## Test Structure

The test suite is organized by service, with each service having its own test file:

```
BlockChainTools.Tests/
├── BalanceServiceTests.cs         # Tests for IBalanceService
├── BlockServiceTests.cs           # Tests for IBlockService
├── GasServiceTests.cs             # Tests for IGasService
├── HdWalletServiceTests.cs        # Tests for IHdWalletService
├── TransactionServiceTests.cs     # Tests for ITransactionService
├── TransferServiceTests.cs        # Tests for ITransferService
├── Web3ProviderServiceTests.cs    # Tests for IWeb3ProviderService
└── BlockChainToolsTests.cs        # Integration tests with Redis/DI
```

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests from Project Directory

```bash
cd tests/BlockChainTools.Tests
dotnet test
```

### Run Specific Test File

```bash
dotnet test --filter "ClassName~BalanceServiceTests"
```

### Run with Detailed Output

```bash
dotnet test --verbosity detailed
```

## Test Categories

### Unit Tests

Most test files contain isolated unit tests that mock dependencies using NSubstitute:

```csharp
[Test]
public async Task GetBalanceOfAsync_ReturnsExpectedBalance()
{
    var expected = new BigInteger(12345);
    _service!.GetBalanceOfAsync(_web3!, "0xabc", Arg.Any<CancellationToken>())
        .Returns(expected);
    
    var result = await _service.GetBalanceOfAsync(_web3!, "0xabc");
    
    Assert.AreEqual(expected, result);
}
```

### Integration Tests

`BlockChainToolsTests.cs` contains integration tests that:
- Spin up a Redis container using Testcontainers
- Configure full DI container with all dependencies
- Test distributed nonce management across multiple instances
- Verify service registration and wiring

## Key Testing Patterns

### 1. Mocking Interfaces with NSubstitute

```csharp
var service = Substitute.For<IBalanceService>();
service.GetBalanceOfAsync(Arg.Any<Web3>(), "0xAddress", Arg.Any<CancellationToken>())
    .Returns(BigInteger.Parse("1000000"));
```

### 2. Mocking Nethereum Components

```csharp
var client = Substitute.For<IClient>();
var web3 = new Web3(client);
```

### 3. Using Testcontainers for Redis

```csharp
_redisContainer = new TestcontainersBuilder<TestcontainersContainer>()
    .WithImage("redis:latest")
    .WithPortBinding(6379, true)
    .Build();
    
await _redisContainer.StartAsync();
```

## Prerequisites

### Development

- .NET 9 SDK
- Docker (for integration tests using Testcontainers)

### CI/CD

The tests are designed to run in CI environments. Ensure Docker is available for container-based integration tests.

## Testing Guidelines

When adding new tests to this project:

1. **Use NSubstitute** for mocking (not Moq)
2. **Follow Arrange/Act/Assert** pattern
3. **Mock external dependencies** (Web3, HttpClient, etc.)
4. **Avoid real network calls** in unit tests
5. **Use descriptive test names** that explain what is being tested
6. **Test both success and failure scenarios**
7. **Use CancellationToken** in async tests where appropriate

## Example Test Pattern

```csharp
[TestFixture]
public class MyServiceTests
{
    private IMyService? _service;
    private Web3? _web3;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IMyService>();
        _web3 = new Web3(Substitute.For<IClient>());
    }

    [Test]
    public async Task MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var expected = "expected result";
        _service!.MethodAsync(_web3!, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _service.MethodAsync(_web3!);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
```

## Continuous Integration

These tests are designed to run in automated CI/CD pipelines. The integration tests using Testcontainers will automatically:
- Pull required Docker images
- Start containers
- Run tests
- Clean up containers after completion

## Coverage

Run tests with coverage reporting:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Contributing

When contributing tests:
1. Ensure all tests pass locally before submitting
2. Add tests for any new functionality
3. Follow existing naming and organization patterns
4. Update this README if adding new test categories or patterns

## License

Same as the main project - MIT License.
