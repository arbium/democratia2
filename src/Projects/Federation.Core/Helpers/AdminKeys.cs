using System;
using System.Collections.Generic;

namespace Federation.Core.Helpers
{
    public static class AdminKeys
    {
        private static Dictionary<string, AdminKeyInfo> _actual = new Dictionary<string, AdminKeyInfo>();

        public static string NewKey(Guid id)
        {
            var key = Guid.NewGuid().ToString();

            _actual.Add(key, new AdminKeyInfo { LastActionTime = DateTime.Now, Id = id });

            return key;
        }

        public static Guid? GetIdByAdminKey(string key)
        {
            if (_actual.ContainsKey(key))
            {
                var aki = _actual[key];
                return aki.Id;
            }

            return null;            
        }

        public static bool ValidateAdminKey(string key)
        {
            if (_actual.ContainsKey(key))
            {
                var aki = _actual[key];
                aki.LastActionTime = DateTime.Now;

                return true;
            }

            return false;
        }

        public static void ClearUnused()
        {
            List<string> keysToDelete = new List<string>();
            foreach (var key in _actual.Keys)
            {
                var aki = _actual[key];

                if (DateTime.Now - aki.LastActionTime > new TimeSpan(1, 0, 0))
                    keysToDelete.Add(key);
            }

            foreach (var key in keysToDelete)
            {
                _actual.Remove(key);
            }
        }

        private struct AdminKeyInfo
        {
            public Guid Id;
            public DateTime LastActionTime;
        }
    }
}