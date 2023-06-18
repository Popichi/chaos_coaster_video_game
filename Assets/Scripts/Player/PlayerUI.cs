using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    public GameObject foreground;
    public GameObject background;
    public Image weaponIcon;
    public Sprite[] allIcons;
    public Sprite[] chargeEmpty;
    public Sprite[] chargeFull;
    public Slider healthBar;
    public TextMeshProUGUI ammo;
    public TextMeshProUGUI danger;
    public TextMeshProUGUI waves;
    private string currentAmmoText = "25";
    private string maxAmmoText = "25";
    private int currentCharge = 0;
    //public Slider WeaponCharge;
    public Image[] main; //Turn an image on and off with every level
    // Start is called before the first frame update
    public void Init()
    {
        foreground.SetActive(true);
        background.SetActive(true);
    }

    public void ChangeWeapon(int weaponIndex, int maxAmmo, int currentAmmo)
    {
        //Change Icon
        weaponIcon.sprite = allIcons[weaponIndex];
        maxAmmoText = maxAmmo.ToString();
        UpdateAmmo(currentAmmo);
    }

    public void UpdateAmmo(int currentAmmo)
    {
        currentAmmoText = currentAmmo.ToString();
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        ammo.text = currentAmmoText + "/" + maxAmmoText;
    }

    public void IncreaseMainCharge()
    {
        main[currentCharge].sprite = chargeFull[currentCharge];
        currentCharge++;

    }

    public void ResetMainCharge()
    {
        currentCharge = 0;

        for (int i = 0; i < main.Length; i++)
        {
            main[i].sprite = chargeEmpty[i];
        } 
    }

    
    //Assume healthRelative [0, 1], assume health is stored and managed elsewhere
    public void UpdatePlayerHealth(float healthRelative)
    {
        if (healthRelative <= 0.1f)
        {
            danger.enabled = true;
        } else
        {
            danger.enabled = false;
        }
        healthBar.value = healthRelative;
    }

    public void UpdateWaveCounter(int currentWave, int totalWaves)
    {
        waves.text = "Wave: " + currentWave + "/" + totalWaves;
    }

    public void SetTutorialText()
    {

    }
}
