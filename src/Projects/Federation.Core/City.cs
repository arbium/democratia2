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
    public partial class City
    {
        #region Primitive Properties
    
        public virtual System.Guid Id
        {
            get;
            set;
        }
    
        public virtual System.Guid RegionId
        {
            get { return _regionId; }
            set
            {
                if (_regionId != value)
                {
                    if (Region != null && Region.Id != value)
                    {
                        Region = null;
                    }
                    _regionId = value;
                }
            }
        }
        private System.Guid _regionId;
    
        public virtual string Title
        {
            get;
            set;
        }
    
        public virtual byte Type
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<Address> Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    var newCollection = new FixupCollection<Address>();
                    newCollection.CollectionChanged += FixupAddresses;
                    _addresses = newCollection;
                }
                return _addresses;
            }
            set
            {
                if (!ReferenceEquals(_addresses, value))
                {
                    var previousValue = _addresses as FixupCollection<Address>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAddresses;
                    }
                    _addresses = value;
                    var newValue = value as FixupCollection<Address>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAddresses;
                    }
                }
            }
        }
        private ICollection<Address> _addresses;
    
        public virtual Region Region
        {
            get { return _region; }
            set
            {
                if (!ReferenceEquals(_region, value))
                {
                    var previousValue = _region;
                    _region = value;
                    FixupRegion(previousValue);
                }
            }
        }
        private Region _region;

        #endregion

        #region Association Fixup
    
        private void FixupRegion(Region previousValue)
        {
            if (previousValue != null && previousValue.Cities.Contains(this))
            {
                previousValue.Cities.Remove(this);
            }
    
            if (Region != null)
            {
                if (!Region.Cities.Contains(this))
                {
                    Region.Cities.Add(this);
                }
                if (RegionId != Region.Id)
                {
                    RegionId = Region.Id;
                }
            }
        }
    
        private void FixupAddresses(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Address item in e.NewItems)
                {
                    item.City = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Address item in e.OldItems)
                {
                    if (ReferenceEquals(item.City, this))
                    {
                        item.City = null;
                    }
                }
            }
        }

        #endregion

    }
}