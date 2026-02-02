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
    [Fact]
    public void TryExecute_PassesRemainingArgumentsToAction()
    {
        string[] receivedArgs = null!;

        var commands = new Dictionary<string, Action<string[]>>
        {
            { "cmd", args => receivedArgs = args }
        };

        ProjetRSA.CommandInput.CommandInput.TryExecute(
            new[] { "cmd", "a", "b" },
            commands
        );

        Assert.NotNull(receivedArgs);
        Assert.Equal(new[] { "a", "b" }, receivedArgs);
    }

    [Fact]
    public void TryExecute_IsCaseInsensitive()
    {
        bool executed = false;

        var commands = new Dictionary<string, Action<string[]>>(
            StringComparer.OrdinalIgnoreCase
        )
    {
        { "CaseCommand", _ => executed = true }
    };

    bool result = ProjetRSA.CommandInput.CommandInput.TryExecute(
        new[] { "casecommand" },
        commands
    );

    Assert.True(result);
    Assert.True(executed);
    }
}
