namespace activity_shell
{
    public partial class FlightsForm : Form
    {
        public FlightsForm() => InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            labelHome.Font = MainForm.Glyphs;
            labelHome.Text = "\uE805";
            labelIcon.Font = MainForm.Glyphs;
            labelIcon.Text = "\uE802";
            labelMenu.Font = MainForm.Glyphs;
            labelMenu.Text = "\uE801";
            labelTitle.Text = GetType().Name.Replace("Form", " Form");
            labelMenu.Click += (sender, e) => {
                contextMenuStrip.Show(labelMenu.PointToScreen(
                    new Point(
                        -100,
                        labelMenu.Height
                    )));
            };
            labelHome.Click +=  (sender, e) =>
                this.SwapForm(nameof(MainForm).GetForm());
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
    }
}
