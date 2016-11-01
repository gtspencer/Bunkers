using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour	{

	//For the handgun animation
	bool hasPlayed = false;
	bool outOfAmmoSlider = false;

	[Header("Bullets Left")]
	//How many bullets there are left
	public int bulletsLeft;

	//Check when reloading and out of ammo
	bool outOfAmmo = false;
	bool isReloading = false;

	//Used for firerate
	float lastFired;

	//Used for dual handguns 
	bool shootLeft = true;
	bool shootRight;

	//Minigun rotaiton speed, start at 0
	float minigunRotationSpeed = 0.0f;
	//Check when the minigun should start shooting
	//Start as false
	bool allowMinigunShooting = false;

	[Header("Spawnpoints & Prefabs")]
	public Transform
		casingSpawnPoint;
	public Transform casingPrefab;
	public Transform magSpawnPoint;
	public Transform emptyMagPrefab;
	//The raycast will start at the bullet spawnpoint, going forward
	public Transform bulletSpawnPoint;

	[Header("Metal")]
	[Header("Bullet Impacts & Tags")]
	public Transform metalImpactStaticPrefab;
	public Transform metalImpactPrefab;
	[Header("Wood")]
	public Transform woodImpactStaticPrefab;
	public Transform woodImpactPrefab;
	[Header("Concrete")]
	public Transform concreteImpactStaticPrefab;
	public Transform concreteImpactPrefab;
	[Header("Dirt")]
	public Transform dirtImpactStaticPrefab;
	public Transform dirtImpactPrefab;

	[Header("Impact Tags")]
	//Default tags for bullet impacts
	public string metalImpactStaticTag = "Metal (Static)";
	public string metalImpactTag = "Metal";
	public string woodImpactStaticTag = "Wood (Static)";
	public string woodImpactTag = "Wood";
	public string concreteImpactStaticTag = "Concrete (Static)";
	public string concreteImpactTag = "Concrete";
	public string dirtImpactStaticTag = "Dirt (Static)";
	public string dirtImpactTag = "Dirt";

	[Header("Customizable Options")]
	public int
		magazineSize;
	public float muzzleFlashDuration = 0.02f;
	public float fireRate;
	public float reloadDuration = 1.5f;
	[Range(1f, 4f)]
	public float
		lightIntensity = 2.0f;
	[Range(5f, 50f)]
	public float
		lightRange = 10.0f;

	public float bulletDistance = 150f;
	public float bulletForce = 15f;

	[Header("Hand Grenade Options")]
	//How much force will be applied to the grenade
	public float grenadeThrowForce = 2500.0f;

	[Header("Hand Grenade Rotation Force")]
	public float minimumGrenadeRotation;
	public float maximumGrenadeRotation;

	//All weapon types
	[System.Serializable]
	public class weaponType
	{  
		[Header("Handgun")]
		public bool handgun;
		public bool handgunSilencer;
		public bool dualHandguns;
		[Header("Sniper")]
		public bool sniper;
		public bool sniperSilencer;
		public bool sniper3;
		public bool sniper6;
		[Header("SMG")]
		public bool smg;
		public bool smgSilencer;
		[Header("Assault Rifle")]
		public bool assaultRifle;
		public bool assaultRifle2;
		public bool assaultRifleSilencer;
		public bool assaultRifleSilencer2;
		[Header("Machine Gun")]
		public bool machineGun;
		[Header("Shotgun")]
		public bool shotgun;
		[Header("RPG")]
		public bool rpg;
		[Header("Sawn Off Shotgun")]
		public bool sawnOffShotgun;
		[Header("Revolver")]
		public bool revolver1;
		public bool revolver2;
		[Header("Grenade Launcher")]
		public bool grenadeLauncher;
		[Header("Hand Grenade")]
		public bool handGrenade;
		[Header("Minigun")]
		public bool minigun;
	}
	public weaponType WeaponType;

	//All animations
	[System.Serializable]
	public class animations
	{  
		//Animations
		public string fullMagInAnim;
		public string recoilAnim;
		public string reloadAnim;
		//Animation for "slide" and "bolt action"
		public string slideReloadAnim;
		//Used when shooting
		public string slideEjectAnim;
		public string reloadDownAnim;
		public string reloadUpAnim;
		//Animation for reloading slider, also for machine gun top, and sawn off shotgun barrel
		public string reloadSlideCloseAnim;
		public string reloadSlideOpenAnim;
		[Header("Handgun Animations")]
		//Handgun animations
		public string
			blowbackAnim;
		public string fullAmmoAnim;
		public string outOfAmmoAnim;
		[Header("Sniper & Shotgun Bullet Animation")]
		//Used for sniper and shotgun when reloading
		public string
			bulletInAnim;
		[Header("Shotgun Animations")]
		//Used only for shotgun
		public string
			shotgunSlideBackAnim;
		public string shotgunSlideForwardAnim;
		[Header("Sawn Off Shotgun Animations")]
		//Used only for sawn off shotgun
		public string
			sawnOffShotgunAmmoOneInAnim;
		public string sawnOffShotgunAmmoTwoInAnim;
		[Header("Machine Gun Animations")]
		//Used only for machine gun
		public string
			machineGunBulletHolderAnim;
		public string machineGunSphereRotateAnim;
	}
	public animations Animations;

	//Components
	[System.Serializable]
	public class components
	{  
		[Header("Muzzleflash Holders")]
		public GameObject
			sideMuzzle;
		public GameObject topMuzzle;
		public GameObject frontMuzzle;
		//Array of muzzleflash sprites
		public Sprite[] muzzleflashSideSprites;
		[Header("Holders")]
		public GameObject
			holder;
		public GameObject slider;
		public GameObject ejectSlider;
		public GameObject mag;
		public GameObject fullMag;
		
		//For sniper 
		[Header("Sniper")]
		public GameObject
			bullet;
		[Header("Sawn Off Shotgun")]
		//For sawn off shotgun
		public GameObject
			barrels;
		public GameObject shotgunAmmo1;
		public GameObject shotgunAmmo2;
		public GameObject ammo1;
		public GameObject ammo2;
		public Transform casingSpawnPoint1;
		public Transform casingSpawnPoint2;
		[Header("Machine Gun")]
		public GameObject
			machineGunTop;
		public GameObject bulletHolderAnim;
		[Header("Bullet Strip Components")]
		public GameObject[]
			bullets;
		public Transform[] bulletSpawnpoints;
		public Transform bulletPrefab;
		public GameObject sphereRotate;
		[Header("Light Front")]
		public Light
			lightFlash;
		[Header("Particle System")]
		public ParticleSystem
			smokeParticles;
		public ParticleSystem
			sparkParticles;
		[Header("RPG & Grenade Launcher")]
		public GameObject
			projectile;
		public Transform projectilePrefab;
		public Transform projectileSpawnPoint;
		[Header("Particle Systems")]
		public ParticleSystem
			smokeParticlesBack;
		[Header("Light Back")]
		public Light
			rpgLightBack;
		
		//For dual handguns, right one
		[Header("Dual Handguns Right")]
		public GameObject
			holderRight;
		public GameObject sliderRight;
		public GameObject magRight;
		public GameObject fullMagRight;
		public Transform bulletSpawnPointRight;
		[Header("Light Right")]
		public Light
			lightFlashRight;
		[Header("Particle System")]
		public ParticleSystem
			smokeParticlesRight;
		[Header("Spawnpoints Right")]
		public Transform
			casingSpawnPointRight;
		public Transform magSpawnPointRight;
		[Header("Muzzleflash Holders Right")]
		public GameObject
			sideMuzzleRight;
		public GameObject topMuzzleRight;
		public GameObject frontMuzzleRight;

		[Header("Revolver Casing Spawnpoints")]
		public GameObject[]
		revolverBullets;
		public Transform[]
		revolverCasingSpawnpoints;

		[Header("Hand Grenade Prefab")]
		public GameObject handGrenadePrefab;

		[Header("Minigun Particles")]
		public ParticleSystem bulletTracerParticles;
	}

	public components Components;

	//All audio sources
	[System.Serializable]
	public class audioSources
	{  
		[Header("Shoot Sounds")]
		public AudioSource shootSound;

		[Header("Reload Sounds")]
		public AudioSource mainReloadSound;
		public AudioSource sliderReloadSound;
		public AudioSource sliderSound;

		public AudioSource removeMagSound;
		public AudioSource insertMagSound;

		public AudioSource outOfAmmoClickSound;

		[Header("Sawn Off Shotgun Sounds")]
		public AudioSource reloadOpenSound;
		public AudioSource reloadCloseSound;

		[Header("Shotgun Sounds")]
		public AudioSource shotgunPumpSound;
		public AudioSource shellInsertSound;

		[Header("Minigun Sounds")]
		public AudioSource minigunSpinLoopSound;

		public AudioClip minigunSpinUpSound;
		public AudioClip minigunSpinDownSound;

		public AudioSource minigunSpinUpDownSource;
	}

	bool spinUpSoundHasPlayed = false;
	bool spinDownSoundHasPlayed = false;
	bool spinLoopHasPlayed = false;

	public audioSources AudioSources;

	[Header("Used In Demo Scenes")]
	//********** USED IN THE DEMO SCENES **********
	//Used to prevent gun switching in the demo scenes, 
	//while reloading and shooting
	public bool noSwitch = false;

	void Start ()
	{
		//Set the magazine size
		bulletsLeft = magazineSize;

		//Make sure the light is off
		//Disable for silenced weapons and hand grenade since they dont need it
		if (!WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2 && !WeaponType.handGrenade) {

			Components.lightFlash.GetComponent<Light> ().enabled = false;

			//Set the light values
			Components.lightFlash.intensity = lightIntensity;
			Components.lightFlash.range = lightRange;
		}
		
		//If dual handguns is true
		if (WeaponType.dualHandguns == true) {

			//Make sure the light is off for the right handgun
			Components.lightFlashRight.GetComponent<Light> ().enabled = false;
			
			//Set the light values for the right handgun
			Components.lightFlashRight.intensity = lightIntensity;
			Components.lightFlashRight.range = lightRange;
			
			//Make sure muzzleflashes are hidden on the right handgun
			Components.sideMuzzleRight.GetComponent<Renderer> ().enabled = false;
			Components.topMuzzleRight.GetComponent<Renderer> ().enabled = false;
			Components.frontMuzzleRight.GetComponent<SpriteRenderer> ().enabled = false;
		}

		//If rpg is true
		if (WeaponType.rpg == true) {

			//Set the light values for the back light on the rpg
			Components.rpgLightBack.intensity = lightIntensity;
			Components.rpgLightBack.range = lightRange;

			//Make sure the light is off
			Components.rpgLightBack.GetComponent<Light> ().enabled = false;
		}
			
		//Hide the muzzleflashes at start
		//Disable for rpg, grenade launcher, silenced weapons and hand grenadesince they dont use any muzzleflashes
		if (!WeaponType.rpg && !WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2 && !WeaponType.grenadeLauncher && !WeaponType.handGrenade) {

			Components.sideMuzzle.GetComponent<Renderer> ().enabled = false;
			Components.topMuzzle.GetComponent<Renderer> ().enabled = false;
		}

		//Hide the front muzzleflash at start
		//Disable for shotgun, sawn off shotgun, rpg, grenade launcher, and silenced weapons since they dont have a front muzzle
		if (WeaponType.handgun == true || WeaponType.dualHandguns == true || WeaponType.smg == true || WeaponType.assaultRifle == true || 
		    WeaponType.machineGun == true || WeaponType.sniper == true || WeaponType.revolver1 == true || WeaponType.revolver2 == true || 
		    WeaponType.assaultRifle2 == true || WeaponType.sniper3 == true || WeaponType.sniper6 == true) {

			//Hide the front muzzleflash
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}

		//For MACHINE GUN only
		if (WeaponType.machineGun == true) {
			//Hide the bullet holders, and full mag at start
			Components.bulletHolderAnim.SetActive (false);
			Components.fullMag.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	//Reload
	IEnumerator Reload ()
	{

		//********** USED IN THE DEMO SCENES **********
		//Prevent gun switching while reloading
		noSwitch = true;
		
		//Start reloading
		isReloading = true;

		//Spawn empty magazine
		//Disable for sniper, shotgun, sawn off shotgun and rpg since they dont have a mag
		if (WeaponType.handgun == true || WeaponType.dualHandguns == true || WeaponType.smg == true || 
		    WeaponType.assaultRifle == true || WeaponType.machineGun == true || WeaponType.handgunSilencer == true || 
		    WeaponType.smgSilencer == true || WeaponType.assaultRifleSilencer == true || WeaponType.assaultRifle2 == true || WeaponType.assaultRifleSilencer2 == true ||
		    WeaponType.sniper3 == true || WeaponType.sniper6 == true) {

			//Spawn empty magazine
			if (!WeaponType.sniper6) {
			Instantiate (emptyMagPrefab, magSpawnPoint.transform.position, 
			            magSpawnPoint.transform.rotation);
			}
			//Hide the magazine
			Components.mag.GetComponent<MeshRenderer> ().enabled = false;

			//If dual handguns is true
			if (WeaponType.dualHandguns == true) {
				//Hide the magazine on the right handgun
				Components.magRight.GetComponent<MeshRenderer> ().enabled = false;

				//Spawn empty magazine on the right handgun
				Instantiate (emptyMagPrefab, Components.magSpawnPointRight.transform.position, 
				            Components.magSpawnPointRight.transform.rotation);
				
				//Play reload and mag in animation on the left handgun
				Components.holder.GetComponent<Animation> ().Play
					(Animations.reloadAnim);
				Components.fullMag.GetComponent<Animation> ().Play
					(Animations.fullMagInAnim);
				//Play reload and mag in animation on the right handgun
				Components.holderRight.GetComponent<Animation> ().Play
					(Animations.reloadAnim);
				Components.fullMagRight.GetComponent<Animation> ().Play
					(Animations.fullMagInAnim);

				//Play reload sounds
				AudioSources.mainReloadSound.Play();
				AudioSources.removeMagSound.Play();
			}
		}

		//If handgun, smg, assault rifle or sniper3 is true
		if (WeaponType.handgun == true || WeaponType.smg == true || WeaponType.assaultRifle == true || WeaponType.handgunSilencer == true || 
		    WeaponType.smgSilencer == true || WeaponType.assaultRifleSilencer == true || WeaponType.assaultRifle2 == true || WeaponType.assaultRifleSilencer2 == true ||
		    WeaponType.sniper3 == true) {

			//Play reload and mag in animation
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadAnim);
			Components.fullMag.GetComponent<Animation> ().Play 
				(Animations.fullMagInAnim);

			//Play main reload sound
			AudioSources.mainReloadSound.Play();
			//Play remove mag sound
			AudioSources.removeMagSound.Play();
		}
		
		//If sniper is true
		if (WeaponType.sniper == true || WeaponType.sniperSilencer == true) {
			//Play reload and slider eject animation
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadDownAnim);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.reloadSlideOpenAnim);

			//Play main reload sound
			AudioSources.mainReloadSound.Play();

			//Play bullet reload animation 5 times
			for (int count = 1; count <= 5; count++) {
				Components.bullet.GetComponent<Animation> ().PlayQueued
					(Animations.bulletInAnim);

				StartCoroutine(ShellInsertSound (5));
			}
		}

		//If machine gun is true
		if (WeaponType.machineGun == true) {
			//Hide all bullets when reloading
			foreach (GameObject bulletObjects in Components.bullets) {
				bulletObjects.gameObject.GetComponent<MeshRenderer> ().enabled = false;
			}

			//Hide the magazine
			Components.mag.SetActive (false);
			
			//Play reload animation
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadAnim);

			//Play main reload sound
			AudioSources.mainReloadSound.Play();
			//Play remove mag sound
			AudioSources.removeMagSound.Play();

			//Wait for animation to finish
			yield return new WaitForSeconds (0.25f);
			//Open the top and play the mag animation
			Components.machineGunTop.GetComponent<Animation> ().Play 
				(Animations.reloadSlideOpenAnim);
			Components.fullMag.GetComponent<Animation> ().Play
				(Animations.fullMagInAnim);

			//Play the insert mag sound
			AudioSources.insertMagSound.Play();
		}

		//If shotgun is true
		if (WeaponType.shotgun == true) {
			//Play reload animations
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadUpAnim);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.shotgunSlideBackAnim);
			
			//Play shell reload animation 8 times
			for (int count = 1; count <= 8; count++) {
				Components.bullet.GetComponent<Animation> ().PlayQueued
					(Animations.bulletInAnim);

				StartCoroutine(ShellInsertSound (8));
			}
		}

		//If rpg is true 
		if (WeaponType.rpg == true) {
			//Hide the projectile and play the reload animation
			Components.projectile.GetComponent<MeshRenderer> ().enabled = false;
			Components.projectile.GetComponent<Animation> ().Play 
				(Animations.fullMagInAnim);

			//Play the main reload sound
			AudioSources.mainReloadSound.Play ();
			//Play the insert mag sound
			AudioSources.insertMagSound.Play ();
		}

		//If sawn off shotgun is true
		if (WeaponType.sawnOffShotgun == true) {
			//Hide the shellcasings that are in the gun
			Components.shotgunAmmo1.GetComponent<MeshRenderer> ().enabled = false;
			Components.shotgunAmmo2.GetComponent<MeshRenderer> ().enabled = false;

			//Play reload and shellcasings in animation
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadDownAnim);
			Components.barrels.GetComponent<Animation> ().Play 
				(Animations.reloadSlideOpenAnim);

			//Play barrels open sound
			AudioSources.reloadOpenSound.Play ();
			
			//Wait some time
			yield return new WaitForSeconds (0.15f);
			
			//Spawn casing prefabs
			Instantiate (casingPrefab, Components.casingSpawnPoint1.transform.position, 
			            Components.casingSpawnPoint1.transform.rotation);
			Instantiate (casingPrefab, Components.casingSpawnPoint2.transform.position, 
			            Components.casingSpawnPoint2.transform.rotation);

			//Animate shells in
			Components.ammo1.GetComponent<Animation> ().Play 
				(Animations.sawnOffShotgunAmmoOneInAnim);
			Components.ammo2.GetComponent<Animation> ().Play 
				(Animations.sawnOffShotgunAmmoTwoInAnim);
		}

		//If revolver 1 or revolver 2 is true 
		if (WeaponType.revolver1 == true || WeaponType.revolver2 == true) {

			//Play the reload animation
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadDownAnim);
			//Wait some time
			yield return new WaitForSeconds(0.25f);

			//Play the reload animation
			Components.mag.GetComponent<Animation>().Play
				(Animations.reloadSlideOpenAnim);

			//Play reload sound
			AudioSources.sliderReloadSound.Play ();

			//Wait some time
			yield return new WaitForSeconds(0.55f);

			//Hide the revolver bullets
			hideBulletsRevolver (0);
			hideBulletsRevolver (1);
			hideBulletsRevolver (2);
			hideBulletsRevolver (3);
			hideBulletsRevolver (4);
			hideBulletsRevolver (5);

			//Spawn the revolver casings
			spawnBulletsRevolver (0);
			spawnBulletsRevolver (1);
			spawnBulletsRevolver (2);
			spawnBulletsRevolver (3);
			spawnBulletsRevolver (4);
			spawnBulletsRevolver (5);

			//Wait some time
			yield return new WaitForSeconds(0.7f);

			//Play full mag in animation
			Components.fullMag.GetComponent<Animation>().Play
				(Animations.fullMagInAnim);
		}

		//If grenade launcher is true 
		if (WeaponType.grenadeLauncher == true) {
			
			//Play the reload animation
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadDownAnim);

			//Play main reload sound
			AudioSources.mainReloadSound.Play();
			//Play reload open sound
			AudioSources.reloadOpenSound.Play();

			//Wait some time
			yield return new WaitForSeconds(0.25f);
			
			//Play the reload animation
			Components.mag.GetComponent<Animation>().Play
				(Animations.reloadSlideOpenAnim);
			//Wait some time
			yield return new WaitForSeconds(0.15f);
			
			//Hide the grenade launcher shells
			hideBulletsRevolver (0);
			hideBulletsRevolver (1);
			hideBulletsRevolver (2);
			hideBulletsRevolver (3);
			hideBulletsRevolver (4);
			hideBulletsRevolver (5);
			
			//Spawn the grenade launcher casings
			spawnBulletsRevolver (0);
			spawnBulletsRevolver (1);
			spawnBulletsRevolver (2);
			spawnBulletsRevolver (3);
			spawnBulletsRevolver (4);
			spawnBulletsRevolver (5);
			
			//Wait some time
			yield return new WaitForSeconds(0.7f);
			
			//Play full mag in animation
			Components.fullMag.GetComponent<Animation>().Play
				(Animations.fullMagInAnim);
		}

		//If sniper 6 is true
		if (WeaponType.sniper6 == true) {
			//Play the slider open animation
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.reloadSlideOpenAnim);
			//Play reload animation
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadAnim);

			//Play main reload sound
			AudioSources.mainReloadSound.Play ();
			//Play slider sound
			AudioSources.sliderSound.Play ();
			//Play remove mag sound
			AudioSources.removeMagSound.Play();

			//Wait some time
			yield return new WaitForSeconds(0.12f);
			//Spawn the empty mag prefab
			Instantiate (emptyMagPrefab, magSpawnPoint.transform.position, 
			             magSpawnPoint.transform.rotation);

			//Play the full mag in animation
			Components.fullMag.GetComponent<Animation> ().Play
				(Animations.fullMagInAnim);
		}

		//Wait for set amount of time
		yield return new WaitForSeconds (reloadDuration);
		
		//Refill bullets
		bulletsLeft = magazineSize;

		//Disable for sniper, shotgun, sawn off shotgun and rpg since they dont have a mag
		if (WeaponType.handgun == true || WeaponType.dualHandguns == true || WeaponType.smg == true || WeaponType.assaultRifle == true || 
		    WeaponType.machineGun == true || WeaponType.handgunSilencer == true || WeaponType.smgSilencer == true || WeaponType.assaultRifleSilencer == true || 
		    WeaponType.assaultRifle2 == true || WeaponType.assaultRifleSilencer2 == true || WeaponType.sniper3 == true || WeaponType.sniper6 == true ) {

			//Make the magazine visible again
			Components.mag.GetComponent<MeshRenderer> ().enabled = true;

			//If dual handguns is true
			if (WeaponType.dualHandguns == true) {

				//Make the magazine visible again
				Components.magRight.GetComponent<MeshRenderer> ().enabled = true;

				hasPlayed = false;

				//Enable shooting again
				outOfAmmo = false;
				isReloading = false;

				//********** USED IN THE DEMO SCENES **********
				//Enable gun switching again
				noSwitch = false;

				//Only play this if gun ran out of bullets before reloading
				if (outOfAmmoSlider == true) {
					Components.slider.GetComponent<Animation> ().Play (
						Animations.fullAmmoAnim);
					Components.sliderRight.GetComponent<Animation> ().Play
						(Animations.fullAmmoAnim);
					outOfAmmoSlider = false;

					//Play slider reload sound
					AudioSources.sliderSound.Play ();
				} else {
					Components.slider.GetComponent<Animation> ().Play
						(Animations.slideReloadAnim);
					Components.sliderRight.GetComponent<Animation> ().Play
						(Animations.slideReloadAnim);

					//Play slider reload sound
					AudioSources.sliderReloadSound.Play ();
				}
			}
		}

		//If handgun is true
		if (WeaponType.handgun == true || WeaponType.handgunSilencer == true) {
			hasPlayed = false;

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;

			//Only play this if gun ran out of bullets before reloading
			if (outOfAmmoSlider == true) {
				Components.slider.GetComponent<Animation> ().Play 
					(Animations.fullAmmoAnim);
				outOfAmmoSlider = false;

				//Play slider reload sound
				AudioSources.sliderSound.Play ();
			} else {
				Components.slider.GetComponent<Animation> ().Play 
					(Animations.slideReloadAnim);

				//Play slider reload sound
				AudioSources.sliderReloadSound.Play ();
			}
		}

		//If sniper is true 
		if (WeaponType.sniper == true || WeaponType.sniperSilencer == true) {
			//Play animations and wait
			yield return new WaitForSeconds (0.2f);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.reloadSlideCloseAnim);

			//Play slider sound
			AudioSources.sliderSound.Play();

			yield return new WaitForSeconds (0.2f);
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadUpAnim);
			//Wait some more before being able to shoot
			yield return new WaitForSeconds (0.1f);
			
			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If smg is true
		if (WeaponType.smg == true || WeaponType.smgSilencer == true) {

			//Play insert mag sound
			AudioSources.insertMagSound.Play ();

			//Wait for the slider animation to finish
			yield return new WaitForSeconds (0.25f);
			
			Components.slider.GetComponent<Animation> ().Play
				(Animations.slideReloadAnim);

			//Play slider reload sound
			AudioSources.sliderReloadSound.Play ();
			
			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If assault rifle is true
		if (WeaponType.assaultRifle == true || WeaponType.assaultRifleSilencer == true || 
		    WeaponType.assaultRifle2 == true || WeaponType.assaultRifleSilencer2 == true) {

			//Play insert mag sound
			AudioSources.insertMagSound.Play ();

			//Wait for the slider animation to finish
			yield return new WaitForSeconds (0.25f);

			if (!WeaponType.assaultRifle2 && !WeaponType.assaultRifleSilencer2) {

			Components.ejectSlider.GetComponent<Animation> ().Play
				(Animations.slideEjectAnim);

				//Play slider reload sound
				AudioSources.sliderReloadSound.Play ();

			} else {
				Components.slider.GetComponent<Animation>().Play
					(Animations.slideReloadAnim);

				//Play slider reload sound
				AudioSources.sliderReloadSound.Play ();
			}

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If machine gun is true
		if (WeaponType.machineGun == true) {

			//Make the magazine visible again and play animation
			Components.mag.SetActive (true);
			Components.bulletHolderAnim.SetActive (true);
			Components.bulletHolderAnim.GetComponent<Animation> ().Play 
				(Animations.machineGunBulletHolderAnim);
			
			//Wait for ammo animation to finish
			yield return new WaitForSeconds (1f);
			//Close the top lid
			Components.machineGunTop.GetComponent<Animation> ().Play 
				(Animations.reloadSlideCloseAnim);

			//Play the slider sound
			AudioSources.sliderSound.Play();

			//Wait for the top lid animation to finish
			yield return new WaitForSeconds (0.15f);
			//Slider animation
			Components.slider.GetComponent<Animation> ().Play
				(Animations.slideReloadAnim);

			//Play the slider reload sound
			AudioSources.sliderReloadSound.Play();
			
			//Hide the animated bullets
			Components.bulletHolderAnim.SetActive (false);
			
			//Show the bullet belt again
			foreach (GameObject bulletObjects in Components.bullets) {
				bulletObjects.gameObject.GetComponent<MeshRenderer> ().enabled = true;
			}

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If shotgun is true
		if (WeaponType.shotgun == true) {

			//Play animations and wait
			Components.holder.GetComponent<Animation> ().Play
				(Animations.reloadDownAnim);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.shotgunSlideForwardAnim);

			//Play pump reload sound
			AudioSources.shotgunPumpSound.Play ();
			yield return new WaitForSeconds (0.2f);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.shotgunSlideBackAnim);
			yield return new WaitForSeconds (0.2f);
			Components.ejectSlider.GetComponent<Animation> ().Play 
				(Animations.shotgunSlideForwardAnim);
			
			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If rpg is true
		if (WeaponType.rpg == true) {

			//Show the projectile again
			Components.projectile.GetComponent<MeshRenderer> ().enabled = true;

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If sawn off shotgun is true
		if (WeaponType.sawnOffShotgun == true) {

			//Play animations
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadUpAnim);
			Components.barrels.GetComponent<Animation> ().Play 
				(Animations.reloadSlideCloseAnim);

			//Play barrels close sound
			AudioSources.reloadCloseSound.Play ();
			
			//Make the shellcasings visible again
			Components.shotgunAmmo1.GetComponent<MeshRenderer> ().enabled = true;
			Components.shotgunAmmo2.GetComponent<MeshRenderer> ().enabled = true;
			
			//Wait until animations are finished
			yield return new WaitForSeconds (0.1f);
			
			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If revolver 1 or revolver 2 is true
		if (WeaponType.revolver1 == true || WeaponType.revolver2 == true) {

			//Play animations
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadUpAnim);

			//Show the bullets again
			showBulletsRevolver (0);
			showBulletsRevolver (1);
			showBulletsRevolver (2);
			showBulletsRevolver (3);
			showBulletsRevolver (4);
			showBulletsRevolver (5);

			//Wait some time
			yield return new WaitForSeconds(.5f);

			//Play the reload close animation
			Components.mag.GetComponent<Animation> ().Play
				(Animations.reloadSlideCloseAnim);

			//Play click sound
			AudioSources.insertMagSound.Play ();

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If grenade launcher is true
		if (WeaponType.grenadeLauncher == true) {
			
			//Play animations
			Components.holder.GetComponent<Animation> ().Play 
				(Animations.reloadUpAnim);

			//Play the reload close animation
			Components.mag.GetComponent<Animation> ().Play
				(Animations.reloadSlideCloseAnim);

			//Play reload close sound
			AudioSources.reloadCloseSound.Play();
			
			//Show the shells again
			showBulletsRevolver (0);
			showBulletsRevolver (1);
			showBulletsRevolver (2);
			showBulletsRevolver (3);
			showBulletsRevolver (4);
			showBulletsRevolver (5);

			//Wait some time before enabling shooting
			yield return new WaitForSeconds(.5f);

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If sniper3 is true
		if (WeaponType.sniper3 == true) {
			//Wait for the slider animation to finish
			yield return new WaitForSeconds (0.05f);

			//Play slider reload animation
			Components.slider.GetComponent<Animation> ().Play
				(Animations.slideReloadAnim);

			//Play slider reload sound
			AudioSources.sliderReloadSound.Play();

			//Let reload animation finish before enabling shooting
			yield return new WaitForSeconds(0.15f);

			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//********** USED IN THE DEMO SCENES **********
			//Enable gun switching again
			noSwitch = false;
		}

		//If sniper6 is true
		if (WeaponType.sniper6 == true) {
			//Wait for reload animation to finish
			yield return new WaitForSeconds(0.1f);

			//Play slider close animation
			Components.slider.GetComponent<Animation> ().Play
				(Animations.reloadSlideCloseAnim);

			//Play slider sound
			AudioSources.sliderSound.Play ();
			
			//Let reload animation finish before enabling shooting
			yield return new WaitForSeconds(0.15f);
			
			//Enable shooting again
			outOfAmmo = false;
			isReloading = false;

			//USED IN THE DEMO SCENES
			//Enable gun switching again
			noSwitch = false;
		}
	}	

	IEnumerator ShellInsertSound(int times)
	{
		for(int i=0; i<times; i++)
		{
			AudioSources.shellInsertSound.Play ();
			yield return new WaitForSeconds(0.55f);
		}
	}

	//For MACHINE GUN only
	//If reloading when bullets left is higher than 0
	IEnumerator ReloadMachineGun ()
	{
		//********** USED IN THE DEMO SCENES **********
		//Prevent gun switching while reloading
		noSwitch = true;

		//Start reloading
		isReloading = true;

		//Hide the magazine
		Components.mag.SetActive (false);

		//Play main reload sound
		AudioSources.mainReloadSound.Play();
		//Play remove mag sound
		AudioSources.removeMagSound.Play();
		
		//Hide all bullets when reloading
		foreach (GameObject bulletObjects in Components.bullets) {
			bulletObjects.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
		
		//Spawn bullets to make it look like they are falling off the "bullet strip"
		//If there are more than 10 bullets left, or 10 bullets, spawn all
		if (bulletsLeft > 10 || bulletsLeft == 10) {
			spawnBullets (0);
			spawnBullets (1);
			spawnBullets (2);
			spawnBullets (3);
			spawnBullets (4);
			spawnBullets (5);
			spawnBullets (6);
			spawnBullets (7);
			spawnBullets (8);
			spawnBullets (9);
		}
		
		//Spawn the same amount of bullets as there is ammo left when reloading, 
		//only works when less than 10 ammo left
		if (bulletsLeft == 9) {
			spawnBullets (9);
			spawnBullets (8);
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
			spawnBullets (3);
			spawnBullets (2);
			spawnBullets (1);
		} 
		if (bulletsLeft == 8) {
			spawnBullets (8);
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
			spawnBullets (3);
			spawnBullets (2);
			spawnBullets (1);
		} 
		if (bulletsLeft == 7) {
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
			spawnBullets (3);
			spawnBullets (2);
			spawnBullets (1);
		} 
		if (bulletsLeft == 6) {
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
			spawnBullets (3);
			spawnBullets (2);
		} 
		if (bulletsLeft == 5) {
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
			spawnBullets (3);
		}
		if (bulletsLeft == 4) {
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
			spawnBullets (4);
		} 
		if (bulletsLeft == 3) {
			spawnBullets (7);
			spawnBullets (6);
			spawnBullets (5);
		} 
		if (bulletsLeft == 2) {
			spawnBullets (7);
			spawnBullets (6);
		} 
		if (bulletsLeft == 1) {
			spawnBullets (7);
		} 
		
		//Instantiate the empty magazine 
		Instantiate (emptyMagPrefab, magSpawnPoint.transform.position, 
		            magSpawnPoint.transform.rotation);
		
		//Play reload animation
		Components.holder.GetComponent<Animation> ().Play
			(Animations.reloadAnim);
		//Wait for animation to finish
		yield return new WaitForSeconds (0.35f);
		//Open top part and animate mag
		Components.machineGunTop.GetComponent<Animation> ().Play 
			(Animations.reloadSlideOpenAnim);
		Components.fullMag.GetComponent<Animation> ().Play
			(Animations.fullMagInAnim);

		//Play the insert mag sound
		AudioSources.insertMagSound.Play();
		
		//Wait for set amount of time
		yield return new WaitForSeconds (reloadDuration);

		//Refill bullets
		bulletsLeft = magazineSize;
		
		//Make the magazine visible again and play animation
		Components.mag.SetActive (true);
		Components.bulletHolderAnim.SetActive (true);

		Components.bulletHolderAnim.GetComponent<Animation> ().Play 
			(Animations.machineGunBulletHolderAnim);
		
		//Wait for the ammo animation to finish
		yield return new WaitForSeconds (1f);
		//Close the top lid
		Components.machineGunTop.GetComponent<Animation> ().Play 
			(Animations.reloadSlideCloseAnim);

		//Play the slider sound
		AudioSources.sliderSound.Play();

		//Wait for the top lid animation to finish
		yield return new WaitForSeconds (0.15f);
		//Slider animation
		Components.slider.GetComponent<Animation> ().Play
			(Animations.slideReloadAnim);

		//Play the slider reload sound
		AudioSources.sliderReloadSound.Play();
		
		//Hide the animated bullets
		Components.bulletHolderAnim.SetActive (false);
		
		//Enable shooting again
		outOfAmmo = false;
		isReloading = false;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = false;
		
		//Show the bullet belt again
		foreach (GameObject bulletObjects in Components.bullets) {
			bulletObjects.gameObject.GetComponent<MeshRenderer> ().enabled = true;
		}
	}	

	//For MACHINE GUN only
	//Used to hide all bullets in the bullet belt/strip
	public void hideBullets (int num)
	{
		for (int i = 0; i < Components.bullets.Length; i++) {
			if (i == num)
				Components.bullets [i].gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	//For MACHINE GUN only
	//Used to spawn bullets in the bullet belt/strip spawnpoints
	public void spawnBullets (int num)
	{
		for (int o = 0; o < Components.bullets.Length; o++) {
			if (o == num)
				//Spawn bullet prefabs
				Instantiate (Components.bulletPrefab, 
				            Components.bulletSpawnpoints [o].transform.position, 
				            Components.bulletSpawnpoints [o].transform.rotation);
		}
	}

	//For REVOLVER 1  and REVOLVER 2 only
	//Used to hide all bullets
	public void hideBulletsRevolver (int num)
	{
		for (int i = 0; i < Components.revolverBullets.Length; i++) {
			if (i == num)
				Components.revolverBullets [i].gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	//For REVOLVER 1 and REVOLVER 2 and GRENADE LAUNCHER only
	//Used to show all bullets
	public void showBulletsRevolver (int num)
	{
		for (int i = 0; i < Components.revolverBullets.Length; i++) {
			if (i == num)
				Components.revolverBullets [i].gameObject.GetComponent<MeshRenderer> ().enabled = true;
		}
	}

	//For REVOLVER 1 and REVOLVER 2 and GRENADE LAUNCHER only
	//Used to spawn casings on the spawnpoints
	public void spawnBulletsRevolver (int num)
	{
		for (int i = 0; i < Components.revolverCasingSpawnpoints.Length; i++) {
			if (i == num)
				//Spawn casing prefabs
				Instantiate (casingPrefab, 
				             Components.revolverCasingSpawnpoints [i].transform.position, 
				             Components.revolverCasingSpawnpoints [i].transform.rotation);
		}
	}

	//Show muzzleflash
	IEnumerator Muzzleflash ()
	{

		//Disable raycast bullet for rpg and grenade launcher, since they dont use it
		if (!WeaponType.rpg && !WeaponType.grenadeLauncher) {

			//Raycast bullet
			RaycastHit hit;
			Ray ray = new Ray (transform.position, transform.forward);
		
			//Send out the raycast from the "bulletSpawnPoint" position
			if (Physics.Raycast (bulletSpawnPoint.transform.position, 
			                     bulletSpawnPoint.transform.forward, out hit, bulletDistance)) {

				//If a rigibody is hit, add bullet force to it
				if (hit.rigidbody != null)
					hit.rigidbody.AddForce (ray.direction * bulletForce);

				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "Target"
				if (hit.transform.tag == "Target") {
					
					//Spawn bullet impact on surface
					Instantiate (metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
					//Toggle the isHit bool on the target object
					hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
				}

				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "ExplosiveBarrel"
				if (hit.transform.tag == "ExplosiveBarrel") {

					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;

					//Spawn metal impact on surface of the barrel
					Instantiate (metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "GasTank"
				if (hit.transform.tag == "GasTank") {
					
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
					
					//Spawn metal impact on surface of the gas tank
					Instantiate (metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Metal (Static)"
				if (hit.transform.tag == metalImpactStaticTag) {
				
					//Spawn bullet impact on surface
					Instantiate (metalImpactStaticPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
			
				//If the raycast hit the tag "Metal"
				if (hit.transform.tag == metalImpactTag) {
				
					//Spawn bullet impact on surface
					Instantiate (metalImpactPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
			
				//If the raycast hit the tag "Wood (Static)"
				if (hit.transform.tag == woodImpactStaticTag) {
				
					//Spawn bullet impact on surface
					Instantiate (woodImpactStaticPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Wood"
				if (hit.transform.tag == woodImpactTag) {
				
					//Spawn bullet impact on surface
					Instantiate (woodImpactPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Concrete (Static)"
				if (hit.transform.tag == concreteImpactStaticTag) {
				
					//Spawn bullet impact on surface
					Instantiate (concreteImpactStaticPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Concrete"
				if (hit.transform.tag == concreteImpactTag) {
				
					//Spawn bullet impact on surface
					Instantiate (concreteImpactPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Dirt (Static)"
				if (hit.transform.tag == dirtImpactStaticTag) {
				
					//Spawn bullet impact on surface
					Instantiate (dirtImpactStaticPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}

				//If the raycast hit the tag "Dirt"
				if (hit.transform.tag == dirtImpactTag) {
				
					//Spawn bullet impact on surface
					Instantiate (dirtImpactPrefab, hit.point, 
				            Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
			}
		}

		//Chooses a random muzzleflash from the array
		//Disable for rpg, grenade launcher, and silenced weapons since they dont use muzzleflashes
		if (!WeaponType.rpg && !WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2 && !WeaponType.grenadeLauncher) {

			Components.sideMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			Components.topMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];

			//Show the muzzleflashes
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			
			//Disable for shotgun and sawn off shotgun, and silenced weapons, since they dont use the front muzzle
			if (WeaponType.handgun == true || WeaponType.smg == true || WeaponType.assaultRifle == true || 
			    WeaponType.machineGun == true || WeaponType.sniper == true || WeaponType.revolver1 == true || WeaponType.revolver2 == true || WeaponType.assaultRifle2 == true
			    || WeaponType.sniper6 == true) {
				Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			}
		}
		
		//Show the light flash
		//Disable for silenced weapons since they dont need it
		if (!WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2) {

			Components.lightFlash.GetComponent<Light> ().enabled = true;
		}

		//Show rpg back light flash if rpg is true
		if (WeaponType.rpg == true) {
			Components.rpgLightBack.GetComponent<Light> ().enabled = true;
		}
		
		//Wait for set amount of time, default value 0.02
		yield return new WaitForSeconds (muzzleFlashDuration);
		
		//Hide the muzzleflashes, disable for rpg, grenade launcher, and silenced weapons since they doent have muzzleflashes
		if (!WeaponType.rpg && !WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2 && !WeaponType.grenadeLauncher) {

			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}

		//Disable for shotgun and sawn off shotgun, and silenced weapons since they dont use the front muzzle
		if (WeaponType.handgun == true || WeaponType.smg == true || WeaponType.assaultRifle == true || 
		    WeaponType.machineGun == true || WeaponType.sniper == true || WeaponType.revolver1 == true || WeaponType.revolver2 == true || WeaponType.assaultRifle2 == true ||
		    WeaponType.sniper3 == true || WeaponType.sniper6 == true) {

			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}
		
		//Hide the light flash
		//Disable for silenced weapons since they dont need it
		if (!WeaponType.handgunSilencer && !WeaponType.smgSilencer && !WeaponType.sniperSilencer 
		    && !WeaponType.assaultRifleSilencer && !WeaponType.assaultRifleSilencer2) {

			Components.lightFlash.GetComponent<Light> ().enabled = false;
		}

		//Hide rpg back light flash if rpg is true
		if (WeaponType.rpg == true) {
			Components.rpgLightBack.GetComponent<Light> ().enabled = false;
		}
	}	

	//For DUAL HANDGUNS only
	//Show muzzleflash for right handgun
	IEnumerator MuzzleflashRight ()
	{
			
			//Raycast bullet
			RaycastHit hit;
			Ray ray = new Ray (transform.position, transform.forward);
			
			//Send out the raycast from the "bulletSpawnPointRight" position
			if (Physics.Raycast (Components.bulletSpawnPointRight.transform.position, Components.bulletSpawnPointRight.transform.forward, out hit, bulletDistance)) {
				
				//If a rigibody is hit, add bullet force to it
				if (hit.rigidbody != null)
					hit.rigidbody.AddForce (ray.direction * bulletForce);
				
				//If the raycast hit the tag "Metal (Static)"
				if (hit.transform.tag == metalImpactStaticTag) {
					
					//Spawn bullet impact on surface
					Instantiate (metalImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Metal"
				if (hit.transform.tag == metalImpactTag) {
					
					//Spawn bullet impact on surface
					Instantiate (metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood (Static)"
				if (hit.transform.tag == woodImpactStaticTag) {
					
					//Spawn bullet impact on surface
					Instantiate (woodImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood"
				if (hit.transform.tag == woodImpactTag) {
					
					//Spawn bullet impact on surface
					Instantiate (woodImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete (Static)"
				if (hit.transform.tag == concreteImpactStaticTag) {
					
					//Spawn bullet impact on surface
					Instantiate (concreteImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete"
				if (hit.transform.tag == concreteImpactTag) {
					
					//Spawn bullet impact on surface
					Instantiate (concreteImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt (Static)"
				if (hit.transform.tag == dirtImpactStaticTag) {
					
					//Spawn bullet impact on surface
					Instantiate (dirtImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt"
				if (hit.transform.tag == dirtImpactTag) {
					
					//Spawn bullet impact on surface
					Instantiate (dirtImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
			}

		//Chooses a random muzzleflash from the array
		Components.sideMuzzleRight.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];
		Components.topMuzzleRight.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
			[Random.Range (0, Components.muzzleflashSideSprites.Length)];
		
		//Show the muzzleflashes
		Components.sideMuzzleRight.GetComponent<SpriteRenderer> ().enabled = true;
		Components.topMuzzleRight.GetComponent<SpriteRenderer> ().enabled = true;
		Components.frontMuzzleRight.GetComponent<SpriteRenderer> ().enabled = true;
		
		//Show the light
		Components.lightFlashRight.GetComponent<Light> ().enabled = true;
		
		//Wait for set amount of time
		yield return new WaitForSeconds (muzzleFlashDuration);
		
		//Hide the muzzleflashes
		Components.sideMuzzleRight.GetComponent<SpriteRenderer> ().enabled = false;
		Components.topMuzzleRight.GetComponent<SpriteRenderer> ().enabled = false;
		Components.frontMuzzleRight.GetComponent<SpriteRenderer> ().enabled = false;
		
		//Hide the light
		Components.lightFlashRight.GetComponent<Light> ().enabled = false;		
	}	

	//Bolt action reload for sniper and pump reload for shotgun
	IEnumerator PumpOrBoltActionReload ()
	{
		//********** USED IN THE DEMO SCENES **********
		//Prevent gun switching while reloading
		noSwitch = true;

		//Disable shooting
		isReloading = true;
		
		//Wait before playing the pump animation
		yield return new WaitForSeconds (0.35f);
		Components.ejectSlider.GetComponent<Animation> ().Play 
			(Animations.slideReloadAnim);

		//Play pump reload sound
		AudioSources.shotgunPumpSound.Play ();

		//Wait for the animation to finish
		yield return new WaitForSeconds (0.08f);
		
		//Spawn shellcasing
		Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
		            casingSpawnPoint.transform.rotation);
		
		//Wait before being able to shoot again
		yield return new WaitForSeconds (0.25f);
		
		//Enable shooting
		isReloading = false;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = false;
	}

	//Revolver delay when shooting, only used for revolver 1
	IEnumerator RevolverDelay () {
		//Disable shooting
		outOfAmmo = true;
		isReloading = true;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = true;

		//Wait some time
		yield return new WaitForSeconds (0.68f);
		//Enable shooting again
		outOfAmmo = false;
		isReloading = false;


		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = false;
	}

	//Delay when shooting, used for grenade launcher, sniper 3 and sniper 6
	IEnumerator ShootingDelay () {
		//Disable shooting
		outOfAmmo = true;
		isReloading = true;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = true;

		//Wait some time
		yield return new WaitForSeconds (0.4f);
		//Enable shooting again
		outOfAmmo = false;
		isReloading = false;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = false;
	}

	//Delay for the hand grenade reload
	IEnumerator HandGrenadeReload () {
		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = true;

		//Disable shooting
		isReloading = true;
		outOfAmmo = true;

		//Play reload animation
		Components.holder.GetComponent<Animation> ().Play ("hand_grenade_reload");

		//Play main reload sound
		AudioSources.mainReloadSound.Play ();

		//Disable hand grenade shooting for set amount of time
		yield return new WaitForSeconds (1.75f);

		//Refill bullets
		bulletsLeft = magazineSize;

		//Enable shooting again
		isReloading = false;
		outOfAmmo = false;

		//********** USED IN THE DEMO SCENES **********
		//Enable gun switching again
		noSwitch = false;
	}

	void Update ()
	{

		//If handgun or silenced handgun is true
		if (WeaponType.handgun == true || WeaponType.handgunSilencer == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
			
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
			
				//Play recoil animations
				Components.holder.GetComponent<Animation> ().Play 
					(Animations.recoilAnim);
				Components.slider.GetComponent<Animation> ().Play 
					(Animations.blowbackAnim);
			
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
			
				//Play smoke particles
				Components.smokeParticles.Play ();
			
				//Spawn casing
				Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
				             casingSpawnPoint.transform.rotation);
			}
		
			//If out of ammo
			if (bulletsLeft == 0) {
				outOfAmmo = true;
				//Play the slider animation once only
				if (!hasPlayed) {
					Components.slider.GetComponent<Animation> ().Play 
						(Animations.outOfAmmoAnim);
					hasPlayed = true;
					outOfAmmoSlider = true;

					//Play slider sound
					AudioSources.sliderSound.Play ();
				}
			}
		}

		//If dual handguns is true
		if (WeaponType.dualHandguns == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				if (shootLeft == true && !shootRight) {
					shootRight = true;
					shootLeft = false;
				} else {
					shootRight = false;
					shootLeft = true;
				}
				
				if (shootLeft) {
					
					//Muzzleflash
					StartCoroutine (Muzzleflash ());
					//Play recoil animations
					Components.holder.GetComponent<Animation> ().Play 
						(Animations.recoilAnim);
					Components.slider.GetComponent<Animation> ().Play 
						(Animations.blowbackAnim);
					
					//Remove 1 bullet everytime you shoot
					bulletsLeft -= 1;

					//Play shoot sound
					AudioSources.shootSound.Play ();

					//Play smoke particles
					Components.smokeParticles.Play ();
					//Spawn casing
					Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
					             casingSpawnPoint.transform.rotation);
					
				} else {
					
					//Muzzleflash
					StartCoroutine (MuzzleflashRight ());
					//Play recoil animations
					Components.holderRight.GetComponent<Animation> ().Play 
						(Animations.recoilAnim);
					Components.sliderRight.GetComponent<Animation> ().Play 
						(Animations.blowbackAnim);
					
					//Remove 1 bullet everytime you shoot
					bulletsLeft -= 1;

					//Play shoot sound
					AudioSources.shootSound.Play ();

					//Play smoke particles
					Components.smokeParticlesRight.Play ();
					//Spawn casing
					Instantiate (casingPrefab, Components.casingSpawnPointRight.transform.position, 
					             Components.casingSpawnPointRight.transform.rotation);
				}
			}

			//If out of ammo
			if (bulletsLeft == 0) {
				outOfAmmo = true;
				//Play the slider animation once only
				if (!hasPlayed) {
					Components.slider.GetComponent<Animation> ().Play 
						(Animations.outOfAmmoAnim);
					Components.sliderRight.GetComponent<Animation> ().Play 
						(Animations.outOfAmmoAnim);
					hasPlayed = true;
					outOfAmmoSlider = true;
				}
			}
		}

		//If sniper, silenced sniper, or shotgun is true
		if (WeaponType.sniper == true || WeaponType.shotgun == true || WeaponType.sniperSilencer == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
				StartCoroutine (PumpOrBoltActionReload ());	
				
				//Play recoil animation
				Components.holder.GetComponent<Animation> ().Play
					(Animations.recoilAnim);
				
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
				
				//Play smoke particles
				Components.smokeParticles.Play ();

				//Play spark particles if shotgun is true
				if (WeaponType.shotgun == true) {
					Components.sparkParticles.Play ();
				}
			}
		}

		//If smg, assault rifle, silenced smg or silenced assault rifle is true
		if (WeaponType.smg == true || WeaponType.assaultRifle == true || WeaponType.smgSilencer == true || WeaponType.assaultRifleSilencer == true || 
		    WeaponType.assaultRifle2 == true || WeaponType.assaultRifleSilencer2 == true) {
			//Shoot when left click is held down
			if (Input.GetMouseButton (0)) {
				if (Time.time - lastFired > 1 / fireRate && !outOfAmmo && !isReloading) {
					lastFired = Time.time;
					
					//Muzzleflash
					StartCoroutine (Muzzleflash ());
					
					//Play recoil and eject animations
					Components.holder.GetComponent<Animation> ().Play
						(Animations.recoilAnim);
					Components.ejectSlider.GetComponent<Animation> ().Play
						(Animations.slideEjectAnim);
					
					//Remove 1 bullet everytime you shoot
					bulletsLeft -= 1;

					//Play shoot sound
					AudioSources.shootSound.Play ();
					
					//Play smoke particles
					Components.smokeParticles.Play ();
					
					//Spawn casing
					Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
					            casingSpawnPoint.transform.rotation);
				}
			}
		}

		//If machine gun is true
		if (WeaponType.machineGun == true) {
			//Shoot when left click is held down
			if (Input.GetMouseButton (0)) {
				if (Time.time - lastFired > 1 / fireRate && !outOfAmmo && !isReloading) {
					lastFired = Time.time;
				
					//Rotate the sphere holding the bullets 
					//Make it look like they are "spinning" to the right
					Components.sphereRotate.GetComponent<Animation> ().Play
					(Animations.machineGunSphereRotateAnim);
				
					//Muzzleflash
					StartCoroutine (Muzzleflash ());
				
					//Play recoil animation
					Components.holder.GetComponent<Animation> ().Play
					(Animations.recoilAnim);
				
					//Remove 1 bullet everytime you shoot
					bulletsLeft -= 1;

					//Play shoot sound
					AudioSources.shootSound.Play ();
				
					//Play smoke particles
					Components.smokeParticles.Play ();
				
					//Spawn casing
					Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
					             casingSpawnPoint.transform.rotation);
				
					//Hide bullets in bullet belt when low ammo
					if (bulletsLeft == 9) {
						hideBullets (9);
					} 
					if (bulletsLeft == 8) {
						hideBullets (8);
					} 
					if (bulletsLeft == 7) {
						hideBullets (7);
					} 
					if (bulletsLeft == 6) {
						hideBullets (6);
					} 
					if (bulletsLeft == 5) {
						hideBullets (5);
					} 
					if (bulletsLeft == 4) {
						hideBullets (4);
					} 
					if (bulletsLeft == 3) {
						hideBullets (3);
					} 
					if (bulletsLeft == 2) {
						hideBullets (2);
					} 
					if (bulletsLeft == 1) {
						hideBullets (1);
					} 
					if (bulletsLeft == 0) {
						hideBullets (0);
					} 
				}
			}

			//Reload when R key is pressed, if reloaded when ammo is at 0
			if (Input.GetKeyDown (KeyCode.R) && bulletsLeft == 0 && !isReloading) {
				StartCoroutine (Reload ());
			}

			//Reload when R key is pressed, if reloaded when ammo is higher than 0
			if (Input.GetKeyDown (KeyCode.R) && bulletsLeft > 0 && bulletsLeft < magazineSize && !isReloading) {
				StartCoroutine (ReloadMachineGun ());
			}
		}

		//If rpg is true
		if (WeaponType.rpg == true) {

			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Reload and show muzzleflash
				StartCoroutine (Muzzleflash ());
				StartCoroutine (Reload ());
				
				//Play recoil animation
				Components.holder.GetComponent<Animation> ().Play
					(Animations.recoilAnim);

				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play();
				
				//Instantiate the projectile
				Instantiate (Components.projectilePrefab, Components.projectileSpawnPoint.transform.position, 
				            Components.projectileSpawnPoint.transform.rotation);
				
				//Play smoke and spark particles
				Components.smokeParticles.Play ();
				Components.smokeParticlesBack.Play ();
				Components.sparkParticles.Play ();
			}
		}

		//If sawn off shotgun is true
		if (WeaponType.sawnOffShotgun == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
			
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
			
				//Play recoil and eject animations
				Components.holder.GetComponent<Animation> ().Play 
					(Animations.recoilAnim);
			
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
			
				//Play smoke and spark particles
				Components.smokeParticles.Play ();
				Components.sparkParticles.Play ();
			}
		}

		//If revolver 1 is true 
		if (WeaponType.revolver1 == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
				
				//Play recoil and eject animations
				Components.holder.GetComponent<Animation> ().Play 
					(Animations.recoilAnim);
				Components.slider.GetComponent<Animation>().Play
					(Animations.slideReloadAnim);
				
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
				
				//Play smoke particles
				Components.smokeParticles.Play ();

				//Start the revolver delay
				StartCoroutine(RevolverDelay());
			}
		}

		//If revolver 2 is true
		if (WeaponType.revolver2 == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
				
				//Play recoil and eject animations
				Components.holder.GetComponent<Animation> ().Play 
					(Animations.recoilAnim);
				Components.slider.GetComponent<Animation>().Play
					(Animations.slideReloadAnim);
				
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
				
				//Play smoke particles
				Components.smokeParticles.Play ();
			}
		}

		//If grenade launcher is true
		if (WeaponType.grenadeLauncher == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
				
				//Play recoil and eject animations
				Components.holder.GetComponent<Animation> ().Play 
					(Animations.recoilAnim);
				Components.slider.GetComponent<Animation>().Play
					(Animations.slideReloadAnim);

				//Instantiate the projectile
				Instantiate (Components.projectilePrefab, Components.projectileSpawnPoint.transform.position, 
				             Components.projectileSpawnPoint.transform.rotation);
				
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play();
				
				//Play smoke and spark particles
				Components.smokeParticles.Play ();
				Components.sparkParticles.Play ();

				//Start the shooting delay
				StartCoroutine(ShootingDelay());
			}
		}

		//If sniper 3 or sniper 6 is true
		if (WeaponType.sniper3 == true || WeaponType.sniper6 == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {
				
				//Muzzleflash
				StartCoroutine (Muzzleflash ());
				
				//Play recoil animation
				Components.holder.GetComponent<Animation> ().Play
					(Animations.recoilAnim);
				Components.ejectSlider.GetComponent<Animation> ().Play
					(Animations.slideEjectAnim);
				
				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play shoot sound
				AudioSources.shootSound.Play ();
				//Play slider reload sound
				AudioSources.sliderReloadSound.Play ();
				
				//Play smoke particles
				Components.smokeParticles.Play ();

				//Spawn casing
				Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
				             casingSpawnPoint.transform.rotation);

				//Start the shooting delay
				StartCoroutine(ShootingDelay());
			}
		}

		//If hand grenade is true
		if (WeaponType.handGrenade == true) {
			//Shoot when left click is pressed
			if (Input.GetMouseButtonDown (0) && !outOfAmmo && !isReloading) {

				//Start the hand grenade reload
				StartCoroutine(HandGrenadeReload());

				//Remove 1 bullet everytime you shoot
				bulletsLeft -= 1;

				//Play throw/shoot sound
				AudioSources.shootSound.Play ();

				//Instantiate the grenade prefab
				GameObject grenadePrefab;
				grenadePrefab = Instantiate(Components.handGrenadePrefab, 
				                            transform.position, transform.rotation) as GameObject;

				//Add forward force to the grenade prefab
				grenadePrefab.GetComponent<Rigidbody>().AddForce
					(transform.forward * grenadeThrowForce);

				//Add rotation force to the grenade prefab
				grenadePrefab.GetComponent<Rigidbody>().AddRelativeTorque (
					Random.Range(minimumGrenadeRotation, maximumGrenadeRotation), //X Axis
					Random.Range(minimumGrenadeRotation, maximumGrenadeRotation), //Y Axis
					Random.Range(minimumGrenadeRotation, maximumGrenadeRotation)  //Z Axis
					* Time.deltaTime);
			}
		}

		//If minigun is true
		if (WeaponType.minigun == true) {
			//Shoot when left click is held down
			if (Input.GetMouseButton (0)) {
				if (Time.time - lastFired > 1 / fireRate) {
					lastFired = Time.time;
					
					if (allowMinigunShooting == true) {
						//Play recoil animation
						Components.holder.GetComponent<Animation> ().Play
							(Animations.recoilAnim);
					
						//Play smoke particles
						Components.smokeParticles.Play ();
						//Play bullet tracer particles
						Components.bulletTracerParticles.Play();

						//Play shoot sound
						AudioSources.shootSound.Play ();
					
						//Spawn casing
						Instantiate (casingPrefab, casingSpawnPoint.transform.position, 
				      	 casingSpawnPoint.transform.rotation);

						//Muzzleflash
						StartCoroutine (Muzzleflash ());

					}
					//Increase the minigun rotation speed while holding down left click
					minigunRotationSpeed += 1900.0f * Time.deltaTime;

					//Play spin up sound
					if (!spinUpSoundHasPlayed) {

						//Set the spin up sound as the current audio clip
						AudioSources.minigunSpinUpDownSource.clip 
							= AudioSources.minigunSpinUpSound;

						//Play spin up sound
						AudioSources.minigunSpinUpDownSource.Play ();

						spinUpSoundHasPlayed = true;
						spinDownSoundHasPlayed = false;
					}
				}
			//Start decreasing the minigun rotation speed when left click is released
			} else if (minigunRotationSpeed > 0) {
				minigunRotationSpeed -= 300.0f * Time.deltaTime;

				spinLoopHasPlayed = false;

				//Stop the spin loop sound
				AudioSources.minigunSpinLoopSound.Stop ();

				//Play spin up sound
				if (!spinDownSoundHasPlayed) {

					//Set the spin down sound as the current audio clip
					AudioSources.minigunSpinUpDownSource.clip 
						= AudioSources.minigunSpinDownSound;

					//Play spin up sound
					AudioSources.minigunSpinUpDownSource.Play ();
					
					spinDownSoundHasPlayed = true;
					spinUpSoundHasPlayed = false;
				}
			}
			//Set the rotation speed to zero if the rotaiton speed is lower than zero
			if (minigunRotationSpeed < 0) {
				minigunRotationSpeed = 0;
			}
			//Stop increasing the rotation speed when it has reached 875.0f
			if (minigunRotationSpeed > 875.0f) {
				minigunRotationSpeed = 875.0f;
			}
			//Allow the minigun to shoot when the rotation speed is higher than 800.0f
			if (minigunRotationSpeed > 800.0f) {
				allowMinigunShooting = true;

				if (!spinLoopHasPlayed) {
					//Play the spin loop sound
					AudioSources.minigunSpinLoopSound.Play ();
					
					spinLoopHasPlayed = true;
				}

			//Dont allow shooting when the rotation speed is lower than 750.0f
			} else if (minigunRotationSpeed < 750.0f) {
				allowMinigunShooting = false;
			}
			//Rotate the barrels depending on the minigun rotation speed
			Components.barrels.transform.Rotate
				(Vector3.forward * minigunRotationSpeed * Time.deltaTime);

		}

		//If out of ammo
		if (bulletsLeft == 0) {
			outOfAmmo = true;
		}

		//Disable for machine gun, hand grenade and minigun
		if (!WeaponType.machineGun && !WeaponType.handGrenade && !WeaponType.minigun) {
			//Reload when R key is pressed
			if (Input.GetKeyDown (KeyCode.R) && bulletsLeft < magazineSize && !isReloading) {
				StartCoroutine (Reload ());
			}
		}

		//Play dry fire sound when out of ammo, disable for minigun since it has unlimited ammo
		if (Input.GetMouseButtonDown(0) && outOfAmmo == true && !isReloading 
		    && !WeaponType.minigun) {
			//Play dry fire sound if clicking when out of ammo
			AudioSources.outOfAmmoClickSound.Play();
		}
	}
}