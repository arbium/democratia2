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
    public partial class AlbumItem
    {
        #region Primitive Properties
    
        public virtual System.Guid Id
        {
            get;
            set;
        }
    
        public virtual System.Guid AlbumId
        {
            get { return _albumId; }
            set
            {
                if (_albumId != value)
                {
                    if (Album != null && Album.Id != value)
                    {
                        Album = null;
                    }
                    _albumId = value;
                }
            }
        }
        private System.Guid _albumId;
    
        public virtual string Title
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description = "";
    
        public virtual byte Type
        {
            get;
            set;
        }
    
        public virtual string Src
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreationDate
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual Album Album
        {
            get { return _album; }
            set
            {
                if (!ReferenceEquals(_album, value))
                {
                    var previousValue = _album;
                    _album = value;
                    FixupAlbum(previousValue);
                }
            }
        }
        private Album _album;
    
        public virtual ICollection<Like> Likes
        {
            get
            {
                if (_likes == null)
                {
                    var newCollection = new FixupCollection<Like>();
                    newCollection.CollectionChanged += FixupLikes;
                    _likes = newCollection;
                }
                return _likes;
            }
            set
            {
                if (!ReferenceEquals(_likes, value))
                {
                    var previousValue = _likes as FixupCollection<Like>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupLikes;
                    }
                    _likes = value;
                    var newValue = value as FixupCollection<Like>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupLikes;
                    }
                }
            }
        }
        private ICollection<Like> _likes;

        #endregion

        #region Association Fixup
    
        private void FixupAlbum(Album previousValue)
        {
            if (previousValue != null && previousValue.Items.Contains(this))
            {
                previousValue.Items.Remove(this);
            }
    
            if (Album != null)
            {
                if (!Album.Items.Contains(this))
                {
                    Album.Items.Add(this);
                }
                if (AlbumId != Album.Id)
                {
                    AlbumId = Album.Id;
                }
            }
        }
    
        private void FixupLikes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Like item in e.NewItems)
                {
                    item.AlbumItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Like item in e.OldItems)
                {
                    if (ReferenceEquals(item.AlbumItem, this))
                    {
                        item.AlbumItem = null;
                    }
                }
            }
        }

        #endregion

    }
}
