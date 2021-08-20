using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] protected Vector3 _speed;
    [SerializeField] private int _pointsValue = 1;

    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    private BulletGenerator[] _weapons;

    [SerializeField] private ParticleSystem _damageParticles;
    [SerializeField] private ParticleSystem _deathParticles;

    [SerializeField] private AudioSource _audioSource;

    //State
    private float _health;
    private bool _alive;

    public event Action<Enemy> OnDeath;
    public event Action<Enemy> OnBoundaryExit;

    protected virtual void Awake()
    {
        _health = _maxHealth;
        _alive = true;
        _weapons = GetComponentsInChildren<BulletGenerator>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    void FixedUpdate()
    {
        if (_alive)
        {
            Move();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            Projectile hittingProjectile;
            if (collision.gameObject.TryGetComponent<Projectile>(out hittingProjectile))
            {
                Damage(hittingProjectile.GetDamage());
                Destroy(collision.gameObject);
            } 
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("GameBoundaries"))
        {
            OnBoundaryExit?.Invoke(this);
        }
    }

    private void Damage(float damage)
    {
        _health -= damage;
        _damageParticles.Emit(20);

        if (_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _alive = false;

        _collider.enabled = false;
        for (int weaponIndex = 0; weaponIndex < _weapons.Length; weaponIndex++)
        {
            _weapons[weaponIndex].enabled = false;
        }

        _deathParticles.Emit(50);
        _audioSource.Play();
        OnDeath?.Invoke(this);
    }

    public int GetPointsValue()
    {
        return _pointsValue;
    }

    protected virtual void Move()
    {
        Vector3 movementVector = _speed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + movementVector);
    }
}
