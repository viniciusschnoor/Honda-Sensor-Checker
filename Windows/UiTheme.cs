namespace HondaSensorChecker
{
    internal static class UiTheme
    {
        internal static readonly Color Primary = Color.FromArgb(21, 74, 124);
        internal static readonly Color PrimaryLight = Color.FromArgb(218, 232, 245);
        internal static readonly Color Surface = Color.White;
        internal static readonly Color Background = Color.FromArgb(247, 249, 252);
        internal static readonly Color Text = Color.FromArgb(38, 48, 58);
        internal static readonly Color MutedText = Color.FromArgb(80, 90, 100);
        internal static readonly Color Border = Color.FromArgb(210, 218, 226);
        internal static readonly Color Danger = Color.FromArgb(177, 45, 45);
        internal static readonly Color Success = Color.FromArgb(25, 126, 75);

        internal static void StyleForm(Form form)
        {
            form.BackColor = Background;
            form.Font = new Font("Segoe UI", 9F);
        }

        internal static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(225, 230, 235);
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 36;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Background;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 246);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(45, 58, 70);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 240, 246);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 58, 70);
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.Padding = new Padding(7, 0, 7, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 231, 247);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(25, 55, 82);
        }

        internal static void StylePrimaryButton(Button button, Color? color = null)
        {
            button.BackColor = color ?? Primary;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
        }

        internal static void StyleOutlinedButton(Button button, Color? color = null)
        {
            var accent = color ?? Primary;
            button.BackColor = Surface;
            button.ForeColor = accent;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = accent;
            button.FlatAppearance.BorderSize = 1;
            button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
        }

        internal static void StyleInput(TextBox input)
        {
            input.BackColor = Surface;
            input.BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
