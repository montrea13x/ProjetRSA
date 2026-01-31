using System;
using System.Collections.Generic;
using Xunit;

namespace unittests;

public class CommandInputTests
{
    [Fact]
    public void TryExecute_ReturnsFalse_WhenNoArguments()
    {
        var commands = new Dictionary<string, Action<string[]>>();

        bool result = ProjetRSA.CommandInput.CommandInput.TryExecute(
            Array.Empty<string>(),
            commands
        );

        Assert.False(result);
    }

    [Fact]
    public void TryExecute_ReturnsFalse_WhenInvalidCommand()
    {
        var commands = new Dictionary<string, Action<string[]>>
        {
            { "validcommand", _ => { } }
        };

        bool result = ProjetRSA.CommandInput.CommandInput.TryExecute(
            new[] { "invalidcommand" },
            commands
        );

        Assert.False(result);
    }

    [Fact]
    public void TryExecute_ExecutesActionAndReturnsTrue_WhenValidCommand()
    {
        bool actionExecuted = false;

        var commands = new Dictionary<string, Action<string[]>>
        {
            { "testcommand", _ => { actionExecuted = true; } }
        };

        bool result = ProjetRSA.CommandInput.CommandInput.TryExecute(
            new[] { "testcommand" },
            commands
        );

        Assert.True(result);
        Assert.True(actionExecuted);
    }
}
