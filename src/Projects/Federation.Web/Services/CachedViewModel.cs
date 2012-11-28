using System;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web
{
    public abstract class CachedViewModel : ICachedViewModel
    {
        private DateTime _timeStamp = DateTime.Now;
        private DateTime _lastAccess = DateTime.Now;
        private TimeSpan _dropPeriod = TimeSpan.FromHours(1);
        private Guid _userId = UserContext.Current != null ? UserContext.Current.Id : Guid.Empty;
        private string _url = HttpContext.Current.Request.Url.ToString();
        private string _label = "";
        private Guid _relativeModelId = Guid.Empty;

        public string CachKey
        {
            get { return _url + (CachUserRelated ? ("_userId:" + _userId.ToString()) : "") + (!string.IsNullOrWhiteSpace(_label) ? "_label:" + _label : ""); }
        }

        public string CachLabel
        {
            get { return _label; }
            set { _label = value; }
        }

        public Guid CachUserId
        {
            get { return _userId; }
        }

        public bool CachIsOutOfDate()
        {
            return (_timeStamp + _dropPeriod < DateTime.Now) && (CachService.IsCachOutOfDate(CachKey));
        }

        public TimeSpan CachDropPeriod
        {
            get { return _dropPeriod; }
            set { _dropPeriod = value; }
        }

        public DateTime CachLastAccess
        {
            get { return _lastAccess; }
            set { _lastAccess = value; }
        }

        public bool CachUserRelated { get; set; }

        public virtual ICachedViewModel Initialize()
        {
            return this;
        }

        public Guid RelativeModelId
        {
            get { return _relativeModelId; }
            set { _relativeModelId = value; }
        }
    }
}
