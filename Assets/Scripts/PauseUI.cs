using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void OnBack()
    {
        Destroy(gameObject);
    }

    public void OnMainMenu()
    {
        SceneSystem.changeScene("MainMenu");
    }
}
