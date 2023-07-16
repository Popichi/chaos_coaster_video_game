using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseSensitivityController : MonoBehaviour
{
    private Slider slider;
    private TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        slider = this.gameObject.GetComponent<Slider>();
        inputField = this.gameObject.GetComponentInChildren<TMP_InputField>();
        inputField.text = (int) slider.value +"";
    }

    public void SliderSensUpdate()
    {
        if(inputField != null)
        inputField.text = (int) slider.value + "";
    }

    public void InputSensUpdate()
    {
        int.TryParse(inputField.text, out int result);
        slider.value = result; // Set(result, true);
    }
}
