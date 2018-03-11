using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace SanD_Windows
{
    class Baseline
    {
        public static void GenerateInitialBaseline()
        {

            List<string> RunningProcesses = new List<string>();
            List<String> Distincts = new List<string>();
            UserSettings settings = new UserSettings();
            StreamWriter writer = new StreamWriter(UserSettings.WhiteListLocation);
            Process[] localAll = Process.GetProcesses();
            Console.WriteLine("Writing currently running process to file...");

            //Error resillient programming technique
            try
            {
                //Get list of all processes and write them to the text doument
                foreach (Process IndividualProcess in localAll)
                {
                    try
                    {
                        String Name = IndividualProcess.ProcessName;
                        Console.WriteLine(Name);
                        Console.WriteLine((Hasher.GenerateFileHash(IndividualProcess.MainModule.FileName)));
                        RunningProcesses.Add(Name + "~" + (Hasher.GenerateFileHash(IndividualProcess.MainModule.FileName.ToString())));
                    }
                    catch { }
                }

            }
            //If there is an error write it to the console and attempt to continue running the program
            catch (Exception ex)
            { Console.WriteLine(ex); }

            Distincts = RunningProcesses.Distinct().ToList();
            Distincts.Sort();
            foreach (String DistinctProcess in Distincts)
            {
                writer.Write(DistinctProcess);
                writer.Write("\r\n");
            }
            writer.Close();


            //Give the user an opportnity to review captured processes
            Console.WriteLine("Please check " + UserSettings.WhiteListLocation + " and delete any processes you do not wish to allow, once finished press enter to continue...");
            Console.ReadLine();
            AppendToBaseline();
        }
        public static void AppendToBaseline()
        {
            List<string> SafeList = new List<string>();
            UserSettings settings = new UserSettings();

            //Get items currently listed in safelist
            foreach (string line in File.ReadAllLines(UserSettings.WhiteListLocation))
            {
                SafeList.Add(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press ESC to stop searching for new processes");
            Console.ForegroundColor = ConsoleColor.White;
            do
            {
                while (!Console.KeyAvailable)
                {
                    List<string> RunningProcesses = new List<string>();
                    Process[] localAll = Process.GetProcesses();

                    //Add running processes to list
                    foreach (Process IndividualProcess in localAll)
                    {
                        string Hash = "0";
                        try
                        {
                            Hash = Hasher.GenerateFileHash(IndividualProcess.MainModule.FileName.ToString());
                        }
                        catch
                        { }
                        String Name = IndividualProcess.ProcessName;
                        RunningProcesses.Add(Name + "~" + Hash);

                    }


                    //Compare the two lists and add any new items
                    foreach (String RunningProcess in RunningProcesses)
                    {
                        if (SafeList.Contains(RunningProcess) == false)
                        {
                            StreamWriter writer = File.AppendText(UserSettings.WhiteListLocation);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("New process detected, whitelisting: ");
                            Console.WriteLine(RunningProcess);
                            SafeList.Add(RunningProcess);
                            writer.Write(RunningProcess);
                            writer.Write("\r");
                            Console.ForegroundColor = ConsoleColor.White;
                            writer.Close();
                        }

                    }

                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Switching to Monitor Mode");
            Console.ForegroundColor = ConsoleColor.White;
            Monitor.MonitorLoop();
        }
    }
}
