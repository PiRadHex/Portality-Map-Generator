using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2Int offset = new Vector2Int(0, 0);
    public float spawnRate = 0.5f;
    [Range(1, 5)]
    public int borderSize = 1;


    // The spacing between buildings in the grid
    public float spacing = 10f;


    // The number of rows and columns in the grid
    public int numRows = 5;
    public int numColumns = 5;



    // The list of building prefabs to randomly choose from
    public List<GameObject> buildingPrefabs;
    public GameObject groundTile;
    public List<GameObject> borderPrefabs;
    public GameObject BorderTile;

    private List<Vector3> occupiedPoints = new List<Vector3>();
    public GameObject truck;
    public GameObject elevator;
    private List<GameObject> buildingInstances = new List<GameObject>();


    void Awake()
    {
        GenerateGround();
        GenerateBorders();
        placeUniques();
        GenerateBuildings();

    }

    void placeUniques()
    {
        placeObject(elevator);
        placeObject(truck);
    }

    void placeObject(GameObject obj)
    {
        while (true)
        {
            int row = Random.Range(0, numRows);
            int column = Random.Range(0, numColumns);

            float xPos = row * spacing + offset.x;
            float zPos = column * spacing + offset.y;
            Vector3 position = new Vector3(xPos, 0.01f, zPos);

            if(!occupiedPoints.Contains(position) )
            {
                obj.transform.position = position;
                occupiedPoints.Add(position);
                break;
            }
        }
    }

    void GenerateBorders()
    {
        for (int row = - borderSize; row < numRows + borderSize; row++)
        {
            for (int column = -borderSize; column < numColumns + borderSize; column++)
            {
                if(row == -1 || row == numRows || column == -1 || column == numColumns)
                {
                    GameObject Instance = Instantiate(BorderTile, transform);

                    // Calculate the position for the building based on the row and column
                    float xPos = row * spacing + offset.x;
                    float zPos = column * spacing + offset.y;
                    Vector3 position = new Vector3(xPos, 0f, zPos);

                    // Set the position of the building instance
                    Instance.transform.position = position;
                }

                if(row < 0 || row >= numRows || column < 0 || column >= numColumns)
                {
                    float a = Random.Range(0f, 1f);
                    if (a > spawnRate) continue;

                    // Choose a random building prefab from the list
                    GameObject itemPrefab = borderPrefabs[Random.Range(0, borderPrefabs.Count)];

                    // Instantiate the building prefab


                    // Calculate the position for the building based on the row and column
                    float xPos = row * spacing + offset.x;
                    float zPos = column * spacing + offset.y;
                    Vector3 position = new Vector3(xPos, 0.01f, zPos);

                    float[] possibleRotations = { 0f, 90f, 180f, 270f };
                    float randomRotation = possibleRotations[Random.Range(0, possibleRotations.Length)];

                    GameObject Instance = Instantiate(itemPrefab, transform);
                    Instance.transform.position = position;
                    Instance.transform.rotation = Quaternion.Euler(new Vector3(0f, randomRotation, 0f));
                }
            }

        }
    
    }

    void GenerateGround()
    {
        for (int row = - borderSize; row < numRows + borderSize; row++)
        {
            for (int column = - borderSize; column < numColumns + borderSize; column++)
            {
                GameObject GroundInstance = Instantiate(groundTile, transform);

                // Calculate the position for the building based on the row and column
                float xPos = row * spacing + offset.x;
                float zPos = column * spacing + offset.y;
                Vector3 position = new Vector3(xPos, 0f, zPos);

                // Set the position of the building instance
                GroundInstance.transform.position = position;
            }

        }
    }

    void GenerateBuildings()
    {
        // Iterate over each row and column
        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numColumns; column++)
            {
                float a = Random.Range(0f, 1f);
                if (a > spawnRate) continue;

                // Choose a random building prefab from the list
                GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];

                // Instantiate the building prefab
                

                // Calculate the position for the building based on the row and column
                float xPos = row * spacing + offset.x;
                float zPos = column * spacing + offset.y;
                Vector3 position = new Vector3(xPos, 0.01f, zPos);

                float[] possibleRotations = { 0f, 90f, 180f, 270f };
                float randomRotation = possibleRotations[Random.Range(0, possibleRotations.Length)];


                // Set the position of the building instance
                if (!occupiedPoints.Contains(position))
                {
                    GameObject buildingInstance = Instantiate(buildingPrefab, transform);
                    buildingInstance.transform.position = position;
                    buildingInstance.transform.rotation = Quaternion.Euler(new Vector3(0f, randomRotation, 0f));
                    occupiedPoints.Add(position);
                    buildingInstances.Add(buildingInstance);
                }
                
            }
        }
    }



    public List<GameObject> GetBuildingInstances()
    {
        return buildingInstances;
    }


}