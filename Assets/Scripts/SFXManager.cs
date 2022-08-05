using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class SFXManager : MonoBehaviour
{
    public AudioClip onBumperHit;
    public AudioClip[] onBumperActivated;
    public AudioClip onBumperDestroyed;

    public AudioSource audioSource; //un audio source solo puede reproducir un sonido
    private AudioClip previousAudioClip;
    private float previousACTimeStamp;

    private void OnEnable()
    {
        Bumper.OnBumperHit+= OnBumperHit;
        Bumper.OnBumperActivated+= OnBumperActivated;
        Bumper.OnBumperDestroyed+=OnBumperDestroyed;
    }

    private void OnBumperHit(Bumper bumper)
    {
        audioSource.pitch=Random.Range(0.9f,1.1f); //con el pitch hacemos un cambio de volumen a los sonidos para que parezcan diferentes
        PlaySFX(onBumperHit,0.5f);
    }
    private void OnBumperActivated(Bumper bumper)
    {
        int randomIndex=Random.Range(0, onBumperActivated.Length);
        PlaySFX(onBumperActivated[randomIndex], 0.5f);
    }
    private void OnBumperDestroyed(Bumper bumper)
    {
        PlaySFX(onBumperDestroyed);
    }

    private void OnDisable()
    {
        Bumper.OnBumperHit-=OnBumperHit;
        Bumper.OnBumperActivated-=OnBumperActivated;
        Bumper.OnBumperDestroyed-=OnBumperDestroyed;
    }
    void Start()
    {
        /*//Para cuando cambiamos al menu principal o para reiniciar la musica... pero para efectos no, porque se cortaria a medias si se produce otro
        audioSource.clip= onBumperActivated;
        audioSource.Play();*/
        
    }

    public void PlaySFX(AudioClip audioClip, float volume=1)
    {
        if (previousAudioClip==audioClip) //Para que 2 sonidos no puedan sonar en el mismo momento y se acople el sonido(se multiplicaria el volumen de ese sonido)
        {
            if(Time.time-previousACTimeStamp<0.05f)
            {
                return;
            }
        }
        previousAudioClip=audioClip;
        previousACTimeStamp=Time.time;

        audioSource.PlayOneShot(audioClip,volume);
    }
}
