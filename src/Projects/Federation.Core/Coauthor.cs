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
    public partial class Coauthor
    {
        #region Primitive Properties
    
        public virtual System.Guid Id
        {
            get;
            set;
        }
    
        public virtual System.Guid UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    if (User != null && User.Id != value)
                    {
                        User = null;
                    }
                    _userId = value;
                }
            }
        }
        private System.Guid _userId;
    
        public virtual System.Guid PetitionId
        {
            get { return _petitionId; }
            set
            {
                if (_petitionId != value)
                {
                    if (Petition != null && Petition.Id != value)
                    {
                        Petition = null;
                    }
                    _petitionId = value;
                }
            }
        }
        private System.Guid _petitionId;
    
        public virtual Nullable<bool> IsAccepted
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual User User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private User _user;
    
        public virtual Petition Petition
        {
            get { return _petition; }
            set
            {
                if (!ReferenceEquals(_petition, value))
                {
                    var previousValue = _petition;
                    _petition = value;
                    FixupPetition(previousValue);
                }
            }
        }
        private Petition _petition;

        #endregion

        #region Association Fixup
    
        private void FixupUser(User previousValue)
        {
            if (previousValue != null && previousValue.Coauthors.Contains(this))
            {
                previousValue.Coauthors.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Coauthors.Contains(this))
                {
                    User.Coauthors.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }
    
        private void FixupPetition(Petition previousValue)
        {
            if (previousValue != null && previousValue.Coauthors.Contains(this))
            {
                previousValue.Coauthors.Remove(this);
            }
    
            if (Petition != null)
            {
                if (!Petition.Coauthors.Contains(this))
                {
                    Petition.Coauthors.Add(this);
                }
                if (PetitionId != Petition.Id)
                {
                    PetitionId = Petition.Id;
                }
            }
        }

        #endregion

    }
}
