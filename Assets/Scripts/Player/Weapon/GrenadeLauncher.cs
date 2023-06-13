using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : SecondaryGun
{
    public float grenadeDamage, grenadeRadius, grenadeForce, grenadeTimer;
    //Shoot grenade projectiles, small delay between shots, more bouncy, explode after a time delay, no explosion on contact, affected by gravity
    public override void ShootHeld()
    {
        attackPressed = true;
    }

    public override void ShootReleased()
    {
        attackPressed = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimeBetweenAttacks <= timeBetweenAttacks)
        {
            currentTimeBetweenAttacks += Time.deltaTime;
        } else if (attackPressed && bulletsLeft > 0)
        {
            currentTimeBetweenAttacks = 0f;
            ShootGrenade();
        }
    }

    void ShootGrenade()
    {
        GameObject projectile = Instantiate(bullet, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
        Grenade grenade = projectile.GetComponent<Grenade>();
        grenade.SetValues(grenadeDamage, grenadeRadius, grenadeForce, grenadeTimer);
        Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
        rbProjectile.velocity = camTran.forward * shootForce;
        playerRb.AddForce((-1f * camTran.forward).normalized * shootPush, ForceMode.Impulse);
        bulletsLeft--;
    }


}
