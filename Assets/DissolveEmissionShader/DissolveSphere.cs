using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DissolveSphere : MonoBehaviour {

    List<Material> oldMat;
    List<Renderer> renderer;
    List<Material> disMats;
    public Material disMat; 
    private void Awake() {
        oldMat = new List<Material>();
        disMats = new List<Material>();
        renderer = new List<Renderer>();

        renderer.AddRange(transform.parent.GetComponentsInChildren<MeshRenderer>().ToList());
        //renderer.Add(GetComponent<MeshRenderer>());
        foreach(var a in renderer)
        {
            //oldMat.Add(a.gameObject.GetComponent<Material>());
            oldMat.Add(a.material);

            disMats.Add(disMat);
        }
        
         
        
        
    }
    float timer = 0;
    bool dissolve;
    public void startDissolving()
    {
        dissolve = true;
        timer = 0;
        int i = 0;
        foreach (var a in renderer)
        {
           
           a.material = disMats[i++];
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
            foreach(var a in disMats)
            {
                a.SetFloat("_DissolveAmount",Mathf.Clamp01(timer / timeToDissolve));
            }
           
        }
           
    }
}