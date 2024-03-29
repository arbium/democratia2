//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Federation.Core
{
    public partial class SmsInfo
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string Phone
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreationDate
        {
            get;
            set;
        }
    
        public virtual System.DateTime ChangeDate
        {
            get;
            set;
        }
    
        public virtual double Cost
        {
            get;
            set;
        }
    
        public virtual Nullable<System.Guid> BaseUserId
        {
            get { return _baseUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_baseUserId != value)
                    {
                        if (BaseUser != null && BaseUser.Id != value)
                        {
                            BaseUser = null;
                        }
                        _baseUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _baseUserId;
    
        public virtual string Message
        {
            get;
            set;
        }
    
        public virtual Nullable<byte> ResponseError
        {
            get;
            set;
        }
    
        public virtual Nullable<byte> SendError
        {
            get;
            set;
        }
    
        public virtual byte State
        {
            get;
            set;
        }
    
        public virtual byte PartsCount
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual BaseUser BaseUser
        {
            get { return _baseUser; }
            set
            {
                if (!ReferenceEquals(_baseUser, value))
                {
                    var previousValue = _baseUser;
                    _baseUser = value;
                    FixupBaseUser(previousValue);
                }
            }
        }
        private BaseUser _baseUser;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupBaseUser(BaseUser previousValue)
        {
            if (previousValue != null && previousValue.SmsInfos.Contains(this))
            {
                previousValue.SmsInfos.Remove(this);
            }
    
            if (BaseUser != null)
            {
                if (!BaseUser.SmsInfos.Contains(this))
                {
                    BaseUser.SmsInfos.Add(this);
                }
                if (BaseUserId != BaseUser.Id)
                {
                    BaseUserId = BaseUser.Id;
                }
            }
            else if (!_settingFK)
            {
                BaseUserId = null;
            }
        }

        #endregion

    }
}
