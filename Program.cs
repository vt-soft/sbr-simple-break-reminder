// **************************************************************************************************************************
// *  Project information:                                                                                                  *
// *                                                                                                                        *
// *  Name        :  SBR - Simple Break Reminder                                                                            *
// *  Description :  Application reminding you to take a break when working with computer                                   *
// *  Language    :  C# 14                                                                                                  *
// *  Framework   :  .NET 10.0                                                                                              *
// *  UI          :  WinForms                                                                                               *
// *  NuGet       :  https://www.nuget.org/packages/WinForms.DataVisualization                                              *
// *  Icons       :  By Icons8 (https://icons8.com/)                                                                        *
// *  Web         :  https://www.vt-soft.com/sbr-simple-break-reminder                                                      *
// *                                                                                                                        *
// *  Please be aware that I am not professional developer so this code is not perfect                                      *
// *  and it is probably not following all best programming practices.                                                      *
// *  However I still hope that you will find it useful and that it will help you to create your own application.           *
// *  For more projects please check https://www.vt-soft.com/                                                               *
// *  Any link to this site is highly appreciated. Enjoy the code! :)                                                       *
// *                                                                                                                        *
// *  Copyright(c) 2025-2026, vt-soft                                                                                       *
// *  All rights reserved.                                                                                                  *
// *                                                                                                                        *
// *  This source code is licensed under the MIT-style license.                                                             *
// *  More info in the license.txt file in the root directory of this source tree.                                          *
// **************************************************************************************************************************


using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SBR
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            ApplicationConfiguration.Initialize();

            // Wait for user to unlock the computer before starting the application.
            // In Win10 and Win11, the application can start (if set in Windows Registry) even if the user is not logged yet,
            // which can cause issues with the application.
            WindowsLoginDetector.WaitForUserUnlock();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.Run(new MainForm());
        }
    }
}