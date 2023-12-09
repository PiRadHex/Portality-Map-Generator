using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaceRandomPrefabs : MonoBehaviour
{
    public List<GameObject> prefabsList = new List<GameObject>();

    private List<GameObject> prefabInstances = new List<GameObject>();

    private void Start()
    {
        //loadPrefabs();
    }

    void ShuffleList<T>(List<T> list)
    {
        // Fisher-Yates shuffle algorithm
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void PlacePrefabsAtPosotions(List<Transform> positionsList)
    {
        positionsList.Remove(positionsList[0]);
        ShuffleList(prefabsList);

        // Place the prefabs
        for (int i = 0; i < Mathf.Min(positionsList.Count, prefabsList.Count); i++)
        {
            GameObject prefab = prefabsList[i];
            Transform position = positionsList[i];

            GameObject prefabInstance = Instantiate(prefab, position.position, position.rotation);
            prefabInstance.transform.Rotate(0, 180, 0);
            prefabInstance.name = prefab.name + " - " + prefabInstance.GetInstanceID();

            prefabInstances.Add(prefabInstance);
        }
    }

    public void DestroyInstances()
    {
        for (int i = 0; i < prefabInstances.Count; i++)
        {
            Destroy(prefabInstances[i]);
        }
        prefabInstances.Clear();
        //Debug.Log("DestroyInstances() is done.");
        //Debug.Log(prefabInstances.Count);
    }

}