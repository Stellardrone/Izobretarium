using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeEffects : MonoBehaviour
{
    public ParticleSystem moneyParticles;
    public AudioClip successSound;
    public AudioClip failSound;

    public void PlaySuccessEffect()
    {
        // Звук монет
        AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);

        // Частицы денег
        if (moneyParticles != null)
            moneyParticles.Play();
    }

    public void PlayFailEffect()
    {
        AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);
    }
}
