using UnityEngine;

public class BGMController : MonoBehaviour 
{ 
    private static BGMController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
           //DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Stop();
        }
    }

    /*public void GamePaused()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.volume = 0.25f;
        }
    }

    public void GameResumed()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.volume = 0.65f;
        }
    }*/
}