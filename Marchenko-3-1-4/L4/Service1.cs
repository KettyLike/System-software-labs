using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Management;
using System.Xml.Linq;
using System.Text;

namespace L4
{
    public partial class Service1 : ServiceBase
    {
        private ManagementEventWatcher watcherCreation;
        private ManagementEventWatcher watcherDeletion;

        private UdpClient udpServer;
        private Thread serverThread;
        private const int ServerPort = 50000;
        private bool isRunning;
        private const string dirPath = @"C:\WORK\LAB4";
        private static readonly string logFilePath = Path.Combine(dirPath, "process_log.txt");


        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                WqlEventQuery queryCreation = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Group'");
                watcherCreation = new ManagementEventWatcher(queryCreation);
                watcherCreation.EventArrived += new EventArrivedEventHandler(OnEventArrived);
                watcherCreation.Start();

                WqlEventQuery queryDeletion = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Group'");
                watcherDeletion = new ManagementEventWatcher(queryDeletion);
                watcherDeletion.EventArrived += new EventArrivedEventHandler(OnEventArrived);
                watcherDeletion.Start();

                isRunning = true;
                serverThread = new Thread(StartServer);
                serverThread.Start();

                Log("Service started.");
            }
            catch (Exception ex)
            {
                Log("Error in OnStart: " + ex.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (watcherCreation != null)
                {
                    watcherCreation.Stop();
                    watcherCreation.EventArrived -= OnEventArrived;
                    watcherCreation.Dispose();
                    watcherCreation = null;
                }

                if (watcherDeletion != null)
                {
                    watcherDeletion.Stop();
                    watcherDeletion.EventArrived -= OnEventArrived;
                    watcherDeletion.Dispose();
                    watcherDeletion = null;
                }

                isRunning = false;
                udpServer?.Close();
                serverThread?.Join();

                Log("Service stopped.");
            }
            catch (Exception ex)
            {
                Log("Error in OnStop: " + ex.Message);
            }
        }

        private void OnEventArrived(object sender, EventArrivedEventArgs e)
        {
            Log("Detected a change in Win32_Group.");
        }

        private void StartServer()
        {
            udpServer = new UdpClient(ServerPort);
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, ServerPort);

            try
            {
                while (isRunning)
                {
                    byte[] requestData = udpServer.Receive(ref clientEndPoint);
                    ThreadPool.QueueUserWorkItem(ProcessRequest, (requestData, clientEndPoint));
                }
            }
            catch (SocketException) { }
        }

        private void ProcessRequest(object state)
        {
            var (requestData, clientEndPoint) = ((byte[], IPEndPoint))state;
            string requestXmlContent = Encoding.UTF8.GetString(requestData);
            requestXmlContent = requestXmlContent.Trim();
            requestXmlContent = requestXmlContent.TrimStart('\uFEFF');
            var requestXml = XElement.Parse(requestXmlContent);
            string requestType = requestXml.Element("RequestType")?.Value;

            string responseXmlContent;
            if (requestType == "1")
            {
                SaveXmlFile(requestXmlContent, "Request-1.xml");
                string directoryPath = requestXml.Element("DirectoryPath")?.Value ?? string.Empty;
                responseXmlContent = HandleProcessSearchRequest(directoryPath);
            }
            else if (requestType == "2")
            {
                SaveXmlFile(requestXmlContent, "Request-2.xml");
                int processId = int.Parse(requestXml.Element("ProcessId")?.Value ?? "0");
                responseXmlContent = HandleProcessTerminationRequest(processId);
            }
            else
            {
                responseXmlContent = "<Response><Message>Unknown Request Type</Message></Response>";
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseXmlContent);
            udpServer.Send(responseBytes, responseBytes.Length, clientEndPoint);
        }

        private string HandleProcessSearchRequest(string directoryPath)
        {
            var responseXml = new XElement("Response");
            try
            {
                SelectQuery query;
                if (string.IsNullOrWhiteSpace(directoryPath))
                {
                    query = new SelectQuery("SELECT Description, ExecutablePath, KernelModeTime, ProcessId FROM Win32_Process");
                }
                else
                {
                    string escapedDirectoryPath = directoryPath.Replace("\\", "\\\\");
                    query = new SelectQuery($@"SELECT Description, ExecutablePath, KernelModeTime, ProcessId FROM Win32_Process WHERE ExecutablePath LIKE '{escapedDirectoryPath}%'");
                }

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject process in searcher.Get())
                    {
                        responseXml.Add(new XElement("Process",
                            new XElement("Description", process["Description"]?.ToString() ?? ""),
                            new XElement("ExecutablePath", process["ExecutablePath"]?.ToString() ?? ""),
                            new XElement("KernelModeTime", process["KernelModeTime"]?.ToString() ?? ""),
                            new XElement("ProcessId", process["ProcessId"]?.ToString() ?? "")));
                    }
                }

                string responseFilePath = Path.Combine(dirPath, "Response-1.xml");
                responseXml.Save(responseFilePath);

                return responseXml.ToString();
            }
            catch (Exception ex)
            {
                Log($"Error in search request: {ex.Message}");
                return $"<Response><Message>Error: {ex.Message}</Message></Response>";
            }
        }

        private string HandleProcessTerminationRequest(int processId)
        {
            var responseXml = new XElement("Response");

            try
            {
                using (var searcher = new ManagementObjectSearcher($@"SELECT * FROM Win32_Process WHERE ProcessId = {processId}"))
                {
                    foreach (ManagementObject process in searcher.Get())
                    {
                        process.InvokeMethod("Terminate", null);
                        responseXml.Add(new XElement("Message", "Process terminated successfully"));
                        break;
                    }
                }

                string responseFilePath = Path.Combine(dirPath, "Response-2.xml");
                responseXml.Save(responseFilePath);
            }
            catch (Exception ex)
            {
                responseXml.Add(new XElement("Message", $"Failed to terminate process: {ex.Message}"));
                Log($"Error in termination request for process {processId}: {ex.Message}");
            }

            return responseXml.ToString();
        }

        private void SaveXmlFile(string xmlContent, string fileName)
        {
            try
            {
                xmlContent = xmlContent.Trim();
                xmlContent = xmlContent.TrimStart('\uFEFF');
                XDocument xmlDocument = XDocument.Parse(xmlContent);
                string filePath = Path.Combine(dirPath, fileName);
                xmlDocument.Save(filePath);
            }
            catch (Exception ex)
            {
                Log($"Помилка при збереженні XML-файлу: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            string logFilePath = Path.Combine(dirPath, "L4.log");
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
