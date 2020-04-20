using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    static Fade instance = null;

    [SerializeField] float m_transitionDuration = 0.5f;
    [SerializeField] float m_transitionDistance = 100;

    SubscriberList m_subscriberList = new SubscriberList();

    Transform m_plane;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<ShowLoadingScreenEvent>.Subscriber(OnFade));
        m_subscriberList.Subscribe();

        m_plane = transform.Find("Image");
        m_plane.localPosition = new Vector3(m_transitionDistance, 0, 0);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnFade(ShowLoadingScreenEvent e)
    {
        if (e.start)
        {
            m_plane.localPosition = new Vector3(m_transitionDistance, 0, 0);
            m_plane.DOLocalMoveX(0, m_transitionDuration);
        }
        else
        {
            m_plane.localPosition = new Vector3(0, 0, 0);
            m_plane.DOLocalMoveX(-m_transitionDistance, m_transitionDuration);
        }
    }
}
