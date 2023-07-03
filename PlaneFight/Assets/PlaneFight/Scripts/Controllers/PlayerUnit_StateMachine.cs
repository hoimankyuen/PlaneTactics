using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerUnit : MonoBehaviour
{
	public enum State
	{
		None,
		Initialize,
		Solidify,
		Idle,
		MoveMenu,
		MoveAnimation,
		AttackMenu,
		AttackAnimation,
		Done,
		Wait,
		AttackPrediction,
		DamageAnimation,
		DownAnimation,
		Downed,
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
		stateMachine.AddState(State.Idle, new StateMachine<State>.StateInfo
		{
			entry = IdleStateEntry,
			body = IdleStateBody,
			exit = IdleStateExit,
			transition = IdleStateTransition
		});
		stateMachine.AddState(State.MoveMenu, new StateMachine<State>.StateInfo
		{
			entry = MoveMenuStateEntry,
			body = MoveMenuStateBody,
			exit = MoveMenuStateExit,
			transition = MoveMenuStateTransition
		});
		stateMachine.AddState(State.MoveAnimation, new StateMachine<State>.StateInfo
		{
			entry = MoveAnimationStateEntry,
			body = MoveAnimationStateBody,
			exit = MoveAnimationStateExit,
			transition = MoveAnimationStateTransition
		});
		stateMachine.AddState(State.AttackMenu, new StateMachine<State>.StateInfo
		{
			entry = AttackMenuStateEntry,
			body = AttackMenuStateBody,
			exit = AttackMenuStateExit,
			transition = AttackMenuStateTransition
		});
		stateMachine.AddState(State.AttackAnimation, new StateMachine<State>.StateInfo
		{
			entry = AttackAnimationStateEntry,
			body = AttackAnimationStateBody,
			exit = AttackAnimationStateExit,
			transition = AttackAnimationStateTransition
		});
		stateMachine.AddState(State.Done, new StateMachine<State>.StateInfo
		{
			entry = DoneStateEntry,
			body = DoneStateBody,
			exit = DoneStateExit,
			transition = DoneStateTransition
		});
		stateMachine.AddState(State.Wait, new StateMachine<State>.StateInfo
		{
			entry = WaitStateEntry,
			body = WaitStateBody,
			exit = WaitStateExit,
			transition = WaitStateTransition
		});
		stateMachine.AddState(State.AttackPrediction, new StateMachine<State>.StateInfo
		{
			entry = AttackPredictionStateEntry,
			body = AttackPredictionStateBody,
			exit = AttackPredictionStateExit,
			transition = AttackPredictionStateTransition
		}); stateMachine.AddState(State.DamageAnimation, new StateMachine<State>.StateInfo
		{
			entry = DamageAnimationStateEntry,
			body = DamageAnimationStateBody,
			exit = DamageAnimationStateExit,
			transition = DamageAnimationStateTransition
		});
		stateMachine.AddState(State.DownAnimation, new StateMachine<State>.StateInfo
		{
			entry = DownAnimationStateEntry,
			body = DownAnimationStateBody,
			exit = DownAnimationStateExit,
			transition = DownAnimationStateTransition
		});
		stateMachine.AddState(State.Downed, new StateMachine<State>.StateInfo
		{
			entry = DownedStateEntry,
			body = DownedStateBody,
			exit = DownedStateExit,
			transition = DownedStateTransition
		});

		stateMachine.SetInitialState(State.Initialize);
	}

	// ========================================================= Driver ========================================================

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
		// initialize plane information
		maxHealth = unitInfo.maxHealth;
		hitboxRadius = unitInfo.hitboxRadius;

		moveArea = unitInfo.moveArea;
		moveAdjust = unitInfo.moveAdjust;

		attackDistances = unitInfo.attackDistance;
		attackDistanceLoseBySteer = unitInfo.attackDistanceLoseBySteer;
		attackMaxAngle = unitInfo.attackMaxAngle;
		attackPower = unitInfo.attackPower;
		attackRolls = unitInfo.attackRolls;
		attackBaseProbability = unitInfo.attackBaseProbability;

		(selectionModelCollider as SphereCollider).radius = unitInfo.hitboxRadius;
		(selectionUICollider as SphereCollider).radius = unitInfo.hitboxRadius;

		// initial pre post information
		postActionHealth = unitInfo.maxHealth * initialHealthPercentage;
		postActionPosition = transform.position;
		postActionRotation = transform.rotation;
		postActionAltitude = initialAltitude;
		postActionSpeed = initialSpeed;
		postActionSteer = initialSteer;
		MoveResult moveResult = moveArea.Evaluate(new MoveInput(initialSpeed, initialSteer));
		postActionMovePath = new SimpleTurnPath(moveResult.distance, moveResult.azimuth, 0);
		postActionTilt = Mathf.Lerp(70f, -70f, postActionSteer);

		// initialize movement caches
		previousMovePaths = new List<IPath>(new IPath[] { postActionMovePath, postActionMovePath, postActionMovePath });
		previousTilts = new List<float>(new float[] { postActionTilt, postActionTilt, postActionTilt, postActionTilt });

		// mesh display
		trailsDisplay.Modify(
				postActionAltitude * level.altitudeHeight,
				previousMovePaths,
				previousTilts,
				1);

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, postActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 0.5f));

		// 2d ui
		SetUIAsDone(false);
		SetUITypeDisplay(IndicatorType.Mini);
		SetUITeamDisplay(teamInfo);
		SetUIUnitDisplay(unitInfo);
		SetUIBuffDisplay(Buff.None, BuffMagnitude.None);
		SetMarkerRadius(unitInfo.hitboxRadius);
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
		if (level.CurrentState == LevelController.State.Solidify)
		{
			return State.Solidify;
		}
		return State.None;
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
		// apply state from last turn
		preActionHealth = postActionHealth;
		preActionPosition = postActionPosition;
		preActionRotation = postActionRotation;
		preActionAltitude = postActionAltitude;
		preActionSpeed = postActionSpeed;
		preActionSteer = postActionSteer;
		preActionMovePath = postActionMovePath;
		preActionTilt = postActionTilt;

		// set transforms
		transform.position = preActionPosition;
		transform.rotation = preActionRotation;
		model.localPosition = new Vector3(0f, preActionAltitude * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, preActionTilt);
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
		if (level.CurrentState == LevelController.State.TeamAction)
		{
			if (level.ActiveTeam == teamInfo)
			{
				return State.Idle;
			}
			else
			{
				return State.Wait;
			}
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void SolidifyStateExit()
	{
	}

	// ========================================================= Idle State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void IdleStateEntry()
	{
		// set transforms
		transform.position = preActionPosition;
		transform.rotation = preActionRotation;
		model.localPosition = new Vector3(0f, preActionAltitude * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, preActionTilt);

		// mesh displays
		currentMoveFullDisplay.Show(false);
		currentMoveFullDisplay.SetColor(new Color(0f, 0.65f, 1f, 0f));
		currentMoveFullDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				moveArea);

		currentMoveAreaDisplay.Show(false);
		currentMoveAreaDisplay.SetColor(new Color(0f, 0.65f, 1f, 0.25f));
		currentMoveAreaDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust));

		attackAreaDisplay.Show(false);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.25f));
		attackAreaDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				attackDistances,
				attackMaxAngle);

		trailsDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				previousMovePaths,
				previousTilts,
				1);

		// effects
		selectionOutline.Show = false;

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 0.5f));

		// 2d uis
		SetUIAsDone(false);
		SetUITypeDisplay(IndicatorType.Mini);
		SetUIHealthDisplay(preActionHealth);
		SetUIAttitudeDisplay(preActionAltitude);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void IdleStateBody()
	{
		// apparence when hovering on this unit
		if (playerInput.IsHovering(this))
		{
			// mesh displays
			currentMoveFullDisplay.Show(true);
			currentMoveAreaDisplay.Show(true);
			attackAreaDisplay.Show(true);

			// effects
			selectionOutline.Color = new Color(0f, 0f, 1f, 1f);
			selectionOutline.Width = 2f;
			selectionOutline.Show = true;

			// 3d uis
			SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 1f));

			// 2d uis
			SetUITypeDisplay(IndicatorType.Normal);
		}
		// apparence when not hovering on this unit
		else
		{
			// mesh displays
			currentMoveFullDisplay.Show(false);
			currentMoveAreaDisplay.Show(false);
			attackAreaDisplay.Show(false);

			// effects
			selectionOutline.Show = false;

			// 3d uis
			SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 0.5f));

			// 2d uis
			SetUITypeDisplay(IndicatorType.Mini);
		}

		// detect selection
		if (level.ActiveUnit == null && playerInput.IsSelected(this))
		{
			level.ActiveUnit = this;
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State IdleStateTransition()
	{
		if (level.CurrentState == LevelController.State.Solidify)
		{
			return State.Solidify;
		}
		if (level.ActiveUnit == this)
		{
			return State.MoveMenu;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void IdleStateExit()
	{
		// mesh displays
		currentMoveFullDisplay.Show(false);
		currentMoveAreaDisplay.Show(false);
		attackAreaDisplay.Show(false);

		// effects
		selectionOutline.Show = false;
	}

	// ========================================================= Move Menu State =========================================================

	protected SimpleTurnPath decidedMovePath;
	protected List<PlayerUnit> lastObstructedUnits = new List<PlayerUnit>();
	protected int selectedAltitude = 0;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void MoveMenuStateEntry()
	{
		// set transforms
		transform.position = preActionPosition;
		transform.rotation = preActionRotation;
		model.localPosition = new Vector3(0f, preActionAltitude * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, preActionTilt);

		// reset selection variables
		decidedMovePath.distance = -10f;
		decidedMovePath.azimuth = 0f;
		decidedMovePath.climb = 0f;

		selectedAltitude = preActionAltitude;

		// projected model
		projectedModel.gameObject.SetActive(false);

		// mesh displays
		currentMoveFullDisplay.Show(true);
		currentMoveFullDisplay.SetColor(new Color(0f, 0.65f, 1f, 0f));

		currentMoveAreaDisplay.Show(true);
		currentMoveAreaDisplay.SetColor(new Color(0f, 0.65f, 1f, 0.5f));

		attackAreaDisplay.Show(true);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.25f));

		nextMoveFullDisplay.Show(false);
		nextMoveFullDisplay.SetColor(new Color(0f, 0.65f, 1f, 0f));

		nextMoveAreaDisplay.Show(false);
		nextMoveAreaDisplay.SetColor(new Color(0f, 0.65f, 1f, 0f));

		movePathDisplay.Show(false);
		movePathDisplay.SetColor(new Color(1f, 0f, 0f, 0.75f));

		attackAreaDisplay.Show(false);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.75f));

		trailsDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				previousMovePaths,
				previousTilts,
				1);

		// effects
		selectionOutline.Color = new Color(0f, 0f, 1f, 1f);
		selectionOutline.Width = 4f;
		selectionOutline.Show = true;

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 1f));

		// 2d uis
		ShowMoveActionMenu(true);
		SetUITypeDisplay(IndicatorType.None);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void MoveMenuStateBody()
	{
		// check if pointing in move area 
		SimpleTurnPath selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);
		bool turnCurveObstructed = false;
		if (playerInput.PolarPlaneCast(transform, selectedAltitude * level.altitudeHeight, out PolarPlaneCastHit hit))
		{
			MoveArea availableArea = moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust);
			MoveArea inputGraceArea = availableArea.Expand(new RangeFloat2(-5f, 5f), new RangeFloat2(-15f, 15f), new RangeFloat2(-15f, 15f));
			if (inputGraceArea.IsInclude(new MoveResult(hit.distance, hit.azimuth)))
			{
				// calculate selected point
				MoveResult moveResult = availableArea.Clamp(new MoveResult(hit.distance, hit.azimuth));
				selectedTurnCurve.distance = moveResult.distance;
				selectedTurnCurve.azimuth = moveResult.azimuth;
				selectedTurnCurve.climb = (selectedAltitude - preActionAltitude) * level.altitudeHeight;

				// check for obstructions
				List<PlayerUnit> obstructedUnits = level.GetAllUnitsInPoint(preActionPosition + preActionRotation * selectedTurnCurve.PointAt(1f), hitboxRadius);
				for (int i = lastObstructedUnits.Count - 1; i >= 0; i--)
				{
					// remove previous obstructions
					lastObstructedUnits[i].RevertCurrentMarker();
					lastObstructedUnits.RemoveAt(i);
				}
				foreach (PlayerUnit obstructedUnit in obstructedUnits)
				{
					// do not obstructed by self
					if (obstructedUnit == this)
					{
						continue;
					}
					// add new obstructions
					turnCurveObstructed = true;
					obstructedUnit.OverrideCurrentMarker(UnitMarkerType.Collision, obstructedUnit.preActionAltitude * level.altitudeHeight, new Color(1f, 0f, 0f, 1f));
					lastObstructedUnits.Add(obstructedUnit);
				}

				// calculate turn results
				if (!turnCurveObstructed)
				{
					// calculate turn results if turn curve is not obstructed
					postActionPosition = preActionPosition + preActionRotation * selectedTurnCurve.PointProjectionAt(1f);
					postActionRotation = Quaternion.FromToRotation(Vector3.forward, selectedTurnCurve.DirectionProjectionAt(1f)) * preActionRotation;
					postActionAltitude = selectedAltitude;
					MoveInput moveInput = moveArea.Inverse(new MoveResult(selectedTurnCurve.distance, selectedTurnCurve.azimuth));
					postActionSpeed = moveInput.speed;
					postActionSteer = moveInput.steer;
					postActionMovePath = selectedTurnCurve;
					postActionTilt = Mathf.Lerp(60f, -60f, moveArea.nearAngles.InverseLerp(selectedTurnCurve.azimuth));
				}

				Vector3 nextMoveLocalPostion = selectedTurnCurve.PointProjectionAt(1f);
				Quaternion nextMoveLocalRotation = Quaternion.FromToRotation(Vector3.forward, selectedTurnCurve.DirectionProjectionAt(1f));

				// projected model
				projectedModel.gameObject.SetActive(true);
				projectedModel.localPosition = nextMoveLocalPostion + Vector3.up * selectedAltitude * level.altitudeHeight;
				projectedModel.localRotation = nextMoveLocalRotation * Quaternion.Euler(0f, 0f, postActionTilt);

				// mesh displays
				currentMoveFullDisplay.Modify(
					selectedAltitude * level.altitudeHeight,
					moveArea);

				currentMoveAreaDisplay.Modify(
						selectedAltitude * level.altitudeHeight,
						moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust));

				nextMoveFullDisplay.Show(true);
				nextMoveFullDisplay.transform.localPosition = nextMoveLocalPostion;
				nextMoveFullDisplay.transform.localRotation = nextMoveLocalRotation;
				nextMoveFullDisplay.Modify(
						selectedAltitude * level.altitudeHeight,
						moveArea);

				nextMoveAreaDisplay.Show(true);
				nextMoveAreaDisplay.transform.localPosition = nextMoveLocalPostion;
				nextMoveAreaDisplay.transform.localRotation = nextMoveLocalRotation;
				nextMoveAreaDisplay.Modify(
						selectedAltitude * level.altitudeHeight,
						moveArea.SubArea(moveArea.Inverse(new MoveResult(selectedTurnCurve.distance, selectedTurnCurve.azimuth)), moveAdjust));

				movePathDisplay.Show(true);
				movePathDisplay.Modify(
						preActionAltitude * level.altitudeHeight,
						selectedTurnCurve);

				attackAreaDisplay.Show(true);
				attackAreaDisplay.transform.localPosition = nextMoveLocalPostion;
				attackAreaDisplay.transform.localRotation = nextMoveLocalRotation;
				attackAreaDisplay.Modify(
						selectedAltitude * level.altitudeHeight,
						attackDistances,
						attackMaxAngle);
				//Debug.Log(selectedTurnCurve.azimuth + ", " + (1f - attackDistanceLoseBySteer.Lerp(selectedTurnCurve.azimuth / 2f + 0.5f)));

				// 3d uis
				if (!turnCurveObstructed)
				{
					SetProjectionMarker(
						UnitMarkerType.Projection,
						selectedAltitude * level.altitudeHeight,
						new Color(0f, 0f, 1f, 1f),
						nextMoveLocalPostion);
				}
				else
				{
					SetProjectionMarker(
						UnitMarkerType.Collision,
						selectedAltitude * level.altitudeHeight,
						new Color(1f, 0f, 01f, 1f),
						nextMoveLocalPostion);
				}
			}
			else
			{
				// set to invalid selected point
				selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);

				// projected model
				projectedModel.gameObject.SetActive(false);

				// mesh displays
				currentMoveFullDisplay.Modify(
					selectedAltitude * level.altitudeHeight,
					moveArea);

				currentMoveAreaDisplay.Modify(
						selectedAltitude * level.altitudeHeight,
						moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust));

				movePathDisplay.Show(false);
				nextMoveFullDisplay.Show(false);
				nextMoveAreaDisplay.Show(false);
				attackAreaDisplay.Show(false);

				// 3d uis
				SetProjectionMarker(UnitMarkerType.None);
			}
		}
		else
		{
			// set to invalid selected point
			selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);

			// projected model
			projectedModel.gameObject.SetActive(false);

			// mesh displays
			currentMoveFullDisplay.Modify(
					selectedAltitude * level.altitudeHeight,
					moveArea);

			currentMoveAreaDisplay.Modify(
					selectedAltitude * level.altitudeHeight,
					moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust));

			movePathDisplay.Show(false);
			nextMoveFullDisplay.Show(false);
			nextMoveAreaDisplay.Show(false);
			attackAreaDisplay.Show(false);

			// 3d uis
			SetProjectionMarker(UnitMarkerType.None);
		}

		// detect user selection
		if (playerInput.IsPressedOnScene())
		{
			if (selectedTurnCurve.distance > 0 && !turnCurveObstructed)
			{
				// move this unit is a vaild path is selected
				decidedMovePath = selectedTurnCurve;
				previousMovePaths.Add(postActionMovePath);
				previousTilts.Add(postActionTilt);
			}
			else
			{
				// unselect this unit otherwise
				level.ActiveUnit = null;
			}
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State MoveMenuStateTransition()
	{
		if (decidedMovePath.distance >= 0)
		{
			return State.MoveAnimation;
		}
		if (level.ActiveUnit != this)
		{
			return State.Idle;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void MoveMenuStateExit()
	{
		// projected model
		projectedModel.gameObject.SetActive(false);

		// mesh displays
		currentMoveFullDisplay.Show(false);
		currentMoveAreaDisplay.Show(false);
		nextMoveFullDisplay.Show(false);
		nextMoveAreaDisplay.Show(false);
		movePathDisplay.Show(false);
		attackAreaDisplay.Show(false);

		// effects
		selectionOutline.Show = false;

		// remove previous obstructions
		for (int i = lastObstructedUnits.Count - 1; i >= 0; i--)
		{
			lastObstructedUnits[i].RevertCurrentMarker();
			lastObstructedUnits.RemoveAt(i);
		}

		// 3d uis
		SetProjectionMarker(UnitMarkerType.None);

		// 2d uis
		ShowMoveActionMenu(false);
	}

	public void CancelMoveState()
	{
		if (level.ActiveUnit == this && stateMachine.Current == State.MoveMenu)
		{
			level.ActiveUnit = null;
		}
	}

	public void ChamgeAttitude(int change)
	{
		if (level.ActiveUnit == this && stateMachine.Current == State.MoveMenu)
		{
			selectedAltitude += change;
		}
	}

	// ========================================================= Move Animation State =========================================================

	protected float moveAnimationTimer = 0;
	protected float moveAnimationDuration = 0.75f;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void MoveAnimationStateEntry()
	{
		transform.position = preActionPosition;
		transform.rotation = preActionRotation;
		model.localPosition = new Vector3(0f, preActionAltitude * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, preActionTilt);
		moveAnimationTimer = 0;
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void MoveAnimationStateBody()
	{
		moveAnimationTimer += Time.deltaTime;

		// calculate transforms
		transform.position = preActionPosition + preActionRotation * decidedMovePath.PointProjectionAt(moveAnimationTimer / moveAnimationDuration);
		transform.rotation = Quaternion.FromToRotation(Vector3.forward, decidedMovePath.DirectionProjectionAt(moveAnimationTimer / moveAnimationDuration)) * preActionRotation;
		model.localPosition = new Vector3(0f, Mathf.SmoothStep(preActionAltitude, postActionAltitude, moveAnimationTimer / moveAnimationDuration) * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, Mathf.SmoothStep(preActionTilt, postActionTilt, moveAnimationTimer / moveAnimationDuration));

		// mesh display
		trailsDisplay.Modify(
				postActionAltitude * level.altitudeHeight,
				previousMovePaths.GetRange(1, previousMovePaths.Count - 1),
				previousTilts.GetRange(1, previousTilts.Count - 1),
				moveAnimationTimer / moveAnimationDuration);

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, Mathf.SmoothStep(preActionAltitude, postActionAltitude, moveAnimationTimer / moveAnimationDuration) * level.altitudeHeight, new Color(0f, 0f, 1f, 1f));
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State MoveAnimationStateTransition()
	{
		if (moveAnimationTimer >= moveAnimationDuration)
		{
			return State.AttackMenu;
		}

		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void MoveAnimationStateExit()
	{
		// calculate transforms
		transform.position = postActionPosition;
		transform.rotation = postActionRotation;
		model.localPosition = new Vector3(0f, postActionAltitude * level.altitudeHeight, 0f);
		model.localRotation = Quaternion.Euler(0f, 0f, postActionTilt);
		moveAnimationTimer = 0;

		// mesh display
		trailsDisplay.Modify(
				postActionAltitude * level.altitudeHeight,
				previousMovePaths.GetRange(1, previousMovePaths.Count - 1),
				previousTilts.GetRange(1, previousTilts.Count - 1),
				1);

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, postActionAltitude * level.altitudeHeight, new Color(0f, 0f, 0f, 1f));
	}

	// ========================================================= Attack Menu State =========================================================

	protected AttackInfo decidedAttack;
	protected PlayerUnit lastAttackTarget = null;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void AttackMenuStateEntry()
	{
		decidedAttack.target = null;
		decidedAttack.probability = 0;

		attackAreaDisplay.Show(true);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.5f));
		attackAreaDisplay.transform.localPosition = Vector3.zero;
		attackAreaDisplay.transform.localRotation = Quaternion.identity;

		targetIndicator.gameObject.SetActive(false);
		targetIndicatorRoll.text = "@0";
		targetIndicatorDamage.text = "0%";

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, postActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 1f));

		// 2d uis
		ShowAttackActionMenu(true);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void AttackMenuStateBody()
	{
		// remove previous target
		if (lastAttackTarget != null)
		{
			lastAttackTarget.RevertCurrentMarker();
		}

		// check if pointing in valid target
		PlayerUnit target = playerInput.AnyHovering();
		float probability = 0;
		if (target != null && target.teamInfo != teamInfo && postActionAltitude == target.preActionAltitude && target.stateMachine.Current == State.Wait)
		{
			// check if target is in range
			float targetDistance = Vector3.Distance(target.transform.position, transform.position);
			float targetAngle = Vector3.SignedAngle(transform.forward, target.transform.position - transform.position, Vector3.up);
			float maxDistanceByTarget = attackDistances.max + target.hitboxRadius;
			float maxAngleByTarget = attackMaxAngle + Mathf.Asin(target.hitboxRadius / targetDistance) * Mathf.Rad2Deg;
			if (targetDistance < maxDistanceByTarget && Mathf.Abs(targetAngle) < maxAngleByTarget)
			{
				// calculate possible damage to target if in range
				probability = attackBaseProbability *
					Mathf.Lerp(1, 0, Mathf.InverseLerp(attackDistances.midg, maxDistanceByTarget, targetDistance)) *
					Mathf.Lerp(1, 0, Mathf.InverseLerp(attackMaxAngle, maxAngleByTarget, targetAngle));

				// show damage information to ui
				targetIndicatorRoll.text = $"@{attackPower}x{attackRolls}";
				targetIndicatorDamage.text = $"{probability * 100f:f0}%";
				targetUI.position = Camera.main.WorldToScreenPoint(target.model.position);
				targetIndicator.gameObject.SetActive(true);

				// show target effect
				target.OverrideCurrentMarker(UnitMarkerType.Damage, target.preActionAltitude * level.altitudeHeight, new Color(1f, 0.29f, 0f, 1f));
				lastAttackTarget = target;
			}
			else
			{
				// target is out of range
				target = null;
				probability = 0;
				lastAttackTarget = null;

				// hide damage ui
				targetIndicator.gameObject.SetActive(false);
			}
		}
		else
		{
			// no valid target
			target = null;
			probability = 0;
			lastAttackTarget = null;

			// hide damage ui
			targetIndicator.gameObject.SetActive(false);
		}

		// action when player presses
		if (playerInput.IsPressedOnScene())
		{
			if (target != null)
			{
				// attack target is a vaild unit is selected
				decidedAttack.target = target;
				decidedAttack.probability = probability;

				// calculate turn results
				List<float> hits = new List<float>();
				for (int i = 0; i < attackRolls; i++)
				{
					hits.Add(attackPower * (UnityEngine.Random.Range(0f, 1f) <= decidedAttack.probability ? 1 : 0));
				}
				decidedAttack.target.TakeDamage(this, hits);
				previousMovePaths.RemoveAt(0);
				previousTilts.RemoveAt(0);
			}
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State AttackMenuStateTransition()
	{
		if (decidedMovePath.distance < 0)
		{
			return State.MoveMenu;
		}
		if (decidedAttack.target != null)
		{
			return State.AttackAnimation;
		}
		if (level.ActiveUnit != this)
		{
			return State.Done;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void AttackMenuStateExit()
	{
		attackAreaDisplay.Show(false);

		targetIndicator.gameObject.SetActive(false);
		targetIndicatorRoll.text = "@0";
		targetIndicatorDamage.text = "0%";

		// 3d uis
		SetCurrentMarker(UnitMarkerType.None);
		if (lastAttackTarget != null)
		{
			lastAttackTarget.RevertCurrentMarker();
		}

		// 2d uis
		ShowAttackActionMenu(false);
	}

	public void UndoAttackState()
	{
		if (level.ActiveUnit != this && CurrentState != State.AttackMenu)
			return;

		decidedMovePath = new SimpleTurnPath(-10f, 0f, 0f);
		previousMovePaths.RemoveAt(previousMovePaths.Count - 1);
		previousTilts.RemoveAt(previousTilts.Count - 1);
	}

	public void SkipAttackState()
	{
		if (level.ActiveUnit != this && CurrentState != State.AttackMenu)
			return;

		level.ActiveUnit = null;
		previousMovePaths.RemoveAt(0);
		previousTilts.RemoveAt(0);
	}

	// ========================================================= Attack Animation State =========================================================

	protected float attackAnimationTimer = 0;
	protected float attackAnimationDuration = 1f;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void AttackAnimationStateEntry()
	{
		attackAnimationTimer = 0;

		ParticleSystem.MainModule main = attackEffect.main;
		main.duration = attackAnimationDuration;
		ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
		startLifetime.constant = attackDistances.max / main.startSpeed.constant;
		main.startLifetime = startLifetime;

		ParticleSystem.EmissionModule emission = attackEffect.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		rateOverTime.constant = attackRolls / attackAnimationDuration;
		emission.rateOverTime = rateOverTime;

		attackEffect.Play();
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void AttackAnimationStateBody()
	{
		attackAnimationTimer += Time.deltaTime;
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State AttackAnimationStateTransition()
	{
		if (attackAnimationTimer >= attackAnimationDuration)
		{
			return State.Done;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void AttackAnimationStateExit()
	{
		level.ActiveUnit = null;

		decidedAttack.target = null;
		decidedAttack.probability = 0;

		attackEffect.Stop();
		attackAnimationTimer = 0;
	}

	// ========================================================= Done State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void DoneStateEntry()
	{
		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, postActionAltitude * level.altitudeHeight, new Color(0f, 0f, 1f, 0.5f));

		// 2d uis
		SetUIAsDone(true);
		SetUITypeDisplay(IndicatorType.Mini);
		SetUIHealthDisplay(postActionHealth);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void DoneStateBody()
	{
		if (level.ActiveUnit == null && playerInput.IsHovering(this))
		{
			SetUITypeDisplay(IndicatorType.Normal);
		}
		else
		{
			SetUITypeDisplay(IndicatorType.Mini);
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State DoneStateTransition()
	{
		if (level.CurrentState == LevelController.State.Solidify)
		{
			return State.Solidify;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void DoneStateExit()
	{
		SetUIAsDone(false);
		SetUITypeDisplay(IndicatorType.None);
	}

	// ========================================================= Wait State =========================================================

	protected DamageInfo recievedDamage;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void WaitStateEntry()
	{
		// set position
		transform.position = preActionPosition;
		transform.rotation = preActionRotation;
		model.localRotation = Quaternion.Euler(0f, 0f, preActionTilt);

		// mesh displays
		currentMoveFullDisplay.Show(false);
		currentMoveFullDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0f));
		currentMoveFullDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				moveArea);

		currentMoveAreaDisplay.Show(false);
		currentMoveAreaDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0.25f));

		currentMoveAreaDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust));

		attackAreaDisplay.Show(false);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.25f));
		attackAreaDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				attackDistances,
				attackMaxAngle);

		trailsDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				previousMovePaths,
				previousTilts,
				1);

		// effects
		selectionOutline.Show = false;

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(1f, 0f, 0f, 0.5f));

		// 2d uis
		SetUITypeDisplay(IndicatorType.Mini);
		SetUIAsDone(false);
		SetUIHealthDisplay(preActionHealth);
		SetUIAttitudeDisplay(preActionAltitude);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void WaitStateBody()
	{
		if (playerInput.IsHovering(this))
		{
			// mesh displays
			currentMoveFullDisplay.Show(true);
			currentMoveAreaDisplay.Show(true);
			attackAreaDisplay.Show(true);

			// effects
			selectionOutline.Color = new Color(1f, 0f, 0f, 1f);
			selectionOutline.Width = 4f;
			selectionOutline.Show = true;

			// 3d uis
			SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(1f, 0f, 0f, 1f));

			// 2d uis
			SetUITypeDisplay(IndicatorType.Normal);
		}
		else
		{
			// mesh displays
			currentMoveFullDisplay.Show(false);
			currentMoveAreaDisplay.Show(false);
			attackAreaDisplay.Show(false);

			// effects
			selectionOutline.Show = false;

			// 3d uis
			SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(1f, 0f, 0f, 0.5f));

			// 2d uis
			SetUITypeDisplay(IndicatorType.Mini);
		}

		if (level.ActiveUnit == null && playerInput.IsSelected(this))
		{
			level.ActiveUnit = this;
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State WaitStateTransition()
	{
		if (level.ActiveUnit == this)
		{
			return State.AttackPrediction;
		}
		if (recievedDamage.source != null)
		{
			return State.DamageAnimation;
		}
		if (level.CurrentState == LevelController.State.Solidify)
		{
			return State.Solidify;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void WaitStateExit()
	{
		// mesh displays
		currentMoveFullDisplay.Show(false);
		currentMoveAreaDisplay.Show(false);
		attackAreaDisplay.Show(false);

		// effects
		selectionOutline.Show = false;
	}

	public void TakeDamage(PlayerUnit origin, List<float> hits)
	{
		if (CurrentState != State.Wait)
			return;

		recievedDamage.source = origin;
		recievedDamage.hits = hits;
	}

	// ========================================================= Attack Predition State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void AttackPredictionStateEntry()
	{
		currentMoveFullDisplay.Show(true);
		currentMoveFullDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0f));

		currentMoveAreaDisplay.Show(true);
		currentMoveAreaDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0.25f));

		nextMoveFullDisplay.Show(false);
		nextMoveFullDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0f));

		nextMoveAreaDisplay.Show(false);
		nextMoveAreaDisplay.SetColor(new Color(1f, 0.25f, 0.25f, 0f));

		movePathDisplay.Show(false);
		movePathDisplay.SetColor(new Color(1f, 0f, 0f, 0.75f));

		attackAreaDisplay.Show(false);
		attackAreaDisplay.SetColor(new Color(1f, 0.29f, 0f, 0.75f));

		attackAreaDisplay.transform.localPosition = Vector3.zero;
		attackAreaDisplay.transform.localRotation = Quaternion.identity;

		// effects
		selectionOutline.Color = new Color(1f, 0f, 0f, 1f);
		selectionOutline.Width = 4f;
		selectionOutline.Show = true;

		// 3d uis
		SetCurrentMarker(UnitMarkerType.Normal, preActionAltitude * level.altitudeHeight, new Color(1f, 0f, 0f, 1f));

		// 2d uis
		SetUITypeDisplay(IndicatorType.None);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void AttackPredictionStateBody()
	{
		// check if pointing in move area 
		SimpleTurnPath selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);
		if (playerInput.PolarPlaneCast(transform, preActionAltitude * level.altitudeHeight, out PolarPlaneCastHit hit))
		{
			MoveArea availableArea = moveArea.SubArea(new MoveInput(preActionSpeed, preActionSteer), moveAdjust);
			MoveArea inputGraceArea = availableArea.Expand(new RangeFloat2(-2f, 2f), new RangeFloat2(-15f, 15f), new RangeFloat2(-15f, 15f));
			if (inputGraceArea.IsInclude(new MoveResult(hit.distance, hit.azimuth)))
			{
				// calculate selected point
				MoveResult moveResult = availableArea.Clamp(new MoveResult(hit.distance, hit.azimuth));
				selectedTurnCurve.distance = moveResult.distance;
				selectedTurnCurve.azimuth = moveResult.azimuth;
				selectedTurnCurve.climb = 0f;

				// recreate meshes
				movePathDisplay.Modify(
						preActionAltitude * level.altitudeHeight,
						selectedTurnCurve);

				nextMoveAreaDisplay.Modify(
						preActionAltitude * level.altitudeHeight,
						moveArea.SubArea(moveArea.Inverse(new MoveResult(selectedTurnCurve.distance, selectedTurnCurve.azimuth)), moveAdjust));

				// reposition meshes
				Vector3 nextMoveLocalPostion = selectedTurnCurve.PointAt(1f);
				Vector3 nextMoveHeight = Vector3.up * preActionAltitude * level.altitudeHeight;
				Quaternion nextMoveLocalRotation = Quaternion.FromToRotation(Vector3.forward, selectedTurnCurve.DirectionProjectionAt(1f));
				nextMoveFullDisplay.transform.localPosition = nextMoveLocalPostion;
				nextMoveFullDisplay.transform.localRotation = nextMoveLocalRotation;
				nextMoveAreaDisplay.transform.localPosition = nextMoveLocalPostion;
				nextMoveAreaDisplay.transform.localRotation = nextMoveLocalRotation;
				attackAreaDisplay.transform.localPosition = nextMoveLocalPostion;
				attackAreaDisplay.transform.localRotation = nextMoveLocalRotation;

				// show required display
				movePathDisplay.Show(true);
				nextMoveFullDisplay.Show(true);
				nextMoveAreaDisplay.Show(true);
				attackAreaDisplay.Show(true);
			}
			else
			{
				// set to invalid selected point
				selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);

				// hide required display
				movePathDisplay.Show(false);
				nextMoveFullDisplay.Show(false);
				nextMoveAreaDisplay.Show(false);
				attackAreaDisplay.Show(false);
			}
		}
		else
		{
			// set to invalid selected point
			selectedTurnCurve = new SimpleTurnPath(-10f, 0f, 0f);

			// hide required display
			movePathDisplay.Show(false);
			nextMoveFullDisplay.Show(false);
			nextMoveAreaDisplay.Show(false);
			attackAreaDisplay.Show(false);
		}

		// action when player presses
		if (playerInput.IsPressedOnScene())
		{
			// always unselect this unit
			level.ActiveUnit = null;
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State AttackPredictionStateTransition()
	{
		if (level.ActiveUnit != this)
		{
			return State.Wait;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void AttackPredictionStateExit()
	{
		currentMoveFullDisplay.Show(false);
		currentMoveAreaDisplay.Show(false);
		nextMoveFullDisplay.Show(false);
		nextMoveAreaDisplay.Show(false);
		movePathDisplay.Show(false);
		attackAreaDisplay.Show(false);
		attackAreaDisplay.transform.localPosition = Vector3.zero;
		attackAreaDisplay.transform.localRotation = Quaternion.identity;
		selectionOutline.Show = false;
	}

	// ========================================================= Damage Animation State =========================================================

	protected float displayedHealth = 0;
	protected int damageCounter = 1;
	protected float damageAnimationTimer = 0;
	protected float damageAnimationDuration = 1f;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void DamageAnimationStateEntry()
	{
		displayedHealth = preActionHealth;
		damageCounter = 0;
		damageAnimationTimer = 0;

		Color newDamageColor = damageOverlay.Color;
		newDamageColor.a = 0f;
		damageOverlay.Color = newDamageColor;
		damageOverlay.Show = true;

		SetUITypeDisplay(IndicatorType.Normal);
		SetUIHealthDisplay(preActionHealth);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void DamageAnimationStateBody()
	{
		damageAnimationTimer += Time.deltaTime;

		if (damageAnimationTimer >= damageCounter * damageAnimationDuration / (recievedDamage.hits.Count + 2))
		{
			if (damageCounter > 0 && damageCounter < recievedDamage.hits.Count + 1)
			{
				if (recievedDamage.hits[damageCounter - 1] > 0)
				{
					displayedHealth -= recievedDamage.hits[damageCounter - 1];

					ParticleSystem.ShapeModule shape = hitEffect.shape;
					Vector3 otherDirection = (recievedDamage.source.transform.position - transform.position).normalized;
					shape.position = Vector3.Cross(otherDirection, transform.up) * UnityEngine.Random.Range(-1f, 1f) + transform.up * UnityEngine.Random.Range(-0.5f, 0.5f);
					shape.rotation = Quaternion.FromToRotation(transform.forward, otherDirection).eulerAngles;
					hitEffect.Play();

					Color newDamageColor = damageOverlay.Color;
					newDamageColor.a = 0.75f;
					damageOverlay.Color = newDamageColor;

					SetUIHealthDisplay(displayedHealth);
				}
			}
			damageCounter++;
		}
		else
		{
			if (damageOverlay.Color.a > 0)
			{
				Color newDamageColor = damageOverlay.Color;
				newDamageColor.a = damageOverlay.Color.a - 5f * Time.deltaTime;
				damageOverlay.Color = newDamageColor;
			}
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State DamageAnimationStateTransition()
	{
		if (displayedHealth <= 0)
		{
			return State.DownAnimation;
		}
		if (damageAnimationTimer >= damageAnimationDuration)
		{
			return State.Wait;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void DamageAnimationStateExit()
	{
		postActionHealth = preActionHealth - recievedDamage.hits.Aggregate(0f, (x, y) => x + y);
		preActionHealth = postActionHealth;
		recievedDamage.source = null;
		recievedDamage.hits = null;

		displayedHealth = 0;
		damageCounter = 0;
		damageAnimationTimer = 0;

		Color newDamageColor = damageOverlay.Color;
		newDamageColor.a = 0f;
		damageOverlay.Color = newDamageColor;
		damageOverlay.Show = false;

		SetUITypeDisplay(IndicatorType.None);
		SetUIHealthDisplay(preActionHealth);
	}

	// ========================================================= Down Animation State =========================================================

	protected float downAnimationFallSpeed = -19.6f;
	protected float downAnimationDuration = 0;
	protected float downAnimationTimer = 0;
	protected IPath[] downAnimationCurves;
	protected float[] downAnimationTilts;
	protected int downAnimationPhase = 0;
	protected bool downAnimationExploded = false;

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void DownAnimationStateEntry()
	{
		downAnimationDuration = Mathf.Sqrt(2f * -60f / downAnimationFallSpeed);
		downAnimationTimer = 0f;
		downAnimationPhase = 0;
		downAnimationExploded = false;

		// add move path to trails calculation
		downAnimationCurves = new IPath[] {
				new DropTrunPath(preActionMovePath, downAnimationFallSpeed, downAnimationDuration),
				new BlankPath(),
				new BlankPath()};
		downAnimationTilts = new float[] {
				preActionTilt,
				preActionTilt,
				preActionTilt};
		previousMovePaths.AddRange(downAnimationCurves);
		previousTilts.AddRange(downAnimationTilts);

		postActionPosition = preActionPosition;
		postActionRotation = preActionRotation;
		postActionTilt = preActionTilt;

		smokeEffect.Play();

		SetUITypeDisplay(IndicatorType.None);
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void DownAnimationStateBody()
	{
		downAnimationTimer += Time.deltaTime;

		// progress phase
		if (downAnimationTimer > downAnimationDuration * (downAnimationPhase + 1) && downAnimationPhase < downAnimationCurves.Length - 1)
		{
			downAnimationPhase++;
			postActionPosition = transform.position;
			postActionRotation = transform.rotation;
			postActionTilt = preActionTilt;
		}

		// perform each curve
		transform.position = postActionPosition + postActionRotation * downAnimationCurves[downAnimationPhase].PointAt((downAnimationTimer % downAnimationDuration) / downAnimationDuration);
		transform.rotation = Quaternion.FromToRotation(Vector3.forward, downAnimationCurves[downAnimationPhase].DirectionProjectionAt((downAnimationTimer % downAnimationDuration) / downAnimationDuration)) * postActionRotation;
		model.localRotation = Quaternion.FromToRotation(Vector3.up, downAnimationCurves[downAnimationPhase].UpAt((downAnimationTimer % downAnimationDuration) / downAnimationDuration)) * Quaternion.Euler(0f, 0f, postActionTilt);

		// recalculate the trails
		trailsDisplay.Modify(
					preActionAltitude * level.altitudeHeight,
					previousMovePaths.GetRange(downAnimationPhase + 1, previousMovePaths.Count - downAnimationCurves.Length),
					previousTilts.GetRange(downAnimationPhase + 1, previousTilts.Count - downAnimationCurves.Length),
					(downAnimationTimer % downAnimationDuration) / downAnimationDuration);

		// show explosion animation at impact
		if (!downAnimationExploded)
		{
			if (transform.position.y < 0)
			{
				smokeEffect.Stop();
				explosionEffect.Play();
				downAnimationExploded = true;
			}
		}
	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State DownAnimationStateTransition()
	{
		if (downAnimationTimer > downAnimationDuration * 3)
		{
			return State.Downed;
		}
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void DownAnimationStateExit()
	{
		transform.position = postActionPosition + postActionRotation * downAnimationCurves[2].PointAt(1);
		transform.rotation = Quaternion.FromToRotation(Vector3.forward, downAnimationCurves[2].DirectionProjectionAt(1)) * postActionRotation;
		model.localRotation = Quaternion.FromToRotation(Vector3.up, downAnimationCurves[2].UpAt(1)) * Quaternion.Euler(0f, 0f, postActionTilt);

		// recalculate the trails
		trailsDisplay.Modify(
				preActionAltitude * level.altitudeHeight,
				previousMovePaths.GetRange(3, previousMovePaths.Count - 3),
				previousTilts.GetRange(3, previousTilts.Count - 3),
				1);

		smokeEffect.Stop();
	}

	// ========================================================= Downed State =========================================================

	/// <summary>
	/// Entry is called on the frame the state is entering, before body is called.
	/// </summary>
	protected void DownedStateEntry()
	{
	}

	/// <summary>
	/// Body is called each frame the state is in.
	/// </summary>
	protected void DownedStateBody()
	{

	}

	/// <summary>
	/// Transition is called each frame the state is in, after body is called. Return None if no transition is needed.
	/// </summary>
	protected State DownedStateTransition()
	{
		return State.None;
	}

	/// <summary>
	/// Exit is called on the frame the state is leaving, after body is called and transition is determined.
	/// </summary>
	protected void DownedStateExit()
	{
	}

}
