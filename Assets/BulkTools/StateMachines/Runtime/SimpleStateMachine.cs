using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.StateMachines 
{
    public class SimpleStateMachine
    {
        public Dictionary<string, StateAction> states = new Dictionary<string, StateAction>();
        private Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        private Dictionary<string, int> intValues = new Dictionary<string, int>();
        private Dictionary<string, float> floatValues = new Dictionary<string, float>();
        private Dictionary<string, List<Transition>> transitions = new Dictionary<string, List<Transition>>();
        private string _currentState;
        public string CurrentState { get => _currentState; }

        private StateAction _currentAction;
        private bool running = false;

        public delegate void StateAction(SimpleStateMachine stateMachine);

        public SimpleStateMachine(string defaultStateName, StateAction stateAction)
        {
            _currentState = defaultStateName;
            _currentAction = stateAction;
            AddState(defaultStateName, stateAction);
        }

        public void AddState(string stateName, StateAction stateAction)
        {
            if (stateAction == null) { throw new InvalidStateException(); }
            if (!states.TryAdd(stateName, stateAction))
            {
                transitions.Add(stateName, new List<Transition>());
                return;
            }
            throw new StateAlreadyExistsException();
        }

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

        public void AddTransition(Transition transition)
        {
            if (transition == null || transition.transitionTest == null) { throw new InvalidStateException(); };
            if (!states.ContainsKey(transition.fromState) && !states.ContainsKey(transition.toState)) { throw new InvalidStateException(); }
            transitions[transition.fromState].Add(transition);
        }

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
        public void SetBool(string boolName, bool value)
        {
            if (boolValues.ContainsKey(boolName))
            {
                boolValues[boolName] = value;
            }
            else
            {
                boolValues.Add(boolName, value);
            }
        }

        public bool GetBool(string boolName)
        {
            if (boolValues.TryGetValue(boolName, out bool value))
            {
                return value;
            }
            return false;
        }

        public void SetInt(string intName, int value)
        {
            if (intValues.ContainsKey(intName))
            {
                intValues[intName] = value;
            }
            else
            {
                intValues.Add(intName, value);
            }
        }

        public int GetInt(string intName)
        {
            if (intValues.TryGetValue(intName, out int value))
            {
                return value;
            }
            return 0;
        }

        public void SetFloat(string floatName, float value)
        {
            if (floatValues.ContainsKey(floatName))
            {
                floatValues[floatName] = value;
            }
            else
            {
                floatValues.Add(floatName, value);
            }
        }

        public float GetFloat(string floatName)
        {
            if (floatValues.TryGetValue(floatName, out float value))
            {
                return value;
            }
            return 0.0f;
        }
        #endregion

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
