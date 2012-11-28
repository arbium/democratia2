using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public static class ConfirmationService
    {
        private static Dictionary<Guid, ConfirmationData> _dict = new Dictionary<Guid, ConfirmationData>();
        private static DateTime _lastCleaningDate = DateTime.Now;

        static ConfirmationService()
        {
        }

        public static Guid GetId(ConfirmationData data)
        {
            var id = Guid.NewGuid();
            data.CreationDate = DateTime.Now;
            _dict.Add(id, data);

            return id;
        }

        public static ConfirmationData GetData(Guid id)
        {
            if (!_dict.ContainsKey(id))
                throw new BusinessLogicException("Данные устарели, вернитесь и обновите страницу");

            var data = _dict[id];

            if (!data.IsUsed)
            {                
                var newData = new ConfirmationData
                {
                    ActionUrl = data.ActionUrl,
                    CreationDate = data.CreationDate,
                    IsUsed = true,
                    Message = data.Message,
                    PostData = data.PostData,
                    ReturnUrl = data.ReturnUrl
                };

                _dict.Remove(id);
                _dict.Add(id, newData);
            }

            return data;
        }

        public static void SetReturnUrl(Guid id, string returnUrl)
        {
            var data = _dict[id];
            var newData = new ConfirmationData
            {
                ActionUrl = data.ActionUrl,
                CreationDate = data.CreationDate,
                IsUsed = data.IsUsed,
                Message = data.Message,
                PostData = data.PostData,
                ReturnUrl = returnUrl
            };

            _dict.Remove(id);
            _dict.Add(id, newData);
        }

        public static void SetPostData(Guid id, object postData)
        {
            if (!_dict.ContainsKey(id))
                throw new BusinessLogicException("Данные устарели, вернитесь и обновите страницу");

            var data = _dict[id];
            data.PostData = postData;

            _dict[id] = data;
        }

        public static void DeleteRecord(Guid id)
        {
            _dict.Remove(id);
        }

        public static void Clear()
        {
            if ((DateTime.Now - _lastCleaningDate).Days > 1)
            {
                foreach (var confirm in _dict)
                    if ((DateTime.Now - confirm.Value.CreationDate).Days > 1)
                        _dict.Remove(confirm.Key);

                _lastCleaningDate = DateTime.Now;
            }
        }
    }

    public struct ConfirmationData
    {
        public DateTime CreationDate;
        public string Message;
        public object PostData;
        public string ActionUrl;
        public string ReturnUrl;
        public bool IsUsed;
    }
}