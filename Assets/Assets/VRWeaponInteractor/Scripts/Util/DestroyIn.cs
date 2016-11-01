using UnityEngine;
using System.Collections;

public class DestroyIn : MonoBehaviour {

	public float seconds;
	private float elapsedTime = 0;

	void Update () 
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > seconds)
			Destroy(gameObject);
	}
}
