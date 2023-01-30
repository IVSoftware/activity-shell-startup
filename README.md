Consider using a command line argument to specify the form you want. Then:

- Run from command line: `activity-shell.exe "Cars"`
- Run from a Shortcut, where Target property is appended with "Cars"
- For debugging, set the Command Line Arguments using a Launch Profile

This won't change the `Application.Run(new MainForm())` command however, *even if the goal is to start with a different form*.

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

***
In this particular example, the `Main` snippet in Program.cs routes the command line argument to a value defined a Settings file that has been added to the application. (This could also be useful to "remember" the form visible the last time the app closed in order to start with the same view.)

[![settings][1]][1]
***
For this routing to work requires forcing a `Handle` in the CTor of the main form. Usually, the `Handle` is created when the main form becomes visible, but in this case there's no guarantee that it will ever be shown. An override to `SetVisibleCore` prevents the main form from becoming visible is a different form is specified for startup.

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();  
            _ = Handle; // A handle is needed before calling BeginInvoke, so get one.
            Extensions.Default = this;
            // Call BeginInvoke on the new handle so as not to block the CTor.
            BeginInvoke(new Action(() =>
            {
                if (!isVisibleOnStartup())
                {
                    this.SwapForm(Settings.Default.StartupForm.GetForm());
                }
            }));
        }
        private bool isVisibleOnStartup()
        {
            var startupForm = Settings.Default.StartupForm;
            if (string.IsNullOrWhiteSpace(startupForm))
            {
                return true;
            }
            return (startupForm.Contains(nameof(MainForm)));
        }
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value && isVisibleOnStartup());
        }
***
**Extensions**

The `SwapForm()` and `GetForm()` calls are extension methods that can be called by any `Form` in the app.

    static class Extensions
    {
        // Required because main form won't be in the OpenForms 
        // collection until and unless it's shown.
        public static Form Default { get; set; }
        // The names of target forms types must end with "Form".
        public static Form GetForm(this string name)
        {
            name = $"{name.Replace("Form", string.Empty)}Form";
            Form form = Application.OpenForms[name];
            if (form == null)
            {
                if (name == Default.GetType().Name)
                {
                    form = Default;
                }
                else
                {
                    var asm = Assembly.GetEntryAssembly();
                    var type = asm.GetTypes().Single(_ => _.Name.Equals(name));
                    form = (Form)Activator.CreateInstance(type);
                    form.StartPosition = FormStartPosition.Manual;
                }
            }
            return form;
        }
        public static void SwapForm(this Form oldForm, Form newForm) 
        {
            newForm.Size = oldForm.Size;
            newForm.Location = oldForm.Location;
            Settings.Default.StartupForm = newForm.GetType().Name;
            Settings.Default.Save();
            newForm.Show();
            oldForm.Hide();
        }
    }

***
[![normal start][2]][2]
***
[![shortcut start][3]][3]
***
[![launch profile][4]][4]
***
*If no command line argument is present, the app will start up with the form that was visible the last time the app closed.*

  [1]: https://i.stack.imgur.com/sxjxp.png
  [2]: https://i.stack.imgur.com/jEcKM.png
  [3]: https://i.stack.imgur.com/hnAI8.png
  [4]: https://i.stack.imgur.com/k8BVT.png