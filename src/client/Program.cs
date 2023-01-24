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

        // TODO ...this won't create TestTables but Menu
        Application.Run(new TestTables());
    }
}