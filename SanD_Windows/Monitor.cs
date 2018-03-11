using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace SanD_Windows
{
    class Monitor
    {
        public static void MonitorLoop()
        {
            //Declare variables to be used
            List<string> ProcessList = new List<string>();
            List<string> SafeList = new List<string>();
            int Index;
            String Trimmed;
            UserSettings settings = new UserSettings();

            //Setup the eventlog to accept events from this app
            if (!EventLog.SourceExists("SanD"))
                EventLog.CreateEventSource("Sand", "Application");


            //Run through the running processlist and compare it to the safe list
            foreach (string line in File.ReadAllLines(UserSettings.WhiteListLocation))
            {
                Index = line.IndexOf("~");
                Trimmed = (Index > 0 ? line.Substring(0, Index) : "");
                SafeList.Add(Trimmed);
            }
            while (0 == 0)
            {
                //Get a list called localAll of all running processes and do something for each one
                Process[] localAll = Process.GetProcesses();
                foreach (Process IndividualProcess in localAll)
                {
                    //If a running process is not in the safelist attempt to kill it
                    if (SafeList.Contains(IndividualProcess.ProcessName) == false)
                    {

                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Illegal process detected: " + IndividualProcess.ProcessName);
                            Console.WriteLine(IndividualProcess.MainModule.FileVersionInfo);
                            Console.WriteLine("Entry Point: " + IndividualProcess.MainModule.EntryPointAddress);
                            IndividualProcess.Kill();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("Process terminated, writing to event log");
                            EventLog.WriteEntry(IndividualProcess.ProcessName, IndividualProcess.MainModule.FileVersionInfo.ToString(), EventLogEntryType.Warning, 666);
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine(ex.Message);
                        }
                        

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Continuing to monitor....");

                    }

                }
            }
        }
    }
}
