using UnityEngine;
using System.Collections;

public class ShipControler : MonoBehaviour
{
    string horizontal = "Horizontal";
    string vertical = "Vertical";

    [SerializeField] float m_treshold = 0.1f;
    [SerializeField] float m_maxSpeed = 5;
    [SerializeField] float m_acceleration = 10;
    [SerializeField] float m_deceleration = 2;
    [SerializeField] float m_rotationSpeed = 360;

    Rigidbody2D m_rigidbody2D = null;
    
    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        var moveDir = m_rigidbody2D.velocity;
        float moveLenght = moveDir.magnitude;
        float moveAngle = m_rigidbody2D.rotation;
        if(moveLenght > 0.01f)
            moveAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

        var targetDir = new Vector2(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));
        float targetLenght = targetDir.magnitude;
        if (targetLenght < m_treshold)
            targetLenght = 0;
        if (targetLenght > 1)
            targetLenght = 1;
        float targetAngle = targetLenght > 0 ? Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg : 0;
        
        float newRotation = targetLenght > 0 ? UpdateRotation(moveAngle, targetAngle) : moveAngle;
        float newLenght = UpdateSpeed(moveLenght, targetLenght);

        m_rigidbody2D.SetRotation(newRotation);
        var speed = new Vector2(Mathf.Cos(newRotation * Mathf.Deg2Rad) * newLenght, Mathf.Sin(newRotation * Mathf.Deg2Rad) * newLenght);

        m_rigidbody2D.velocity = speed;
    }

    // angles in [-180;180]
    float UpdateRotation(float current, float target)
    {
        Debug.Log(current + " " + target);
        float delta = target - current;
        while (Mathf.Abs(delta) > 180)
            delta -= 360 * Mathf.Sign(delta);

        float nextRot = Mathf.Sign(delta) * Time.deltaTime * m_rotationSpeed;
        if (Mathf.Abs(nextRot) > Mathf.Abs(delta))
            nextRot = delta;
        current += nextRot;

        return current;
    }

    float UpdateSpeed(float current, float boost)
    {
        float targetSpeed = boost * m_maxSpeed;

        if(current > targetSpeed)
        {
            float delta = -m_deceleration * Time.deltaTime;
            float maxDelta = targetSpeed - current;
            if (delta < maxDelta)
                delta = maxDelta;

            return current + delta;
        }
        else
        {
            float delta = m_acceleration * Time.deltaTime;
            float maxDelta = targetSpeed - current;
            if (delta > maxDelta)
                delta = maxDelta;

            return current + delta;
        }
    }
}
