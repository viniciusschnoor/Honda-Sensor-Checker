namespace HondaSensorChecker
{
    internal sealed partial class AdminAuthorizationDialog : Form
    {
        internal string AdminRe => txtAdminRe.Text.Trim().ToUpperInvariant();

        internal AdminAuthorizationDialog()
        {
            InitializeComponent();
            UiTheme.StyleOutlinedButton(btnCancel);
            UiTheme.StylePrimaryButton(btnAuthorize);
            Shown += (_, _) => txtAdminRe.Focus();
        }
    }
}
