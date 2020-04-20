using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] string m_gameScene = "";

    private void Awake()
    {
        
    }

    public void OnStartClick()
    {
        SceneSystem.changeScene(m_gameScene);
    }

    public void OnHowToPlayClick()
    {

    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
