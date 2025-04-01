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

    [SerializeField] private List<TMP_Text> minigameTexts = new List<TMP_Text>();
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Dictionary<PlayerHandler, int> placeOrder = new Dictionary<PlayerHandler, int>();

    private Transform minigameCanvas;
    private GameObject minigamePanel;
    private Animator whiteScreenAnimator;
    private TMP_Text winnerText;
    private TMP_Text startText;
    private List<string> remainingChosenMinigames = new List<string>();

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

        for (int i = 0; i < allMinigameScenes.Count; i++)
        {
            remainingChosenMinigames.Add(allMinigameScenes[i]);
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
            BoardGameManager.Instance.HandleReturnToBoardGame(placeOrder);
        }
        
    }
    public IEnumerator HandleMinigameTime()
    {
        minigameText.SetActive(true);
        playerUI.SetActive(false);
        yield return new WaitForSeconds(1);
        minigameText.SetActive(false);
        minigameScreen.SetActive(true);

        StartCoroutine(PlayMinigameChooseAnimation());

    }

    private IEnumerator PlayMinigameChooseAnimation()
    {
        GameObject[] allImages = GameObject.FindGameObjectsWithTag("Arrow");

        List<string> minigameRouletteNames = GetMinigameRouletteNames();

        for (int i = 0; i < minigameTexts.Count; i++)
        {
            minigameTexts[i].text = minigameRouletteNames[i];
        }
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

        int randomChosenMinigame = Random.Range(0, 3);
        if (remainingChosenMinigames.Count < 3)
        {
            randomChosenMinigame = Random.Range(0, remainingChosenMinigames.Count);
        } 

        allImages[randomChosenMinigame].SetActive(true);

        yield return new WaitForSeconds(2);
        foreach (GameObject image in allImages)
        {
            print(image.transform.name);
            image.SetActive(true);
        }
        minigameScreen.SetActive(false);

        RemoveMinigameFromList(minigameRouletteNames[randomChosenMinigame]);
        StartCoroutine(SceneSwithToMinigame(minigameRouletteNames[randomChosenMinigame]));

    }

    private void RemoveMinigameFromList(string minigame)
    {
        remainingChosenMinigames.Remove(minigame);

        if (remainingChosenMinigames.Count == 0)
        {
            for (int i = 0; i < allMinigameScenes.Count; i++)
            {
                remainingChosenMinigames.Add(allMinigameScenes[i]);
            }
        }
    }
    private List<string> GetMinigameRouletteNames()
    {
        List<string> possibleList = new List<string>();
        
        if (remainingChosenMinigames.Count >= 3)
        {
            List<string> copyOfMinigameList = new List<string>(remainingChosenMinigames);
            for (int i = 0; i < 3; i++)
            {
                int randomMinigame = Random.Range(0, copyOfMinigameList.Count);
                possibleList.Add(copyOfMinigameList[randomMinigame]);

                copyOfMinigameList.RemoveAt(randomMinigame);
            }
            
        }

        else
        {
            int timesHappened = 0;
            int maximumTimes = 3;
            List<string> possibleMinigamesCopy = new List<string>(remainingChosenMinigames);
            List<string> allMinigamesCopy = new List<string>(allMinigameScenes);
            for (int i = 0; i < possibleMinigamesCopy.Count; i++)
            {
                int randomMinigame = Random.Range(0, possibleMinigamesCopy.Count);

                possibleList.Add(possibleMinigamesCopy[randomMinigame]);
                allMinigamesCopy.Remove(possibleMinigamesCopy[randomMinigame]);
                timesHappened++;
            }

            for (int i = timesHappened; i < maximumTimes; i++)
            {
                int randomMinigame = Random.Range(0, allMinigamesCopy.Count);

                possibleList.Add(allMinigamesCopy[randomMinigame]);
                allMinigamesCopy.RemoveAt(randomMinigame);
            }
        }

        return possibleList;
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

    public void ThrowPlayerInDictionary(PlayerHandler handler, int placement)
    {
        placeOrder.Add(handler, placement);
    }

    public void EndMinigame()
    {
        StartCoroutine(ShowPlayerWonText());
    }

    private IEnumerator ShowPlayerWonText()
    {
        PlayerHandler winningPlayer = null;

        foreach (KeyValuePair<PlayerHandler, int> keyValuePair in placeOrder)
        {
            if (keyValuePair.Value == 1)
            {
                winningPlayer = keyValuePair.Key;
            }
        }
        winnerText.gameObject.SetActive(true);
        winnerText.text = "FINISH!";

        yield return new WaitForSeconds(1);
        GetComponentInChildren<AudioSource>().Play();
        winnerText.text = $"{winningPlayer.name} WON!";
        yield return new WaitForSeconds(5);
        blackScreenAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2);
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("BoardGame");

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneSwitch;
    }
}
