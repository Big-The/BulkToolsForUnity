using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BTools.ObjectPooling
{
    public class PoolOwned : MonoBehaviour
    {
        [HideInInspector]
        public ObjectPool myPool;
        [HideInInspector]
        public int poolObjectID = -1;
        [HideInInspector]
        public float timeTillDespawn = -1;

        public void StartDespawn()
        {
            if (timeTillDespawn > 0)
            {
                StartCoroutine(Despawn());
            }
        }

        IEnumerator Despawn()
        {
            yield return new WaitForSeconds(timeTillDespawn);
            ObjectPool.DestroyOrRepool(gameObject);
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded) //Was Deleted
            {
                if (myPool)
                {
                    Debug.LogError("Pooled object destroyed. Try using ObjectPool.DestroyOrRepool() instead.");
                    myPool.CreateNewObject(poolObjectID);
                }
            }
        }
    }
}
