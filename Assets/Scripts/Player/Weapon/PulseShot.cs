using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseShot : SecondaryGun
{
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
        else if (bulletsShot < bulletsPerAttack)
        {
            //Debug.Log("Hello");
            //shooting = true;
            if (currentTimeBetweenBullets >= timeBetweenBullets)
            {
                ShootPulseRifle();
            }
            currentTimeBetweenBullets += Time.deltaTime;



        }
        else if (attackPressed && bulletsLeft > 0)
        {
            bulletsShot = 0;
            visuals.ChangeShooting();
        }
    }

    void ShootPulseRifle()
    {
        GameObject projectile = Instantiate(bullet, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
        projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
        float spreadX = Random.Range(-spreadRange, spreadRange);
        float spreadY = Random.Range(-spreadRange, spreadRange);
        Vector3 direction = camTran.forward + new Vector3(spreadX, spreadY, 0);
        rbProjectile.velocity = direction * shootForce;
        rbProjectile.useGravity = false;
        playerRb.AddForce((-1f * camTran.forward).normalized * shootPush, ForceMode.Impulse);
        bulletsLeft--;
        bulletsShot++;
        UpdateAmmoUI();
        currentTimeBetweenBullets = 0;
        if (bulletsShot >= bulletsPerAttack)
        {
            visuals.ChangeShooting();
            //bulletsShot = 0;
            shooting = false;
            currentTimeBetweenAttacks = 0;
        }
    }


}
