using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Federation.Core;
using System.IO;

namespace Federation.Scheduler.Tests
{
    public partial class SchedulerForm : Form
    {
        public SchedulerForm()
        {
            InitializeComponent();
        }

        private void butTask1_Click(object sender, EventArgs e)
        {
                ActionScheduleTask ActionScheduleTask = new ActionScheduleTask(TestAction.Test);
                var job = ScheduleService.AddJob(ActionScheduleTask, DateTime.Now.AddSeconds(Int32.Parse(txtFileName.Text)), true, 30, false);
                ActionScheduleTask ActionScheduleTask2 = new ActionScheduleTask(TestAction.Test2);
                var job2 = ScheduleService.AddJob(ActionScheduleTask2, DateTime.Now.AddSeconds(Int32.Parse(txtFileName.Text)), true, 30, false);
        }

        private void butStart_Click(object sender, EventArgs e)
        {
            ScheduleService.Start();
            lblStatus.Text = "Работает";
            lblStatus.ForeColor = Color.Green;
        }

        private void butStop_Click(object sender, EventArgs e)
        {
            ScheduleService.Stop();
            lblStatus.Text = "Не работает";
            lblStatus.ForeColor = Color.Red;
        }
    }

    [Serializable]
    public static class TestAction
    {
        public static void Test()
        {
            Guid g = Guid.NewGuid();
            var sw = File.CreateText("C:\\" + g.ToString() + ".txt");
            sw.WriteLine("Hi!");
            sw.Close();
        }

        public static void Test2()
        {
            var e1 = new NullReferenceException();
            var e2 = new NullReferenceException("бла бла", e1);
            var e3 = new Exception("Ураааа", e2);
            throw e3;
        }
    }
}
