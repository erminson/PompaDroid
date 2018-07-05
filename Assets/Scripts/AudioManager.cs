using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        Debug.Log("Awake");
        if (Instance == null)
        {
            Debug.Log("Awake - if");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Debug.Log("Awake - else");
            if (Instance != this)
            {
                Debug.Log("Awake - else - if");
                Destroy(gameObject);
            }
        }
    }
}
