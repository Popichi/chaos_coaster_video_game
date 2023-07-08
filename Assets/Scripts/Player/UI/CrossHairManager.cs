using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairManager : MonoBehaviour
{

    public Dictionary<string, CrossHair> crossHairs;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        crossHairs = new Dictionary<string, CrossHair>();
        var d = GameObject.FindObjectsOfType<CrossHair>(true);
        foreach(var c in d)
        {
            crossHairs.Add(c.cName, c);
        }
        if(!audioSource)
        audioSource = GetComponentInChildren<AudioSource>();
    }
    public CrossHair GetCrossHairByName(string s)
    {
        if (crossHairs.ContainsKey(s))
        {
            return crossHairs[s];
        }
        else
        {
            Debug.LogError("Key:" + s+ " not found");
            return null;
        }
       
        
    }
    public class MySimpleTimer
    {
        public bool isActive;
        public float aimedTime;
        public float currentTime;
        public bool done;
        public void reset()
        {
            done = false;
            currentTime = 0;
            isActive = false;
        }
        public void start(float g)
        {
            reset();
            aimedTime = g;
            isActive = true;

        }
        public void update(float f)
        {
            if (isActive)
            {
                currentTime += f;
                if (currentTime >= aimedTime)
                {
                    done = true;
                    
                }
            }
           
           
        }
        public MySimpleTimer(float f)
        {
            start(f);
        }
        public MySimpleTimer()
        {
            reset();
        }
        public float normalizedTime()
        {

            
            if(aimedTime != 0)
            {
                return Mathf.Clamp01(currentTime / aimedTime);
            }
            else
            {
                return 0;
            }
            
        }
 
    }
    public void PlaySound(string s)
    { var a = crossHairs[s];
        audioSource.clip = a.clip;
        audioSource.Play();
    }
    // Update is called once per frame




    void Update()
    {

    }
}
