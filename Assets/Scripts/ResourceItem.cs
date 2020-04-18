using UnityEngine;
using System.Collections;

public class ResourceItem : MonoBehaviour
{
    [SerializeField] int m_resourceNb = 0;

    GameObject m_resourceObject;
    GameObject m_emptyObject;

    private void Awake()
    {
        m_resourceObject = transform.Find("Resource").gameObject;
        m_emptyObject = transform.Find("Empty").gameObject;

        SetResourceNb(m_resourceNb);
    }

    public void SetResourceNb(int nb)
    {
        m_resourceNb = nb;
        if (m_resourceNb < 0)
            m_resourceNb = 0;

        m_resourceObject.SetActive(m_resourceNb > 0);
        m_emptyObject.SetActive(m_resourceNb <= 0);
    }

    public bool HaveResource()
    {
        return m_resourceNb > 0;
    }

    public void RemoveOneResource()
    {
        SetResourceNb(m_resourceNb - 1);
    }
}
