using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePathEnemy : Enemy
{
    [SerializeField] private float moveFrequency;
    private float moveTimeOffset = -1;

    private void Start()
    {
        
    }

    protected override void Move()
    {
        if (moveTimeOffset == -1)
        {
            moveTimeOffset = Random.Range(0f, 3.14f);
        }

        Vector3 actualSpeed = new Vector3(Mathf.Cos(Time.time * moveFrequency + moveTimeOffset) * _speed.x, _speed.y, _speed.z);
        Vector3 movementVector = actualSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + movementVector);
    }
}
