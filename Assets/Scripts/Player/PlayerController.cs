using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public Slider SensX;
    public Slider SensY;
    private float xRotation;
    private float yRotation;

    [Header("Movement")]
    public float moveSpeed;
    public float moveSpeedHard;
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
    private bool playerFrozen;
    private bool introFinished;
    Vector3 flatVel;

    [Header("Health")]
    public int health;
    public float invicibilityDuration;
    public float pushbackHorizontal;
    public float pushbackVertical;

    public float currentInvincibilityDuration;
    public bool isInvincible;


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
    public AudioSource damageSound;
    public CinemachineImpulseSource cameraShakeImpulseSource;
    private void Awake()
    {
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
        isInvincible = false;
        currentInvincibilityDuration = 0f;
        playerFrozen = introSequenceEnabled;
        SensX.onValueChanged.AddListener((v) =>
        {
            ChangeSensitivityX(v);
        });
        SensX.value = StatManager.sensitivityX;
        SensY.onValueChanged.AddListener((v) =>
        {
            ChangeSensitivityY(v);
        });
        SensY.value = StatManager.sensitivityY;
        if (introSequenceEnabled)
        {
            introFinished = false;
            virtualCamera.Follow = cameraPointStart;
            virtualCamera.LookAt = cameraLookAtIntro;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Invoke(nameof(StartGame), introDuration);
        } else
        {
            introFinished = true;
            playerUI.Init();
            playerUI.UpdatePlayerHealth(1f);
            playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
            playerUI.ResetMainCharge();
        }

        sensitivityX = StatManager.sensitivityX;
        sensitivityY = StatManager.sensitivityY;
    }

    private void StartGame()
    {
        StartCoroutine(nameof(IntroFadeIn));
        
    }

    public void Freeze(bool status)
    {
        if (introFinished)
            playerFrozen = status;

        
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
        playerUI.Init();
        playerUI.UpdatePlayerHealth(1f);
        playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
        playerUI.ResetMainCharge();
        playerFrozen = false;
        introFinished = true;
    }

    public void ChangeSensitivityX(float x)
    {
        sensitivityX = x;
        StatManager.sensitivityX = x;
    }

    public void ChangeSensitivityY(float y)
    {
        sensitivityY = y;
        StatManager.sensitivityY = y;
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
        flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
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
    
    void reactToHit(Transform t, int damage, float forceH, float forceV)
    {
        TakeDamage(damage);
        Vector3 forceDir = t.forward;
        rb.AddForce(-transform.forward * forceH, ForceMode.Impulse);
        rb.AddForce(transform.up * forceV, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            var a = collision.gameObject.GetComponentInParent<SpiderAgent>();
            if (a && a.alive)
            {
                if(a.enemyType == EnemyType.SmallSpider)
                {
                    reactToHit(collision.transform, 25, pushbackHorizontal, pushbackVertical);
                }
                if (a.enemyType == EnemyType.Grunt)
                {
                    reactToHit(collision.transform, 1000, pushbackHorizontal*3, pushbackVertical*3);
                }

            }
            else
            {
                Debug.LogError("spider dead or no spider script for taking damage");
            }
           
        }
    }

    
    public bool MyTakeDamage(int damage, bool lastChance=true)
    {
        if (isInvincible)
        {
            return false;
        }
        isInvincible = true;
        if(damageSound != null)
        {
            damageSound.Play();
        }
        cameraShakeImpulseSource.GenerateImpulseWithForce(2f);
        if (health >= 20 && (health - damage) <= 0 && lastChance)
        {
            
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
        if (playerFrozen)
            return;
        Vector2 mouseInput = value.Get<Vector2>();
        float mouseY = mouseInput.x * sensitivityX * Time.fixedDeltaTime * 0.5f; //delta time?
        float mouseX = mouseInput.y * sensitivityY * Time.fixedDeltaTime * 0.5f;

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
        if (flatVel.magnitude <= moveSpeed)
        {
            if (grounded)
            {
                rb.AddForce(10f * moveSpeed * direction, ForceMode.Force);

            }
            else
            {
                rb.AddForce(10f * airMultiplier * moveSpeed * direction, ForceMode.Force);
            }
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
        if (playerFrozen)
            return;
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
            currentMainProjectile = Instantiate(primaryProjectile, visualMainSpawnPoint.position, Quaternion.identity, transform.parent);
            currentMainProjectile.GetComponent<MainProjectile>().damage = lvl0Damage;
            currentMainRb = currentMainProjectile.GetComponent<Rigidbody>();
            currentMainRb.isKinematic = true;
            currentMainProjectile.GetComponent<SphereCollider>().enabled = false;
            currentMainProjectile.layer = 11; //Weapon layer, for rendering properly
            //currentMainProjectile.transform.parent = transform.parent;
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
            currentMainProjectile.layer = 13;
            //currentMainProjectile.transform.position = rangedSpawnPoint.position;
            currentMainRb.isKinematic = false;
            currentMainProjectile.GetComponent<SphereCollider>().enabled = true;
            //currentMainProjectile.transform.parent = transform.parent;
            currentMainProjectile.transform.localScale *= 2;
            
            currentMainRb.velocity = cameraPos.forward * baseProjectileSpeed;
            rb.AddForce((1 + lvlCharge) * weaponPush * (-1f * baseProjectileSpeed * cameraPos.forward).normalized, ForceMode.Impulse);
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
        if (playerFrozen)
            return;
        weaponAudioSource.clip = clip;
        weaponAudioSource.Play();
    }

    void OnSpawn()
    {
        transform.position = playerTempSpawnPoint.position;
    }

    void OnShootSecondary()
    {
        if (playerFrozen)
            return;
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
        
        if (flatVel.magnitude > moveSpeedHard)
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
        WeaponSwap((currentWeapon + 1) % weapons.Length);
    }

    void OnWeaponSwapBackwards()
    {
        int newWeapon = (currentWeapon - 1) % weapons.Length;
        if (newWeapon < 0)
        {
            newWeapon += weapons.Length;
        }
        WeaponSwap(newWeapon);
    }

    void OnWeaponSwap0()
    {
        WeaponSwap(0);
    }

    void OnWeaponSwap1()
    {
        WeaponSwap(1);
    }

    void OnWeaponSwap2()
    {
        WeaponSwap(2);
    }

    void OnWeaponSwap3()
    {
        WeaponSwap(3);
    }

    void WeaponSwap(int newWeapon)
    {
        if (!secondaryPressed && currentPulseSwapDelay <= 0f)
        {

            weapons[currentWeapon].enabled = false;
            currentWeapon = newWeapon;
            weapons[currentWeapon].enabled = true;
            weaponVisuals.ChangeWeapon(currentWeapon);
            playerUI.ChangeWeapon(currentWeapon, weapons[currentWeapon].magazineSize, weapons[currentWeapon].bulletsLeft);
            currentSwapTime = 0;

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

    bool reacted;
    public void ReactOnDeathPlane()
    {
        if (!reacted)
        {
            reacted = true;
            Debug.Log("Damage taken");
            MyTakeDamage(100000, false);
        }

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
