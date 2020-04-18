using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NRand;

public class AsteroidPopulator : MonoBehaviour
{
    [Serializable]
    class AsteroidData
    {
        public GameObject prefab = null;
        public float weight = 1;
        public float resourcePercent = 0;
        public int resourceNb = 1;
    }

    [SerializeField] List<AsteroidData> m_asteroidData = new List<AsteroidData>();
    [SerializeField] float m_minSpeed = 1;
    [SerializeField] float m_maxSpeed = 5;
    [SerializeField] int m_count = 50;
    [SerializeField] float m_size = 100;
    [SerializeField] float m_minRotSpeed = 1;
    [SerializeField] float m_maxRotSpeed = 15;

    List<GameObject> m_asteroids = new List<GameObject>();
    
    void Start()
    {
        GenerateInitialPopulation();
    }
    
    void Update()
    {
        DestroyFarAsteroids();

        if (m_asteroids.Count < m_count)
            Generate(m_count - m_asteroids.Count, true);
    }

    void GenerateInitialPopulation()
    {
        Generate(m_count, false);
    }

    void Generate(int nb, bool fromBorder)
    {
        var generator = new StaticRandomGenerator<DefaultRandomGenerator>();
        var dSpeed = new UniformFloatDistribution(m_minSpeed, m_maxSpeed);
        var dDir = new UniformFloatDistribution(0, 2 * Mathf.PI);
        List<float> weights = new List<float>();
        foreach (var a in m_asteroidData)
            weights.Add(a.weight);
        var dType = new DiscreteDistribution(weights);
        var dRot = new UniformFloatDistribution(m_minRotSpeed, m_maxRotSpeed);
        var dRotDir = new BernoulliDistribution();

        for (int i = 0; i < nb; i++)
        {
            int index = dType.Next(generator);

            var obj = Instantiate(m_asteroidData[index].prefab);
            obj.transform.parent = transform;

            var resource = obj.GetComponent<ResourceItem>();
            if (resource != null)
            {
                if (new BernoulliDistribution(m_asteroidData[index].resourcePercent).Next(generator))
                    resource.SetResourceNb(m_asteroidData[index].resourceNb);
                else resource.SetResourceNb(0);
            }

            if(fromBorder)
            {
                var pos = new UniformVector2CircleSurfaceDistribution(m_size).Next(generator);
                obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }
            else
            {
                var pos = new UniformVector2CircleDistribution(m_size).Next(generator);
                obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }
            
            var rigidbody = obj.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                var rot = dRot.Next(generator);
                if (dRotDir.Next(generator))
                    rot *= -1;
                rigidbody.angularVelocity = rot;

                var speed = dSpeed.Next(generator);
                var dir = dDir.Next(generator);
                rigidbody.velocity = new Vector2(Mathf.Cos(dir) * speed, Mathf.Sin(dir) * speed);
            }

            m_asteroids.Add(obj);
        }
    }

    void DestroyFarAsteroids()
    {
        float squaredDist = (m_size * 1.1f) * (m_size * 1.1f);

        for(int i = 0; i < m_asteroids.Count; i++)
        {
            int index = m_asteroids.Count - i - 1;

            Vector2 pos = m_asteroids[index].transform.localPosition;
            if(pos.sqrMagnitude > squaredDist)
            {
                Destroy(m_asteroids[index]);
                m_asteroids.RemoveAt(index);
            }
        }
    }
}
