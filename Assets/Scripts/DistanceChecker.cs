using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DistanceChecker : MonoBehaviour
{
    [SerializeField] float m_maxDistance = 90;
    [SerializeField] float m_deathDistance = 100;
    [SerializeField] float m_maxBorderPower = 0.5f;
    [SerializeField] float m_maxCenterPower = 0.5f;
    [SerializeField] float m_maxBorderDistance = 96;
    [SerializeField] float m_minCenterDistance = 94;
    [SerializeField] float m_shakePower = 5;
    [SerializeField] float m_warningPulseTime = 2;
    [SerializeField] AudioClip m_warningSound = null;

    List<Image> m_borderImages = new List<Image>();
    Image m_centerImage = null;
    Image m_waringImage = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_target = Vector2.zero;

    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            var img = obj.GetComponent<Image>();
            if (img == null)
                continue;
            if (obj.name.Contains("Border"))
                m_borderImages.Add(img);
            else if (obj.name.Contains("Center"))
                m_centerImage = img;
            else if (obj.name.Contains("Danger"))
                m_waringImage = img;
        }

        m_subscriberList.Add(new Event<CameraTargetChangeEvent>.Subscriber(OnTargetChange));
        m_subscriberList.Add(new Event<InstantMoveCameraEvent>.Subscriber(OnInstantTargetChange));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        float maxBorderNorm = (m_maxBorderDistance - m_maxDistance) / (m_deathDistance - m_maxDistance);
        float minCenterNorm = (m_minCenterDistance - m_maxDistance) / (m_deathDistance - m_maxDistance);

        float distance = m_target.magnitude;
        float distanceNorm = (distance - m_maxDistance) / (m_deathDistance - m_maxDistance);
        if (distanceNorm < 0)
            distanceNorm = 0;

        if(distanceNorm > 1)
        {
            Event<EnableControlesEvent>.Broadcast(new EnableControlesEvent(false));
            GameOverUI.OnDeath("YOU HAVE LOST THE CONTACT WITH THE STATION");
            distanceNorm = 1;
        }

        if (distanceNorm > 0)
            Event<ShakeFrameEvent>.Broadcast(new ShakeFrameEvent(m_shakePower * distanceNorm));

        float borderNorm = 0;
        float centerNorm = 0;

        if (distanceNorm > maxBorderNorm)
            borderNorm = 1;
        else borderNorm = distanceNorm / maxBorderNorm;
        if (borderNorm > 1)
            borderNorm = 1;

        if (distanceNorm < minCenterNorm)
            centerNorm = 0;
        else centerNorm = (distanceNorm - minCenterNorm) / (1 - minCenterNorm);
        if (centerNorm > 1)
            centerNorm = 1;

        foreach(var i in m_borderImages)
        {
            var color = i.color;
            color.a = borderNorm * m_maxBorderPower;
            i.color = color;
        }
        var centerColor = m_centerImage.color;
        centerColor.a = centerNorm * m_maxCenterPower;
        m_centerImage.color = centerColor;

        var warningColor = m_waringImage.color;
        if (distanceNorm > 0)
        {
            float cycle = Time.time / m_warningPulseTime;
            float oldCycle = (Time.time - Time.deltaTime) / m_warningPulseTime;
            if (Mathf.FloorToInt(cycle) != Mathf.FloorToInt(oldCycle) && SoundSystem.instance != null)
                SoundSystem.instance.PlaySound(m_warningSound, 0.5f);
            if (cycle - Mathf.Floor(cycle) < 0.5f)
                warningColor.a = 1;
            else warningColor.a = 0;
        }
        else warningColor.a = 0;
        m_waringImage.color = warningColor;
    }

    void OnTargetChange(CameraTargetChangeEvent e)
    {
        m_target = new Vector2(e.x, e.y);
    }

    void OnInstantTargetChange(InstantMoveCameraEvent e)
    {
        m_target = new Vector2(e.x, e.y);
    }
}
