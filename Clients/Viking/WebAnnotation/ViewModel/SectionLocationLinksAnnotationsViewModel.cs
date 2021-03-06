﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viking.Common;
using Geometry;
using System.Collections.Concurrent;
using Viking.VolumeModel;
using WebAnnotationModel;
using Viking.ViewModels;
using System.Diagnostics;

namespace WebAnnotation.ViewModel
{
    /// <summary>
    /// Track location links for a section
    /// </summary>
    class SectionLocationLinkAnnotationsViewModel
    {
        protected KeyTracker<LocationLinkKey> KnownLinks = new KeyTracker<LocationLinkKey>();

        /// <summary>
        /// The ID's of locations on the adjacent section which we know are linked and overlapped.
        /// </summary>
        public RefCountingKeyTracker<long> OverlappedAdjacentLocationIDs = new RefCountingKeyTracker<long>();

        protected ConcurrentDictionary<LocationLinkKey, LocationLinkView> LocationLinks = new ConcurrentDictionary<LocationLinkKey, LocationLinkView>();

        public SortedSet<LocationLinkKey> OverlappedLinkKeys = new SortedSet<LocationLinkKey>();
        public RTree.RTree<LocationLinkKey> NonOverlappedLinksSearch = new RTree.RTree<LocationLinkKey>();

        /// <summary>
        /// The section that we represent links on the canvas for
        /// </summary>
        public SectionViewModel Section;

        public SectionLocationLinkAnnotationsViewModel(SectionViewModel section)
        {
            this.Section = section;
        }

        public void AddLocationLinks(IEnumerable<LocationObj> locations)
        {
            locations.ForEach(loc => AddLocationLinks(loc, true));
        }

        public void RemoveLocationLinks(IEnumerable<LocationObj> locations)
        {
            locations.ForEach(loc => RemoveLocationLinks(loc, true));
        }

        public void AddLocationLinks(IEnumerable<LocationLinkKey> links)
        {
            links.ForEach(link => AddLocationLink(link, true));
        }

        public void RemoveLocationLinks(IEnumerable<LocationLinkKey> links)
        {
            links.ForEach(link => RemoveLocationLink(link, true));
        }

        protected void AddLocationLinks(LocationObj loc, bool subscribe)
        {
            foreach (long linkedID in loc.LinksCopy)
            {
                LocationLinkKey linkKey = new LocationLinkKey(loc.ID, linkedID);
                AddLocationLink(linkKey, subscribe);
            }
        }

        protected void RemoveLocationLinks(LocationObj loc, bool unsubscribe)
        {
            foreach (long linkedID in loc.LinksCopy)
            {
                LocationLinkKey linkKey = new LocationLinkKey(loc.ID, linkedID);
                RemoveLocationLink(linkKey, unsubscribe);
            }
        }

        protected void AddLocationLink(LocationLinkKey key, bool subscribe)
        {
            if (!(Store.Locations.Contains(key.A) && Store.Locations.Contains(key.B)))
                return;

            LocationObj AOBj = Store.Locations.Contains(key.A) ? Store.Locations[key.A] : null;
            LocationObj BOBj = Store.Locations.Contains(key.B) ? Store.Locations[key.B] : null;

            if (AOBj == null || BOBj == null)
                return;

            if (!(AOBj.Z == this.Section.Number || BOBj.Z == this.Section.Number))
            {
                return;
            }

            KnownLinks.TryAdd(key, () =>
            {
                LocationLinkView lv = new LocationLinkView(key, this.Section.Number, this.Section.VolumeViewModel);
                bool added = LocationLinks.TryAdd(key, lv);
                Debug.Assert(added);

                if (lv.LinksOverlap())
                {
                    OverlappedLinkKeys.Add(key);
                    OverlappedAdjacentLocationIDs.AddRef(key.A);
                    OverlappedAdjacentLocationIDs.AddRef(key.B);
                }
                else
                {
                    NonOverlappedLinksSearch.Add(lv.BoundingBox.ToRTreeRect(lv.Z), key);
                }
            });
        }

        protected void RemoveLocationLink(LocationLinkKey key, bool unsubscribe)
        {
            KnownLinks.TryRemove(key, () =>
            {
                LocationLinkView lv;
                Debug.Assert(LocationLinks.ContainsKey(key));
                bool removed = LocationLinks.TryRemove(key, out lv);
                Debug.Assert(removed);

                if (OverlappedLinkKeys.Contains(key))
                {
                    OverlappedLinkKeys.Remove(key);
                    OverlappedAdjacentLocationIDs.ReleaseRef(key.A);
                    OverlappedAdjacentLocationIDs.ReleaseRef(key.B);
                }

                if (NonOverlappedLinksSearch.Contains(key))
                {
                    LocationLinkKey removedKey;
                    NonOverlappedLinksSearch.Delete(key, out removedKey);
                }
            });
        }

        public List<HitTestResult> GetAnnotationsAtPosition(GridVector2 WorldPosition)
        {
            IEnumerable<LocationLinkKey> intersecting_IDs = NonOverlappedLinksSearch.Intersects(WorldPosition.ToRTreeRect(this.Section.Number));
            IEnumerable<LocationLinkView> intersecting_objs = intersecting_IDs.Select(id => LocationLinks[id]).Where(l => l.Intersects(WorldPosition));

            return new List<HitTestResult>(intersecting_objs.Select(l => new HitTestResult(l, this.Section.Number, l.DistanceFromCenterNormalized(WorldPosition)))).ToList();
        }

        private List<LocationLinkView> KeysToViews(ICollection<LocationLinkKey> listKeys)
        {
            List<LocationLinkView> listLocLinkView = new List<LocationLinkView>(listKeys.Count);
            foreach (LocationLinkKey linkKey in listKeys)
            {
                LocationLinkView locLinkView = null;
                if (this.LocationLinks.TryGetValue(linkKey, out locLinkView))
                {
                    listLocLinkView.Add(locLinkView);
                }
            }

            return listLocLinkView;
        }

        public ICollection<LocationLinkView> NonOverlappedLinks
        {
            get
            {
                return KeysToViews(NonOverlappedLinksSearch.Items);
            }
        }

        public ICollection<LocationLinkView> NonOverlappedLinksInRegion(GridRectangle region)
        {
            List<LocationLinkKey> listKeys = NonOverlappedLinksSearch.Intersects(region.ToRTreeRect(this.Section.Number));
            return KeysToViews(listKeys); 
        }

        public ICollection<LocationLinkView> GetLocationLinks(GridVector2 point)
        {
            List<LocationLinkKey> intersectingIDs = NonOverlappedLinksSearch.Intersects(point.ToRTreeRect((float)this.Section.Number));
            return intersectingIDs.Select(id =>
            {
                if (LocationLinks.ContainsKey(id))
                    return LocationLinks[id];
                return null;
            }
            ).Where(l => l != null && l.Intersects(point)).ToList();
        }
    }
}
