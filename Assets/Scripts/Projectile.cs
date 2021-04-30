using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 1;
    [SerializeField] float _timeToLive = 5;
    [SerializeField] float _damage = 1;


    [SerializeField] Rigidbody2D _rb2D;

    private void Awake()
    {
        SetSpeed(_speed);
        Destroy(gameObject, _timeToLive);
    }

    public void SetSpeed(float speed)
    {
        _rb2D.velocity = transform.up * speed;
    }

    public float GetDamage()
    {
        return _damage;
    }
}
