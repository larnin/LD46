using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;

public class StationBehaviour : MonoBehaviour
{
    class ConnectorData
    {
        public GameObject obj = null;
        public int moduleIndex = 0;

        public ConnectorData(GameObject _obj, int _moduleIndex)
        {
            obj = _obj;
            moduleIndex = _moduleIndex;
        }
    }

    [SerializeField] List<GameObject> m_modulePrefab = new List<GameObject>();
    [SerializeField] GameObject m_connectorPrefab = null;
    [SerializeField] float m_moduleSize = 1;
    [SerializeField] float m_connectorSize = 1;
    [SerializeField] float m_rotationSpeed = 10;

    SubscriberList m_subscriberList = new SubscriberList();

    List<GameObject> m_baseModulePositions = new List<GameObject>();
    List<GameObject> m_modules = new List<GameObject>();
    List<ConnectorData> m_connectors = new List<ConnectorData>();

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
            //remove the associed connector
            if(m_connectors.Count > 0 && m_connectors[m_connectors.Count - 1].moduleIndex == m_modules.Count - 1)
            {
                Destroy(m_connectors[m_connectors.Count - 1].obj);
                m_connectors.RemoveAt(m_connectors.Count - 1);
            }

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
        Vector2 pos;
        float rot;
        bool needConnector;

        GetModuleData(m_modules.Count, out parent, out pos, out rot, out needConnector);

        if(needConnector)
        {
            var connector = Instantiate(m_connectorPrefab);
            connector.transform.parent = parent.transform;
            connector.transform.localRotation = Quaternion.identity;
            connector.transform.localPosition = new Vector3(pos.x - m_moduleSize, pos.y, 0);
            connector.transform.localScale = new Vector3(1, 1, 1);

            m_connectors.Add(new ConnectorData(connector, m_modules.Count));
        }

        var index = new UniformIntDistribution(0, m_modulePrefab.Count).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
        
        var obj = Instantiate(m_modulePrefab[index]);
        obj.transform.parent = parent.transform;
        obj.transform.rotation = Quaternion.Euler(0, 0, rot);
        obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        obj.transform.localScale = new Vector3(1, 1, 1);

        m_modules.Add(obj);
    }

    void GetModuleData(int index, out GameObject outModule, out Vector2 outPos, out float outRot, out bool outNeedConnector)
    {
        outModule = m_baseModulePositions[index % m_baseModulePositions.Count];

        float baseRot = (index % m_baseModulePositions.Count == 0 ? 0 : 90) + transform.rotation.eulerAngles.z;

        int indexInBranch = index / 3;
        int connectorIndex = (int)Mathf.Sqrt(indexInBranch);

        if(connectorIndex * connectorIndex == indexInBranch)
        {
            outRot = baseRot + 90;
            outNeedConnector = connectorIndex != 0;

            outPos = new Vector2(connectorIndex * (m_moduleSize + m_connectorSize), 0);
        }
        else
        {
            outRot = baseRot;
            outNeedConnector = false;

            int moduleIndex = indexInBranch - connectorIndex * connectorIndex;

            outPos = new Vector2(connectorIndex * (m_moduleSize + m_connectorSize) - m_moduleSize, (moduleIndex % 2 == 0 ? 1 : -1) * m_moduleSize * ((1 + moduleIndex) / 2));
        }
    }
}
