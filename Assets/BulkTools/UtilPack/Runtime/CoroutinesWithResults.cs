using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BTools.UtilPack
{
    public static class CoroutinesWithResults
    {
        /// <summary>
        /// Starts a coroutine that can return results via the object returned here.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="routine"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Starts a coroutine that can return results via this object.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="routine"></param>
        public CoroutineWithResults(MonoBehaviour owner, IEnumerator<T> routine)
        {
            this.owner = owner;
            this.routine = routine;
            this.MyCoroutine = owner.StartCoroutine(Run());
        }

        /// <summary>
        /// A standard unity coroutine that manages the custom coroutine logic
        /// </summary>
        /// <returns></returns>
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
        }

        /// <summary>
        /// Stops the coroutine
        /// </summary>
        public void StopCoroutine()
        {
            if (MyCoroutine == null) { return; }
            owner.StopCoroutine(MyCoroutine);
        }
    }
}