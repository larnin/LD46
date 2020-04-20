using UnityEngine;
using System.Collections;

public class PauseOpen : MonoBehaviour
{
    [SerializeField] GameObject m_pausePrefab;

    GameObject m_pauseObj = null;

    string pauseKey = "Cancel";

    private void Update()
    {
        if (m_pauseObj != null)
            return;

        if (Input.GetButtonDown(pauseKey))
            m_pauseObj = Instantiate(m_pausePrefab);
    }
}
