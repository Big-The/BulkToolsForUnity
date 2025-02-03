using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BTools.UtilPack
{
    public static class CoroutinesWithResults
    {
        public static CoroutineWithResults<T> StartCoroutineWithResults<T>(this MonoBehaviour owner, IEnumerator<T> routine)
        {
            return new CoroutineWithResults<T>(owner, routine);
        }
    }

    public class CoroutineWithResults<T>
    {
        public Coroutine MyCoroutine { get; private set; }
        public T Current { get; private set; }
        public bool IsDone { get; private set; }
        public UnityEvent<T> onDone = new UnityEvent<T>();
        private IEnumerator<T> routine;
        private MonoBehaviour owner;

        public CoroutineWithResults(MonoBehaviour owner, IEnumerator<T> routine)
        {
            this.owner = owner;
            this.routine = routine;
            this.MyCoroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (routine.MoveNext())
            {
                if (routine.Current != null && typeof(T).IsAssignableFrom(routine.Current.GetType())) 
                {
                    Current = routine.Current;
                }
                yield return Current;
            }
            IsDone = true;
            onDone.Invoke(Current);
            Debug.Log("Run Fin");
        }

        public void StopCoroutine()
        {
            if (MyCoroutine == null) { return; }
            owner.StopCoroutine(MyCoroutine);
        }
    }
}