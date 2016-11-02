using UnityEngine;
using System.Collections;

public class AmmoAssign : MonoBehaviour {
    public GameObject ammoVendingMachine;

    public Transform spawnPoint;

    public enum ammoType
    {
        revolver,
        shotgun,
        smg,
        assault,
        sniper,
        sniperCal,
        grenadeLauncher,
        minigun,
        none
    }
    private int num;
    public ammoType typeOfAmmo = ammoType.none;

    // Use this for initialization
    void Start() {
        switch (typeOfAmmo)
        {
            case ammoType.revolver:
                num = 1;
                break;
            case ammoType.shotgun:
                num = 2;
                break;
            case ammoType.smg:
                num = 3;
                break;
            case ammoType.assault:
                num = 4;
                break;
            case ammoType.sniper:
                num = 5;
                break;
            case ammoType.sniperCal:
                num = 6;
                break;
            case ammoType.grenadeLauncher:
                num = 7;
                break;
            case ammoType.minigun:
                num = 8;
                break;
            default:
                num = 0;
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate()
    {

    }
}
