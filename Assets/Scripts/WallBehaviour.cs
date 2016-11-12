using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class WallBehaviour : MonoBehaviour {
	public int hitPoints = 100;
    public bool wood;
    public bool metal;
    public bool cement;
    public Material woodMat;
    public Material metalMat;
    public Material cementMat;

	//Use this for initialization
	void Start () {
        
		if (wood)
        {
            hitPoints = 100;
            //GetComponent<Renderer>().material.color = wood;
        } else if (metal)
        {
            hitPoints = 500;
            //GetComponent<Renderer>().material.color = metal;
        }
        else if (cement)
        {
            hitPoints = 1000;
            //GetComponent<Renderer>().material.color = cement;
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