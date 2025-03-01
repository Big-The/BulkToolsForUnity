using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.ObjectPooling
{
    public class ObjectPool : MonoBehaviour
    {
        public enum OverDrawMode
        {
            [Tooltip("Will reuse the oldest object in already in use.")]
            UseOldest,
            [Tooltip("Will reuse the most recent object in already in use.")]
            UseYoungest,
            [Tooltip("Temporarily creates objects normaly to fufill the overdraw request")]
            DynamicResize,
            [Tooltip("Permanetly increases the pools size")]
            PermanentResize
        }

        public GameObject pooledObjectPrefab;
        public int poolSize = 10;
        [Tooltip("What to do when the pool is overdrawn.")]
        public OverDrawMode overDrawMode = OverDrawMode.UseOldest;
        [Tooltip("Should the object be reset automaticaly after a set time")]
        public bool autoDespawn = false;
        public float timeTillDespawn = 10f;
        private List<int> activeIDs = new List<int>();
        private List<int> inactiveIDs = new List<int>();

        private List<GameObject> pool = new List<GameObject>();

        //populate the pool
        void Awake()
        {
            if (poolSize < 1) { poolSize = 1; }
            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(CreateNewObject(i));
            }
            for (int i = 0; i < pool.Count; i++)
            {
                inactiveIDs.Add(i);
            }
        }

        /// <summary>
        /// Get the next object from the pool
        /// </summary>
        /// <returns></returns>
        public GameObject PullFromPool()
        {
            if (activeIDs.Count == pool.Count)//check if the pool is fully used
            {
                //deal with overdraw
                int idToUse = -1;
                GameObject newGO = null;
                switch (overDrawMode)
                {
                    case OverDrawMode.UseOldest:
                        idToUse = activeIDs[0];
                        activeIDs.RemoveAt(0);
                        activeIDs.Add(idToUse);
                        pool[idToUse].SetActive(true);
                        pool[idToUse].GetComponent<PoolOwned>().StartDespawn();
                        return pool[idToUse];

                    case OverDrawMode.UseYoungest:
                        idToUse = activeIDs[activeIDs.Count - 1];
                        pool[idToUse].SetActive(true);
                        pool[idToUse].GetComponent<PoolOwned>().StartDespawn();
                        return pool[idToUse];

                    case OverDrawMode.DynamicResize:
                        idToUse = activeIDs.Count;
                        newGO = CreateNewObject(idToUse);
                        pool.Add(newGO);
                        activeIDs.Add(idToUse);
                        pool[idToUse].SetActive(true);
                        pool[idToUse].GetComponent<PoolOwned>().StartDespawn();
                        return pool[idToUse];

                    case OverDrawMode.PermanentResize:
                        idToUse = activeIDs.Count;
                        newGO = CreateNewObject(idToUse);
                        pool.Add(newGO);
                        activeIDs.Add(idToUse);
                        pool[idToUse].SetActive(true);
                        pool[idToUse].GetComponent<PoolOwned>().StartDespawn();
                        return pool[idToUse];
                }
            }
            else
            {
                //simple pull from pool
                int idToUse = inactiveIDs[0];
                inactiveIDs.RemoveAt(0);
                activeIDs.Add(idToUse);
                pool[idToUse].SetActive(true);
                pool[idToUse].GetComponent<PoolOwned>().StartDespawn();
                return pool[idToUse];
            }
            return null;
        }

        /// <summary>
        /// Use this method in replace of Destroy on objects that may be used in pools
        /// </summary>
        public static void DestroyOrRepool(GameObject toDestroy)
        {
            PoolOwned po = toDestroy.GetComponent<PoolOwned>();
            if (!po) { Destroy(toDestroy); return; }//if it's not owned we just call normal destroy and move on
            po.StopAllCoroutines();
            toDestroy.SetActive(false);
            po.myPool.activeIDs.Remove(po.poolObjectID);

            //if this object is an overdraw item and we are in dynamic resize mode we need to destroy it
            if (po.myPool.overDrawMode == OverDrawMode.DynamicResize)
            {
                if (po.poolObjectID >= po.myPool.poolSize)
                {
                    po.myPool.pool.Remove(toDestroy);
                    po.myPool = null;
                    Destroy(toDestroy);
                    return;//return here so we don't add the active id back to the inactive list
                }
            }

            po.transform.SetParent(po.myPool.transform);
            po.myPool.inactiveIDs.Add(po.poolObjectID);
        }

        /// <summary>
        /// Makes sure the pool is properly populated
        /// </summary>
        public void ValidatePool()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                Debug.Log(pool[i].name);
                if (pool[i] == null)
                {
                    pool[i] = CreateNewObject(i);
                }
            }
        }

        /// <summary>
        /// Creates a new GameObject and places it into the pool at the specified index
        /// </summary>
        /// <param name="poolObjectID"></param>
        /// <returns></returns>
        internal GameObject CreateNewObject(int poolObjectID)
        {
            GameObject newGO = Instantiate(pooledObjectPrefab, transform);
            newGO.SetActive(false);
            newGO.name = $"{pooledObjectPrefab.name}({poolObjectID})";
            PoolOwned po = newGO.GetComponent<PoolOwned>();
            if (!po)
            {
                po = newGO.AddComponent<PoolOwned>();
            }
            po.myPool = this;
            po.poolObjectID = poolObjectID;
            if (autoDespawn) { po.timeTillDespawn = timeTillDespawn; }
            return newGO;
        }
    }
}