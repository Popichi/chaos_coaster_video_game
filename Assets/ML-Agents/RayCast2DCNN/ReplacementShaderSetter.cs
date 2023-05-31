using UnityEngine;

public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader replacementShader;

    void Start()
    {
        if (replacementShader != null)
        {
            GetComponent<Camera>().SetReplacementShader(replacementShader, "");
        }
        else
        {
            Debug.LogError("No replacement shader assigned!");
        }
    }
}