using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHandling : MonoBehaviour
{
    public float countdownTime;
    [SerializeField] private GameObject explosion;
    private Color _color1 = Color.red;
    private Color _color2 = Color.black;

    private Renderer _bombRenderer;

    private void Start()
    {
        countdownTime = Random.Range(20f, 30f);
        _bombRenderer = GetComponent<Renderer>();
        StartCoroutine(FlickerBomb());
        Invoke(nameof(Explode), countdownTime);
    }

    private IEnumerator FlickerBomb()
    {
        float timeLeft = countdownTime;
        bool flickerState = false;

        while (timeLeft > 0)
        {
            float tickInterval = Mathf.Lerp(0.1f, 1f, timeLeft / countdownTime);

            _bombRenderer.material.color = flickerState ? _color1 : _color2;
            flickerState = !flickerState;

            yield return new WaitForSeconds(tickInterval);
            timeLeft -= tickInterval;
        }
    }

    private void Explode()
    {
        AudioHandling.Instance.ExplosionSound();
        Instantiate(explosion, transform.position, Quaternion.identity);
        HotPotatoManager.Instance.KillPlayer(transform.parent);
        Destroy(transform.parent.gameObject);
    }
}
