using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Federation.Core
{
    public class ServiceLogger
    {
        public StringBuilder Log = new StringBuilder();
        private int _savePeriodInSeconds = 15;
        private DateTime _nextSaveTime = DateTime.MinValue;
        private readonly string _serviceName;
        private bool _autosave;
        private bool _isSaveInProcess;
        static object locker = new object();

        public string ServiceName
        {
            get { return _serviceName; }
        }        

        public DateTime NextSaveTime
        {
            get { return _nextSaveTime; }
        }        

        public bool IsAutosaveEnabled
        {
            get { return _autosave; }
        }

        public ServiceLogger(string serviceName)
        {
            _serviceName = serviceName;
        }

        public void EnableAutoSave(int savePeriodInSeconds)
        {
            _autosave = true;
            _savePeriodInSeconds = savePeriodInSeconds;
            _nextSaveTime = DateTime.Now.AddSeconds(savePeriodInSeconds);
        }

        public void DisableAutoSave()
        {
            _autosave = false;
        }

        private bool IsitTimeToSave()
        {
            return _nextSaveTime < DateTime.Now;
        }

        public void Write(string text, bool newline = true, bool withTimeStamp = true)
        {
            if (withTimeStamp)
                text = string.Format("[{0}] {1}", DateTime.Now, text);

            if (newline)
            {
                lock (locker)
                {
                    Console.WriteLine(text);
                    Log.AppendLine(text);
                }
            }
            else
            {
                lock (locker)
                {
                    Console.Write(text);
                    Log.Append(text);
                }
            }

            if (_autosave && IsitTimeToSave())
                SaveLog();
        }

        public void Write(Exception exception, string errorTitle = "ПРОИЗОШЛА ОШИБКА")
        {
            string errorMesage = string.Format("-----{0}-----", errorTitle);
            Write(errorMesage, true, false);
            Write(exception.Message);
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                Write("InnerException: " + exception.Message, true, false);
            }
        }

        public bool SaveLog()
        {
            if (!_isSaveInProcess)
            {
                lock (locker)
                {
                    try
                    {
                        _isSaveInProcess = true;
                        if (!Directory.Exists(ConstHelper.AppPath + "\\logs"))
                            Directory.CreateDirectory(ConstHelper.AppPath + "\\logs");

                        string logtext = Log.ToString();
                        Log.Clear();
                        string filename = ConstHelper.AppPath + "\\logs\\" + _serviceName + "___" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
                        if (File.Exists(filename))
                            File.AppendAllText(filename, logtext);
                        else
                            File.WriteAllText(filename, logtext);

                        _nextSaveTime = DateTime.Now.AddSeconds(_savePeriodInSeconds);
                        _isSaveInProcess = false;
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
                return true;
            }
            
            return false;
        }

        public void Close()
        {
            while (_isSaveInProcess)
            {
                Thread.Sleep(10);
            }
            Write("====Окончание работы====");
            SaveLog();
        }
    }
}
