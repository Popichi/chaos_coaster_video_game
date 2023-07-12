using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoType; //0 = pulse shot, 1 = singularity, 2 = grenade, 3 = Machine gun
    public Light boxHighlight;
    private MeshRenderer meshR;
    public int[] ammo;
    public Material[] ammoMaterials;
    public AmmoRespawn respawner;
    public int respawnerIndex;
    
    // 
    // Start is called before the first frame update
    void Start()
    {
        InitBox(ammoType);
    }

    public void InitBox(int type)
    {
        ammoType = type;
        meshR = gameObject.GetComponent<MeshRenderer>();
        List<Material> list = new List<Material>();
        list.Add(meshR.materials[0]);
        list.Add(ammoMaterials[type]);
        list.Add(meshR.materials[2]);
        meshR.SetMaterials(list);
        //meshR.materials[1] = ammoMaterials[ammoType];
        boxHighlight.color = ammoMaterials[type].color;
    }


    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (!player.WeaponFull(ammoType))
            {
                player.WeaponReload(ammoType, ammo[ammoType]);
                //Play a sound or something
                Destroy(this.gameObject);
                if (respawner != null)
                {
                    respawner.NotifyDestroyed(respawnerIndex);
                }
            }
        }
    }

    //In case it looks bad
    public void EnableLight()
    {
        boxHighlight.enabled = !boxHighlight.enabled;
    }
}
