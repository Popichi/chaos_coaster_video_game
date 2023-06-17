using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisuals : MonoBehaviour
{
    public Animator animator;
    public Material materialSphere;
    public Light lightSphere;

    private bool shootingSecondary;
    private int currentSecondary;
    //Maybe other stuff eventually
    // Start is called before the first frame update
    void Start()
    {
        currentSecondary = 0;
        lightSphere.color = Color.red;
        materialSphere.color = Color.red;
        //materialSphere.EnableKeyword("_EMISSION"); Does not work //Stick to color for now
        //materialSphere.SetColor("_EmissionColor", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Change color to one according to weapon
    //Notify animator
    public void ChangeWeapon(int newWeapon)
    {
        Color color = DetermineColor(newWeapon);
        lightSphere.color = color;
        materialSphere.color = color;
        animator.SetInteger("CurrentWeapon", newWeapon);

    }

    private Color DetermineColor(int index)
    {
        Color color = Color.white;
        if (index == 0)
            color = Color.red;
        else if (index == 1)
            color = Color.blue;
        else if (index == 2)
            color = Color.yellow;
        else if (index == 3)
            color = Color.cyan;

        return color;
    } 


    public void ChangeCharge(int charge)
    {
        animator.SetInteger("MainCharge", charge);
    }

    public void ChangeShooting()
    {
        shootingSecondary = !shootingSecondary;
        animator.SetBool("ShootingSecondary", shootingSecondary);
    }

    public void ChangeShooting(bool value)
    {
        shootingSecondary = value;
        animator.SetBool("ShootingSecondary", shootingSecondary);
    }
}
