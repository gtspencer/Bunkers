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
        
        /**
		if (wood)
        {
            hitPoints = 100;
            GetComponent<Renderer>().material = wood;
        } else if (metal)
        {
            hitPoints = 500;
            GetComponent<Renderer>().material = metal;
        }
        else if (cement)
        {
            hitPoints = 1000;
            GetComponent<Renderer>().material = cement;
        }
    */
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

    //button used to repair
    public void Activate()
    {
        if (wood)
        {
            hitPoints = 100;
        }
        else if (metal)
        {
            hitPoints = 500;
        }
        else if (cement)
        {
            hitPoints = 1000;
        }
    }
}