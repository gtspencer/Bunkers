using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class WallBehaviour : MonoBehaviour {
	public int hitPoints = 100;
    public bool wood;
    public bool metal;
    public bool cement;

	//Use this for initialization
	void Start () {
        
		if (wood)
        {
            hitPoints = 100;
            //GetComponent<Renderer>().material.color = new Color(97, 61, 7, 255);
        } else if (metal)
        {
            hitPoints = 500;
            //GetComponent<Renderer>().material.color = new Color(43, 47, 42, 255);
        }
        else if (cement)
        {
            hitPoints = 1000;
            //GetComponent<Renderer>().material.color = new Color(163, 163, 163, 255);
        }
        
    }

    void FixedUpdate()
    {
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

	public void ProcessDamage(int attackPoints) {
		hitPoints -= attackPoints;
	}

	public int GetHitPoints() {
		return hitPoints;
	}
}