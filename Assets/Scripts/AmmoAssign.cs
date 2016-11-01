using UnityEngine;
using System.Collections;

public class AmmoAssign : MonoBehaviour {

    private VRButtonExample button;
    public Transform spawnPoint;
    public GameObject magazinePrefab;

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
    private string num;
    public ammoType typeOfAmmo = ammoType.none;

    // Use this for initialization
    void Start() {
        button = GetComponent<VRButtonExample>();
        switch (typeOfAmmo)
        {
            case ammoType.revolver:
                num = "1";
                break;
            case ammoType.shotgun:
                num = "2";
                break;
            case ammoType.smg:
                num = "2";
                break;
            case ammoType.assault:
                num = "2";
                break;
            case ammoType.sniper:
                num = "2";
                break;
            case ammoType.sniperCal:
                num = "2";
                break;
            case ammoType.grenadeLauncher:
                num = "2";
                break;
            case ammoType.minigun:
                num = "2";
                break;
            default:
                num = "0";
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
