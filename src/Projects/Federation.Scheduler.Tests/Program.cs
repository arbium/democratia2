using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Federation.Core;

namespace Federation.Scheduler.Tests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            (new Bootstrapper()).Run();

            ScheduleService.Initialize(2);
            ScheduleService.Start();

            DataService.PerThread.BeginWork();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SchedulerForm());            
        }
    }
}
