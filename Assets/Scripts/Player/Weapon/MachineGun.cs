using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : SecondaryGun
{
    public bool smartMode;
    //For now just implement the machine gun as is
    public float shootDelay;
    public float mgRadius;
    public float targetAngle;
    private GameObject currentTarget;
    private float animDelay;
    public float damage;
    public float force;


    public override void ShootHeld()
    {
        PlayGunshotSound();
        attackPressed = true;
        visuals.ChangeShooting();
    }

    public override void ShootReleased()
    {
        StopGunshotSound();
        attackPressed = false;
        visuals.ChangeShooting();
    }

    // Start is called before the first frame update
    void Start()
    {
        animDelay = timeBetweenAttacks;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimeBetweenAttacks <= timeBetweenAttacks)
        {
            currentTimeBetweenAttacks += Time.deltaTime;
        }
        else if (attackPressed && bulletsLeft > 0)
        {
            if (!shooting)
            {
                shooting = true;
            }
            ShootMachineGun();
        } else if (!attackPressed && bulletsLeft > 0 && shooting)
        {
            shooting = false;
        }
        
        visuals.ChangeShooting(shooting);
    }

    void FindTarget()
    {
        //Find entities in radius
        Collider[] potentialTargets = Physics.OverlapSphere(transform.position, mgRadius);

        //Check angle with camera front

        //Raycast to check for obstructions

        //Choose shortest rayhit as target

    }

    void ShootMachineGun()
    {       
        GameObject projectile = Instantiate(bullet, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
        MachineGunBullet currentBullet = projectile.GetComponent<MachineGunBullet>();
        currentBullet.SetValues(damage, force);
        Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
        float spreadX = Random.Range(-spreadRange, spreadRange);
        float spreadY = Random.Range(-spreadRange, spreadRange);
        Vector3 direction = (camTran.forward + new Vector3(spreadX, spreadY, 0)).normalized;
        rbProjectile.velocity = direction * shootForce;
        rbProjectile.useGravity = false;
        playerRb.AddForce((-1f * camTran.forward).normalized * shootPush, ForceMode.Impulse);
        currentTimeBetweenAttacks = 0;
        bulletsLeft--;
        UpdateAmmoUI();
        if (bulletsLeft <= 0)
        {
            shooting = false;
        }
    }
}
