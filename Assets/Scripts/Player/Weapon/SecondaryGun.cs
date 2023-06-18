using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SecondaryGun : MonoBehaviour
{
    //Credit to https://www.youtube.com/@davegamedevelopment for the tutorial and code base


    public string weaponName;
    //bullet
    public GameObject bullet;
    public Transform rangedSpawnPoint;
    public Transform camTran;
    public bool isActive;
    //bullet force
    public float shootForce, upwardShootForce, shootPush;

    //Gun Stats
    public float timeBetweenAttacks, spreadRange, reloadTime, timeBetweenBullets;
    public int magazineSize, bulletsPerAttack;

    protected float currentTimeBetweenAttacks, currentTimeBetweenBullets;

    public int bulletsLeft, bulletsShot;

    //Recoil
    public Rigidbody playerRb;
    public float recoilForce; //Maybe add more complex recoil

    //bools
    protected bool shooting, readyToShoot, reloading;
    public bool attackPressed;

    //Graphics
    public GameObject muzzleFlash; //Need to do this later or change it entirely
    //Textmeshpro for basic UI

    bool firstShot;
    public WeaponVisuals visuals;
    public PlayerUI ui;

    [Header("Sound")]
    public AudioClip gunshotSound;
    public AudioClip chargingSound;
    public AudioSource weaponSoundAudioSource;


    // Start is called before the first frame update
    //Start or awake
    //Handle differently depending on how the weapon is acquired
    void Start()
    {
        //magazine full at the start
        isActive = false;
    }

    // Update is called once per frame
    //No direct input handling, weapon is called to shoot from controller
    void Update()
    {
        //Change display of bullets here, this may also be better done in the controller
    }


    public void PlayGunshotSound()
    {
        if (gunshotSound != null)
        {
            weaponSoundAudioSource.clip = gunshotSound;
            weaponSoundAudioSource.Play();
        }
    }

    public void StopGunshotSound()
    {
        if (weaponSoundAudioSource != null)
        {         
            weaponSoundAudioSource.Stop();
        }
    }

    public void PlayChargingSound()
    {
        if(chargingSound != null)
        {
            weaponSoundAudioSource.clip = chargingSound;
            weaponSoundAudioSource.Play();
        }        
    }

    public abstract void ShootHeld();


    public abstract void ShootReleased();

    public void setUpWeapon(Rigidbody playerRb, Transform cameraTran, Transform projectileSpawnPoint, WeaponVisuals _visuals, PlayerUI _ui)
    {
        this.playerRb = playerRb;
        this.camTran = cameraTran;
        this.rangedSpawnPoint = projectileSpawnPoint;
        currentTimeBetweenAttacks = timeBetweenAttacks;
        currentTimeBetweenBullets = 0;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        firstShot = true;
        attackPressed = false;
        bulletsShot = bulletsPerAttack;
        visuals = _visuals;
        ui = _ui;
    }

    //No animation this time
    public void Reload(int amount)
    {
        bulletsLeft += amount;
        if (bulletsLeft > magazineSize)
        {
            bulletsLeft = magazineSize;
        } 
    }

    public void UpdateAmmoUI()
    {
        ui.UpdateAmmo(bulletsLeft);
    }

    /*
    //Needs to be public I think
    public void Shoot(Transform camTram, Transform rangedSpawnPoint)
    {
        readyToShoot = false;

        //Calculate direction from attackPoint to target point
        //Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        Vector3 directionWithoutSpread = camTram.forward;

        //Calculate Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //New direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, rangedSpawnPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(camTram.up * upwardShootForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, rangedSpawnPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if (firstShot)
        {
            firstShot = false;

            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //Need to think about this one, testings
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShots);
        }
    }
    */


}
