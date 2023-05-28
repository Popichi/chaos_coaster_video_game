using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    bool grounded;

    [Header("Camera Control")]
    public Transform cameraPos;
    public float sensitivityX;
    public float sensitivityY;

    private float xRotation;
    private float yRotation;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public Transform parent;

    [Header("Shooting")]
    public Transform rangedSpawnPoint;
    public GameObject primaryProjectile;
    public float baseProjectileSpeed = 10f;
    public float weaponCooldown;
    public float weaponPush;
    private float currentCooldown;
    private bool primaryCharging;
    private float chargeTimer;
    private float chargeLvl1 = 1f;
    private float chargeLvl2 = 3f;
    private bool testCharge1, testCharge2;
    private bool secondaryPressed;

    private float horizontalMov;
    private float verticalMov;

    [Header("Other Weapons")]
    public SecondaryGun[] weapons;
    public int currentWeapon;

    [Header("Pulse Rifle")]
    public float timeBetweenShots;
    public int ammoCapacity;
    public float basePulseSpeed;
    public float pulsePush;


    float currentTimeBetweenShots;
    GameObject follower;



    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentCooldown = weaponCooldown;
        readyToJump = true;
        primaryCharging = false;
        secondaryPressed = false;
        chargeTimer = 0;
        testCharge1 = false;
        testCharge2 = false;
        currentTimeBetweenShots = timeBetweenShots;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].setUpWeapon(this.GetComponent<Rigidbody>(), cameraPos, rangedSpawnPoint);
        }
        currentWeapon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Movement and Rotation
        //ignore rotation of the follower
        if (this.gameObject.transform.parent != null)
            this.gameObject.transform.parent.parent.rotation = Quaternion.Euler(0, 0, -90);
        cameraPos.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, groundLayer);

        if (grounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }
        SpeedControl();

        //Weapon Cooldown, needs more work for multiple weapons
        if (currentCooldown >= 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (primaryCharging)
        {
            chargeTimer += Time.deltaTime;
            if (!testCharge2 && chargeTimer >= chargeLvl2)
            {
                testCharge2 = true;
                Debug.Log("lvl 2 Charged");
            } else if (!testCharge1 && chargeTimer >= chargeLvl1)
            {
                testCharge1 = true;
                Debug.Log("lvl 1 Charged");
            }
        } 
        if (false)
        {
            if (currentTimeBetweenShots >= timeBetweenShots)
            {
                ShootPulseRifle();
            }
            currentTimeBetweenShots += Time.deltaTime;

        }
    }

    private void FixedUpdate()
    {
        //Framerate indepent movement
        MovePlayer();
    }

    void OnCamera(InputValue value)
    {
        Vector2 mouseInput = value.Get<Vector2>();
        float mouseY = mouseInput.x * sensitivityX * Time.deltaTime; //delta time?
        float mouseX = mouseInput.y * sensitivityY * Time.deltaTime;

        yRotation += mouseY;
        xRotation -= mouseX;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

    }

    void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        horizontalMov = moveInput.x;
        verticalMov = moveInput.y;
    }

    void MovePlayer()
    {
        Vector3 direction = (transform.forward * verticalMov + transform.right * horizontalMov).normalized;
        if (grounded)
        {
            rb.AddForce(direction * moveSpeed * 10f, ForceMode.Force);
        } else
        {
            rb.AddForce(direction * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }
    }

    void OnJump(InputValue value)
    {

        if (readyToJump && grounded)
        {
            readyToJump = false;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void OnShootPrimary(InputValue context)
    {
        if (!primaryCharging)
        {
            primaryCharging = true;
        } else
        //if (currentCooldown <= 0)
        {
            int lvlCharge = 0;
            if (testCharge2)
            {
                lvlCharge = 2;
            }
            else if (testCharge1)
            {
                lvlCharge = 1;
            }
            Debug.Log("Shot with charge = " + lvlCharge);
            GameObject projectile = Instantiate(primaryProjectile, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
            //Scale the projectile based on charge, might need no make this differently
            projectile.transform.localScale *= (1 + lvlCharge);
            Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
            rbProjectile.velocity = cameraPos.forward * baseProjectileSpeed;
            rb.AddForce((-1f * baseProjectileSpeed * cameraPos.forward).normalized * weaponPush * (1 + lvlCharge), ForceMode.Impulse);
            currentCooldown = weaponCooldown;


            primaryCharging = false;
            chargeTimer = 0;
            testCharge1 = false;
            testCharge2 = false;
        }
    }

    void OnShootSecondary()
    {
        if (!secondaryPressed)
        {
            //Do something while the button is being held
            weapons[currentWeapon].ShootHeld();
            secondaryPressed = true;
        } else
        {
            weapons[currentWeapon].ShootReleased();
            //Do something when the button is released
            secondaryPressed = false;
        }
    }

    void ShootPulseRifle()
    {
        GameObject projectile = Instantiate(primaryProjectile, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
        projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Rigidbody rbProjectile = projectile.GetComponent<Rigidbody>();
        float spreadRange = 0.05f;
        float spreadX = Random.Range(-spreadRange, spreadRange);
        float spreadY = Random.Range(-spreadRange, spreadRange);
        Vector3 direction = cameraPos.forward + new Vector3(spreadX, spreadY, 0);
        rbProjectile.velocity = direction * basePulseSpeed;
        rbProjectile.useGravity = false;
        rb.AddForce((-1f * basePulseSpeed * cameraPos.forward).normalized * pulsePush, ForceMode.Impulse);
        currentTimeBetweenShots = 0;
    }

    //May need to remove this if the player is getting launched, try without this first or set a different max speed
    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 newVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
