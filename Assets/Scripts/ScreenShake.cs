using UnityEngine;
using System.Collections;
using NRand;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float m_shakePowerMultiplier = 1;
    [SerializeField] float m_powerDissipation = 10;

    SubscriberList m_subscriberList = new SubscriberList();

    float m_framePower = 0;
    float m_remainingPower = 0;

    private void Awake()
    {
        m_subscriberList.Add(new Event<ShakeFrameEvent>.Subscriber(OnShake));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        if (m_framePower > m_remainingPower)
            m_remainingPower = m_framePower;

        var gen = new StaticRandomGenerator<DefaultRandomGenerator>();
        var distrib = new UniformVector2CircleDistribution(m_remainingPower * m_shakePowerMultiplier);

        var dir = distrib.Next(gen);

        transform.localPosition = dir;

        m_remainingPower -= m_powerDissipation * Time.deltaTime;
        if (m_remainingPower < 0)
            m_remainingPower = 0;

        m_framePower = 0;
    }

    void OnShake(ShakeFrameEvent e)
    {
        m_framePower += e.power;
    }
}
