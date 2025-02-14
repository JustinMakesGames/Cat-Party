using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;
    [SerializeField] private List<string> allMinigameScenes = new List<string>();
    [SerializeField] private GameObject minigameText;
    [SerializeField] private GameObject minigameScreen;
    [SerializeField] private Animator blackScreenAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public IEnumerator HandleMinigameTime()
    {
        minigameText.SetActive(true);
        yield return new WaitForSeconds(1);
        minigameText.SetActive(false);
        minigameScreen.SetActive(true);

        StartCoroutine(PlayMinigameChooseAnimation());

    }

    private IEnumerator PlayMinigameChooseAnimation()
    {
        GameObject[] allImages = GameObject.FindGameObjectsWithTag("Arrow");

        foreach (GameObject image in allImages)
        {
            print(image.transform.name);
            image.SetActive(false);
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < allImages.Length; j++)
            {
                allImages[j].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                allImages[j].gameObject.SetActive(false);
            }
        }

        int randomChosenMinigame = Random.Range(0, allImages.Length);

        allImages[randomChosenMinigame].SetActive(true);

        yield return new WaitForSeconds(2);

        StartCoroutine(PlayThisMinigame(allMinigameScenes[0]));

    }

    private IEnumerator PlayThisMinigame(string scene)
    {
        blackScreenAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
    }
}
