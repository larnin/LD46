using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_speedPow = 1;
    [SerializeField] float m_minSpeed = 0;
    [SerializeField] float m_maxSpeed = 100;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_target;
    
    void Awake()
    {
        m_target = transform.position;

        m_subscriberList.Add(new Event<CameraTargetChangeEvent>.Subscriber(OnTargetMove));
        m_subscriberList.Add(new Event<InstantMoveCameraEvent>.Subscriber(OnInstantMove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void FixedUpdate()
    {
        var pos = transform.position;

        Vector2 dir = m_target - new Vector2(pos.x, pos.y);

        var dist = dir.magnitude;
        if (dist < 0.01f)
            return;

        float speed = m_speed * Mathf.Pow(dist, m_speedPow);
        if (speed < m_minSpeed)
            speed = m_minSpeed;
        if (speed > m_maxSpeed)
            speed = m_maxSpeed;

        float delta = speed * Time.deltaTime;
        if (delta > dist)
            delta = dist;

        dir = dir / dist * delta;

        pos.x += dir.x;
        pos.y += dir.y;

        transform.position = pos;

        Event<CameraMovedEvent>.Broadcast(new CameraMovedEvent(pos.x, pos.y));
    }

    void OnTargetMove(CameraTargetChangeEvent e)
    {
        m_target = new Vector2(e.x, e.y);
    }

    void OnInstantMove(InstantMoveCameraEvent e)
    {
        m_target = new Vector2(e.x, e.y);

        float z = transform.position.z;
        transform.position = new Vector3(m_target.x, m_target.y, z);

        Event<CameraMovedEvent>.Broadcast(new CameraMovedEvent(e.x, e.y));
    }
}
