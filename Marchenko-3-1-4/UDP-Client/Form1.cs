using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace UDP_Client
{
    public partial class Form1 : Form
    {
        private string serverIpAddress;
        private const int serverPort = 50000;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;

            //textBox1.Text = "192.168.163.140";
            //textBox2.Text = @"C:\Windows\System32";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverIpAddress = textBox1.Text;
            string directoryPath = textBox2.Text;

            if (string.IsNullOrWhiteSpace(serverIpAddress))
            {
                MessageBox.Show("Введіть IP-адресу сервера.");
                return;
            }

            var requestXml = new XElement("Request",
                new XElement("RequestType", "1"),
                new XElement("DirectoryPath", directoryPath));

            string requestFilePath = Path.Combine(Environment.CurrentDirectory, "Request-1.xml");
            requestXml.Save(requestFilePath);

            string responseXml = SendRequest(requestFilePath);

            string responseFilePath = Path.Combine(Environment.CurrentDirectory, "Response-1.xml");
            File.WriteAllText(responseFilePath, responseXml);
            PopulateComboBox(responseFilePath);
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Оберіть процес для завершення.");
                return;
            }

            var selectedProcessId = ((ComboBoxItem)comboBox1.SelectedItem).ProcessId;
            var requestXml = new XElement("Request",
                new XElement("RequestType", "2"),
                new XElement("ProcessId", selectedProcessId));

            string requestFilePath = Path.Combine(Environment.CurrentDirectory, "Request-2.xml");
            requestXml.Save(requestFilePath);

            string responseXml = SendRequest(requestFilePath);

            string responseFilePath = Path.Combine(Environment.CurrentDirectory, "Response-2.xml");
            File.WriteAllText(responseFilePath, responseXml);
            MessageBox.Show(ParseResponse(responseFilePath));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string SendRequest(string requestFilePath)
        {
            byte[] requestData = File.ReadAllBytes(requestFilePath);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);
            byte[] responseBytes = new byte[1000000];

            using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                clientSocket.Connect(ipEndPoint);
                clientSocket.Send(requestData);
                clientSocket.ReceiveTimeout = 5000;
                try
                {
                    int receivedBytes = clientSocket.Receive(responseBytes);
                    return Encoding.UTF8.GetString(responseBytes, 0, receivedBytes);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Сервер не відповідає.");
                    return null;
                }
            }
        }

        private void PopulateComboBox(string responseFilePath)
        {
            comboBox1.Items.Clear();
            var responseXml = XElement.Load(responseFilePath);
            var processes = responseXml.Elements("Process")
                .Select(p => new ComboBoxItem(
                    p.Element("Description")?.Value,
                    p.Element("ExecutablePath")?.Value,
                    int.Parse(p.Element("ProcessId")?.Value),
                    long.Parse(p.Element("KernelModeTime")?.Value ?? "0")
                ))
                .OrderByDescending(p => p.KernelModeTime);

            foreach (var process in processes)
            {
                comboBox1.Items.Add(process);
            }
        }

        private string ParseResponse(string responseFilePath)
        {
            var responseXml = XElement.Load(responseFilePath);
            return responseXml.Element("Message")?.Value ?? "Невідома помилка";
        }

        private class ComboBoxItem
        {
            public string Description { get; }
            public string ExecutablePath { get; }
            public int ProcessId { get; }
            public long KernelModeTime { get; }

            public ComboBoxItem(string description, string executablePath, int processId, long kernelModeTime)
            {
                Description = description;
                ExecutablePath = executablePath;
                ProcessId = processId;
                KernelModeTime = kernelModeTime;
            }

            public override string ToString()
            {
                return $"{Description} ({ExecutablePath})";
            }
        }
    }
}
