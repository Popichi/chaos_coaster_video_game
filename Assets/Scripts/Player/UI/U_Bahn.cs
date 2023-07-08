using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class U_BahnEngineNoise : MonoBehaviour
{
    public float speed = 0f; // The speed of your U-Bahn
    public float maxSpeed = 100f; // The maximum speed of your U-Bahn
    private AudioSource audioSource;
    private float frequency1, frequency2;
    private float phase1, phase2;
    private const int SAMPLE_RATE = 44100;

    GetMovement getMovement;
    private void Start()
    {
        getMovement = FindAnyObjectByType<GetMovement>();

        audioSource = GetComponent<AudioSource>();
        AudioClip myClip = AudioClip.Create("Electric Motor Noise", SAMPLE_RATE * 2, 1, SAMPLE_RATE, true, OnAudioRead);
        audioSource.clip = myClip;
        audioSource.loop = true;
        audioSource.Play();
    }





    private void OnAudioRead(float[] data)
    {
        frequency1 = 50f + speed; // Update frequencies based on speed
        frequency2 = 100f + speed * 2;

        for (int i = 0; i < data.Length; i++)
        {
            phase1 = phase1 + (frequency1 / SAMPLE_RATE);
            phase2 = phase2 + (frequency2 / SAMPLE_RATE);

            float sine1 = Mathf.Sin(2 * Mathf.PI * phase1);
            float sine2 = Mathf.Sin(2 * Mathf.PI * phase2);

            data[i] = (sine1 + sine2) / 4f; // Combine two sine waves
        }
    }
    public void Update()
    {
        speed = getMovement.GetSpeed().magnitude;
    }
}
