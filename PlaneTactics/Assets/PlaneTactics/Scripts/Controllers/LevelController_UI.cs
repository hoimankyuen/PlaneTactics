using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public partial class LevelController : MonoBehaviour
{
	protected Canvas uiCanvas;

	// normal frame
	protected RectTransform normalFrame;
	protected Button nextUnitButton;
	protected Button nextTurnButton;
	protected Image activeTeamInsignia;
	protected Image nextTeamInsignia;
	protected Image[] frameAccents;
	protected TextMeshProUGUI turnText;

	protected Animator turnDisplayAnimator;
	protected Image[] turnDisplayInsignias;
	protected TextMeshProUGUI turnDisplayPreviousText;
	protected TextMeshProUGUI turnDisplayNextText;

	// mini map
	protected Vector3 lastViewPosition;
	protected RectTransform minimapMask;
	protected UILineRenderer minimapViewDisplayLineRenderer;
	protected RectTransform minimapUnitMarkerPrefab;
	protected List<Tuple<PlayerUnit, RectTransform>> minimapUnitMarkers = new List<Tuple<PlayerUnit, RectTransform>>();

	// attack frame
	protected RectTransform attackFrame;
	protected Image attackFrameImage;

	// cache
	protected TeamInfo currentDisplayedTeamInfo;
	protected Coroutine setTeamInfoDisplayCoroutine;

	// ========================================================= Initiation and Destruction =========================================================

	/// <summary>
	/// Retrieve reference related to UI.
	/// </summary>
	protected void RetrieveUIReferences()
	{
		// normal frame
		uiCanvas = transform.Find("UIs").GetComponent<Canvas>();
		uiCanvas.worldCamera = Camera.main;
		normalFrame = uiCanvas.transform.Find("NormalFrame").GetComponent<RectTransform>();
		nextUnitButton = normalFrame.transform.Find("NextUnitButton").GetComponent<Button>();
		nextTurnButton = normalFrame.transform.Find("NextTurnButton").GetComponent<Button>();
		activeTeamInsignia = normalFrame.transform.Find("TopBar/Insignia").GetComponent<Image>();
		nextTeamInsignia = normalFrame.transform.Find("TopBar/InsigniaNext").GetComponent<Image>();
		frameAccents = new Image[] {
			normalFrame.transform.Find("TopBar/TopFrameAccentLeft").GetComponent<Image>(),
			normalFrame.transform.Find("TopBar/TopFrameAccentRight").GetComponent<Image>(),
		};
		turnText = normalFrame.transform.Find("TopBar/TurnText").GetComponent<TextMeshProUGUI>();


		turnDisplayAnimator = normalFrame.transform.Find("TurnDisplay").GetComponent<Animator>();
		turnDisplayInsignias = new Image[] {
			turnDisplayAnimator.transform.Find("InsigniaUpper/Insignia").GetComponent<Image>(),
			turnDisplayAnimator.transform.Find("InsigniaLower/Insignia").GetComponent<Image>(),
		};
		turnDisplayPreviousText = turnDisplayAnimator.transform.Find("TurnPreviousText").GetComponent<TextMeshProUGUI>();
		turnDisplayNextText = turnDisplayAnimator.transform.Find("TurnNextText").GetComponent<TextMeshProUGUI>();

		// map
		minimapMask = normalFrame.transform.Find("Map/MapMask").GetComponent<RectTransform>();
		minimapUnitMarkerPrefab = minimapMask.transform.Find("MapUnitMarker").GetComponent<RectTransform>();
		minimapViewDisplayLineRenderer = minimapMask.transform.Find("MapViewLineRenderer").GetComponent<UILineRenderer>();
		minimapUnitMarkerPrefab.gameObject.SetActive(false);

		// attack frame
		attackFrame = uiCanvas.transform.Find("AttackFrame").GetComponent<RectTransform>();
		attackFrameImage = attackFrame.transform.Find("Frame").GetComponent<Image>();

		// cache
		currentDisplayedTeamInfo = null;
	}

	// ========================================================= Driver =========================================================

	/// <summary>
	/// Per frame updates for UI.
	/// </summary>
	protected void UpdateUIs()
	{
		RedrawMinimap();
		UpdateUIButtons();
	}

	// ========================================================= Per frame Updates =========================================================

	/// <summary>
	/// Redraw the minimap
	/// </summary>
	protected void RedrawMinimap()
	{
		// rotate map
		minimapMask.transform.localEulerAngles = new Vector3(0f, 0f, cameraControl.transform.localEulerAngles.y);

		// redraw view display
		if (cameraControl.Main.transform.position != lastViewPosition)
		{
			Plane boardPlane = new Plane(Vector3.up, new Vector3(0f, 50f, 0f));
			float distance = 0;
			Ray cornerRay;
			Vector3 conerPoint;

			// bottom left
			cornerRay = cameraControl.Main.ViewportPointToRay(new Vector3(0, 0, 1));
			boardPlane.Raycast(cornerRay, out distance);
			conerPoint = cornerRay.GetPoint(distance);
			minimapViewDisplayLineRenderer.Points[0] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);
			minimapViewDisplayLineRenderer.Points[4] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);

			// bottom right
			cornerRay = cameraControl.Main.ViewportPointToRay(new Vector3(1, 0, 1));
			boardPlane.Raycast(cornerRay, out distance);
			conerPoint = cornerRay.GetPoint(distance);
			minimapViewDisplayLineRenderer.Points[1] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);
			minimapViewDisplayLineRenderer.Points[5] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);

			// top right
			cornerRay = cameraControl.Main.ViewportPointToRay(new Vector3(1, 1, 1));
			boardPlane.Raycast(cornerRay, out distance);
			if (distance > 0)
			{
				conerPoint = cornerRay.GetPoint(distance);
				minimapViewDisplayLineRenderer.Points[2] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);
			}
			else
			{
				float fov = Camera.VerticalToHorizontalFieldOfView(cameraControl.Main.fieldOfView, cameraControl.Main.aspect);
				Vector2 centerPos = new Vector2(cameraControl.transform.position.x / 2000f + 0.5f, cameraControl.transform.position.z / 2000f + 0.5f);
				minimapViewDisplayLineRenderer.Points[2] = new Vector2(
					centerPos.x + Mathf.Cos((cameraControl.transform.eulerAngles.y * -1f - fov / 2f + 90f) * Mathf.Deg2Rad) * 2f,
					centerPos.y + Mathf.Sin((cameraControl.transform.eulerAngles.y * -1f - fov / 2f + 90f) * Mathf.Deg2Rad) * 2f);
			}

			// top left
			cornerRay = cameraControl.Main.ViewportPointToRay(new Vector3(0, 1, 1));
			boardPlane.Raycast(cornerRay, out distance);
			if (distance > 0)
			{
				conerPoint = cornerRay.GetPoint(distance);
				minimapViewDisplayLineRenderer.Points[3] = new Vector2(conerPoint.x / 2000f + 0.5f, conerPoint.z / 2000f + 0.5f);
			}
			else
			{
				float fov = Camera.VerticalToHorizontalFieldOfView(cameraControl.Main.fieldOfView, cameraControl.Main.aspect);
				Vector2 centerPos = new Vector2(cameraControl.transform.position.x / 2000f + 0.5f, cameraControl.transform.position.z / 2000f + 0.5f);
				minimapViewDisplayLineRenderer.Points[3] = new Vector2(
					centerPos.x + Mathf.Cos((cameraControl.transform.eulerAngles.y * -1f + fov / 2f + 90f) * Mathf.Deg2Rad) * 2f,
					centerPos.y + Mathf.Sin((cameraControl.transform.eulerAngles.y * -1f + fov / 2f + 90f) * Mathf.Deg2Rad) * 2f);
			}

			minimapViewDisplayLineRenderer.SetVerticesDirty();
			lastViewPosition = cameraControl.Main.transform.position;
		}

		// redraw unit marker
		foreach (Tuple<PlayerUnit, RectTransform> mapUnitMarker in minimapUnitMarkers)
		{
			if (mapUnitMarker.Item1.CurrentState != PlayerUnit.State.Downed)
			{
				mapUnitMarker.Item2.anchoredPosition = new Vector2(
					Mathf.Lerp(minimapMask.sizeDelta.x / -2f, minimapMask.sizeDelta.x / 2f, Mathf.InverseLerp(-1000f, 1000f, mapUnitMarker.Item1.transform.position.x)),
					Mathf.Lerp(minimapMask.sizeDelta.y / -2f, minimapMask.sizeDelta.y / 2f, Mathf.InverseLerp(-1000f, 1000f, mapUnitMarker.Item1.transform.position.z)));
				mapUnitMarker.Item2.localEulerAngles = new Vector3(0, 0, mapUnitMarker.Item1.transform.localEulerAngles.y * -1f);
				mapUnitMarker.Item2.gameObject.SetActive(true);
			}
			else
			{
				mapUnitMarker.Item2.gameObject.SetActive(false);
			}
		}
	}
	
	/// <summary>
	/// Activate buttons accordingly.
	/// </summary>
	protected void UpdateUIButtons()
	{
		if (ActiveTeam != null)
		{
			nextUnitButton.interactable = units[ActiveTeam].Any(x => x.gameObject.activeInHierarchy && x.CurrentState == PlayerUnit.State.Idle);
			nextTurnButton.interactable = units[ActiveTeam].TrueForAll(x => x.gameObject.activeInHierarchy && (x.CurrentState == PlayerUnit.State.Done || x.CurrentState == PlayerUnit.State.Downed));
		}
	}

	// ========================================================= Controls =========================================================

	/// <summary>
	/// Set the turn display on the main frame.
	/// </summary>
	protected void SetTurnDisplay(int turn)
	{
		turnText.text = $"Turn : {turn}";
	}

	/// <summary>
	/// Set the team info display on the main frame.
	/// </summary>
	protected void SetTeamInfoDisplay(TeamInfo team)
	{
		if (currentDisplayedTeamInfo == null)
		{
			// directly set display if none set
			activeTeamInsignia.sprite = team.insignia;
			nextTeamInsignia.enabled = false;
			foreach (Image frameAccent in frameAccents)
			{
				frameAccent.color = team.color;
			}
			currentDisplayedTeamInfo = team;
		}
		else
		{
			// show simple animation
			if (setTeamInfoDisplayCoroutine != null)
			{
				StopCoroutine(setTeamInfoDisplayCoroutine);
			}
			setTeamInfoDisplayCoroutine = StartCoroutine(SetTeamInfoDisplayAnimation(team));
		}
	}
	protected IEnumerator SetTeamInfoDisplayAnimation(TeamInfo team)
	{
		// timer
		float duration = 0.5f;
		float timer = 0f;

		// initialization
		nextTeamInsignia.enabled = true;
		nextTeamInsignia.sprite = team.insignia;

		// animation
		while (timer <= duration)
		{
			activeTeamInsignia.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, timer / duration));
			nextTeamInsignia.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, timer / duration));
			foreach (Image frameAccent in frameAccents)
			{
				frameAccent.color = Color.Lerp(currentDisplayedTeamInfo.color, team.color, timer / duration);
			}
			timer += Time.deltaTime;
			yield return null;
		}

		// finalize
		activeTeamInsignia.sprite = team.insignia;
		activeTeamInsignia.color = Color.white;
		nextTeamInsignia.enabled = false;
		foreach (Image frameAccent in frameAccents)
		{
			frameAccent.color = team.color;
		}
		currentDisplayedTeamInfo = team;
		setTeamInfoDisplayCoroutine = null;
	}

	/// <summary>
	/// Play the animation of turn display and retrieve the animation length.
	/// </summary>
	protected float PlayTurnDisplayAnimation(TeamInfo team, int currentTurn, int nextTurn)
	{
		foreach (Image turnDisplayInsignia in turnDisplayInsignias)
		{
			turnDisplayInsignia.sprite = team.insignia;
		}
		turnDisplayPreviousText.text = string.Format("<mspace=0.5em>{0,4}</mspace>", currentTurn);
		turnDisplayNextText.text = string.Format("<mspace=0.5em>{0,4}</mspace>", nextTurn);

		if (currentTurn == nextTurn)
		{
			
			turnDisplayAnimator.SetTrigger("ChangeTeam");
			return 1f + 40f / 60f;
		}
		else
		{
			turnDisplayAnimator.SetTrigger("ChangeTeamAndTurn");
			return 2f + 20f / 60f;
		}
	}
}
