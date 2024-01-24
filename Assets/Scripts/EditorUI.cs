using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    }

    private void PrintPaths()
    {
        string pathString = "";
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

    public void UpdateUI()
    {
        int.TryParse(seedInput.text, out int seed);
        if (seedInput.text != "Enter Seed...") { SeedGenerator.Instance.SetCustomSeed(seed); }
        RefreshUI();
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
