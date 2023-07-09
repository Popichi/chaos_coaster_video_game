using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExoBrain : MonoBehaviour
{
    SpiderAgent s;
    BodyPartController c;
    // Start is called before the first frame update
    void Start()
    {
        s = GetComponentInParent<SpiderAgent>();
        c = GetComponentInParent<BodyPartController>();
    }
    public bool doExo = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            if(s.alive && s.state == EnemyState.playing)
            {
                if (doExo)
                {
                    c.SwitchModelToExo();
                }
                else
                {
                   
                   s.mainBody.transform.position += s.trainingGround.transform.up * 2.6f;
                   s.mainBody.transform.rotation = s.m_OrientationCube.transform.rotation;
                }
                
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
