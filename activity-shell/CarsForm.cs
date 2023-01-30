namespace activity_shell
{
    public partial class CarsForm : Form
    {
        public CarsForm() => InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            labelHome.Font = MainForm.Glyphs;
            labelHome.Text = "\uE805";
            labelIcon.Font = MainForm.Glyphs;
            labelIcon.Text = "\uE804";
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
            contextMenuStrip.ItemClicked += (sender, e) =>
            {
                if (e.ClickedItem.Text.Equals("Exit"))
                {
                    Application.Exit();
                }
                else
                {
                    this.SwapForm(e.ClickedItem.Text.GetForm());
                }
            };
            labelHome.Click += (sender, e) =>
                this.SwapForm(nameof(MainForm).GetForm());
        }
    }
}
