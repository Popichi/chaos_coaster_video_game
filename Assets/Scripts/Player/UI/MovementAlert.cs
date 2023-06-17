using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MovementAlert : MonoBehaviour
{
    public Sprite roadSign;
    public float alertTime = 4f;
    public GameObject spritePlaceholder;
    private GameObject movementIndicatorFrame;
    private UnityEngine.UI.Image spritePlaceholderImage;
    private UnityEngine.AudioSource spritePlaceholderSound;

    // Start is called before the first frame update
    void Start()
    {
        Resources.Load(roadSign.name);
        if (spritePlaceholder == null)
        {
            spritePlaceholder = GameObject.Find("AlertSpritePlaceholder");            
        }
        movementIndicatorFrame = GameObject.Find("MovementIndicatorFrame");
        spritePlaceholderImage = spritePlaceholder.GetComponent<UnityEngine.UI.Image>();
        spritePlaceholderSound = spritePlaceholder.GetComponent<UnityEngine.AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Follower")
        {
            spritePlaceholderImage.sprite = roadSign;
            spritePlaceholderImage.enabled = true;
            movementIndicatorFrame.SetActive(false);
            StartCoroutine(WaitAndDisableAlert());
            spritePlaceholderSound.Play();
        }
    }

    private IEnumerator WaitAndDisableAlert()
    {
        yield return new WaitForSeconds(alertTime);
        spritePlaceholderImage.enabled = false;
        movementIndicatorFrame.SetActive(true);
    }
}
