using OkoClient.Forms;

namespace OkoClient;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new TestTables());
    }
}