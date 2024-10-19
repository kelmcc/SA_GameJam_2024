using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// A task that executes work on a new thread and returns a result. Tasks are thread-safe provided the work being done is thread safe itself.
    /// </summary>
    /// <typeparam name="T">The type of result that the task returns</typeparam>
    public class ThreadTask<T>
    {
        /// <summary>
        /// The result of the completed task. Throws an exception if the task is not yet completed.
        /// </summary>
        public T Result
        {
            get
            {
                if (_workDone) return _result;
                throw new UnityException("Thread work not complete.");
            }
        }

        /// <summary>
        /// Whether or not the thread has completed its work.
        /// </summary>
        public bool IsCompleted => _workDone;

        /// <summary>
        /// The thread that the work is performed on.
        /// </summary>
        public Thread Thread => _thread;

        private volatile bool _workDone = false;
        private Thread _thread;
        private T _result;

        /// <summary>
        /// Creates a new ThreadTask to execute work on a new thread. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <param name="work">The action to perform on the new thread</param>
        public ThreadTask(Func<T> work)
        {
            _thread = new Thread(RunFunc);
            _thread.Start(work);
        }

        /// <summary>
        /// Creates a new ThreadTask (that returns a result) to execute work on a new thread. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <typeparam name="T">The type of result that the work returns</typeparam>
        /// <param name="work">The action to perform on the new thread</param>
        /// <returns>The ThreadTask</returns>
        public static ThreadTask<T> Start(Func<T> work)
        {
            return new ThreadTask<T>(work);
        }

        /// <summary>
        /// Creates a new ThreadTask (that returns a result) to execute work on a new thread, and then invokes a callback when the task is complete. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <typeparam name="T">The type of result that the work returns</typeparam>
        /// <param name="work">The action to perform on the new thread</param>
        /// <param name="completionCallback">A callback that is invoked when the task is complete. Recieves the task result as a parameter</param>
        /// <returns>The ThreadTask</returns>
        public static ThreadTask<T> Start(Func<T> work, Action<T> completionCallback)
        {
            ThreadTask<T> task = new ThreadTask<T>(work);
            CoroutineUtils.StartCoroutine(WaitForTask(task, completionCallback));
            return task;
        }

        private void RunFunc(object work)
        {
            _result = ((Func<T>)work)();
            _workDone = true;
        }



        /// <summary>
        /// Starts a coroutine that yields until the task is complete.
        /// </summary>
        /// <returns>The "waiting" coroutine</returns>
        public Coroutine WaitForCompletion()
        {
            return CoroutineUtils.StartCoroutine(WhileWorkNotDone());
        }

        private IEnumerator WhileWorkNotDone()
        {
            while (!_workDone)
            {
                yield return null;
            }
        }

        protected static IEnumerator WaitForTask(ThreadTask<T> task, Action<T> callback)
        {
            yield return task.WaitForCompletion();
            callback(task.Result);
        }
    }

    /// <summary>
    /// A task that executes work on a new thread. Tasks are thread-safe provided the work being done is thread safe itself.
    /// </summary>
    public class ThreadTask
    {

        /// <summary>
        /// Whether or not the thread has completed its work.
        /// </summary>
        public bool IsCompleted => _workDone;

        /// <summary>
        /// The thread that the work is performed on.
        /// </summary>
        public Thread Thread => _thread;

        private volatile bool _workDone = false;
        private Thread _thread;


        /// <summary>
        /// Creates a new ThreadTask to execute work on a new thread. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <param name="work">The action to perform on the new thread</param>
        public ThreadTask(Action work)
        {
            _thread = new Thread(RunAction);
            _thread.Start(work);
        }

        /// <summary>
        /// Starts a coroutine that yields until the task is complete.
        /// </summary>
        /// <returns>The "waiting" coroutine</returns>
        public Coroutine WaitForCompletion()
        {
            return CoroutineUtils.StartCoroutine(WhileWorkNotDone());
        }

        private IEnumerator WhileWorkNotDone()
        {
            while (!_workDone)
            {
                yield return null;
            }
        }

        private void RunAction(object work)
        {
            ((Action)work)();
            _workDone = true;
        }


        /// <summary>
        /// Creates a new ThreadTask to execute work on a new thread. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <param name="work">The action to perform on the new thread</param>
        /// <returns>The ThreadTask</returns>
        public static ThreadTask Start(Action work)
        {
            return new ThreadTask(work);
        }

        /// <summary>
        /// Executes work on a new thread, and returns a coroutine that yields until the task is complete. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <param name="work">The action to perform on the new thread</param>
        /// <returns>A coroutine that will yield until the task is completed</returns>
        public static Coroutine StartAndWait(Action work)
        {
            return new ThreadTask(work).WaitForCompletion();
        }

        /// <summary>
        /// Creates a new ThreadTask to execute work on a new thread, and then invokes a callback when the task is complete. Tasks are thread-safe provided the work being done is thread safe itself.
        /// </summary>
        /// <param name="work">The action to perform on the new thread</param>
        /// <param name="completionCallback">A callback that is invoked when the task is complete</param>
        /// <returns>The ThreadTask</returns>
        public static ThreadTask Start(Action work, Action completionCallback)
        {
            ThreadTask task = new ThreadTask(work);
            CoroutineUtils.StartCoroutine(WaitForTask(task, completionCallback));
            return task;
        }

        private static IEnumerator WaitForTask(ThreadTask task, Action callback)
        {
            yield return task.WaitForCompletion();
            callback();
        }
    }
}