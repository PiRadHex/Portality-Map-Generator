using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    private static SeedGenerator instance;
    private int seed;

    public static SeedGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SeedGenerator>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<SeedGenerator>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateNewSeed();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetSeed()
    {
        return seed;
    }

    public void GenerateNewSeed()
    {
        // Get the current system time
        System.DateTime currentTime = System.DateTime.Now;

        // Format the time as hh:mm:ss:dd:mm:yyyy
        string formattedTime = currentTime.ToString("HHmmssddMMyyyy");

        // Convert the formatted time to an integer for use as the seed
        seed = int.Parse(formattedTime);
    }

    public void SetCustomSeed(int customSeed)
    {
        seed = customSeed;
    }
}
