using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DissolveSphere : MonoBehaviour {

    List<Material> oldMat;
    List<MeshRenderer> renderer;
    public Material disMat; 
    private void Awake() {
        oldMat = new List<Material>();
        renderer = new List<MeshRenderer>();

        renderer = GetComponentsInChildren<MeshRenderer>().ToList();
        renderer.Add(GetComponent<MeshRenderer>());
        foreach(var a in renderer)
        {
            oldMat.Add(a.material);
        }
        
         
        
        
    }
    float timer = 0;
    bool dissolve;
    public void startDissolving()
    {
        dissolve = true;
        timer = 0;
        foreach (var a in renderer)
        {
           Material m = disMat;
           a.material = m;
        }


    }
    public void resetMaterials()
    {
        dissolve = false;
        int i = 0;
        foreach (var a in renderer)
        {
            a.material = oldMat[i++];
        }

    }
    public float timeToDissolve=3;
    private void Update() {
        timer += Time.deltaTime;
        if (dissolve)
        {
            disMat.SetFloat("_DissolveAmount", timer/timeToDissolve);
        }
           
    }
}