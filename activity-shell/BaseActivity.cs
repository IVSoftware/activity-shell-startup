using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace activity_shell
{
#if false
    public class BaseActivity : Form
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                FormClosing -= OnActivityClosing; // In case handle is recreated
                FormClosing += OnActivityClosing;
            }
        } 
        protected void OnActivityClosing(object? sender, FormClosingEventArgs e) { 
            e.Cancel = true; // Prevent destruction of handle if `Close` is called.
            Hide();
        }

        internal void OnClickHome(object? sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                form.Visible = form.Name.Equals(nameof(HomeActivity));
            }
        }
        public static Font? Glyphs { get; protected set; }
        public static HomeActivity? MainWnd { get; protected set; }
        protected virtual void MenuItemClicked(string text)
        {
            if (text == "Exit")
            {
                // Remove the handler. Allow close.
                foreach (var open in Application.OpenForms.OfType<BaseActivity>())
                {
                    open.FormClosing -= open.OnActivityClosing;
                }
                Application.Exit();
            }
            else
            {
                showForm($"{text}Activity");
            }
        }

        private void showForm(string activity)
        {
            var visibleNow = CurrentForm;
            var showForm = Application.OpenForms[activity];
            if (showForm == null)
            {   // Create if NOT EXIST
                switch (activity)
                {
                    case nameof(FlightsActivity): showForm = new FlightsActivity(); break;
                    case nameof(CarsActivity): showForm = new CarsActivity(); break;
                    case nameof(CartActivity): showForm = new CartActivity(); break;
                    default:
                        throw new NotImplementedException($"{activity}");
                }
            }
            if (activity.Equals(nameof(HomeActivity)))
            {
                _back.Clear();
            }
            else
            {
                _back.Push(CurrentActivity);
            }
            if(!ReferenceEquals(showForm, visibleNow))
            {
                showForm.StartPosition = FormStartPosition.Manual;
                showForm.Size = visibleNow.Size;
                showForm.Location = visibleNow.PointToScreen(new Point());
                showForm.Show();
                visibleNow.Hide();
                CurrentActivity = activity;
            }
            else
            {

            }
        }
        private string CurrentActivity { get; set; } = nameof(HomeActivity);
        private Form CurrentForm => 
            Application
            .OpenForms
            .Cast<Form>().First(_=>_.Name.Equals(CurrentActivity));

        protected static Stack<string> _back= new Stack<string>();
        internal void OnClickX(object? sender, EventArgs e)
        {
            string activity;
            if (_back.Any())
            {
                activity = _back.Pop();
            }
            else
            {
                activity = nameof(HomeActivity);
            }
            showForm(activity);
        }
    }
#endif
}
