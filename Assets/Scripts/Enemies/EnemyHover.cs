using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHover : Enemy
{
    [Header("Boundaries")]
    [SerializeField]
    private float _minY;
    [SerializeField]
    private float _maxY;
    [SerializeField]
    private float _minX;
    [SerializeField]
    private float _maxX;

    [Header("Movement")]
    [SerializeField]
    [Range(0,1)]
    private float _lerpSpeed;
    [SerializeField]
    private float _retargetWaitTime;

    private Vector3 _targetPosition;

    protected override void Move()
    {
        if (Vector3.Distance(_targetPosition, transform.position) < 0.1)
        {
            StartCoroutine(PrepareNewPosition());
        }

        _rb.MovePosition(Vector3.Lerp(transform.position, _targetPosition, _lerpSpeed * Time.fixedDeltaTime));
    }

    protected override void Awake()
    {
        base.Awake();
        AcquireNewTargetPosition();
    }

    private void AcquireNewTargetPosition()
    {
        float _newX = Random.Range(_minX, _maxX);
        float _newY = Random.Range(_minY, _maxY);

        _targetPosition = new Vector3(_newX, _newY, 0f);
    }

    private IEnumerator PrepareNewPosition()
    {
        yield return new WaitForSeconds(_retargetWaitTime);
        AcquireNewTargetPosition();
    }
}
