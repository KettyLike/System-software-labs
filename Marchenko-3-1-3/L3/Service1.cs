using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace L3
{
    public partial class Service1 : ServiceBase
    {
        private Thread claimCheckerThread;
        private Thread taskProcessorThread;
        private static string logPath = @"C:\Windows\Logs\TaskQueue_18-11-2013.log";
        private static int taskExecutionDuration;
        private static int taskClaimCheckPeriod;
        private static int maxConcurrentTasks;
        private List<string> taskQueue = new List<string>();
        private static object queueLock = new object();
        private volatile bool stopRequested = false;
        private List<Task> runningTasks = new List<Task>();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service Task Queue has STARTED");
            ReadConfiguration();
            
            claimCheckerThread = new Thread(ClaimChecker);
            taskProcessorThread = new Thread(TaskProcessor);

            claimCheckerThread.Start();
            taskProcessorThread.Start();
        }

        protected override void OnStop()
        {
            stopRequested = true;
            claimCheckerThread.Join();
            taskProcessorThread.Join();
            WriteLog("Service Task Queue is STOPPED");
        }

        private void ReadConfiguration()
        {
            taskExecutionDuration = GetRegistryValue("Task_Execution_Duration", 60, 30, 180);
            taskClaimCheckPeriod = GetRegistryValue("Task_Claim_Check_Period", 30, 10, 45);
            maxConcurrentTasks = GetRegistryValue("Task_Execution_Quantity", 1, 1, 3);
        }

        private int GetRegistryValue(string subKeyPath, int defaultValue, int minValue, int maxValue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Task_Queue\Parameters"))
                {
                    if (key != null)
                    {
                        object regValue = key.GetValue(subKeyPath);
                        if (regValue != null && int.TryParse(regValue.ToString(), out int value))
                        {
                            if (value >= minValue && value <= maxValue)
                                return value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"ПОМИЛКА при читанні реєстру {subKeyPath}: {ex.Message}");
            }
            return defaultValue;
        }

        private void ClaimChecker()
        {
            while (!stopRequested)
            {
                try
                {
                    using (RegistryKey claimsKey = Registry.LocalMachine.OpenSubKey(@"Software\Task_Queue\Claims", true))
                    {
                        if (claimsKey != null)
                        {
                            string[] claimNames = claimsKey.GetValueNames();
                            foreach (string claimName in claimNames.OrderBy(c => c))
                            {
                                if (IsValidTaskName(claimName))
                                {
                                    using (RegistryKey tasksKey = Registry.LocalMachine.OpenSubKey(@"Software\Task_Queue\Tasks", true))
                                    {
                                        int underscoreIndex = claimName.LastIndexOf('_');
                                        string number = claimName.Substring(underscoreIndex, 5);

                                        if (CanAddTaskToQueue(number))
                                        {
                                            WriteLog($"Задача {claimName} успішно прийнята в обробку...");
                                        }
                                        else
                                        {
                                            WriteLog($"ПОМИЛКА розміщення заявки {claimName}. Номер вже існує ...");
                                        }
                                    }
                                }
                                else
                                {
                                    WriteLog($"ПОМИЛКА розміщення заявки {claimName}. Некоректний синтаксис ...");
                                }
                                claimsKey.DeleteValue(claimName);
                                break;
                            }
                        }
                    }
                    Thread.Sleep(taskClaimCheckPeriod * 1000);
                }
                catch (Exception ex)
                {
                    WriteLog($"ПОМИЛКА в ClaimChecker: {ex.Message}");
                }
            }
        }

        private bool IsValidTaskName(string taskName)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(taskName, @"^Task_\d{4}$");
        }

        private bool CanAddTaskToQueue(string number)
        {
            lock (queueLock)
            {
                using (RegistryKey tasksKey = Registry.LocalMachine.OpenSubKey(@"Software\Task_Queue\Tasks", true))
                {
                    string[] progress = (string[])tasksKey.GetValue("Tasks_Progress", new string[] { });
                    foreach (string name in progress)
                    {
                        int underscoreIndex = name.LastIndexOf('_');
                        string number2 = name.Substring(underscoreIndex, 5);
                        if (number == number2)
                        {
                            return false;
                        }
                    }
                    string taskName = $"Task{number}";
                    taskQueue.Add(taskName);
                    UpdateTaskProgress(taskName, "QUEUED");
                    return true;
                }
            }
        }

        private void UpdateTaskProgress(string taskName, string status)
        {
            lock (queueLock) 
            {
                using (RegistryKey tasksKey = Registry.LocalMachine.OpenSubKey(@"Software\Task_Queue\Tasks", true))
                {
                    if (tasksKey != null)
                    {
                        string[] progress = (string[])tasksKey.GetValue("Tasks_Progress", new string[] { });
                        List<string> updatedProgress = new List<string>(progress);

                        bool taskFound = false;
                        for (int i = 0; i < updatedProgress.Count; i++)
                        {
                            if (updatedProgress[i].StartsWith(taskName))
                            {
                                updatedProgress[i] = $"{taskName} ........ {status}";
                                taskFound = true;
                                break;
                            }
                        }

                        if (!taskFound)
                        {
                            updatedProgress.Add($"{taskName} ........ {status}");
                        }

                        tasksKey.SetValue("Tasks_Progress", updatedProgress.ToArray(), RegistryValueKind.MultiString);
                    }
                }
            }
        }

        private void TaskProcessor()
        {
            while (!stopRequested)
            {
                try
                {
                    lock (queueLock)
                    {

                        runningTasks.RemoveAll(t => t.IsCompleted);
                        while (runningTasks.Count < maxConcurrentTasks && taskQueue.Count > 0)
                        {
                            string taskName = taskQueue[0];
                            taskQueue.RemoveAt(0);

                            Task task = Task.Run(() =>
                            {
                                ProcessTask(taskName);
                            });

                            runningTasks.Add(task);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"ПОМИЛКА в TaskProcessor: {ex.Message}");
                }
            }
        }

        private void ProcessTask(string taskName)
        {
            try
            {
                int interval = 2; 
                int totalUpdates = taskExecutionDuration / interval;

                for (int i = 0; i < totalUpdates; i++)
                {
                    int percent = (i * 100) / totalUpdates; 
                    UpdateTaskProgress(taskName, $"{percent}% IN PROGRESS");
                    Thread.Sleep(interval * 1000); 
                }

                UpdateTaskProgress(taskName, "100% IN PROGRESS");
                UpdateTaskProgress(taskName, "COMPLETED");
                WriteLog($"Задача {taskName} успішно ВИКОНАНА!");
            }
            catch (Exception ex)
            {
                WriteLog($"ПОМИЛКА в ProcessTask для {taskName}: {ex.Message}");
            }
        }

        private static void WriteLog(string z)
        {
            string logEntry = $"-----------------------------------------{DateTime.Now}----------------------------------------------------------------\n{z}\n";
            File.AppendAllText(logPath, logEntry);
        }
    }
}
