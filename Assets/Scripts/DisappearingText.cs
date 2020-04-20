using UnityEngine;
using System.Collections;

using TMPro;

public class DisappearingText : MonoBehaviour
{
    [SerializeField] float m_lifeTime = 2;
    [SerializeField] float m_speed = 2;

    TextMeshPro m_text = null;
    float m_timer = 0;

    private void Awake()
    {
        m_text = GetComponentInChildren<TextMeshPro>();   
    }
    
    void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_lifeTime)
            Destroy(gameObject);
        else
        {
            var color = m_text.color;
            color.a = 1 - (m_timer / m_lifeTime);
            m_text.color = color;
        }

        var pos = transform.position;
        pos.y += m_speed * Time.deltaTime;
        transform.position = pos;
    }
}
