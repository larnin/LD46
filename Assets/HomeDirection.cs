using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeDirection : MonoBehaviour
{
    [SerializeField] float m_border = 50;
    [SerializeField] float m_arrowOffset = 10;
    [SerializeField] float m_minDistance = 5;
    [SerializeField] float m_hideTimer = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_pos = Vector2.zero;
    RectTransform m_canvasTransform = null;

    float m_appearTime = 0;

    Image m_arrow;
    Image m_home;

    private void Awake()
    {
        m_subscriberList.Add(new Event<CameraMovedEvent>.Subscriber(OnCameraMove));
        m_subscriberList.Subscribe();

        var canvas = transform.GetComponentInParent<Canvas>();
        if(canvas != null)
            m_canvasTransform = canvas.GetComponent<RectTransform>();

        var renderers = GetComponentsInChildren<Image>();
        foreach(var r in renderers)
        {
            if (r.gameObject.name.Contains("Arrow"))
                m_arrow = r;
            else if (r.gameObject.name.Contains("Home"))
                m_home = r;
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        var dir = -m_pos;
        var angle = Mathf.Atan2(dir.y, dir.x);
        float dist = dir.magnitude;

        var bounds = m_canvasTransform.rect;
        var scale = m_canvasTransform.localScale;
        bounds.width *= scale.x;
        bounds.height *= scale.y;

        bounds.width -= 2 * m_border;
        bounds.height -= 2 * m_border;

        Vector2 rayEnd = dir / dist * (bounds.width + bounds.height);

        bool found = false;
        Vector2 target = Vector2.zero;
        found = MathEx.Intersect(Vector2.zero, rayEnd, new Vector2(bounds.width / 2, bounds.height / 2), new Vector2(-bounds.width / 2, bounds.height / 2), out target);
        if(!found)
            found = MathEx.Intersect(Vector2.zero, rayEnd, new Vector2(-bounds.width / 2, bounds.height / 2), new Vector2(-bounds.width / 2, -bounds.height / 2), out target);
        if (!found)
            found = MathEx.Intersect(Vector2.zero, rayEnd, new Vector2(-bounds.width / 2, -bounds.height / 2), new Vector2(bounds.width / 2, -bounds.height / 2), out target);
        if (!found)
            found = MathEx.Intersect(Vector2.zero, rayEnd, new Vector2(bounds.width / 2, -bounds.height / 2), new Vector2(bounds.width / 2, bounds.height / 2), out target);

        if(!found)
        {
            m_arrow.transform.position = Vector3.zero;
            m_home.transform.position = Vector3.zero;
            return;
        }

        m_home.transform.position = new Vector3(target.x, target.y, 0) + transform.position;
        var arrowPos = target + dir / dist * m_arrowOffset;
        m_arrow.transform.position = new Vector3(arrowPos.x, arrowPos.y, 0) + transform.position;
        m_arrow.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 90);

        if (dist > m_minDistance)
            m_appearTime += Time.deltaTime;
        else m_appearTime -= Time.deltaTime;
        if (m_appearTime < 0)
            m_appearTime = 0;
        if (m_appearTime > m_hideTimer)
            m_appearTime = m_hideTimer;

        var color = m_home.color;
        color.a = m_appearTime / m_hideTimer;
        m_home.color = color;
        color = m_arrow.color;
        color.a = m_appearTime / m_hideTimer;
        m_arrow.color = color;
    }

    void OnCameraMove(CameraMovedEvent e)
    {
        m_pos = new Vector2(e.x, e.y);
    }
}
