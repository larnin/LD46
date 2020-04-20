using UnityEngine;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] string m_gameScene = "";

    static string m_deathReason = "";
    
    void Awake()
    {
        var text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = m_deathReason;
    }
    
    public void OnMenu()
    {
        SceneSystem.changeScene("MainMenu");
    }

    public void OnRetry()
    {
        SceneSystem.changeScene(m_gameScene);
    }

    public static void OnDeath(string reason)
    {
        m_deathReason = reason;
        SceneSystem.changeScene("Death");
    }
}
