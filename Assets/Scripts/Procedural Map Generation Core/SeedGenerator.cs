using System;
using UnityEngine;

/// <summary>
/// Manages the generation and customization of seed values used for Unity's Random number generator.
/// </summary>
public class SeedGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The seed used for Unity's Random number generator")]
    private int seed;


    private static SeedGenerator instance;

    /// <summary>
    /// Singleton instance of the SeedGenerator that allows easy access to its functionality from other scripts.
    /// </summary>
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

    /// <summary>
    /// Ensures that only one instance of SeedGenerator exists throughout the game's lifetime,
    /// initializes the seed value on the first creation, and persists it across scene loads.
    /// </summary>
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

    /// <summary>
    /// Generates a new seed based on the current system time
    /// and initializes Unity's Random number generator with the generated seed.
    /// </summary>
    public void GenerateNewSeed()
    {
        // Get the current system time
        System.DateTime currentTime = System.DateTime.Now;

        // Format the time as HH:mm:ss:dd:MM:yyyy
        string formattedTime = currentTime.ToString("HHmmssddMMyyyy");
        Debug.Log("formattedTime: " + formattedTime);

        // Use the hash code of the formatted time as the seed
        seed = formattedTime.GetHashCode();

        // Initialize Unity's Random number generator with the generated seed
        UnityEngine.Random.InitState(seed);
#pragma warning disable CS0618 // Type or member is obsolete
        Debug.Log("UnityEngine.Random.seed = " + UnityEngine.Random.seed);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Retrieves the current seed used for Unity's Random number generator.
    /// </summary>
    /// <returns>The current seed.</returns>
    public int GetSeed()
    {
        return seed;
    }

    /// <summary>
    /// Sets a custom seed and initializes Unity's Random number generator with the specified seed.
    /// </summary>
    /// <param name="customSeed">The custom seed to set.</param>
    public void SetCustomSeed(int customSeed)
    {
        seed = customSeed;

        // Initialize Unity's Random number generator with the custom seed
        UnityEngine.Random.InitState(seed);
#pragma warning disable CS0618 // Type or member is obsolete
        Debug.Log("UnityEngine.Random.seed = " + UnityEngine.Random.seed);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}

