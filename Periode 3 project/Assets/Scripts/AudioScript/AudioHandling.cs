using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandling : MonoBehaviour
{
    public static AudioHandling Instance;
    [SerializeField] private AudioSource diceRoll;
    [SerializeField] private AudioSource pointSound;
    [SerializeField] private AudioSource pointLoseSound;
    [SerializeField] private AudioSource yourTurnSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private AudioSource spaceSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void DiceRoll()
    {
        diceRoll.Play();
    }

    public void PointSound()
    {
        pointSound.Play();
    }

    public void PointLoseSound()
    {
        pointLoseSound.Play();
    }

    public void YourTurnSound()
    {
        yourTurnSound.Play();
    }

    public void ExplosionSound()
    {
        explosionSound.Play();
    }

    public void SpaceSound()
    {
        spaceSound.Play();
    }

    
}
