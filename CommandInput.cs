namespace ProjetRSA.CommandInput;

public static class CommandInput
{
    public static bool TryExecute(string[] args, Dictionary<string, Action<string[]>> commands)
    {
        if (args.Length == 0)
            return false;

        string command = args[0];

        if (commands.TryGetValue(command, out var action))
        {
            action(args.Skip(1).ToArray());
            return true;
        }

        return false;
    }
}