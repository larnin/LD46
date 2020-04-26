using UnityEngine;
using System.Collections;

public class ParralaxPlane : MonoBehaviour
{
    [SerializeField] float m_moveSpeed = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector3 m_originalPos = Vector3.zero;

    private void Awake()
    {
        m_subscriberList.Add(new Event<CameraMovedEvent>.Subscriber(OnCameraMove));
        m_subscriberList.Subscribe();

        m_originalPos = transform.position;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnCameraMove(CameraMovedEvent e)
    {
        transform.position = new Vector3(e.x * m_moveSpeed, e.y * m_moveSpeed, 0) + m_originalPos;
    }
}
