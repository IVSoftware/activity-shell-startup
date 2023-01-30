namespace activity_shell
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var args = Environment.GetCommandLineArgs();
            if(args.Length > 1 ) 
            {
                Settings.Default.StartupForm = args[1];
                Settings.Default.Save();
            }
            Application.Run(new MainForm());
        }
    }
}