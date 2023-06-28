using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;

public interface ITakeDamage
{
    //dead or not
    bool TakeDamage(float d);
   

}
public interface ICanDie
{
    bool Die();
    
}
public class PlayerController : MonoBehaviour, IReactOnDeathPlane, ITakeDamage, ICanDie
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
    public Transform cameraPointStart;
    public Transform cameraLookAtIntro;
    public Transform cameraLookAtGame;
    public Transform cameraFollowGame;
    public CinemachineVirtualCamera virtualCamera;
    public float introDuration;
    public float introSpeed;

    private float xRotation;
    private float yRotation;

    [Header("Movement")]
    public float moveSpeed;
    public float maxSpeedY;
    public float speedDecay;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool speedControlEnabled;
    public Transform parent;
    public Transform playerTempSpawnPoint;
    public bool introSequenceEnabled;

    [Header("Health")]
    public int health;
    public float invicibilityDuration;
    public float pushbackHorizontal;
    public float pushbackVertical;

    private float currentInvincibilityDuration;
    private bool isInvincible;


    [Header("Shooting")]
    public Transform rangedSpawnPoint;
    public Transform visualMainSpawnPoint;
    public GameObject primaryProjectile;
    public float baseProjectileSpeed = 10f;
    public float weaponCooldown;
    public float weaponPush;
    private float currentCooldown;
    private bool primaryCharging;
    private float chargeTimer;
    private float chargeLvl1 = 1f;
    private float chargeLvl2 = 3f;
    public int lvl0Damage;
    public int lvl1Damage;
    public int lvl2Damage;
    private bool testCharge1, testCharge2;
    private bool secondaryPressed;
    private bool swapPressed;
    private float currentSwapTime;
    private float wheelThreshold;
    private bool mainFailed;
    private bool secondaryFailed;
    private GameObject currentMainProjectile;
    private Rigidbody currentMainRb;

    [Header("Sound")]
    public AudioClip mainWeaponChargeSound;
    public AudioClip mainWeaponReleaseSound;
    public AudioClip pulseWeaponSound;
    public AudioSource weaponAudioSource;

    private float horizontalMov;
    private float verticalMov;
    

    [Header("Other Weapons")]
    public SecondaryGun[] weapons;
    public int currentWeapon;
    public WeaponVisuals weaponVisuals;
    public PlayerUI playerUI;




    float currentTimeBetweenShots;
    public GameObject follower;

    private float pulseSwapDelay;
    private float currentPulseSwapDelay;
    public FootstepSoundController damageSound;
    private void Awake()
    {
        damageSound = gameObject.GetComponent<FootstepSoundController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pulseSwapDelay = 1f;
        currentPulseSwapDelay = 0f;
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
        for (int i = 0; i < weapons.Length; i++)
        {
           // weapons[i].setUpWeapon(this.GetComponent<Rigidbody>(), cameraPos, rangedSpawnPoint, weaponVisuals, playerUI);
            weapons[i].setUpWeapon(this.GetComponent<Rigidbody>(), cameraPos, visualMainSpawnPoint, weaponVisuals, playerUI);
            weapons[i].enabled = false;
        }
        //weaponVisuals.ChangeWeapon(currentWeapon);
        weapons[currentWeapon].enabled = true;
        swapPressed = false;
        currentSwapTime = 0f;
        wheelThreshold = 1f;
        mainFailed = false;
        secondaryFailed = false;
        playerUI.Init();
        playerUI.UpdatePlayerHealth(1f);
        playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
        playerUI.ResetMainCharge();
        isInvincible = false;
        currentInvincibilityDuration = 0f;
        if (introSequenceEnabled)
        {
            virtualCamera.Follow = cameraPointStart;
            virtualCamera.LookAt = cameraLookAtIntro;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Invoke(nameof(StartGame), introDuration);
        }
    }

    private void StartGame()
    {
        StartCoroutine(nameof(IntroFadeIn));
        
    }

    private IEnumerator IntroFadeIn()
    {
        float startTime = Time.time;
        Vector3 startPoint = cameraPointStart.position;
        float journeyLength = Vector3.Distance(cameraPointStart.position, transform.position);
        float distCovered = 0;
        while (distCovered <= journeyLength - 0.5f)
        {
            distCovered = (Time.time - startTime) * introSpeed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            cameraPointStart.position = Vector3.Lerp(startPoint, transform.position, fractionOfJourney);
            yield return new WaitForEndOfFrame();
        }
        virtualCamera.Follow = cameraFollowGame;
        virtualCamera.LookAt = cameraLookAtGame;
        rb.constraints = RigidbodyConstraints.None;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Movement and Rotation
        //ignore rotation of the follower
        //if (this.gameObject.transform.parent != null)
            //this.gameObject.transform.parent.parent.rotation = Quaternion.Euler(0, 0, -90);
        if (currentPulseSwapDelay > 0)
        {
            currentPulseSwapDelay -= Time.deltaTime;
        }
        cameraPos.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);

        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, groundLayer);

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0.3f;
        }
        if (!isInvincible)
            SpeedControl();

        if (currentCooldown >= 0)
        {
            currentCooldown -= Time.deltaTime;
        } else if (mainFailed)
        {
            mainFailed = !mainFailed;
            OnShootPrimary(null);
        }

        if (primaryCharging)
        {
            chargeTimer += Time.deltaTime;
            if (!testCharge2 && chargeTimer >= chargeLvl2)
            {

                testCharge2 = true;
                Debug.Log("lvl 2 Charged");
                weaponVisuals.ChangeCharge(3);
                playerUI.IncreaseMainCharge();
                currentMainProjectile.transform.localScale *= 2;
                currentMainRb.mass *= 2;
            }
            else if (!testCharge1 && chargeTimer >= chargeLvl1)
            {
                testCharge1 = true;
                Debug.Log("lvl 1 Charged");
                weaponVisuals.ChangeCharge(2);
                playerUI.IncreaseMainCharge();
                currentMainProjectile.transform.localScale *= 2;
                currentMainRb.mass *= 2;
            }
        }

        if (swapPressed)
        {
            currentSwapTime += Time.deltaTime;
            if (currentSwapTime >= wheelThreshold)
            {
                //Display weapon wheel
                Debug.Log("Wheapon Wheel displayed");
            }
        }


        if (currentMainProjectile != null) {
            currentMainProjectile.transform.position = visualMainSpawnPoint.position;
        }

        if (isInvincible)
        {
            currentInvincibilityDuration += Time.deltaTime;
            if (currentInvincibilityDuration >= invicibilityDuration)
            {
                isInvincible = false;
                currentInvincibilityDuration = 0f;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent") && !isInvincible)
        {
            isInvincible = true;
            TakeDamage(25);
            Vector3 forceDir = collision.transform.forward;
            rb.AddForce(-transform.forward * pushbackHorizontal, ForceMode.Impulse);
            rb.AddForce(transform.up * pushbackVertical, ForceMode.Impulse);
        }
    }

    public bool MyTakeDamage(int damage, bool lastChance=true)
    {
        damageSound.PlayFootstepSound(damage);
        if (health >= 20 && (health - damage) <= 0)
        {
            if(lastChance)
            health = 1; //Last chance 
        } 
        else
        {
            health -= damage;
        }
        Debug.Log("Health: " + health);
        playerUI.UpdatePlayerHealth(((float)health) / 100f);
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
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
            
        }
        else
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
        if (currentCooldown > 0 || mainFailed)
        {
            mainFailed = !mainFailed;
            return;
        }
        if (!primaryCharging)
        {
            PlayWeaponSound(mainWeaponChargeSound);
            primaryCharging = true;
            weaponVisuals.ChangeCharge(1);
            playerUI.IncreaseMainCharge();
            currentMainProjectile = Instantiate(primaryProjectile, visualMainSpawnPoint.position, Quaternion.identity);
            currentMainProjectile.GetComponent<MainProjectile>().damage = lvl0Damage;
            currentMainRb = currentMainProjectile.GetComponent<Rigidbody>();
            currentMainRb.isKinematic = true;
            currentMainProjectile.GetComponent<SphereCollider>().enabled = false;
            currentMainProjectile.layer = 11; //Weapon layer, for rendering properly
            currentMainProjectile.transform.parent = transform.parent;
        }
        else
        //if (currentCooldown <= 0)
        {
            Rigidbody rbProjectile = currentMainProjectile.GetComponent<Rigidbody>();
            int lvlCharge = 0;
            if (testCharge2)
            {
                lvlCharge = 2;
                currentMainProjectile.GetComponent<MainProjectile>().damage = lvl2Damage;

            }
            else if (testCharge1)
            {
                lvlCharge = 1;
                currentMainProjectile.GetComponent<MainProjectile>().damage = lvl1Damage;
            }
            rbProjectile.mass *= (lvlCharge + 1);
            PlayWeaponSound(mainWeaponReleaseSound);
            Debug.Log("Shot with charge = " + lvlCharge);
            //GameObject projectile = Instantiate(primaryProjectile, rangedSpawnPoint.position, Quaternion.identity, transform.parent);
            //Scale the projectile based on charge, might need no make this differently
            //projectile.transform.localScale *= (1 + lvlCharge);
            currentMainProjectile.layer = 13;
            //currentMainProjectile.transform.position = rangedSpawnPoint.position;
            currentMainRb.isKinematic = false;
            currentMainProjectile.GetComponent<SphereCollider>().enabled = true;
            currentMainProjectile.transform.parent = transform.parent;
            currentMainProjectile.transform.localScale *= 2;
            
            currentMainRb.velocity = cameraPos.forward * baseProjectileSpeed;
            rb.AddForce((-1f * baseProjectileSpeed * cameraPos.forward).normalized * weaponPush * (1 + lvlCharge), ForceMode.Impulse);
            currentCooldown = weaponCooldown;

            currentMainProjectile = null;
            currentMainRb = null;
            primaryCharging = false;
            weaponVisuals.ChangeCharge(0);
            playerUI.ResetMainCharge();
            chargeTimer = 0;
            testCharge1 = false;
            testCharge2 = false;
            if (secondaryFailed)
            {
                secondaryFailed = false;
                OnShootSecondary();
            }
        }
    }

    void PlayWeaponSound(AudioClip clip)
    {
        weaponAudioSource.clip = clip;
        weaponAudioSource.Play();
    }

    void OnSpawn()
    {
        transform.position = playerTempSpawnPoint.position;
    }

    void OnShootSecondary()
    {
        if (primaryCharging || secondaryFailed)
        {
            secondaryFailed = !secondaryFailed;
            return;
        }
        var currentWeaponScript = weapons[currentWeapon];
        if (!secondaryPressed)
        {
            //Do something while the button is being held
            currentWeaponScript.ShootHeld();
            //exception for pulse shot
            if (currentWeapon == 0)
            {
                currentPulseSwapDelay = pulseSwapDelay;
            }
            secondaryPressed = true;
        }
        else
        {
            currentWeaponScript.ShootReleased();
            //Do something when the button is released
            secondaryPressed = false;
        }
    }

    

    //May need to remove this if the player is getting launched, try without this first or set a different max speed
    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {

            Vector3 newVel = flatVel.normalized * speedDecay;
            rb.velocity -= new Vector3(newVel.x * Time.deltaTime, 0, newVel.z * Time.deltaTime);
        }
        if (Mathf.Abs(rb.velocity.y) > maxSpeedY)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sign(rb.velocity.y) * maxSpeedY, rb.velocity.z);
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void OnWeaponSwap()
    {
        //Messy interaction,
        if (!secondaryPressed && currentPulseSwapDelay <= 0f)
        {
            swapPressed = !swapPressed;
            //When it has been released and it was not pressed long enough to take out wheapon wheel
            if (!swapPressed && currentSwapTime < wheelThreshold)
            {
                weapons[currentWeapon].enabled = false;
                currentWeapon = (currentWeapon + 1) % weapons.Length;
                weapons[currentWeapon].enabled = true;
                weaponVisuals.ChangeWeapon(currentWeapon);
                playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
                currentSwapTime = 0;
            }
        }
    }

    public void WeaponReload(int weapon, int amount)
    {
        weapons[weapon].Reload(amount);
        //weaponVisuals.ChangeWeapon(currentWeapon);
        if (weapon == currentWeapon)
            playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
        //Reload UI?
    }

    public bool WeaponFull(int weapon)
    {
        return weapons[weapon].bulletsLeft >= weapons[weapon].magazineSize;
    }

    public void ReactOnDeathPlane()
    {
        MyTakeDamage(100000, false);
    }

    public bool TakeDamage(float d)
    {
        if(MyTakeDamage((int)d, true)){
            
            return true;
        }
        return false;
    }

    public bool Die()
    {
        Debug.Log("I died lol");
        SceneManager.LoadScene("Menu");
        return true;
    }
}
