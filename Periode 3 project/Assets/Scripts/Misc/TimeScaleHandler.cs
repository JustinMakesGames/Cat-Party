using UnityEngine;

public class TimeScaleHandler : MonoBehaviour
{
    [SerializeField] private float timeScale;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
