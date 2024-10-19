using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Utility class for common coroutine usages.
    /// </summary>
    public static class CoroutineUtils
    {

        class CorooutineHolder : MonoBehaviour
        {

        }

        private static CorooutineHolder _coroutineHolder;

        private static CorooutineHolder CoroutineHolder
        {
            get
            {
                if (_coroutineHolder == null)
                {
                    CreateCoroutineHolder();
                }
                return _coroutineHolder;
            }
        }

        //Creates a special MonoBehaviour just to host coroutines
        private static void CreateCoroutineHolder()
        {
            _coroutineHolder = new GameObject("Coroutine Holder").AddComponent<CorooutineHolder>();
            _coroutineHolder.gameObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(_coroutineHolder.gameObject);
        }

        /// <summary>
        /// Starts a coroutine on the CoroutineUtils GameObject, useful for starting coroutines from non MonoBehaviour classes
        /// </summary>
        /// <param name="routine">The coroutine to start</param>
        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return CoroutineHolder.StartCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine routine)
        {
            CoroutineHolder.StopCoroutine(routine);
        }

        /// <summary>
        /// Waits until a condition is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator WaitUntilTrue(System.Func<bool> condition)
        {
            while (!condition())
            {
                yield return null;
            }
        }

        /// <summary>
        /// Waits while a condition is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator WaitWhileTrue(System.Func<bool> condition)
        {
            while (condition())
            {
                yield return null;
            }
        }

        /// <summary>
        /// Performs an action when a condition becomes true.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator WhenTrue(System.Func<bool> condition, System.Action action)
        {
            while (!condition())
            {
                yield return null;
            }
            action();
        }


        /// <summary>
        /// Performs an action in the next frame.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator OnNextFrame(System.Action action)
        {
            yield return null;
            action();
        }


        /// <summary>
        /// Performs an action after some time has passed.
        /// </summary>
        /// <param name="seconds">The amount of time to wait</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator AfterSeconds(float seconds, System.Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }


        /// <summary>
        /// Waits for some unscaled time to pass.
        /// </summary>
        /// <param name="seconds">The amount of time to wait</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator WaitForRealSeconds(float seconds)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < seconds)
            {
                yield return null;
            }
        }


        /// <summary>
        /// Performs an action after some unscaled time has passed.
        /// </summary>
        /// <param name="seconds">The amount of time to wait</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator AfterRealSeconds(float seconds, System.Action action)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < seconds)
            {
                yield return null;
            }
            action();
        }



        /// <summary>
        /// Waits for an animation to stop playing.
        /// </summary>
        /// <param name="animation">The animation</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator WaitForAnimation(Animation animation)
        {
            while (animation != null && animation.isPlaying)
            {
                yield return null;
            }
        }



        /// <summary>
        /// Performs an action after an animation has stopped playing.
        /// </summary>
        /// <param name="animation">The animation</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The coroutine enumerator</returns>
        public static IEnumerator AfterAnimation(Animation animation, System.Action action)
        {
            while (animation != null && animation.isPlaying)
            {
                yield return null;
            }
            action();
        }

        public static void InvokeDelayed(Action action, float delay, bool unscaledTime = false)
        {
            CoroutineHolder.StartCoroutine(InvokeDelayedRoutine(action, delay, unscaledTime));
        }

        static IEnumerator InvokeDelayedRoutine(Action action, float delay, bool unscaledTime)
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            action();
        }

    }
}
