using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using Federation.Core.Services;
using Federation.Core.Tests.Services;

namespace Federation.Core.Tests
{
    class Program
    {
        [STAThread]//TODO: возможно тут косяк дебага
        static void Main(string[] args)
        {
                //(new Bootstrapper()).Run();

                //DataService.PerThread.BeginWork();

                //var a = new MultiThreadLoggerTests();
                //a.Test();

            bool a = BankCardValidationService.ValidateTest("rm66mail@gmail.com", "Roman", "Yamaletdinov");
            bool a2 = BankCardValidationService.ValidateTest("rm66mail@e1.ru", "Roman", "Yamaletdinov");
            //a = BankCardValidationService.Validate("arbium@ya.ru", "Вася", "Пупкин");
            //a = BankCardValidationService.Validate("sdfdsaf@gmail.com", "Dmitry", "Chirkov");
        }
    }
}
