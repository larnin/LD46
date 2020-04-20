using UnityEngine;
using System.Collections;

public class ParralaxPlane : MonoBehaviour
{
    [SerializeField] float m_moveSpeed = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<CameraMovedEvent>.Subscriber(OnCameraMove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnCameraMove(CameraMovedEvent e)
    {
        float z = transform.position.z;

        transform.position = new Vector3(e.x * m_moveSpeed, e.y * m_moveSpeed, z);
    }
}
