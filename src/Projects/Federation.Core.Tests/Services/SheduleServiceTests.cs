using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Federation.Core.Tests.Services
{
    [TestFixture]
    public class SheduleServiceTests : BaseFixture
    {
        [Test]
        public void StartWithoutJobs()//сделать объект, который отвечает за состояние сервиса и тут соб-но его и тетсировать
        {
            ScheduleService.Start();
            //var job = ScheduleService.AddJob(TestAction.Test, DateTime.Now.AddMinutes(1));
            //CollectionAssert.Contains(DataManager.ScheduleJobSet, job);
        }

        //[Test]
        //public void StartWithJobsInQuee()
        //{
        //    ScheduleService.Start(10, 100);
        //    var job = ScheduleService.AddJob(TestAction.Test, DateTime.Now.AddSeconds(25));
        //    CollectionAssert.Contains(DataManager.ScheduleJobSet, job);
        //    Thread.Sleep(60000);
        //}

        //[Test]
        //public void StartWithoutJobsInDataBase()
        //{
        //    var job = ScheduleService.AddJob(TestAction.Test, DateTime.Now.AddMinutes(1));
        //    CollectionAssert.Contains(DataManager.ScheduleJobSet, job);
        //}

        //[Test]
        //public void AddJob()
        //{
        //    var job = ScheduleService.AddJob(TestAction.Test, DateTime.Now.AddMinutes(1));
        //    CollectionAssert.Contains(DataManager.ScheduleJobSet, job);
        //}
    }

    [Serializable]
    public static class TestAction
    {
        public static void Test()
        {
            var sw = File.CreateText("C:\\test666.txt");
            sw.WriteLine("Hi!");
            sw.Close();
        }
    }
}
