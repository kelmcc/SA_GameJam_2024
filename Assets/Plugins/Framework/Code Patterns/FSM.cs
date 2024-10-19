using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    public class FSMBehaviour<T> : MonoBehaviour where T : struct, IConvertible, IComparable
    {
        /// <summary>
        /// The current state.
        /// </summary>
        public T State
        {
            get => FSM.State;
            set => FSM.ChangeState(value);
        }

        /// <summary>
        /// The previous state. Can be default if the state has never changed.
        /// </summary>
        public T PreviousState => FSM.PreviousState;

        private FSM<T> FSM
        {
            get
            {
                if (_fsm == null)
                {
                    _fsm = new FSM<T>(this, true, false);
                }

                return _fsm;
            }
        }

        private FSM<T> _fsm;

        /// <summary>
        /// Changes the FSM's active state.
        /// </summary>
        /// <param name="newState">The new state to change to</param>
        public void ChangeState(T newState)
        {
            FSM.ChangeState(newState);
        }

        protected virtual void OnEnable()
        {
            FSM.Start();
        }

        protected virtual void OnDisable()
        {
            FSM.Stop();
        }
    }


    /// <summary>
    /// A finite state machine that automatically calls methods for states.
    /// Supports Update (void or coroutine), Enter and Exit methods.
    /// State method delegates must either be registered manually (using RegisterState()), or they can be auto-registered reflectively by method name.
    /// Eg: WalkingUpdate, WalkingEnter and WalkingExit (for the Walking state), or alternatively Walking_Update, Walking_Enter and Walking_Exit
    /// </summary>
    /// <typeparam name="T">A state enum</typeparam>
    public class FSM<T> where T : struct, IConvertible, IComparable
    {
        /// <summary>
        /// A coroutine update method.
        /// </summary>
        /// <returns>The coroutine enumerator</returns>
        public delegate IEnumerator StateUpdateRoutine();

        //Internal structure for keeping track of state methods.
        struct StateMethods
        {
            public Action onStateEnter;
            public Action onStateExit;
            public Action onStateUpdate;
            public StateUpdateRoutine onStateUpdateRoutine;

            public StateMethods(Action onStateUpdate, StateUpdateRoutine onStateUpdateRoutine, Action onStateEnter, Action onStateExit)
            {
                this.onStateUpdate = onStateUpdate;
                this.onStateUpdateRoutine = onStateUpdateRoutine;
                this.onStateEnter = onStateEnter;
                this.onStateExit = onStateExit;
            }
        }

        public event Action<T> OnStateChanged;
        private Dictionary<T, StateMethods> _states = new Dictionary<T, StateMethods>();
        private MonoBehaviour _component;
        private bool _started;
        private bool _paused;
        private T _currentState;
        private T _previousState;
        private Coroutine _updateRoutine;

        /// <summary>
        /// The current state.
        /// </summary>
        public T State
        {
            get => _currentState;
            set => ChangeState(value);
        }

        /// <summary>
        /// The previous state. Can be default if the state has never changed.
        /// </summary>
        public T PreviousState => _previousState;

        /// <summary>
        /// Whether or not the FSM update routine has been started.
        /// </summary>
        public bool IsStarted => _started;

        /// <summary>
        /// Whether or not the FSM is currently updating (provided it has been started).
        /// </summary>
        public bool IsPaused => _paused;

        /// <summary>
        /// The number of states currently registered with the FSM.
        /// </summary>
        public int NumRegisteredStates => _states.Count;

        /// <summary>
        /// Pauses the state machine routine.
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Unpauses the state machine routine.
        /// </summary>
        public void UnPause()
        {
            _paused = false;
        }

        /// <summary>
        /// Starts the FSM so that it will begin updating for the current state.
        /// </summary>
        public void Start(bool waitUntilNextFrameToUpdate = true)
        {
            if (!_started)
            {
                _started = true;

                StateMethods stateMethods;
                if (_states.TryGetValue(_currentState, out stateMethods))
                {
                    if (stateMethods.onStateEnter != null)
                    {
                        stateMethods.onStateEnter();
                    }
                }

                _updateRoutine = _component.StartCoroutine(UpdateRoutine(waitUntilNextFrameToUpdate));
            }
        }

        /// <summary>
        /// Stops the FSM and pauses update execution. It can be restarted.
        /// </summary>
        public void Stop()
        {
            if (_started)
            {
                _started = false;

                if (_updateRoutine != null)
                {
                    _component.StopCoroutine(_updateRoutine);
                    _updateRoutine = null;
                }
            }
        }

        /// <summary>
        /// Creates a new finite state machine. By default it will start on the next frame, but can be started manually.
        /// </summary>
        /// <param name="component">The FSM needs a MonoBehavior to start the update coroutine on. This is also the object that will be reflected for auto-registration</param>
        /// <param name="autoRegister">Whether or not to automatically register the state delegates using reflection</param>
        public FSM(MonoBehaviour component, bool autoRegister = true, bool autoStart = true)
        {
            DebugUtils.AssertIsEnumType<T>();
            _component = component;

            if (autoRegister)
            {
                AutoRegister();
            }

            _currentState = default(T);

            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// Creates a new finite state machine.
        /// </summary>
        /// <param name="component">The FSM needs a MonoBehavior to start the update coroutine on. This is also the object that will be reflected with auto-registration</param>
        /// <param name="initialState">The starting state of the FSM</param>
        /// <param name="autoRegister">Whether or not to automatically register the state delegates using reflection</param>
        public FSM(MonoBehaviour component, T initialState, bool autoRegister = true, bool autoStart = true)
        {
            DebugUtils.AssertIsEnumType<T>();
            _component = component;

            if (autoRegister)
            {
                AutoRegister();
            }

            _currentState = initialState;

            if (autoStart)
            {
                Start();
            }
        }

        #region Registration Methods

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateRoutine">The method that is called every frame that this state is active</param>
        /// <param name="onStateEnterMethod">The method that is called when this state is entered</param>
        /// <param name="onStateExitMethod">The method that is called when this state is exited</param>
        public void RegisterState(T state, StateUpdateRoutine onUpdateRoutine, Action onStateEnterMethod, Action onStateExitMethod)
        {

            StateMethods stateMethods = new StateMethods(null, onUpdateRoutine, onStateEnterMethod, onStateExitMethod);
            if (_states.ContainsKey(state))
            {
                _states[state] = stateMethods;
            }
            else
            {
                _states.Add(state, stateMethods);
            }
        }

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateRoutine">The method that is called every frame that this state is active</param>
        /// <param name="onStateEnterMethod">The method that is called when this state is entered</param>
        public void RegisterState(T state, StateUpdateRoutine onUpdateRoutine, Action onStateEnterMethod)
        {
            RegisterState(state, onUpdateRoutine, onStateEnterMethod, null);
        }

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateRoutine">The method that is called every frame that this state is active</param>
        public void RegisterState(T state, StateUpdateRoutine onUpdateRoutine)
        {
            RegisterState(state, onUpdateRoutine, null, null);
        }

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateMethod">The method that is called every frame that this state is active</param>
        /// <param name="onStateEnterMethod">The method that is called when this state is entered</param>
        /// <param name="onStateExitMethod">The method that is called when this state is exited</param>
        public void RegisterState(T state, Action onUpdateMethod, Action onStateEnterMethod, Action onStateExitMethod)
        {

            StateMethods stateMethods = new StateMethods(onUpdateMethod, null, onStateEnterMethod, onStateExitMethod);
            if (_states.ContainsKey(state))
            {
                _states[state] = stateMethods;
            }
            else
            {
                _states.Add(state, stateMethods);
            }
        }

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateMethod">The method that is called every frame that this state is active</param>
        /// <param name="onStateEnterMethod">The method that is called when this state is entered</param>
        public void RegisterState(T state, Action onUpdateMethod, Action onStateEnterMethod)
        {
            RegisterState(state, onUpdateMethod, onStateEnterMethod, null);
        }

        /// <summary>
        /// Registers methods that should be called when the state is appropriate.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <param name="onUpdateMethod">The method that is called every frame that this state is active</param>
        public void RegisterState(T state, Action onUpdateMethod)
        {
            RegisterState(state, onUpdateMethod, null, null);
        }

        #endregion

        /// <summary>
        /// Degristers state methods for a specific staSte.
        /// </summary>
        /// <param name="state">The state in question</param>
        public void DeregisterState(T state)
        {
            Assert.IsFalse(state.Equals(_currentState), "Cannot deregister active FSM state: " + state);
            _states.Remove(state);
        }

        /// <summary>
        /// Whether or not a state currently has methods registed with this FSM.
        /// </summary>
        /// <param name="state">The state in question</param>
        /// <returns>True if the state is registered</returns>
        public bool IsStateRegistered(T state)
        {
            return _states.ContainsKey(state);
        }

        /// <summary>
        /// Changes the FSM's active state.
        /// </summary>
        /// <param name="newState">The new state to change to</param>
        public void ChangeState(T newState)
        {
            if (!newState.Equals(_currentState))
            {
                _previousState = _currentState;
                _currentState = newState;

                StateMethods stateMethods;
                if (_states.TryGetValue(_previousState, out stateMethods))
                {
                    if (stateMethods.onStateExit != null)
                    {
                        stateMethods.onStateExit();
                    }
                }

                if (_states.TryGetValue(_currentState, out stateMethods))
                {
                    if (stateMethods.onStateEnter != null)
                    {
                        stateMethods.onStateEnter();
                    }
                }


                OnStateChanged?.Invoke(newState);
            }

        }

        //Use reflection to register state methods
        void AutoRegister()
        {
            MethodInfo[] methods = _component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            string[] stateNames = Enum.GetNames(typeof(T));

            for (int i = 0; i < stateNames.Length; i++)
            {


                StateUpdateRoutine updateRoutine = null;
                Action updateMethod = null;
                Action enterMethod = null;
                Action exitMethod = null;

                for (int j = 0; j < methods.Length; j++)
                {


                    if (updateRoutine == null && methods[j].ReturnType == typeof(IEnumerator) && (methods[j].Name == stateNames[i] + "Update" || methods[j].Name == stateNames[i] + "_Update"))
                    {
                        updateRoutine = Delegate.CreateDelegate(typeof(StateUpdateRoutine), _component, methods[j]) as StateUpdateRoutine;
                    }
                    else if (updateMethod == null && methods[j].ReturnType == typeof(void) && (methods[j].Name == stateNames[i] + "Update" || methods[j].Name == stateNames[i] + "_Update"))
                    {
                        updateMethod = Delegate.CreateDelegate(typeof(Action), _component, methods[j]) as Action;
                    }
                    else if (enterMethod == null && methods[j].ReturnType == typeof(void) && (methods[j].Name == stateNames[i] + "Enter" || methods[j].Name == stateNames[i] + "_Enter"))
                    {
                        enterMethod = Delegate.CreateDelegate(typeof(Action), _component, methods[j]) as Action;
                    }
                    else if (exitMethod == null && methods[j].ReturnType == typeof(void) && (methods[j].Name == stateNames[i] + "Exit" || methods[j].Name == stateNames[i] + "_Exit"))
                    {
                        exitMethod = Delegate.CreateDelegate(typeof(Action), _component, methods[j]) as Action;
                    }
                }

                if (updateRoutine != null)
                {
                    RegisterState((T)Enum.Parse(typeof(T), stateNames[i]), updateRoutine, enterMethod, exitMethod);
                }
                else
                {
                    RegisterState((T)Enum.Parse(typeof(T), stateNames[i]), updateMethod, enterMethod, exitMethod);
                }
            }
        }

        // This coroutine handles the updating of the current state
        IEnumerator UpdateRoutine(bool waitUntilNextFrameToUpdate)
        {
            if (waitUntilNextFrameToUpdate) yield return null;

            while (_started)
            {
                StateMethods stateMethods;
                if (_states.TryGetValue(State, out stateMethods))
                {

                    T updatingState = _currentState;
                    StateUpdateRoutine updateRoutine = stateMethods.onStateUpdateRoutine;
                    Action updateMethod = stateMethods.onStateUpdate;

                    if (updateRoutine != null)
                    {

                        IEnumerator iterator = updateRoutine();

                        while (_currentState.Equals(updatingState) && stateMethods.onStateUpdateRoutine == updateRoutine)
                        {
                            while (_paused) yield return null;
                            yield return iterator.Current;
                            if (!iterator.MoveNext()) break;
                        }
                    }
                    else if (updateMethod != null)
                    {
                        while (_currentState.Equals(updatingState) && stateMethods.onStateUpdate == updateMethod)
                        {
                            while (_paused) yield return null;
                            updateMethod();
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

    }
}