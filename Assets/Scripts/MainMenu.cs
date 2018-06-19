using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	void Start () {
        Application.targetFrameRate = 60;
	}

    public void GoToGame() {
        GameManager.CurrenLevel = 0;
        SceneManager.LoadScene("Game");
    }
}
