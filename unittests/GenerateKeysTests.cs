using Xunit;
using ProjetRSA.KeyOperations;

namespace ProjetRSA.Tests;

public class GenerateKeysTests
{
    [Fact]
    public void Execute_WithSize2048_ShouldGenerateKeys()
    {
        // Arrange
        
        
        // Act
        
        
        // Assert
        
    }

    [Fact]
    public void Execute_WithSize3072_ShouldGenerateKeys()
    {
        // Arrange
        
        
        // Act
        
        
        // Assert
        
    }

    [Fact]
    public void Execute_WithSize4096_ShouldGenerateKeys()
    {
        // Arrange
        
        
        // Act
        
        
        // Assert
        
    }

    [Fact]
    public void Execute_WithMatchingPasswords_ShouldEncryptPrivateKey()
    {
        // Arrange
        
        
        // Act
        
        
        // Assert
        
    }

    [Fact]
    public void Execute_WithNonMatchingPasswords_ShouldPromptAgain()
    {
        // Arrange
        
        
        // Act
        
        
        // Assert
        
    }
}
