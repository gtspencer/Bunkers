using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class WallHUDBehaviour : MonoBehaviour {
	private TextMesh textMesh;
	public GameObject wall;
    private WallBehaviour wallBehaviour;

	//Use this for initialization
	void Start () {
		textMesh = GetComponent<TextMesh>();
		wallBehaviour = wall.GetComponent<WallBehaviour>();
	}

	void FixedUpdate() {
		//textMesh.transform.LookAt(Camera.main.transform.position);
        int hp = wallBehaviour.GetHitPoints();
        if (hp <= 0)
        {
            textMesh.text = "HP: 0";
        } else
        {
            textMesh.text = "HP: " + hp;
        }
	}
}