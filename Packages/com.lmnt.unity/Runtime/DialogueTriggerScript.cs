using LMNT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMNT {

public class DialogueTriggerScript : MonoBehaviour {
   // private Animator animator;
    private AudioSource audioSource;
	private LMNTSpeech speech;
    private bool triggered;

    void Start() {
        //animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        speech = GetComponent<LMNTSpeech>();
        StartCoroutine(speech.Prefetch());
        triggered = false;
    }
        bool sp;
    public void speak(string s)
        {
            speech.dialogue = s;
            sp = true;
        }
    void Update() {


        if (!audioSource.isPlaying) {
            triggered = false;
        }
        if (triggered) {
            return;
        }
            if (sp)
            {
                StartCoroutine(speech.Talk());
                sp = false;
            }
            if (Input.GetKey("k"))
            {
                speak("okay");
            }



            if (audioSource.isPlaying) {
            //animator.SetTrigger("Talk");
            triggered = true;
        }
    }
}

}
