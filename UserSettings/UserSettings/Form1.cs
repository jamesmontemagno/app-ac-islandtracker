using System.Text;

namespace UserSettings
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonEncode_Click(object sender, EventArgs e)
        {
            try
            {

                var encoding = Encoding.GetEncoding("iso-8859-1");
                var authenticationBytes = encoding.GetBytes($"{TextBoxPrivate.Text.Trim()}|{TextBoxPublic.Text.Trim()}");
                TextBoxEncoded.Text = Convert.ToBase64String(authenticationBytes);
            }
            catch (Exception)
            {

            }
        }
        private void ButtonDecode_Click(object sender, EventArgs e)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var s = encoding.GetString(Convert.FromBase64String(TextBoxExported.Text));

                var keys = s.Split('|');
                if (keys.Length != 2)
                    return;

                TextBoxDecodedPrivate.Text = keys[0];
                TextBoxDecodedPublic.Text = keys[1];

            }
            catch (Exception)
            {
            }        
        }
    }
}