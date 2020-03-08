using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource source;
    
    private void Awake() {
        source = GetComponent<AudioSource>();
    }
    public void PlayEffect(AudioClip clip){
        source.PlayOneShot(clip);
    }
}
