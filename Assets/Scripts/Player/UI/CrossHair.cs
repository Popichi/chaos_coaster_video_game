using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CrossHairManager;
[RequireComponent(typeof(AnimationCurve))]
[RequireComponent(typeof(AudioClip))]
public class CrossHair : MonoBehaviour
{
    public AudioClip clip;
    public AnimationCurve alphaCurve;
    public RawImage crossHairImage;
    public float startAlpha;
    public float animationLength = 1;
    MySimpleTimer animationTimer;
    public string cName;
    // Start is called before the first frame update
    void Awake()
    {
        animationTimer = new MySimpleTimer();
        
        crossHairImage = GetComponentInChildren<RawImage>();
       
        startAlpha = crossHairImage.color.a;
    }
    public void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
    // Update is called once per frame
    void Update()
    {
        animationTimer.update(Time.deltaTime);
        updateTexture();
    }
    public void startAnimation(float damage = 0, bool killed = false)
    {
        animationTimer.start(animationLength);

    }
    void updateTexture()
    {
        // value between 0-animationLength
        float time01 = Mathf.Lerp( 0, animationLength, animationTimer.normalizedTime());
        //making between 0-1
        time01 /= animationLength;
        float value = Mathf.Clamp01(alphaCurve.Evaluate(time01));
     
        Color c = crossHairImage.color;
        c.a = value;
        crossHairImage.color = c;
    }
}
