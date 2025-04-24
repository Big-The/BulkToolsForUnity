using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.StateMachines 
{
    /// <summary>
    /// StateMachine provides a structure to build a state machine using classes specific to each state and enforces transition logic.
    /// </summary>
    public class StateMachine<T> where T : class
    {
        private T machineDataObject;

        private Dictionary<string, State> states = new Dictionary<string, State>();
        private string currentStateName;
        private State currentState;

        public StateMachine(T machineDataObject)
        {
            this.machineDataObject = machineDataObject;
        }

        /// <summary>
        /// Call to run a single tick of the current state
        /// </summary>
        public void Tick() 
        {
            if(currentState != null) 
            {
                if (currentState.DoTick(out string nextStateName) && states.TryGetValue(nextStateName, out State nextState)) 
                {
                    currentStateName = nextStateName;
                    currentState = nextState;
                }
            }
        }

        /// <summary>
        /// Adds a state to the state machine
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="StateNameAlreadyExistsException"></exception>
        public StateMachine<T> AddState(string stateName, State state) 
        {
            if (states.ContainsKey(stateName))
            {
                throw new StateNameAlreadyExistsException();
            }
            else 
            {
                states.Add(stateName, state);
                if (states.Count == 1) 
                {
                    currentState = state;
                    currentStateName = stateName;
                }
                state.OwnerMachine = this;
            }
            return this;
        }

        /// <summary>
        /// Inherit from this class to create states for a state machine
        /// </summary>
        public class State
        {
            protected T MachineDataObject { get => OwnerMachine.machineDataObject; }
            internal StateMachine<T> OwnerMachine { get; set; }
            private List<Transition> transitions = new List<Transition>();

            /// <summary>
            /// Adds a transition to the target state when the transitionTest returns true. States are evaluated in the order they are added.
            /// </summary>
            /// <param name="to"></param>
            /// <param name="transitionTest"></param>
            /// <returns></returns>
            public State AddTransition(string targetState, TransitionTest transitionTest) 
            {
                transitions.Add(new Transition(targetState, transitionTest));
                return this;
            }

            //Perform all steps for a tick
            internal bool DoTick(out string nextStateName) 
            {
                Tick();
                return CheckTransitions(out nextStateName);
            }

            /// <summary>
            /// Override this method to perform all state logic
            /// </summary>
            protected virtual void Tick() { }

            //Check all transitions for if we need to switch to a new state. Returns true and sets nextStateName when a transition evaluates true.
            private bool CheckTransitions(out string nextStateName) 
            {
                nextStateName = null;
                foreach (Transition t in transitions) 
                {
                    if (t.transitionTest(MachineDataObject)) 
                    {
                        nextStateName = t.toState;
                        return true;
                    }
                }
                return false;
            }
        }

        private class Transition
        {
            internal string toState;
            
            public TransitionTest transitionTest;
            internal Transition(string toState, TransitionTest transitionTest)
            {
                this.toState = toState;
                this.transitionTest = transitionTest;
            }
        }

        #region Delegates 
        /// <summary>
        /// Defines a test case for a transition
        /// </summary>
        /// <param name="machineDataObject"></param>
        /// <returns></returns>
        public delegate bool TransitionTest(T machineDataObject);
        #endregion

        #region Exceptions
        public class StateNameAlreadyExistsException : System.Exception { }
        #endregion
    }

    public class Test : StateMachine<SimpleStateMachine>.State 
    {
        
    }
}
