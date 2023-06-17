using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class MovementAlert : MonoBehaviour
{
    public Sprite roadSign;
    public float alertTime = 2f;
    public GameObject spritePlaceholder;
    private UnityEngine.UI.Image spritePlaceholderImage;

    // Start is called before the first frame update
    void Start()
    {
        Resources.Load(roadSign.name);
        spritePlaceholder = GameObject.Find("AlertSpritePlaceholder");
        spritePlaceholderImage = spritePlaceholder.GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Follower")
        {
            spritePlaceholderImage.sprite = roadSign;
            spritePlaceholderImage.enabled = true;
            StartCoroutine(WaitAndDisableAlert());
        }
    }

    private IEnumerator WaitAndDisableAlert()
    {
        yield return new WaitForSeconds(alertTime);
        spritePlaceholderImage.enabled = false;
    }
}
