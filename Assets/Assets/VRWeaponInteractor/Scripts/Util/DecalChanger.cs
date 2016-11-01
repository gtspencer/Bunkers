using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecalChanger : MonoBehaviour 
{
	public GameObject defaultDecal;
	public List<GameObject> decalPrefabs = new List<GameObject>();
	public List<string> tags = new List<string>();

	public void SetMaterialTo(string targetTag)
	{
		if (defaultDecal == null)
		{
			Debug.LogError("No default decal specified");
			return;
		}
		if (decalPrefabs.Count != tags.Count)
		{
			Debug.LogError("Materials and tags don't line up. Make sure there are as many tags as materials");
			return;
		}
		for(int i=0; i<tags.Count; i++)
		{
			if (tags[i] == targetTag)
			{
				if (decalPrefabs[i] != null)
				{
					GameObject newDecal = (GameObject)Instantiate(decalPrefabs[i]);
					newDecal.transform.parent = transform;
					newDecal.transform.position = transform.position;
					newDecal.transform.rotation = transform.rotation;
				}
				break;
			}
		}
		GameObject newDefaultDecal = (GameObject)Instantiate(defaultDecal);
		newDefaultDecal.transform.parent = transform;
		newDefaultDecal.transform.position = transform.position;
		newDefaultDecal.transform.rotation = transform.rotation;
	}
}
