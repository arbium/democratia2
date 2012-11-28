using System.Configuration;
using System.IO;
using System.Threading;
using Federation.Core;
using System.Text;
using System;
using System.Reflection;

namespace Federation.MailService
{
    public class MainClass
    {
        public static bool TESTING = true;
        public static ServiceLogger _mainLog = new ServiceLogger("DailyMail");
        //IF NOT EXIST $(ProjectDir)$(OutDir)Template MKDIR $(ProjectDir)$(OutDir)Template
        //XCOPY $(ProjectDir)Template\MailMessageTemplate.cshtml $(ProjectDir)$(OutDir)Template /Y

        static void Main(string[] args)
        {
            var bootsraper = new Bootstrapper();
            bootsraper.Run();
            ConstHelper.AppPath = ConfigurationManager.AppSettings["PhysicalPath"];
            ConstHelper.AppUrl = ConfigurationManager.AppSettings["AppUrlPath"];
            TESTING = bool.Parse(ConfigurationManager.AppSettings["TESTING"]);
            _mainLog.EnableAutoSave(15);

            try
            {
                //DailyMailTask task = new DailyMailTask(DateTime.Parse("28.02.2012"));
                DailyMailTask task = new DailyMailTask(DateTime.Now.AddDays(-1));
                task.Initialize(_mainLog);
                task.Start();
            }
            catch (Exception exception)
            {
                _mainLog.Write(exception);
            }
            finally
            {
                var end = string.Format("----- Завершение работы -----");
                _mainLog.Write(end);
                _mainLog.SaveLog();
            }
        }
    }
}
