using SimpleMaskCutoff;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerUnit : MonoBehaviour
{
	protected enum IndicatorType
	{
		None,
		Normal,
		Mini
	}

	// containers
	protected Canvas uiCanvas;
	protected RectTransform selfUI;
	protected RectTransform targetUI;
	protected Transform worldUIs;

	// normal indicator
	protected RectTransform normalIndicator;
	protected Image normalIndicatorTeam;
	protected Image normalIndicatorFrame;
	protected Image normalIndicatorClass;
	protected CutoffImage normalIndicatorHealth;
	protected Image normalIndicatorAltitude;
	protected Image normalIndicatorTick;
	protected Image normalIndicatorBuff;
	protected Image normalIndicatorBuffMagnitude;
	protected ImageDatabase normalIndicatorClassDatabase;
	protected ImageDatabase normalIndicatorAltitudeDatabase;
	protected ImageDatabase normalIndicatorBuffDatabase;
	protected ImageDatabase normalIndicatorBuffMagnitudeDatabase;
	protected TextMeshProUGUI normalIndicatorName;

	// mini indicator
	protected RectTransform miniIndicator;
	protected Image miniIndicatorFrame;
	protected Image miniIndicatorTick;
	protected Image miniIndicatorAltitude;
	protected ImageDatabase miniIndicatorFrameDatabase;
	protected ImageDatabase miniIndicatorAltitudeDatabase;

	// target indicator
	protected RectTransform targetIndicator;
	protected TextMeshProUGUI targetIndicatorRoll;
	protected TextMeshProUGUI targetIndicatorDamage;

	// move action menu
	protected RectTransform moveActionMenu;
	protected Button moveActionUpButton;
	protected Button moveActionDownButton;
	protected Button moveActionBackButton;
	protected List<RectTransform> moveActionButtonContents;

	// attack action menu
	protected RectTransform attackActionMenu;
	protected Button attackActionSkipButton;
	protected Button attackActionUndoButton;
	protected List<RectTransform> attackActionButtonContents;

	// marker
	protected SpriteRenderer currentMarker;
	protected SpriteRenderer projectionMarker;
	protected SpriteRendererDatabase currentMarkerDatabase;
	protected SpriteRendererDatabase projectionMarkerDatabase;

	// ========================================================= Initiation and Destruction =========================================================

	/// <summary>
		/// Retrieve reference related to UI.
		/// </summary>
	protected void RetrieveUIReferences()
	{
		// containers
		uiCanvas = transform.Find("2DUIs").GetComponent<Canvas>();
		selfUI = uiCanvas.transform.Find("SelfUI").GetComponent<RectTransform>();
		targetUI = uiCanvas.transform.Find("TargetUI").GetComponent<RectTransform>();
		worldUIs = transform.Find("3DUIs");

		// normal indicator
		normalIndicator = selfUI.transform.Find("UnitIndicator").GetComponent<RectTransform>();
		normalIndicatorFrame = normalIndicator.transform.Find("Frame").GetComponent<Image>();
		normalIndicatorTeam = normalIndicatorFrame.transform.Find("Team").GetComponent<Image>();
		normalIndicatorClass = normalIndicatorFrame.transform.Find("Class").GetComponent<Image>();
		normalIndicatorHealth = normalIndicatorFrame.transform.Find("HealthBackground/Health").GetComponent<CutoffImage>();
		normalIndicatorAltitude = normalIndicator.transform.Find("Altitude").GetComponent<Image>();
		normalIndicatorTick = normalIndicator.transform.Find("Tick").GetComponent<Image>();
		normalIndicatorBuff = normalIndicator.transform.Find("Buff").GetComponent<Image>();
		normalIndicatorBuffMagnitude = normalIndicator.transform.Find("Buff/Magnitude").GetComponent<Image>();
		normalIndicatorClassDatabase = normalIndicatorClass.GetComponent<ImageDatabase>();
		normalIndicatorAltitudeDatabase = normalIndicatorAltitude.GetComponent<ImageDatabase>();
		normalIndicatorBuffDatabase = normalIndicatorBuff.GetComponent<ImageDatabase>();
		normalIndicatorBuffMagnitudeDatabase = normalIndicatorBuffMagnitude.GetComponent<ImageDatabase>();
		normalIndicatorName = normalIndicatorFrame.transform.Find("Name").GetComponent<TextMeshProUGUI>();

		// mini indicator
		miniIndicator = selfUI.transform.Find("MiniIndicator").GetComponent<RectTransform>();
		miniIndicatorFrame = miniIndicator.transform.Find("Frame").GetComponent<Image>();
		miniIndicatorTick = miniIndicator.transform.Find("Tick").GetComponent<Image>();
		miniIndicatorAltitude = miniIndicator.transform.Find("Altitude").GetComponent<Image>();
		miniIndicatorFrameDatabase = miniIndicatorFrame.GetComponent<ImageDatabase>();
		miniIndicatorAltitudeDatabase = miniIndicatorAltitude.GetComponent<ImageDatabase>();

		// move action menu
		moveActionMenu = selfUI.transform.Find("MoveActionMenu").GetComponent<RectTransform>();
		moveActionUpButton = moveActionMenu.transform.Find("UpButton").GetComponent<Button>();
		moveActionDownButton = moveActionMenu.transform.Find("DownButton").GetComponent<Button>();
		moveActionBackButton = moveActionMenu.transform.Find("BackButton").GetComponent<Button>();
		moveActionButtonContents = new List<RectTransform>(new RectTransform[] {
				moveActionUpButton.transform.Find("ButtonContent").GetComponent<RectTransform>(),
				moveActionDownButton.transform.Find("ButtonContent").GetComponent<RectTransform>(),
				moveActionBackButton.transform.Find("ButtonContent").GetComponent<RectTransform>()
		});

		// target indicator
		targetIndicator = targetUI.transform.Find("TargetIndicator").GetComponent<RectTransform>();
		targetIndicatorRoll = targetIndicator.transform.Find("Roll").GetComponent<TextMeshProUGUI>();
		targetIndicatorDamage = targetIndicator.transform.Find("Damage").GetComponent<TextMeshProUGUI>();

		// attack action menu
		attackActionMenu = selfUI.transform.Find("AttackActionMenu").GetComponent<RectTransform>();
		attackActionSkipButton = attackActionMenu.transform.Find("SkipButton").GetComponent<Button>();
		attackActionUndoButton = attackActionMenu.transform.Find("UndoButton").GetComponent<Button>();
		attackActionButtonContents = new List<RectTransform>(new RectTransform[] {
				attackActionSkipButton.transform.Find("ButtonContent").GetComponent<RectTransform>(),
				attackActionUndoButton.transform.Find("ButtonContent").GetComponent<RectTransform>(),
		});

		// marker
		currentMarker = worldUIs.transform.Find("CurrentMarker").GetComponent<SpriteRenderer>();
		projectionMarker = worldUIs.transform.Find("ProjectionMarker").GetComponent<SpriteRenderer>();
		currentMarkerDatabase = currentMarker.GetComponent<SpriteRendererDatabase>();
		projectionMarkerDatabase = projectionMarker.GetComponent<SpriteRendererDatabase>();

	}

	// ========================================================= Driver =========================================================	// 

	/// <summary>
	/// Per frame updates for UI.
	/// </summary>
	protected void UpdateUIs()
	{
		UpdateSelfUIPosition();
		UpdateSelfUIActionButtons();
		UpdateMarkerRotations();
	}

	// ========================================================= 2D UI Per frame Updates =========================================================

	/// <summary>
	/// Move the UI buttons to the correct position on screen.
	/// </summary>
	protected void UpdateSelfUIPosition()
	{
		selfUI.position = camera.Main.WorldToScreenPoint(model.position);
		if (selfUI.position.z > 0)
		{
			selfUI.gameObject.SetActive(true);
			normalIndicator.localPosition = new Vector3(0f, 4800f / Vector3.Distance(transform.position, Camera.main.transform.position), 0f);
			miniIndicator.localPosition = new Vector3(0f, 4800f / Vector3.Distance(transform.position, Camera.main.transform.position), 0f);
		}
		else
		{
			selfUI.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Modify all action buttons to the correct position and rotataion. 
	/// </summary>
	protected void UpdateSelfUIActionButtons()
	{
		if (moveActionMenu.gameObject.activeInHierarchy)
		{
			Vector2 directionInScreen = camera.Main.WorldToScreenPoint(model.position + transform.forward) - camera.Main.WorldToScreenPoint(model.position);
			float angleInScreen = Vector2.SignedAngle(Vector2.up, directionInScreen.normalized);
			moveActionMenu.transform.localEulerAngles = new Vector3(0, 0, angleInScreen - 180f);

			foreach (RectTransform moveActionContent in moveActionButtonContents)
			{
				moveActionContent.transform.eulerAngles = Vector3.zero;
			}

			moveActionUpButton.interactable = (selectedAltitude - preActionAltitude < 1) && (selectedAltitude < level.altitudeRange.max);
			moveActionDownButton.interactable = (selectedAltitude - preActionAltitude > -1) && (selectedAltitude > level.altitudeRange.min);
		}

		if (attackActionMenu.gameObject.activeInHierarchy)
		{
			Vector2 directionInScreen = camera.Main.WorldToScreenPoint(model.position + transform.forward) - camera.Main.WorldToScreenPoint(model.position);
			float angleInScreen = Vector2.SignedAngle(Vector2.up, directionInScreen.normalized);
			attackActionMenu.transform.localEulerAngles = new Vector3(0, 0, angleInScreen - 180f);

			foreach (RectTransform attackActionContent in attackActionButtonContents)
			{
				attackActionContent.transform.eulerAngles = Vector3.zero;
			}
		}
	}

	// ========================================================= 2D UI Controls =========================================================

	/// <summary>
	/// Show the move action menu.
	/// </summary>
	protected void ShowMoveActionMenu(bool show)
	{
		moveActionMenu.gameObject.SetActive(show);
	}

	/// <summary>
	/// Show the attack action menu.
	/// </summary>
	protected void ShowAttackActionMenu(bool show)
	{
		attackActionMenu.gameObject.SetActive(show);
	}

	/// <summary>
	/// Set the team display on all UI of this unit.
	/// </summary>
	protected void SetUITeamDisplay(TeamInfo teamInfo)
	{
		normalIndicatorTeam.sprite = teamInfo.insignia;
		miniIndicatorFrame.color = teamInfo.color;
	}

	/// <summary>
	/// Set the unit display on all UI of this unit.
	/// </summary>
	protected void SetUIUnitDisplay(UnitInfo unitInfo)
	{
		normalIndicatorName.text = unitInfo.name;
		normalIndicatorClassDatabase.SetSpriteByName(unitInfo.unitClass.ToString());
		miniIndicatorFrameDatabase.SetSpriteByName(unitInfo.unitClass.ToString());
	}

	/// <summary>
	/// Set the display as done on all UI of this unit.
	/// </summary>
	protected void SetUIAsDone(bool done)
	{
		if (done)
		{
			// normal indicators.
			normalIndicatorFrame.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			normalIndicatorTeam.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			normalIndicatorHealth.color = normalIndicatorHealth.color * new Color(1f, 1f, 1f, 0.5f);
			normalIndicatorAltitude.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			normalIndicatorBuff.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			normalIndicatorBuffMagnitude.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			normalIndicatorTick.enabled = true;

			// mini indicators
			miniIndicatorFrame.color = teamInfo.color * new Color(0.5f, 0.5f, 0.5f, 1f);
			miniIndicatorAltitude.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			miniIndicatorTick.enabled = true;
		}
		else
		{
			// normal indicators
			normalIndicatorFrame.color = Color.white;
			normalIndicatorTeam.color = Color.white;
			normalIndicatorHealth.color = normalIndicatorHealth.color;
			normalIndicatorAltitude.color = Color.white;
			normalIndicatorBuff.color = Color.white;
			normalIndicatorBuffMagnitude.color = Color.white;
			normalIndicatorTick.enabled = false;

			// mini indicators
			miniIndicatorFrame.color = teamInfo.color;
			miniIndicatorAltitude.color = Color.white;
			miniIndicatorTick.enabled = false;
		}
	}

	/// <summary>
	/// Set the unit type display on all UI of this unit.
	/// </summary>
	protected void SetUITypeDisplay(IndicatorType type)
	{
		switch (type)
		{
			case IndicatorType.None:
				normalIndicator.gameObject.SetActive(false);
				miniIndicator.gameObject.SetActive(false);
				break;
			case IndicatorType.Normal:
				normalIndicator.gameObject.SetActive(true);
				miniIndicator.gameObject.SetActive(false);
				break;
			case IndicatorType.Mini:
				normalIndicator.gameObject.SetActive(false);
				miniIndicator.gameObject.SetActive(true);
				break;
		}
	}

	/// <summary>
	/// Set the health display on all UI of this unit.
	/// </summary>
	protected void SetUIHealthDisplay(float health)
	{
		Color targetColor = Color.HSVToRGB(Mathf.Lerp(0f, 0.333f, health / maxHealth), 1f, 1f);
		normalIndicatorHealth.color = targetColor;
		normalIndicatorHealth.CutoffFrom(1f - health / maxHealth);
	}

	/// <summary>
	/// Set the altitude display on all UI of this unit.
	/// </summary>
	protected void SetUIAttitudeDisplay(int attitude)
	{
		normalIndicatorAltitudeDatabase.SetSpriteByName(((int)Mathf.Clamp(attitude, 0, 5)).ToString());
		miniIndicatorAltitudeDatabase.SetSpriteByName(((int)Mathf.Clamp(attitude, 0, 5)).ToString());
	}

	/// <summary>
	/// Set the buff display on all UI of this unit.
	/// </summary>
	protected void SetUIBuffDisplay(Buff buff, BuffMagnitude buffMagnitude)
	{
		normalIndicatorBuffDatabase.SetSpriteByName(buff.ToString());
		normalIndicatorBuffMagnitudeDatabase.SetSpriteByName(buffMagnitude.ToString());
	}

	// ========================================================= 3D UI Per frame Updates =========================================================

	/// <summary>
	/// Update the rotation of all markers.
	/// </summary>
	protected void UpdateMarkerRotations()
	{
		currentMarker.transform.localEulerAngles = new Vector3(90f, transform.localEulerAngles.y * -1f, 0);
		projectionMarker.transform.localEulerAngles = new Vector3(90f, transform.localEulerAngles.y * -1f, 0);
	}

	// ========================================================= 3D UI Controls =========================================================

	protected UnitMarkerType currentMarkerType;
	protected float currentMarkerHeight;
	protected Color currentMarkerColor;

	/// <summary>
	/// Set the type of unit marker used.
	/// </summary>
	protected void SetCurrentMarker(UnitMarkerType unitMarkerType)
	{
		SetCurrentMarker(unitMarkerType, 0, Color.clear);
	}

	/// <summary>
	/// Set the type of unit marker used.
	/// </summary>
	protected void SetCurrentMarker(UnitMarkerType unitMarkerType, float height, Color color)
	{
		currentMarkerType = unitMarkerType;
		currentMarkerHeight = height;
		currentMarkerColor = color;

		OverrideCurrentMarker(unitMarkerType, height, color);
	}

	/// <summary>
	/// Override the type of unit marker used that allow reverting to the original displays.
	/// </summary>
	protected void OverrideCurrentMarker(UnitMarkerType unitMarkerType, float height, Color color)
	{
		currentMarker.gameObject.SetActive(unitMarkerType != UnitMarkerType.None);
		currentHeightDisplay.Show(unitMarkerType != UnitMarkerType.None);
		if (unitMarkerType != UnitMarkerType.None)
		{
			currentMarkerDatabase.SetSpriteByName(unitMarkerType.ToString());
			currentHeightDisplay.Modify(height - 3, level.altitudeHeight);
			currentHeightDisplay.SetColor(color);
			currentMarker.color = color;
		}
	}

	/// <summary>
	/// Return the unit marker to pior overriding. 
	/// </summary>
	protected void RevertCurrentMarker()
	{
		OverrideCurrentMarker(currentMarkerType, currentMarkerHeight, currentMarkerColor);
	}

	/// <summary>
	/// Set the type of projection mark used.
	/// </summary>
	protected void SetProjectionMarker(UnitMarkerType unitMarkerType)
	{
		SetProjectionMarker(unitMarkerType, 0, Color.clear, Vector3.zero);
	}

	/// <summary>
	/// Set the type of projection mark used.
	/// </summary>
	protected void SetProjectionMarker(UnitMarkerType unitMarkerType, float height, Color color, Vector3 localPosition)
	{
		projectionMarker.gameObject.SetActive(unitMarkerType != UnitMarkerType.None);
		nextHeightDisplay.Show(unitMarkerType != UnitMarkerType.None);
		if (unitMarkerType != UnitMarkerType.None)
		{
			projectionMarkerDatabase.SetSpriteByName(unitMarkerType.ToString());
			projectionMarker.transform.localPosition = localPosition;
			nextHeightDisplay.transform.localPosition = localPosition;
			nextHeightDisplay.Modify(height - 3, level.altitudeHeight);
			nextHeightDisplay.SetColor(color);
			projectionMarker.color = color;
		}
	}

	/// <summary>
	/// Set the radius size of all markers.
	/// </summary>
	protected void SetMarkerRadius(float radius)
	{
		currentMarker.transform.localScale = Vector3.one * radius;
		projectionMarker.transform.localScale = Vector3.one * radius;
	}
}
