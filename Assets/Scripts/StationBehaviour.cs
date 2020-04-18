using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationBehaviour : MonoBehaviour
{
    [SerializeField] GameObject m_modulePrefab = null;
    [SerializeField] float m_moduleSize = 1;
    [SerializeField] float m_rotationSpeed = 10;

    SubscriberList m_subscriberList = new SubscriberList();

    List<GameObject> m_baseModulePositions = new List<GameObject>();
    List<GameObject> m_modules = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;

            if (obj.name.Contains("ModulePos"))
                m_baseModulePositions.Add(obj);
        }

        m_subscriberList.Add(new Event<UpdateStationModulesEvent>.Subscriber(OnModuleNbChange));
        m_subscriberList.Subscribe();



        Event<UpdateStationModulesEvent>.Broadcast(new UpdateStationModulesEvent(100));
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, m_rotationSpeed * Time.deltaTime));
    }

    void OnModuleNbChange(UpdateStationModulesEvent e)
    {
        if (e.moduleNb == m_modules.Count)
            return;

        while (e.moduleNb < m_modules.Count)
        {
            var obj = m_modules[m_modules.Count - 1];
            Destroy(obj);
            m_modules.RemoveAt(m_modules.Count - 1);
        }

        while (e.moduleNb > m_modules.Count)
            AddNewModule();
    }

    void AddNewModule()
    {
        GameObject parent = null;
        float distance = 0;

        GetParentModule(m_modules.Count, out parent, out distance);

        var obj = Instantiate(m_modulePrefab);
        obj.transform.parent = parent.transform;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localPosition = new Vector3(distance, 0, 0);
        obj.transform.localScale = new Vector3(1, 1, 1);

        m_modules.Add(obj);
    }

    void GetParentModule(int index, out GameObject outModule, out float outDistance)
    {
        outModule = m_baseModulePositions[index % m_baseModulePositions.Count];
        outDistance = (index / m_baseModulePositions.Count) * m_moduleSize;
    }
}
