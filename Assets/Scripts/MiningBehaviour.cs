using UnityEngine;
using System.Collections;

public class MiningBehaviour : MonoBehaviour
{
    string click = "Fire1";

    [SerializeField] float m_maxMiningDistance = 5;
    [SerializeField] float m_mouseMiningRadius = 1;
    [SerializeField] float m_miningDuration = 1;
    [SerializeField] LayerMask m_miningLayer;
    [SerializeField] int m_maxCargo = 5;

    int m_cargo = 0;
    float m_miningTime = 0;
    ResourceItem m_clickedResource = null;

    void Update()
    {
        if (Input.GetButtonDown(click))
            OnMouseClick();
        if (Input.GetButton(click))
            OnMouseHold();
    }

    void OnMouseClick()
    {
        var clicPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dist = (new Vector2(clicPos.x, clicPos.y) - new Vector2(transform.position.x, transform.position.y)).magnitude;

        if (dist > m_maxMiningDistance)
        {
            OnClickTooFar();
            return;
        }

        var colliders = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), m_mouseMiningRadius, m_miningLayer);

        foreach (var c in colliders)
        {
            var comp = c.GetComponent<ResourceItem>();
            if (comp != null)
            {
                StartMining(comp);
                continue;
            }

            if (c.GetComponent<StationInteract>() != null)
            {
                OnInteactStation();
                continue;
            }
        }
    }

    void OnMouseHold()
    {
        if (m_clickedResource == null)
            return;

        var clicPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var colliders = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), m_mouseMiningRadius, m_miningLayer);
        
        bool found = false;
        foreach(var c in colliders)
            if(c.gameObject == m_clickedResource.gameObject)
            {
                found = true;
                break;
            }

        if(!found)
        {
            OnMouseRelease();
            return;
        }

        m_miningTime += Time.deltaTime;
        if(m_miningTime >= m_miningDuration)
        {
            m_miningTime = 0;
            m_clickedResource.RemoveOneResource();
            m_cargo++;
            OnResourceMined();

            if (m_clickedResource.HaveResource())
            {
                if (m_cargo > m_maxCargo)
                {
                    OnCargoFull();
                    OnMouseRelease();
                }
            }
            else OnMouseRelease();
        }
    }

    void OnMouseRelease()
    {
        m_clickedResource = null;
    }

    void StartMining(ResourceItem asteroid)
    {
        if (m_cargo >= m_maxCargo)
            OnCargoFull();

        if (m_clickedResource != null)
            return;

        if (!asteroid.HaveResource())
            return;

        m_clickedResource = asteroid;
        m_miningTime = 0;
    }

    void OnCargoFull()
    {

    }

    void OnClickTooFar()
    {

    }

    void OnResourceMined()
    {

    }

    void OnInteactStation()
    {

    }
}
