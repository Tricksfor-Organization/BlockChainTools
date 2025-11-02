# BlockChainTools

## Overview
BlockChainTools is a .NET application designed to provide various tools and utilities for working with blockchain technology. This project serves as a foundation for building and testing blockchain-related functionalities.

## Project Structure
The solution consists of the following projects:

- **BlockChainTools**: The main application project containing the core logic and functionalities.
- **BlockChainTools.Tests**: The test project that includes unit tests to ensure the reliability and correctness of the application.

## Getting Started

### Prerequisites
- .NET SDK (version 9.0 or later)
- A code editor (e.g., Visual Studio Code)

### Installation
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd BlockChainTools
   ```
3. Restore the dependencies:
   ```
   dotnet restore
   ```

### Running the Application
To run the BlockChainTools application, use the following command:
```
dotnet run --project src/BlockChainTools/BlockChainTools.csproj
```

### Running Tests
To execute the unit tests, run the following command:
```
dotnet test tests/BlockChainTools.Tests/BlockChainTools.Tests.csproj
```

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.