using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    [SerializeField] private List<Transform> players;
    [SerializeField] private Animator blackScreenAnimator;
    [SerializeField] private Transform resultsScreen;
    [SerializeField] private GameObject uiOrder;
    [SerializeField] private GameObject menuScreen;

    private List<Transform> playerRankOrder = new List<Transform>();
    private Transform _cam;
    private Vector3 _originalCamPosition;
    private List<Vector3> originalPlayerPositions = new List<Vector3>();

    private void Awake()
    {
        _cam = Camera.main.transform;
        _originalCamPosition = _cam.position;
        foreach (Transform p in players)
        {
            originalPlayerPositions.Add(p.position);
        }
    }
    public IEnumerator EndGame()
    {
        _cam = Camera.main.transform;
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(AnnounceEndOfGame());
        yield return StartCoroutine(TeleportPlayersToStart());
        yield return StartCoroutine(WaitForAnnouncingWinner());
        yield return StartCoroutine(AnnounceWinner());
        yield return StartCoroutine(ShowResults());
        CalculateResults();
    }

    private IEnumerator AnnounceEndOfGame()
    {
        string endText = "AnnounceEnd";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == endText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));
    }

    private IEnumerator TeleportPlayersToStart()
    {
        blackScreenAnimator.SetTrigger("FadeInOut");
        yield return new WaitForSeconds(0.25f);
        
        for (int i = 0; i < players.Count; i++)
        {
            players[i].position = originalPlayerPositions[i];
        }

        _cam.position = _originalCamPosition;

    }

    private IEnumerator WaitForAnnouncingWinner()
    {
        string announceWinnerText = "AnnounceWinner";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == announceWinnerText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));

    }

    private IEnumerator AnnounceWinner()
    {
        yield return new WaitForSeconds(3);
        playerRankOrder = players
        .OrderByDescending(stats => stats.GetComponent<PlayerHandler>().yarnAmount)
        .ThenByDescending(stats => stats.GetComponent<PlayerHandler>().coinAmount)
        .ToList();

        _cam.position = playerRankOrder[0].GetChild(1).position;
        string winningPlayer = playerRankOrder[0].name;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(winningPlayer));
        yield return new WaitForSeconds(3);
        
    }

    private IEnumerator ShowResults()
    {
        blackScreenAnimator.SetTrigger("FadeInOut");
        yield return new WaitForSeconds(0.25f);
        resultsScreen.gameObject.SetActive(true);
    }

    private void CalculateResults()
    {
        GameObject newScreen = Instantiate(uiOrder, resultsScreen.transform);

        newScreen.SetActive(true);
        newScreen.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
        menuScreen.SetActive(true);
        players[0].GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
        players[0].GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(menuScreen);
    }
}
