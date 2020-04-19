using UnityEngine;
using System.Collections;

public class MiningBehaviour : MonoBehaviour
{
    string click = "Fire1";

    [SerializeField] float m_maxMiningDistance = 5;
    [SerializeField] float m_mouseMiningRadius = 1;
    [SerializeField] float m_miningDuration = 1;
    [SerializeField] float m_miningShake = 1;
    [SerializeField] float m_resourceShake = 5;
    [SerializeField] LayerMask m_miningLayer = 0;
    [SerializeField] int m_maxCargo = 5;
    [SerializeField] GameObject m_stationMenuPrefab;

    int m_cargo = 0;
    float m_miningTime = 0;
    ResourceItem m_clickedResource = null;

    Animator m_animator = null;

    SpriteRenderer m_laser = null;
    SpriteRenderer m_laserImpact = null;

    bool m_controleEnabled = true;

    SubscriberList m_subscriberList = new SubscriberList();

    static MiningBehaviour m_instance = null;
    static public MiningBehaviour instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;

        m_subscriberList.Add(new Event<EnableControlesEvent>.Subscriber(OnControlesEnabled));
        m_subscriberList.Subscribe();

        m_animator = GetComponent<Animator>();

        m_laser = transform.Find("Laser").GetComponent<SpriteRenderer>();
        m_laserImpact = transform.Find("LaserImpact").GetComponent<SpriteRenderer>();

        UpdateLaser(false);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        if (Input.GetButtonDown(click))
            OnMouseClick();
        if (Input.GetButton(click))
            OnMouseHold();
        if (Input.GetButtonUp(click))
            OnMouseRelease();
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

        Event<ShakeFrameEvent>.Broadcast(new ShakeFrameEvent(m_miningShake));

        if(m_miningTime >= m_miningDuration)
        {
            m_miningTime = 0;
            m_clickedResource.RemoveOneResource();
            m_cargo++;
            OnResourceMined();

            Event<ShakeFrameEvent>.Broadcast(new ShakeFrameEvent(m_resourceShake));

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

        UpdateLaser(true);
    }

    void OnMouseRelease()
    {
        m_clickedResource = null;

        m_animator.SetBool("Laser", false);

        UpdateLaser(false);
    }

    void StartMining(ResourceItem asteroid)
    {
        if (m_cargo >= m_maxCargo)
            OnCargoFull();

        if (m_clickedResource != null)
            return;

        if (!asteroid.HaveResource())
            return;

        m_animator.SetBool("Laser", true);

        UpdateLaser(true);

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
        Instantiate(m_stationMenuPrefab);
    }

    void UpdateLaser(bool show)
    {
        m_laser.gameObject.SetActive(show);
        m_laserImpact.gameObject.SetActive(show);

        if (!show)
            return;

        if (m_clickedResource == null)
        {
            UpdateLaser(false);
            return;
        }

        Vector2 pos = transform.position;
        Vector2 target = m_clickedResource.transform.position;
        var dir = target - pos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;

        float dist = dir.magnitude;

        m_laser.transform.rotation = Quaternion.Euler(0, 0, angle);
        m_laser.transform.position = new Vector3((target.x + pos.x) / 2.0f, (target.y + pos.y) / 2.0f, m_laser.transform.position.z);
        m_laser.size = new Vector2(m_laser.size.x, dist);

        m_laserImpact.transform.position = new Vector3(target.x, target.y, m_laserImpact.transform.position.z);
        m_laserImpact.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public int GetCargo()
    {
        return m_cargo;
    }

    public int GetMaxCargo()
    {
        return m_maxCargo;
    }

    public void EmptyCargo()
    {
        m_cargo = 0;
    }

    void OnControlesEnabled(EnableControlesEvent e)
    {
        m_controleEnabled = e.enabled;
    }
}
