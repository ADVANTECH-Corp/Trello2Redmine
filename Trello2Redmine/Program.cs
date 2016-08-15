﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trello2Redmine
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        //static int exitCode = 0;

        //public static ExitApplication(int exitCode)
        //{
        //    Program.exitCode = exitCode;
        //    Application.Exit();
        //}

        class GuiRedirect
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool AttachConsole(int dwProcessId);
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(StandardHandle nStdHandle);
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool SetStdHandle(StandardHandle nStdHandle, IntPtr handle);
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern FileType GetFileType(IntPtr handle);

            private enum StandardHandle : uint
            {
                Input = unchecked((uint)-10),
                Output = unchecked((uint)-11),
                Error = unchecked((uint)-12)
            }

            private enum FileType : uint
            {
                Unknown = 0x0000,
                Disk = 0x0001,
                Char = 0x0002,
                Pipe = 0x0003
            }

            private static bool IsRedirected(IntPtr handle)
            {
                FileType fileType = GetFileType(handle);

                return (fileType == FileType.Disk) || (fileType == FileType.Pipe);
            }

            public static void Redirect()
            {
                if (IsRedirected(GetStdHandle(StandardHandle.Output)))
                {
                    var initialiseOut = Console.Out;
                }

                bool errorRedirected = IsRedirected(GetStdHandle(StandardHandle.Error));
                if (errorRedirected)
                {
                    var initialiseError = Console.Error;
                }

                AttachConsole(-1);

                if (!errorRedirected)
                    SetStdHandle(StandardHandle.Error, GetStdHandle(StandardHandle.Output));
            }
        }

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GuiRedirect.Redirect();
            // Attach to the parent process via AttachConsole SDK call
            AttachConsole(ATTACH_PARENT_PROCESS);
            Application.Run(new Form1(args));

            
        }
    }
}
