using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.StateMachines 
{
    public class SimpleStateMachine
    {
        /// <summary>
        /// Stores all states with their ids
        /// </summary>
        private Dictionary<string, StateAction> states = new Dictionary<string, StateAction>();
        private Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        private Dictionary<string, int> intValues = new Dictionary<string, int>();
        private Dictionary<string, float> floatValues = new Dictionary<string, float>();
        private Dictionary<string, List<Transition>> transitions = new Dictionary<string, List<Transition>>();
        private string _currentState;
        public string CurrentState { get => _currentState; }

        private StateAction _currentAction;
        private bool running = false;

        /// <summary>
        /// A method that performs the action of the current state. Called durring every Run call where it is the active state.
        /// </summary>
        /// <param name="stateMachine"></param>
        public delegate void StateAction(SimpleStateMachine stateMachine);

        public SimpleStateMachine(string defaultStateName, StateAction stateAction)
        {
            _currentState = defaultStateName;
            _currentAction = stateAction;
            AddState(defaultStateName, stateAction);
        }

        /// <summary>
        /// Add a state to the state machine.
        /// </summary>
        /// <param name="stateName">A unique state name</param>
        /// <param name="stateAction">Action to perform durring run when the state is active</param>
        /// <exception cref="InvalidStateException"></exception>
        /// <exception cref="StateAlreadyExistsException"></exception>
        public SimpleStateMachine AddState(string stateName, StateAction stateAction)
        {
            if (stateAction == null) { throw new InvalidStateException(); }
            if (states.TryAdd(stateName, stateAction))
            {
                transitions.Add(stateName, new List<Transition>());
                return this;
            }
            throw new StateAlreadyExistsException();
        }

        /// <summary>
        /// Force the state machine to switch to the specified state.
        /// </summary>
        /// <param name="stateName">The name of the state to switch to. Must be a name that exists.</param>
        /// <exception cref="StateDoesNotExistException"></exception>
        public void SetCurrentState(string stateName)
        {
            if (states.TryGetValue(stateName, out StateAction stateAction))
            {
                _currentAction = stateAction;
                _currentState = stateName;
            }
            else
            {
                throw new StateDoesNotExistException();
            }
        }

        /// <summary>
        /// Add a transition to the state machine.
        /// </summary>
        /// <param name="transition">The transition to add</param>
        /// <exception cref="InvalidStateException"></exception>
        public SimpleStateMachine AddTransition(Transition transition)
        {
            if (transition == null || transition.transitionTest == null) { throw new InvalidStateException(); };
            if (!states.ContainsKey(transition.fromState) && !states.ContainsKey(transition.toState)) { throw new InvalidStateException(); }
            transitions[transition.fromState].Add(transition);
            return this;
        }

        /// <summary>
        /// Add a transition to the state machine.
        /// </summary>
        /// <param name="transition">The transition to add</param>
        /// <exception cref="InvalidStateException"></exception>
        public SimpleStateMachine AddTransition(string fromState, string toState, Transition.TransitionTest transitionTest)
        {
            Transition transition = new Transition(fromState, toState, transitionTest);
            return AddTransition(transition);
        }

        /// <summary>
        /// Call this to run the current state once. After the state completes transition conditions are checked.
        /// (Recomended to call in an Update)
        /// </summary>
        /// <exception cref="AlreadyRunningException"></exception>
        public void Run()
        {
            if (running)
            {
                throw new AlreadyRunningException();
            }
            running = true;
            try
            {
                _currentAction.Invoke(this);
                foreach (Transition t in transitions[_currentState])
                {
                    if (t.transitionTest(this))
                    {
                        SetCurrentState(t.toState);
                        break;
                    }
                }
            }
            catch
            {
                running = false;
                throw;
            }
            running = false;
        }

        #region Values
        /// <summary>
        /// Set a bool value on the state machine.
        /// </summary>
        /// <param name="boolName"></param>
        /// <param name="value"></param>
        public SimpleStateMachine SetBool(string boolName, bool value)
        {
            if (boolValues.ContainsKey(boolName))
            {
                boolValues[boolName] = value;
            }
            else
            {
                boolValues.Add(boolName, value);
            }
            return this;
        }

        /// <summary>
        /// Get a bool value from the state machine.
        /// </summary>
        /// <param name="boolName"></param>
        /// <returns></returns>
        public bool GetBool(string boolName)
        {
            if (boolValues.TryGetValue(boolName, out bool value))
            {
                return value;
            }
            return false;
        }

        /// <summary>
        /// Set an int value on the state machine.
        /// </summary>
        /// <param name="intName"></param>
        /// <param name="value"></param>
        public SimpleStateMachine SetInt(string intName, int value)
        {
            if (intValues.ContainsKey(intName))
            {
                intValues[intName] = value;
            }
            else
            {
                intValues.Add(intName, value);
            }
            return this;
        }

        /// <summary>
        /// Get an int value from the state machine.
        /// </summary>
        /// <param name="intName"></param>
        /// <returns></returns>
        public int GetInt(string intName)
        {
            if (intValues.TryGetValue(intName, out int value))
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// Set a float value on the state machine
        /// </summary>
        /// <param name="floatName"></param>
        /// <param name="value"></param>
        public SimpleStateMachine SetFloat(string floatName, float value)
        {
            if (floatValues.ContainsKey(floatName))
            {
                floatValues[floatName] = value;
            }
            else
            {
                floatValues.Add(floatName, value);
            }
            return this;
        }

        /// <summary>
        /// Get a float value from the state machine.
        /// </summary>
        /// <param name="floatName"></param>
        /// <returns></returns>
        public float GetFloat(string floatName)
        {
            if (floatValues.TryGetValue(floatName, out float value))
            {
                return value;
            }
            return 0.0f;
        }
        #endregion

        /// <summary>
        /// Defines the condition under which the state machine will switch between the specified states.
        /// </summary>
        public class Transition
        {
            public string fromState;
            public string toState;

            public delegate bool TransitionTest(SimpleStateMachine stateMachine);
            public TransitionTest transitionTest;
            public Transition(string fromState, string toState, TransitionTest transitionTest)
            {
                this.fromState = fromState;
                this.toState = toState;
                this.transitionTest = transitionTest;
            }
        }

        public class StateDoesNotExistException : Exception { }
        public class StateAlreadyExistsException : Exception { }
        public class ModifiedWhileRunningException : Exception { }
        public class AlreadyRunningException : Exception { }
        public class InvalidStateException : Exception { }
        public class InvalidTransitionException : Exception { }
    }
}
