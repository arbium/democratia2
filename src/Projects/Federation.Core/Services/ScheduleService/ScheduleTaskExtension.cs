using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Federation.Core
{
    public static class ScheduleTaskExtension
    {
        public static byte[] Serialize(this ScheduleTask task)
        {
            var serializedTask = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(serializedTask, task);
            serializedTask.Position = 0;
            return serializedTask.ToArray();
        }
    }
}
