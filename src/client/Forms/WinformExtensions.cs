namespace OkoClient.Forms;

/// <summary>
///     Extension methods for WinForms to add placeholder text to TextBoxes.
///     Simoly defines operations that are performed when the user clicks or focuses on the TextBox.
/// </summary>
public static class WinformExtensions
{
    public static void PlaceholderText(this TextBox textBox, string text)
    {
        textBox.Text = text;
        textBox.ForeColor = Color.Gray;

        // Blocking moving the caret with arrow keys
        textBox.KeyDown += (_, e) =>
        {
            if (textBox.Text == text && e.KeyCode is Keys.Left or Keys.Right or Keys.Up or Keys.Down)
            {
                e.Handled = true;
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
                return;
            }

            if (textBox.Text == text)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        };

        // Blocking moving the caret with mouse
        textBox.MouseDown += (_, e) =>
        {
            if (textBox.Text == text && e.Button == MouseButtons.Left)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
            }
        };


        textBox.LostFocus += (_, _) =>
        {
            if (textBox.Text == "")
            {
                textBox.Text = text;
                textBox.ForeColor = Color.Gray;
            }
        };
    }
}