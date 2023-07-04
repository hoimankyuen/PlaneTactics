using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class LevelController : MonoBehaviour
{
	public enum State
	{
		None,
		Initialize,
		Solidify,
		PreTeamAction,
		TeamAction,
		EnvironmentalAction,
		Ended,
	}

	// working variables
	protected StateMachine<State> stateMachine = new StateMachine<State>();
	public State CurrentState { get { return stateMachine.Current; } }

	// ========================================================= Defination =========================================================

	/// <summary>
	/// Define all necessary components of the state machine.
	/// </summary>
	protected void DefineStateMachine()
	{
		stateMachine.AddState(State.Initialize, new StateMachine<State>.StateInfo
		{
			entry = InitializeStateEntry,
			body = InitializeStateBody,
			exit = InitializeStateExit,
			transition = InitializeStateTransition
		});
		stateMachine.AddState(State.Solidify, new StateMachine<State>.StateInfo
		{
			entry = SolidifyStateEntry,
			body = SolidifyStateBody,
			exit = SolidifyStateExit,
			transition = SolidifyStateTransition
		});
		stateMachine.AddState(State.PreTeamAction, new StateMachine<State>.StateInfo
		{
			entry = PreTeamActionStateEntry,
			body = PreTeamActionStateBody,
			exit = PreTeamActionStateExit,
			transition = PreTeamActionStateTransition
		});
		stateMachine.AddState(State.TeamAction, new StateMachine<State>.StateInfo
		{
			entry = TeamActionStateEntry,
			body = TeamActionStateBody,
			exit = TeamActionStateExit,
			transition = TeamActionStateTransition
		});
		stateMachine.AddState(State.EnvironmentalAction, new StateMachine<State>.StateInfo
		{
			entry = EnvironmentalActionStateEntry,
			body = EnvironmentalActionStateBody,
			exit = EnvironmentalActionStateExit,
			transition = EnvironmentalActionStateTransition
		});
		stateMachine.AddState(State.Ended, new StateMachine<State>.StateInfo
		{
			entry = EndedStateEntry,
			body = EndedStateBody,
			exit = EndedStateExit,
			transition = EndedStateTransition
		});
		stateMachine.SetInitialState(State.Initialize);
	}

	// ========================================================= Driver =========================================================	// 

	/// <summary>
	/// Run the state machine.
	/// </summary>
	protected void RunStateMachine()
	{
		stateMachine.Run();
	}

	// ========================================================= Initialize State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void InitializeStateEntry()
	{
		CurrentTurn = 1;
		ActiveTeamIndex = 0;

		SetTurnDisplay(CurrentTurn);
		SetTeamInfoDisplay(ActiveTeam);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void InitializeStateBody()
	{
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State InitializeStateTransition()
	{
		return State.Solidify;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void InitializeStateExit()
	{
	}

	// ========================================================= Solidify State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void SolidifyStateEntry()
	{
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void SolidifyStateBody()
	{
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State SolidifyStateTransition()
	{
		if (units.All(x => x.Value.All(y => y.CurrentState == PlayerUnit.State.Solidify || y.CurrentState == PlayerUnit.State.Downed)))
		{
			return State.PreTeamAction;
		}

		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void SolidifyStateExit()
	{
	}

	// ========================================================= Pre Team Action State =========================================================

	float preTeamStartTime = 0;
	float preTeamDuration = 0;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void PreTeamActionStateEntry()
	{
		SetTurnDisplay(CurrentTurn);
		SetTeamInfoDisplay(ActiveTeam);
		preTeamStartTime = Time.time;
		preTeamDuration = PlayTurnDisplayAnimation(ActiveTeam, (ActiveTeamIndex == 0 && CurrentTurn != 1 ? CurrentTurn - 1 : CurrentTurn), CurrentTurn);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void PreTeamActionStateBody()
	{

	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State PreTeamActionStateTransition()
	{
		if (Time.time - preTeamStartTime > preTeamDuration)
		{
			return State.TeamAction;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void PreTeamActionStateExit()
	{
	}

	// ========================================================= Team Action State =========================================================

	int lastActiveTeamIndex = 0;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void TeamActionStateEntry()
	{
		lastActiveTeamIndex = ActiveTeamIndex;
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void TeamActionStateBody()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			FindNextAvailableUnit();
			ProgressTurn();
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State TeamActionStateTransition()
	{
		if (ActiveTeamIndex != lastActiveTeamIndex)
		{
			return State.Solidify;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void TeamActionStateExit()
	{
	}

	public void FindNextAvailableUnit()
	{
		// check if any unit is available
		List<PlayerUnit> planes = units[ActiveTeam].Where(x => x.gameObject.activeInHierarchy && x.teamInfo == ActiveTeam).ToList();

		// only find if any unit is available
		if (planes.Any(x => x.CurrentState == PlayerUnit.State.Idle))
		{
			// shift the list if needed
			if (ActiveUnit != null && planes.Contains(ActiveUnit))
			{
				if (planes.IndexOf(ActiveUnit) != planes.Count - 1)
				{
					planes.AddRange(planes.GetRange(0, planes.IndexOf(ActiveUnit) + 1));
					planes.RemoveRange(0, planes.IndexOf(ActiveUnit) + 1);
				}
			}

			// get the first available unit in shifted list
			PlayerUnit target = planes.First(x => x.CurrentState == PlayerUnit.State.Idle);

			// move camera to unit
			cameraControl.GotoPosition(target.transform.position + target.transform.forward * Mathf.Lerp(target.unitInfo.moveArea.distances.min, target.unitInfo.moveArea.distances.max, 0.5f));

			// select said unit
			ActiveUnit = target;
		}
	}

	public void ProgressTurn()
	{
		if (CurrentState != State.TeamAction)
			return;

		// check if all units have made their moves
		bool allMoveMade = true;
		foreach (KeyValuePair<TeamInfo, List<PlayerUnit>> kvp in units)
		{
			foreach (PlayerUnit unit in kvp.Value)
			{
				if (kvp.Key == ActiveTeam)
				{
					if (unit.CurrentState != PlayerUnit.State.Done && unit.CurrentState != PlayerUnit.State.Downed && unit.CurrentState != PlayerUnit.State.DownAnimation)
					{
						allMoveMade = false;
						break;
					}
				}
				else
				{
					if (unit.CurrentState != PlayerUnit.State.Wait && unit.CurrentState != PlayerUnit.State.Downed && unit.CurrentState != PlayerUnit.State.DownAnimation)
					{
						allMoveMade = false;
						break;
					}
				}
			}
			
		}
		if (allMoveMade)
		{
			// progress turn
			ActiveTeamIndex++;
			if (ActiveTeamIndex < teams.Count && units[ActiveTeam].All(x => x.CurrentState == PlayerUnit.State.Downed || x.CurrentState == PlayerUnit.State.DownAnimation))
			{
				ActiveTeamIndex++;
			}
			if (ActiveTeamIndex >= teams.Count)
			{
				CurrentTurn++;
				ActiveTeamIndex = 0;
			}
		}
	}

	// ========================================================= Environmental Action State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void EnvironmentalActionStateEntry()
	{
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void EnvironmentalActionStateBody()
	{
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State EnvironmentalActionStateTransition()
	{
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void EnvironmentalActionStateExit()
	{
	}

	// ========================================================= Ended State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void EndedStateEntry()
	{
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void EndedStateBody()
	{
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State EndedStateTransition()
	{
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void EndedStateExit()
	{
	}

}
