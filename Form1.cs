using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace SAGO_MQTT
{
    public partial class Form1 : Form
    {

        private string ip = "";
        private int port = 1883;
        MqttClient mqttClient;
        private string[,] devices = new string[100, 10];

        private string pattern = @"\/(.+)";
        private int deviceCount = 0;
        DataTable table = new DataTable();
        DataTable table2 = new DataTable();


        public void SetIP(string value)
        {
            ip = value;
            // Optionally update UI elements or perform other actions here
        }

        public void SetPort(int value)
        {
            port = value;
            // Optionally update UI elements or perform other actions here
        }

        public Form1()
        {



            dataGridView1.DataSource = table;
            dataGridView2.DataSource = table2;
            InitializeComponent();




        }

        private void InitializeMQTT(string ip, int port)
        {
            mqttClient = new MqttClient(ip);
            mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            mqttClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            mqttClient.Connect("client");
            if (mqttClient.IsConnected) { listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("Connected"))); }
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var topic = e.Topic;


            Match match = Regex.Match(e.Topic, pattern);
            Match matchT1 = Regex.Match(System.Text.Encoding.UTF8.GetString(e.Message), @"IRTemp=(.*):OrtTemp=(.*):ST=(.*)");
            String ir = matchT1.Groups[1].Value;
            String ort = matchT1.Groups[2].Value;
            String st = matchT1.Groups[3].Value;
            String match2 = match.Groups[1].Value;

            if (match.Success)
            {

                bool exists = ContainsValue(devices, match2);
                if (!exists)
                {
                    devices[deviceCount, 0] = match2;
                    devices[deviceCount, 2] = DateTime.Now.ToString();
                    devices[deviceCount, 5] = ir;
                    devices[deviceCount, 6] = ort;
                    devices[deviceCount, 7] = st;
                    devices[deviceCount, 9] = DateTime.Now.ToString();
                    deviceCount++;
                    UpdateTable(devices);

                }
                else
                {
                    for (int order = 0; order < deviceCount; order++)
                    {
                        if (devices[order, 0] == match2)
                        {
                            devices[order, 5] = ir;
                            devices[order, 6] = ort;
                            devices[order, 7] = st;
                            devices[deviceCount, 9] = DateTime.Now.ToString();
                            UpdateTable(devices);
                        }
                    }

                }

                for (int order = 0; order < deviceCount; order++)
                {
                    TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(devices[order, 9]);


                    if (elapsedTime.TotalSeconds > 60)
                    {
                        devices[order, 3] = DateTime.Now.ToString();

                    }
                    int second = (int)elapsedTime.TotalSeconds;
                    devices[order, 4] = second.ToString();
                }


            }

            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(topic + " " + message)));
        }

        public bool ContainsValue<T>(T[,] array, T value) where T : IEquatable<T>
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {

                for (int j = 0; array[i, j] != null && j < array.GetLength(1); j++)
                {
                    if (array[i, j].Equals(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        private void UpdateTable(string[,] devices)
        {

            // Convert 2D array to DataTable




            // Assuming all rows in the array have the same number of columns
            int numberOfColumns = devices.GetLength(1);

            // Add columns to DataTable
            for (int i = 0; i < numberOfColumns; i++)
            {
                table.Columns.Add($"Column {i + 1}");
            }

            // Add rows to DataTable
            for (int i = 0; i < devices.GetLength(0); i++)
            {
                DataRow row = table.NewRow();
                for (int j = 0; j < numberOfColumns; j++)
                {
                    row[j] = devices[i, j] == null ? DBNull.Value : devices[i, j];
                }
                table.Rows.Add(row);
            }

            table2 = table.Copy();
            UpdateDataGridView(table);
            UpdateDataGridView2(table2);

        }


        void UpdateDataGridView(DataTable table)
        {
            // Check if the invoke is required
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new Action(() => UpdateDataGridView(table)));
            }
            else
            {



                dataGridView1.Columns[0].HeaderText = "MAC";
                dataGridView1.Columns[1].HeaderText = "Cihaz Adi";
                dataGridView1.Columns[2].HeaderText = "Baslangic Zamani";
                dataGridView1.Columns[3].HeaderText = "Bitis Zamani";
                dataGridView1.Columns[4].HeaderText = "Toplam Sure";
                dataGridView1.Columns[5].HeaderText = "T1";
                dataGridView1.Columns[6].HeaderText = "T2";
                dataGridView1.Columns[7].HeaderText = "ST";
                dataGridView1.Columns[8].HeaderText = "Bagli Oldugu Makine";
            }
        }

        void UpdateDataGridView2(DataTable table2)
        {
            // Check if the invoke is required
            if (dataGridView2.InvokeRequired)
            {
                dataGridView2.Invoke(new Action(() => UpdateDataGridView2(table2)));
            }
            else
            {


                table2.Columns.RemoveAt(0);
                table2.Columns.RemoveAt(0);
                table2.Columns.RemoveAt(7);





                dataGridView2.Columns[0].HeaderText = "Baslangic Zamani";
                dataGridView2.Columns[1].HeaderText = "Bitis Zamani";
                dataGridView2.Columns[2].HeaderText = "Toplam Sure";
                dataGridView2.Columns[3].HeaderText = "T1";
                dataGridView2.Columns[4].HeaderText = "T2";
                dataGridView2.Columns[5].HeaderText = "ST";
                dataGridView2.Columns[6].HeaderText = "Bagli Oldugu Makine";
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InitializeMQTT(ip, port);
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("Connected")));
        }
    }
}
