using UnityEngine;
using System.Collections;

public class DisplayAmmo : MonoBehaviour {

	public VRMagazine magazine;
	private TextMesh text;

	private float elapsedTime = 0;
	private float checkTime = 0;

	void Start () 
	{
		if (magazine == null)
		{
			Debug.LogError("No magazine referenced");
			enabled = false;
			return;
		}
		text = GetComponent<TextMesh>();
		if (text == null)
		{
			Debug.LogError("Can't find text to render to");
			enabled = false;
			return;
		}
	}

	void Update () 
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > checkTime)
		{
			checkTime = elapsedTime + 1;
			if (magazine == null || text == null) return;
			text.text = magazine.bulletCount + " Bullets";
		}
	}
}
