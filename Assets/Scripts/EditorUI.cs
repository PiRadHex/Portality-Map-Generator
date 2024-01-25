using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] TMP_InputField seedInput;
    [SerializeField] GameObject editorPanel;
    [SerializeField] CinemachineVirtualCamera VCam;

    [Header("Info")]
    [SerializeField] TextMeshProUGUI eteranlRoomsTextMesh;
    [SerializeField] string eteranlRoomsText = "Eternal Rooms: ";
    [SerializeField] TextMeshProUGUI randomRoomsTextMesh;
    [SerializeField] string randomRoomsText = "Random Rooms: ";
    [SerializeField] TextMeshProUGUI totalRoomsTextMesh;
    [SerializeField] string totalRoomsText = "Total: ";
    [SerializeField] TextMeshProUGUI portalPairsTextMesh;
    [SerializeField] string portalPairsText = "Portal Pairs: ";
    [SerializeField] TextMeshProUGUI portalsTextMesh;
    [SerializeField] string portalsText = "Portals: ";

    [Header("Path Info")]
    [SerializeField] GameObject pathPanel;
    [SerializeField] TextMeshProUGUI pathTextMesh;
    [SerializeField] TextMeshProUGUI connectionTextMesh;
    [SerializeField] ScrollRect pathScrollRect;
    [SerializeField] ScrollRect connectionScrollRect;

    [Header("Touch Screen")]
    [SerializeField] List<GameObject> joysticks = new List<GameObject>();

    public bool isTouchControl = false;

    private RoomGenerator roomGenerator;

    private void Start()
    {
        seedInput.text = SeedGenerator.Instance.GetSeed().ToString();
        editorPanel.SetActive(false);
        VCam.enabled = false;
        roomGenerator = FindObjectOfType<RoomGenerator>();
        ClosePathPanel();
    }

    public void RefreshUI()
    {
        seedInput.text = SeedGenerator.Instance.GetSeed().ToString();
        eteranlRoomsTextMesh.text = eteranlRoomsText + roomGenerator.GetNumOfEternalRooms().ToString();
        randomRoomsTextMesh.text = randomRoomsText + roomGenerator.GetNumOfRandomRooms().ToString();
        totalRoomsTextMesh.text = totalRoomsText + (roomGenerator.GetNumOfEternalRooms() + roomGenerator.GetNumOfRandomRooms()).ToString();

        portalPairsTextMesh.text = portalPairsText + roomGenerator.GetUsedPortalPairsCount().ToString();
        portalsTextMesh.text = portalsText + (roomGenerator.GetUsedPortalPairsCount() * 2).ToString();

        PrintPaths();
        PrintConnections();
    }

    private void PrintPaths()
    {
        string pathString = "\n";
        int pathIndex = 0;
        foreach (var path in roomGenerator.paths)
        {
            pathString += "path[" + pathIndex + "] = {";
            int[] sequence = path.GetSequence();
            sequence.Reverse();
            for (int i = 0; i + 1 < sequence.Length; i++)
            {
                pathString += sequence[i] + "}, {";
            }
            pathString += sequence.Last() + "}\n\n";
            pathIndex++;
        }
        pathTextMesh.text = pathString;
    }

    private void PrintConnections()
    {
        Dictionary<int, List<int>> adjacentRooms = new Dictionary<int, List<int>>();

        // Initialize adjacentRooms for the range of room indices
        int totalRooms = roomGenerator.GetNumOfEternalRooms() + roomGenerator.GetNumOfRandomRooms();
        for (int i = 0; i < totalRooms; i++)
        {
            adjacentRooms[i] = new List<int>();
        }

        // Build the adjacency list
        foreach (var path in roomGenerator.paths)
        {
            int[] sequence = path.GetSequence();
            for (int i = 0; i < sequence.Length - 1; i++)
            {
                if (!adjacentRooms[sequence[i + 1]].Contains(sequence[i]))
                {
                    adjacentRooms[sequence[i]].Add(sequence[i + 1]);
                    adjacentRooms[sequence[i + 1]].Add(sequence[i]);
                }
            }
        }

        // Convert the adjacency list to a string
        StringBuilder result = new StringBuilder();
        foreach (var room in adjacentRooms)
        {
            result.Append($"room[{room.Key}]: ");
            result.AppendJoin(", ", room.Value.Select(adjacentRoom => $"{{{adjacentRoom}}}"));
            result.AppendLine("\n");
        }

        connectionTextMesh.text = "\n" + result.ToString();
    }

    public void UpdateUI()
    {
        //int.TryParse(seedInput.text, out int seed);
        if (seedInput.text != "Enter Seed...") { SeedGenerator.Instance.SetCustomSeed(seedInput.text); }
        RefreshUI();

        Canvas.ForceUpdateCanvases();
        pathScrollRect.verticalNormalizedPosition = 1f;
        connectionScrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleEditMode();
        }

    }

    public void ToggleEditMode()
    {
        RefreshUI();
        ClosePathPanel();
        editorPanel.SetActive(!editorPanel.activeInHierarchy);
        VCam.enabled = editorPanel.activeInHierarchy;
        if (Application.platform != RuntimePlatform.Android)
        {
            Cursor.visible = editorPanel.activeInHierarchy;
            if (!isTouchControl) Cursor.lockState = editorPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
        }
        foreach(var joystick in joysticks)
        {
            joystick.SetActive(!editorPanel.activeInHierarchy);
        }
        Canvas.ForceUpdateCanvases();
        pathScrollRect.verticalNormalizedPosition = 1f;
        connectionScrollRect.verticalNormalizedPosition = 1f;
    }

    public void ToggleTouchControl()
    {
        isTouchControl = !isTouchControl;
        Cursor.lockState = isTouchControl ? CursorLockMode.None : (editorPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked);
    }

    public void ShowPathPanel()
    {
        pathPanel.SetActive(true);
    }

    public void ClosePathPanel()
    {
        pathPanel.SetActive(false);
    }

}
