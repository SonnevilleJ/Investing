using CommandLine;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class DemoOptions
    {
        [Option(shortName: 'u', longName: "username", Required = false, HelpText = "The username to use when logging into Fidelity.")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "The password to use when logging into Fidelity.")]
        public string Password { get; set; }

        [Option('s', "save", Required = false, HelpText = "Indicates options should be persisted.")]
        public bool SaveOptions { get; set; }
    }
}
