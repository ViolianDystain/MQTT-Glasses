

namespace SAGO_MQTT
{
    public partial class Form2 : Form
    {
        private Form1 form1Instance;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            form1Instance = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form1Instance.SetIP(textBox1.Text);
            form1Instance.SetPort(Int32.Parse(textBox2.Text));
            this.Close();
        }
    }
}
