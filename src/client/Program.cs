using OkoClient.Forms;

namespace OkoClient;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length > 0 && (args[0] == "--test" || args[0] == "-t"))
            Application.Run(new TestTables());
        else
            Application.Run(new Menu());
    }
}