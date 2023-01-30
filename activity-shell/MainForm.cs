
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace activity_shell
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            makeGlyphFont();            
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

        private void makeGlyphFont()
        {
            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Fonts",
                "glyphs.ttf");
            privateFontCollection.AddFontFile(path);
            var fontFamily = privateFontCollection.Families[0];
            Glyphs = new Font(fontFamily, 20F);
        }

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

        protected PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        public static Font Glyphs { get; private set; }
    }
    static class Extensions
    {
        public static Form Default { get; set; }
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
}