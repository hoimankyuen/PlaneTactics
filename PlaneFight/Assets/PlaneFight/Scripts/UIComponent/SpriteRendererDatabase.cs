using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererDatabase : MonoBehaviour
{
	[System.Serializable]
	public class SpriteSet
	{
		public string name;
		public Sprite sprite;
	}
	public SpriteRenderer SpriteRenderer { get; protected set; }
	public List<SpriteSet> sprites = new List<SpriteSet>();
	protected Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

	// ========================================================= Monobehaviour Methods =========================================================

	/// <summary>
	/// Awake is called when the game object was created. It is always called before start and is 
	/// independent of if the game object is active or not.
	/// </summary>
	void Awake()
    {
		foreach (SpriteSet spriteSet in sprites)
		{
			spriteDictionary[spriteSet.name] = spriteSet.sprite;
		}
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	// ========================================================= Functionalities =========================================================

	/// <summary>
	/// Set the sprite of the accociated renderer by sprite name in the database.
	/// </summary>
	public void SetSpriteByName(string name)
	{
		if (spriteDictionary.ContainsKey(name))
		{
			SpriteRenderer.sprite = spriteDictionary[name];
		}
		else
		{
			SpriteRenderer.sprite = null;
		}
	}
}
