using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityWeapon : SecondaryGun
{

    SingularityProjectile currentProjectile;
    public float grenadeDamage;
    public float grenadeForce;
    public float grenadeRadius;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ShootHeld()
    {       
        attackPressed = true;
    }

    public override void ShootReleased()
    {
        attackPressed = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentTimeBetweenAttacks <= timeBetweenAttacks)
        {
            currentTimeBetweenAttacks += Time.deltaTime;
        }
        else if (attackPressed && !shooting && bulletsLeft > 0)
        {
            ShootSingularity();
        }

        if (!attackPressed && shooting)
        {
            DetonateSingularity();
        }
    }

    void ShootSingularity()
    {
        PlayGunshotSound();
        visuals.ChangeShooting();
        GameObject projectile = Instantiate(bullet, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
        currentProjectile = projectile.GetComponent<SingularityProjectile>();
        currentProjectile.SetValues(grenadeDamage, grenadeRadius, grenadeForce);
        Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
        rbProjectile.useGravity = false;
        rbProjectile.velocity = camTran.forward * shootForce;
        playerRb.AddForce((-1f * camTran.forward).normalized * shootPush, ForceMode.Impulse);
        shooting = true;
        bulletsLeft--;
    }

    void DetonateSingularity()
    {
        visuals.ChangeShooting();
        currentProjectile.Explode();
        currentProjectile = null;
        shooting = false;
        currentTimeBetweenAttacks = 0;
        UpdateAmmoUI();
    }
}
