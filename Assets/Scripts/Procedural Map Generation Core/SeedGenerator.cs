using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the generation and customization of seed values used for Unity's Random number generator.
/// </summary>
public class SeedGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The input/output used for seed generator")]
    private string rawSeed;

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

        // Generate a random string (you can adjust the length as needed)
        string randomString = GenerateRandomString(5);
        Debug.Log("randomString: " + randomString);

        // Concatenate the formatted time and random string
        rawSeed = formattedTime + randomString;

        // Use the hash code of the concatenated string as the seed
        seed = rawSeed.GetHashCode();

        // Initialize Unity's Random number generator with the generated seed
        UnityEngine.Random.InitState(seed);

        Debug.Log("Generated Seed = " + seed);
    }

    // Helper method to generate a random string of a specified length
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Retrieves the current seed used for Unity's Random number generator.
    /// </summary>
    /// <returns>The current seed.</returns>
    public string GetSeed()
    {
        return rawSeed;
    }

    /// <summary>
    /// Sets a custom seed and initializes Unity's Random number generator with the specified seed.
    /// </summary>
    /// <param name="customSeed">The custom seed to set.</param>
    public void SetCustomSeed(string customSeed)
    {
        rawSeed = customSeed;
        seed = rawSeed.GetHashCode();
        
        // Initialize Unity's Random number generator with the custom seed
        UnityEngine.Random.InitState(seed);

        Debug.Log("Custom Seed = " + seed);
    }
}

