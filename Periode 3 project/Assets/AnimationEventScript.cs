using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{
    [SerializeField] private PlayerHandler playerHandler;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        _animator.SetTrigger("PlayIntroAnimation");
    }

    public void AnimationEvent()
    {
        gameObject.SetActive(false);
        playerHandler.CheckIfPlayerOrCPU();
    }
}
