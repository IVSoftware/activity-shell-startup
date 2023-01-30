Consider implementing the use of an optional command line argument as explained below. This would provide a flexible way to specify the form you want on-the-fly. For purposes of example, suppose one of the app forms is named `CarsForm`. This could be specified as the startup form in a variety of ways such as:

- Run from a Shortcut, where Target property is appended with "Cars"
- Run from command line: `activity-shell.exe "Cars"
- For debugging, set the Command Line Arguments to "Cars" using a Launch Profile

In the absence of a command line argument, the implementation below shows the `MainForm` if this is the _first_ time the app is run _or_ the form that was visible on close the _last_ time the app was run.

***
Even if the goal is to start with a different form,
this shouldn't change the `Application.Run(new MainForm())` command. It's just that we're providing a means to startup with "some other" form.

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
For this routing to work requires forcing a `Handle` in the CTor of the main form. Usually, the `Handle` is created when the main form becomes visible, but in this case there's no guarantee that it will ever be shown. An override to `SetVisibleCore` _prevents_ the main form from becoming visible if a _different_ form is specified for startup.

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
        .
        .
        .
    }
***
**Extensions**

`SwapForm()` and `GetForm()` are extension methods. These can be called by any `Form` in the app. However, functionality requires class name to end in "Form".

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
**Typical `OnLoad` override for application Forms**

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        labelTitle.Text = GetType().Name.Replace("Form", " Form");
        labelMenu.Font = Glyphs;
        labelMenu.Text = "\uE801";
        labelIcon.Font = Glyphs;
        labelIcon.Text = "\uE805";
        labelMenu.Click += (sender, e) => {
            contextMenuStrip.Show(labelMenu.PointToScreen(
                new Point(
                    -100,
                    labelMenu.Height
                )));
        };
        contextMenuStrip.ItemClicked += (sender, e) =>
        {
            if (e.ClickedItem.Text.Equals("Exit"))
            {
                Settings.Default.StartupForm = GetType().Name;
                Settings.Default.Save();
                Application.Exit();
            }
            else
            {
                this.SwapForm(e.ClickedItem.Text.GetForm());
            }
        };
    }

***
If _no_ command line argument is present, the app will start up with the form that was visible the last time the app closed. Images are also shown for a couple of different various ways to set the command line.

[![normal start][2]][2]
***
[![shortcut start][3]][3]
***
[![launch profile][4]][4]

  [1]: https://i.stack.imgur.com/sxjxp.png
  [2]: https://i.stack.imgur.com/jEcKM.png
  [3]: https://i.stack.imgur.com/hnAI8.png
  [4]: https://i.stack.imgur.com/k8BVT.png