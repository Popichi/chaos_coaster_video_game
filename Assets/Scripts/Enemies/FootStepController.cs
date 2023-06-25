using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FootstepSoundController : MonoBehaviour
{
    public AudioSource footstepAudioSource;
    public float minImpulse = 0;
    public float maxImpulse = 10;
    public float minVolume = 0.1f;
    public float maxVolume = 1;

    void Start()
    {
        footstepAudioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepSound(float impulse)
    {
        // Clamp the impulse between minImpulse and maxImpulse
        impulse = Mathf.Clamp(impulse, minImpulse, maxImpulse);

        // Map the impulse to a volume level between minVolume and maxVolume
        float volume = ((impulse - minImpulse) / (maxImpulse - minImpulse)) * (maxVolume - minVolume) + minVolume;

        // Set the volume
        footstepAudioSource.volume = volume;

        // Play the footstep sound
        footstepAudioSource.Play();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            PlayFootstepSound(collision.impulse.magnitude);
        }
       
    }
}