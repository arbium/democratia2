using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Federation.Core;

namespace Federation.InvitesListGeneration
{
    public class Program
    {
        public static EntityModelContainer container = new EntityModelContainer();

        static void Main(string[] args)
        {
            new Bootstrapper().Run();

            var stream = File.Open("C:\\invites2.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
               ParseLine(reader.ReadLine());
            }

            container.SaveChanges();
            reader.Close();
        }

        private static void ParseLine(string line)
        {
            string[] results = line.Split(' ', '\t');
            Invite result = new Invite()
                                      { 
                                          Surname = results[0],
                                          Name = results[1],
                                          Patronymic = results[2],
                                          Key = "20110" + results[3],
                                          Email =  results[4]
                                      };
            container.InviteSet.AddObject(result);
        }

    }

    public struct InviteStruct
    {
        public string Key;
        public string Name;
        public string LastName;
    }
}
   