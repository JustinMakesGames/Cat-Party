using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class YarnPlacement : MonoBehaviour
{
    public static YarnPlacement Instance;

    [SerializeField] private float cameraSpeed;
    [SerializeField] private Vector3 cameraSpaceOffset;
    [SerializeField] private GameObject yarnSpaceRenderer;
    [SerializeField] private List<Transform> spaceFolders;
    [SerializeField] private float endRotation;
    private SpaceHandler yarnSpace;
    private List<SpaceHandler> _spaceHandlers = new List<SpaceHandler>();
    private List<SpaceHandler> _yarnSpacesToGo = new List<SpaceHandler>();
    

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

        for (int s = 0; s < spaceFolders.Count; s++)
        {
            for (int i = 0; i < spaceFolders[s].childCount; i++)
            {
                if (spaceFolders[s].GetChild(i).GetComponent<SpaceHandler>().spaceKind == SpaceHandler.SpaceKind.YarnPlace)
                {
                    _spaceHandlers.Add(spaceFolders[s].GetChild(i).GetComponent<SpaceHandler>());
                    _yarnSpacesToGo.Add(spaceFolders[s].GetChild(i).GetComponent<SpaceHandler>());
                }

            }
        }
        
    }

    public IEnumerator StartShowingYarnPlacement()
    {
        ChangeSpaceToStar();
        yield return StartCoroutine(MoveCameraToNewYarnPlace());
        yield return StartCoroutine(ShowTheNewYarnPlace());
    }

    private void ChangeSpaceToStar()
    {
        int randomSpace = Random.Range(0, _yarnSpacesToGo.Count);
        yarnSpace = _yarnSpacesToGo[randomSpace];

        yarnSpace.transform.GetComponent<Renderer>().enabled = false;
        yarnSpace.SetSpaceAsYarnSpace();

        GameObject yarnClone = Instantiate(yarnSpaceRenderer, yarnSpace.transform.position, yarnSpace.transform.rotation);
        yarnClone.transform.parent = yarnSpace.transform;

        if (_yarnSpacesToGo.Count == 0)
        {
            for (int i = 0; i < _spaceHandlers.Count; i++)
            {
                _yarnSpacesToGo.Add(_spaceHandlers[i]);
            }
        }
        _yarnSpacesToGo.Remove(yarnSpace);

    }

    private IEnumerator MoveCameraToNewYarnPlace()
    {
        Camera.main.transform.parent = null;

        Vector3 endUpPosition = Camera.main.transform.position + cameraSpaceOffset;
        Quaternion lookRotation = Quaternion.Euler(endRotation, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
        while (Vector3.Distance(Camera.main.transform.position, endUpPosition) > 0.02f)
        {

            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endUpPosition, cameraSpeed * Time.deltaTime);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, lookRotation, cameraSpeed * Time.deltaTime);
            yield return null;
        }
        
        Vector3 endPosition = yarnSpace.transform.position + cameraSpaceOffset;
        while (Vector3.Distance(Camera.main.transform.position, endPosition) > 0.02f)
        {
            
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endPosition, cameraSpeed * Time.deltaTime);
            yield return null;
        }

    }

    private IEnumerator ShowTheNewYarnPlace()
    {
        string yarnText = "FindYarnText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == yarnText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));

        Camera.main.transform.rotation = Quaternion.Euler(25, 0, 0);

    }
}

