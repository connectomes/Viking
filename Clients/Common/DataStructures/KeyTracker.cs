﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.Common
{

    /// <summary>
    /// A thread-safe class that maintains a list of IDs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyTracker<T> where T : IComparable<T>
    {
        private System.Threading.ReaderWriterLockSlim rwKnownLocationsLock = new System.Threading.ReaderWriterLockSlim();

        private SortedSet<T> TrackedKeys = new SortedSet<T>();

        public IEnumerable<T> ValuesCopy()
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                return new SortedSet<T>(TrackedKeys);
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        public bool Contains(T ID)
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                return TrackedKeys.Contains(ID);
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        public int Count()
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                return TrackedKeys.Count();
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add the ID.  If it is added execute the action.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool TryAdd(T ID, Action a = null)
        {
            try
            {
                rwKnownLocationsLock.EnterUpgradeableReadLock();
                if (TrackedKeys.Contains(ID))
                    return false;

                try
                {
                    rwKnownLocationsLock.EnterWriteLock();
                    if (TrackedKeys.Contains(ID))
                        return false;

                    TrackedKeys.Add(ID);
                    if (a != null)
                    {
                        try
                        {
                            a();
                        }
                        catch
                        {
                            TrackedKeys.Remove(ID);
                            throw;
                        }
                    }
                        
                }
                finally
                {
                    rwKnownLocationsLock.ExitWriteLock();
                }
                return TrackedKeys.Contains(ID);
            }
            finally
            {
                rwKnownLocationsLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Add the ID if it is not in the set and the function returns true
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool TryAdd(T ID, Func<bool> CanAdd)
        {
            try
            {
                rwKnownLocationsLock.EnterUpgradeableReadLock();
                if (TrackedKeys.Contains(ID))
                    return false;

                try
                {
                    rwKnownLocationsLock.EnterWriteLock();
                    if (TrackedKeys.Contains(ID))
                        return false;
                    
                    if (CanAdd())
                    {
                        TrackedKeys.Add(ID);
                    } 
                }
                finally
                {
                    rwKnownLocationsLock.ExitWriteLock();
                }
                return TrackedKeys.Contains(ID);
            }
            finally
            {
                rwKnownLocationsLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Try to remove the ID.  If the ID is removed execute the action
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool TryRemove(T ID, Action a = null)
        {
            try
            {
                rwKnownLocationsLock.EnterUpgradeableReadLock();
                if (!TrackedKeys.Contains(ID))
                    return false;

                try
                {
                    rwKnownLocationsLock.EnterWriteLock();
                    TrackedKeys.Remove(ID);
                    if (a != null)
                        a();
                }
                finally
                {
                    rwKnownLocationsLock.ExitWriteLock();
                }
                return TrackedKeys.Contains(ID);
            }
            finally
            {
                rwKnownLocationsLock.ExitUpgradeableReadLock();
            }
        }
    }
    
    
    /// <summary>
    /// A thread-safe class that maintains a count of references for a list of IDs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RefCountingKeyTracker<T> where T : IComparable<T>
    {
        private System.Threading.ReaderWriterLockSlim rwKnownLocationsLock = new System.Threading.ReaderWriterLockSlim();

        private SortedDictionary<T, int> TrackedKeys = new SortedDictionary<T,int>();
        public bool Contains(T ID)
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                return TrackedKeys.ContainsKey(ID);
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        public int Count()
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                return TrackedKeys.Count();
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        protected int RefCount(T ID)
        {
            try
            {
                rwKnownLocationsLock.EnterReadLock();
                if(!TrackedKeys.ContainsKey(ID))
                {
                    return 0;
                }
                else
                {
                    return TrackedKeys[ID];
                }
            }
            finally
            {
                rwKnownLocationsLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add the ID.  If it is added execute the action.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="a">Action to execute if this is the first reference taken to the key</param>
        /// <returns></returns>
        public void AddRef(T ID, Action<T> OnFirstReferenceAction = null)
        {
            try
            {
                rwKnownLocationsLock.EnterWriteLock();

                int RefCount;
                if (TrackedKeys.ContainsKey(ID))
                {
                    RefCount = TrackedKeys[ID];
                }
                else
                {
                    RefCount = 0;
                    TrackedKeys.Add(ID, 0);
                }

                if (RefCount == 0 && OnFirstReferenceAction != null)
                {
                    try
                    {
                        OnFirstReferenceAction(ID);
                    }
                    catch
                    {
                        TrackedKeys.Remove(ID);
                        throw;
                    }
                }
                    

                RefCount++;

                TrackedKeys[ID] = RefCount;
            }
            finally
            {
                rwKnownLocationsLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Try to remove the ID.  If the ID is removed execute the action
        /// </summary>
        /// <param name="ID">Key to release</param>
        /// <param name="a">Action to take if this removes the last refe</param>
        /// <returns>True if the key was present</returns>
        public bool ReleaseRef(T ID, Action<T> OnLastReferenceReleasedAction = null)
        { 
            try
            {
                rwKnownLocationsLock.EnterWriteLock();

                if (!TrackedKeys.ContainsKey(ID))
                    return false;

                int RefCount = TrackedKeys[ID];
                RefCount -= 1;

                if (RefCount == 0)
                {
                    if (OnLastReferenceReleasedAction != null)
                    {
                        OnLastReferenceReleasedAction(ID);
                    }

                    TrackedKeys.Remove(ID);
                }
                else
                {
                    TrackedKeys[ID] = RefCount;
                }

                return true;
            }
            finally
            {
                rwKnownLocationsLock.ExitWriteLock();
            }
        }
    }
}
