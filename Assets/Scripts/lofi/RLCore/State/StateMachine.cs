using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class StateMachine
    {
        public IState currentState;

        Dictionary<IState, List<IState>> transitions;

        public bool UseTransitions { get; set; } // default false

        public StateMachine()
        {
            UseTransitions = false;
            transitions = new Dictionary<IState, List<IState>>();
        }

        public bool ChangeState(IState newState)
        {
            if (UseTransitions)
            {
                List<IState> states;

                if (transitions.TryGetValue(currentState, out states))
                {
                    if(states.Contains(newState))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (currentState != null)
                currentState.Exit();

            currentState = newState;
            currentState.Enter();

            return true;
        }

        public void AddTransition(IState from, IState to)
        {
            if (!UseTransitions)
                UseTransitions = true;

            List<IState> states;

            if (transitions.TryGetValue(from, out states))
            {
                states.Add(to);
            }
            else
            {
                transitions.Add(from, new List<IState> { to });
            }

        }

        public IState GetRandomTransition()
        {
            if (!UseTransitions)
                return null;

            List<IState> states;
            IState state;

            if (transitions.TryGetValue(currentState, out states))
            {
                int index = Random.Range(0, states.Count);
                state = states[index];
            }
            else
            {
                return null;
            }

            return state;
        }

        public void Update()
        {
            if (currentState != null) 
                currentState.Execute();
        }
    }
}