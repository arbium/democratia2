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
    public partial class Candidate
    {
        #region Primitive Properties
    
        public virtual System.Guid Id
        {
            get;
            set;
        }
    
        public virtual byte Status
        {
            get;
            set;
        }
    
        public virtual System.Guid ElectionId
        {
            get { return _electionId; }
            set
            {
                if (_electionId != value)
                {
                    if (Election != null && Election.Id != value)
                    {
                        Election = null;
                    }
                    _electionId = value;
                }
            }
        }
        private System.Guid _electionId;

        #endregion

        #region Navigation Properties
    
        public virtual GroupMember GroupMember
        {
            get { return _groupMember; }
            set
            {
                if (!ReferenceEquals(_groupMember, value))
                {
                    var previousValue = _groupMember;
                    _groupMember = value;
                    FixupGroupMember(previousValue);
                }
            }
        }
        private GroupMember _groupMember;
    
        public virtual ICollection<ElectionBulletin> Electorate
        {
            get
            {
                if (_electorate == null)
                {
                    var newCollection = new FixupCollection<ElectionBulletin>();
                    newCollection.CollectionChanged += FixupElectorate;
                    _electorate = newCollection;
                }
                return _electorate;
            }
            set
            {
                if (!ReferenceEquals(_electorate, value))
                {
                    var previousValue = _electorate as FixupCollection<ElectionBulletin>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupElectorate;
                    }
                    _electorate = value;
                    var newValue = value as FixupCollection<ElectionBulletin>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupElectorate;
                    }
                }
            }
        }
        private ICollection<ElectionBulletin> _electorate;
    
        public virtual Election Election
        {
            get { return _election; }
            set
            {
                if (!ReferenceEquals(_election, value))
                {
                    var previousValue = _election;
                    _election = value;
                    FixupElection(previousValue);
                }
            }
        }
        private Election _election;
    
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
    
        private void FixupGroupMember(GroupMember previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.Candidate, this))
            {
                previousValue.Candidate = null;
            }
    
            if (GroupMember != null)
            {
                GroupMember.Candidate = this;
            }
        }
    
        private void FixupElection(Election previousValue)
        {
            if (previousValue != null && previousValue.Candidates.Contains(this))
            {
                previousValue.Candidates.Remove(this);
            }
    
            if (Election != null)
            {
                if (!Election.Candidates.Contains(this))
                {
                    Election.Candidates.Add(this);
                }
                if (ElectionId != Election.Id)
                {
                    ElectionId = Election.Id;
                }
            }
        }
    
        private void FixupPetition(Petition previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.Candidate, this))
            {
                previousValue.Candidate = null;
            }
    
            if (Petition != null)
            {
                Petition.Candidate = this;
            }
        }
    
        private void FixupElectorate(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ElectionBulletin item in e.NewItems)
                {
                    if (!item.Result.Contains(this))
                    {
                        item.Result.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ElectionBulletin item in e.OldItems)
                {
                    if (item.Result.Contains(this))
                    {
                        item.Result.Remove(this);
                    }
                }
            }
        }

        #endregion

    }
}
