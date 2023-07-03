using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : System.Enum
{
	public interface IStateInfo
	{ 
		void Entry();
		void Body();
		void Exit();
		T Transition();
	}

	// state delegate defination
	public class StateInfo
	{
		public Action entry = null;
		public Action body = null;
		public Action exit = null;
		public Func<T> transition = null;
	}

	// working variables
	public T Current { get; protected set; } = default;
	protected List<T> possibleNext = new List<T>();
	protected T next = default;
	protected Dictionary<T, List<StateInfo>> states = new Dictionary<T, List<StateInfo>>();
	protected Dictionary<T, List<IStateInfo>> statesI = new Dictionary<T, List<IStateInfo>>();

	// ========================================================= Setup =========================================================

	/// <summary>
	/// Add a new state into the state machine.
	/// </summary>
	public void AddState(T state, IStateInfo newStateInfo)
    {
		if (!statesI.ContainsKey(state))
        {
			statesI[state] = new List<IStateInfo>();
        }
		statesI[state].Add(newStateInfo);
    }

	/// <summary>
	/// Add a new state into the state machine with the corisponding state marker.
	/// </summary>
	public void AddState(T state, StateInfo newStateInfo)
	{
		if (!states.ContainsKey(state))
		{
			states[state] = new List<StateInfo>();
		}
		states[state].Add(newStateInfo);
	}

	/// <summary>
	/// Set the state where the state machine starts in.
	/// </summary>
	public void SetInitialState(T stateName)
	{
		possibleNext.Add(stateName);
		next = stateName;
	}

	// ========================================================= Run =========================================================

	/// <summary>
	/// Perform the state machine logic, invoking registered state method.
	/// </summary>
	public void Run()
	{
		// entry
		if (!Current.Equals(next))
		{
			Current = next;
			if (states.ContainsKey(Current))
			{
                foreach (StateInfo stateInfo in states[Current])
				{
					stateInfo.entry?.Invoke();
				}
			}
			if (statesI.ContainsKey(Current))
			{
				foreach (IStateInfo stateInfo in statesI[Current])
				{
					stateInfo.Entry();
				}
			}
		}

		// body
		if (states.ContainsKey(Current))
		{
			foreach (StateInfo stateInfo in states[Current])
			{
				stateInfo.body?.Invoke();
			}
		}
		if (statesI.ContainsKey(Current))
		{
			foreach (IStateInfo stateInfo in statesI[Current])
			{
				stateInfo.Body();
			}
		}

		// transistion
		possibleNext.Clear();
		if (states.ContainsKey(Current))
		{
			foreach (StateInfo stateInfo in states[Current])
			{
				possibleNext.Add(stateInfo.transition != null ? stateInfo.transition.Invoke() : default(T));
			}
		}
		if (statesI.ContainsKey(Current))
		{
			foreach (IStateInfo stateInfo in statesI[Current])
			{
				possibleNext.Add(stateInfo.Transition());
			}
		}
		next = default;
		if (possibleNext.Count > 0)
        {
			possibleNext.Sort();
			next = possibleNext[0];
		}

		// exit
		if (!next.Equals(default(T)))
		{
			if (states.ContainsKey(Current))
			{
				foreach (StateInfo stateInfo in states[Current])
				{
					stateInfo.exit?.Invoke();
				}
			}
			if (statesI.ContainsKey(Current))
			{
				foreach (IStateInfo stateInfo in statesI[Current])
				{
					stateInfo.Exit();
				}
			}
		}
		else
		{
			next = Current;
		}
	}

	/*
	// ========================================================= Template State =========================================================

	public class TemplateState: StateMachine.IStateInfo
    {
		/// <summary>
		/// Entry is called on the frame the state is entering, before body is called.
		/// </summary>
		public void Entry()
        {

        }

		/// <summary>
		/// Body is called each frame the state is in.
		/// </summary>
		public void Body()
        {

        }

		/// <summary>
		/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
		/// </summary>
		public State Transition()
        {
			return State.None;
        }

		/// <summary>
		/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
		/// </summary>
		public void Exit()
		{

		}
	}
	*/

	/*
	// ========================================================= Template State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void TemplateStateEntry()
	{
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void TemplateStateBody()
	{
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected State TemplateStateTransition()
	{
		return State.None;
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected void TemplateStateExit()
	{
	}
	*/
}
