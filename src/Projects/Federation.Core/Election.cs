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
    public partial class Election : Voting
    {
        #region Primitive Properties
    
        public virtual int Quorum
        {
            get;
            set;
        }
    
        public virtual short AgitationDuration
        {
            get;
            set;
        }
    
        public virtual byte Stage
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<ElectionBulletin> ElectionBulletins
        {
            get
            {
                if (_electionBulletins == null)
                {
                    var newCollection = new FixupCollection<ElectionBulletin>();
                    newCollection.CollectionChanged += FixupElectionBulletins;
                    _electionBulletins = newCollection;
                }
                return _electionBulletins;
            }
            set
            {
                if (!ReferenceEquals(_electionBulletins, value))
                {
                    var previousValue = _electionBulletins as FixupCollection<ElectionBulletin>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupElectionBulletins;
                    }
                    _electionBulletins = value;
                    var newValue = value as FixupCollection<ElectionBulletin>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupElectionBulletins;
                    }
                }
            }
        }
        private ICollection<ElectionBulletin> _electionBulletins;
    
        public virtual ICollection<Candidate> Candidates
        {
            get
            {
                if (_candidates == null)
                {
                    var newCollection = new FixupCollection<Candidate>();
                    newCollection.CollectionChanged += FixupCandidates;
                    _candidates = newCollection;
                }
                return _candidates;
            }
            set
            {
                if (!ReferenceEquals(_candidates, value))
                {
                    var previousValue = _candidates as FixupCollection<Candidate>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCandidates;
                    }
                    _candidates = value;
                    var newValue = value as FixupCollection<Candidate>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCandidates;
                    }
                }
            }
        }
        private ICollection<Candidate> _candidates;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupElectionBulletins(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ElectionBulletin item in e.NewItems)
                {
                    item.Election = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ElectionBulletin item in e.OldItems)
                {
                    if (ReferenceEquals(item.Election, this))
                    {
                        item.Election = null;
                    }
                }
            }
        }
    
        private void FixupCandidates(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Candidate item in e.NewItems)
                {
                    item.Election = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Candidate item in e.OldItems)
                {
                    if (ReferenceEquals(item.Election, this))
                    {
                        item.Election = null;
                    }
                }
            }
        }

        #endregion

    }
}
