using System.CommandLine;
using System.CommandLine.Binding;

namespace IKA9ntCrypto.CLI;
public static class CommandLine
{
    public static void Init(string[] args)
    {
        RootCommand rootCommand = RegisterOptions();
        rootCommand.Invoke(args);
    }

    public static RootCommand RegisterOptions()
    {
        RootCommand rootCommand = [];
        rootCommand.SetHandler(Program.Run, new OptionsBinder(rootCommand));

        return rootCommand;
    }
}

public record Options
{
    public bool Extension { get; set; }
    public int KeySize { get; set; }
    public DeriveType Derive { get; set; }
    public DirectoryInfo? Input { get; set; }
    public DirectoryInfo? Output { get; set; }
    public string? Password { get; set; }
}

public class OptionsBinder : BinderBase<Options>
{
    private readonly Option<bool> _extension;
    private readonly Option<int> _keySize;
    private readonly Option<DeriveType> _derive;
    private readonly Argument<DirectoryInfo> _input;
    private readonly Argument<DirectoryInfo> _output;
    private readonly Argument<string> _password;

    public OptionsBinder(RootCommand rootCommand)
    {
        rootCommand.Add(_extension = new("--ext", "Specify if extesnion should be used while generating salt."));
        rootCommand.Add(_keySize = new("--key_size", "Specify key size."));
        rootCommand.Add(_derive = new("--derive", "Specify derive type."));
        rootCommand.Add(_input = new("input", "Path to input directory."));
        rootCommand.Add(_output = new("output", "Path to output directory."));
        rootCommand.Add(_password = new("password", "password used for derive."));

        _keySize.FromAmong("128", "256");

        _extension.SetDefaultValue(false);
        _keySize.SetDefaultValue(256);
        _derive.SetDefaultValue(DeriveType.PBKDF2);

        _input.ExistingOnly();
        _output.LegalFilePathsOnly();
    }

    protected override Options GetBoundValue(BindingContext bindingContext) => new()
    {
        Extension = bindingContext.ParseResult.GetValueForOption(_extension),
        KeySize = bindingContext.ParseResult.GetValueForOption(_keySize),
        Derive = bindingContext.ParseResult.GetValueForOption(_derive),
        Input = bindingContext.ParseResult.GetValueForArgument(_input),
        Output = bindingContext.ParseResult.GetValueForArgument(_output),
        Password = bindingContext.ParseResult.GetValueForArgument(_password),
    };
}