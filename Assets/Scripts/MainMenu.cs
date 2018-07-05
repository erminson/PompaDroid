using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private Coroutine loadingCoroutine;

	void Start ()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.GetComponent<AudioSource>().Play();
	}

    public void GoToGame()
    {
        if (loadingCoroutine == null)
        {
            loadingCoroutine = StartCoroutine(LoadGameScene(2.0f));
        }
    }

    private IEnumerator LoadGameScene(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        GameManager.CurrenLevel = 0;
        SceneManager.LoadScene("Game");
    }
}
