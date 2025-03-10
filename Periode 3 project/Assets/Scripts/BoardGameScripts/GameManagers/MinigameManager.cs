using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    public PlayerHandler winningPlayerHandler;
    public string previousScene;
    [SerializeField] private Transform players;
    [SerializeField] private Transform minigamePlayers;
    [SerializeField] private List<string> allMinigameScenes = new List<string>();
    [SerializeField] private GameObject minigameText;
    [SerializeField] private GameObject minigameScreen;
    [SerializeField] private Animator blackScreenAnimator;

    private Transform minigameCanvas;
    private GameObject minigamePanel;
    private Animator whiteScreenAnimator;
    private TMP_Text winnerText;
    private TMP_Text startText;

    private bool hasComeFromMinigame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        
        print("Subscribed");
    }

    private void Start()
    {
        SceneManager.sceneLoaded += HandleSceneSwitch;
    }

   

    private void HandleSceneSwitch(Scene scene, LoadSceneMode mode)
    {

        if (allMinigameScenes.Contains(SceneManager.GetActiveScene().name))
        {
            print($"hi noob in {transform.name}");
            minigamePlayers = GameObject.FindGameObjectWithTag("MinigamePlayerFolder").transform;
            blackScreenAnimator.SetTrigger("FadeOut");

            foreach (Transform playerHandler in minigamePlayers)
            {
                print("YEs");
                playerHandler.GetComponent<MinigamePlayerHandler>().SetMinigamePlayerOn();
            }

            minigameCanvas = GameObject.FindGameObjectWithTag("MinigameCanvas").transform;

            minigamePanel = minigameCanvas.GetChild(0).gameObject;
            whiteScreenAnimator = minigameCanvas.GetComponentInChildren<Animator>();
            winnerText = minigameCanvas.GetChild(2).GetComponent<TMP_Text>();
            startText = minigameCanvas.GetChild(3).GetComponent<TMP_Text>();

        }

        if (SceneManager.GetActiveScene().name == "BoardGame" && allMinigameScenes.Contains(previousScene))
        {
            blackScreenAnimator.SetTrigger("FadeOut");
            BoardGameManager.Instance.HandleReturnToBoardGame(winningPlayerHandler);
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
        foreach (GameObject image in allImages)
        {
            print(image.transform.name);
            image.SetActive(true);
        }
        minigameScreen.SetActive(false);
        StartCoroutine(SceneSwithToMinigame(allMinigameScenes[0]));

    }

    private IEnumerator SceneSwithToMinigame(string scene)
    {
        blackScreenAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(3);
        
        SceneManager.LoadScene(scene);
    }

    public IEnumerator StartMinigame()
    {
        for (int i = 0; i < players.childCount; i++)
        {
            minigamePlayers.GetChild(i).GetComponent<MinigamePlayerHandler>().isStartingMinigame = false;
            minigamePlayers.GetChild(i).GetComponent<MinigamePlayerHandler>().SetPlayerHandler(players.GetChild(i).GetComponent<PlayerHandler>());
        }

        whiteScreenAnimator.SetTrigger("FadeInOut");
        yield return new WaitForSeconds(0.2f);
        minigamePanel.SetActive(false);

        yield return new WaitForSeconds(1);
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        startText.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("MinigameManager").GetComponent<IMinigameManager>().BeginMinigame();
    }

    public void EndMinigame(PlayerHandler playerHandler)
    {
        winningPlayerHandler = playerHandler;
        StartCoroutine(ShowPlayerWonText(winningPlayerHandler));
    }

    private IEnumerator ShowPlayerWonText(PlayerHandler playerHandler)
    {
        
        winnerText.gameObject.SetActive(true);
        winnerText.text = "FINISH!";

        yield return new WaitForSeconds(1);
        winnerText.text = $"{playerHandler.name} WON!";
        yield return new WaitForSeconds(5);
        blackScreenAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2);
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("BoardGame");

    }
}
