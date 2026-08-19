namespace HondaSensorChecker
{
    internal sealed class AdminAuthorizationDialog : Form
    {
        private readonly TextBox _txtAdminRe = new();

        internal string AdminRe => _txtAdminRe.Text.Trim().ToUpperInvariant();

        internal AdminAuthorizationDialog()
        {
            Text = "Autorização administrativa";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(480, 260);
            Font = new Font("Segoe UI", 9F);
            BackColor = UiTheme.Background;

            var header = new Panel
            {
                BackColor = UiTheme.Primary,
                Dock = DockStyle.Top,
                Height = 82
            };
            header.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 17F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 12),
                Text = "Autorização necessária"
            });
            header.Controls.Add(new Label
            {
                AutoSize = true,
                ForeColor = UiTheme.PrimaryLight,
                Location = new Point(24, 51),
                Text = "Informe o RE de qualquer administrador cadastrado"
            });

            var content = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 18, 24, 18)
            };
            var label = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = UiTheme.MutedText,
                Location = new Point(24, 20),
                Text = "RE DO ADMINISTRADOR"
            };
            _txtAdminRe.BorderStyle = BorderStyle.FixedSingle;
            _txtAdminRe.CharacterCasing = CharacterCasing.Upper;
            _txtAdminRe.Font = new Font("Segoe UI", 13F);
            _txtAdminRe.Location = new Point(24, 45);
            _txtAdminRe.Size = new Size(432, 31);

            var btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new Point(218, 102),
                Size = new Size(110, 38),
                Text = "CANCELAR"
            };
            var btnAuthorize = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new Point(340, 102),
                Size = new Size(116, 38),
                Text = "AUTORIZAR"
            };
            UiTheme.StyleOutlinedButton(btnCancel);
            UiTheme.StylePrimaryButton(btnAuthorize);

            content.Controls.AddRange(new Control[] { label, _txtAdminRe, btnCancel, btnAuthorize });
            Controls.Add(content);
            Controls.Add(header);
            AcceptButton = btnAuthorize;
            CancelButton = btnCancel;
            Shown += (_, _) => _txtAdminRe.Focus();
        }
    }
}
