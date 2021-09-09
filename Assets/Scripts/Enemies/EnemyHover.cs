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
    [SerializeField]
    private float _retargetMinDistance;

    private Vector3 _targetPosition;

    private bool _targetReached;

    protected override void Move()
    {
        if (!_targetReached && Vector3.Distance(_targetPosition, transform.position) < 0.3)
        {
            _targetReached = true;
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
        float _newX, _newY;

        do
        {
            _newX = Random.Range(_minX, _maxX);
            _newY = Random.Range(_minY, _maxY);

            _targetPosition = new Vector3(_newX, _newY, 0f);
        } while (Vector3.Distance(_targetPosition, transform.position) < _retargetMinDistance);

        _targetReached = false;
    }

    private IEnumerator PrepareNewPosition()
    {
        yield return new WaitForSeconds(_retargetWaitTime);
        AcquireNewTargetPosition();
    }

    void OnDrawGizmosSelected()
    {
        Vector2[] corners =
        {
            new Vector2(_minX, _minY),
            new Vector2(_maxX, _minY),
            new Vector2(_maxX, _maxY),
            new Vector2(_minX, _maxY),
        };

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
    }
}
