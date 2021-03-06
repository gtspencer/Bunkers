﻿using UnityEngine;
using System.Collections;

public class AmmoAssign : MonoBehaviour {
    public GameObject ammoVendingMachine;

    public Transform spawnPoint;
    public GameObject ammo;

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

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Activate()
    {
        Instantiate(ammo, spawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
    }
}
