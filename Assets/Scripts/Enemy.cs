using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private Vector3 _speed;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    private BulletGenerator[] _weapons;

    [SerializeField] private ParticleSystem _damageParticles;
    [SerializeField] private ParticleSystem _deathParticles;

    //State
    private float _health;
    private bool _alive;

    public event Action OnDeath;


    private void Awake()
    {
        _health = _maxHealth;
        _alive = true;
        _weapons = GetComponentsInChildren<BulletGenerator>();
    }

    void Update()
    {
        if (_alive)
        {
            Vector3 movementVector = _speed * Time.deltaTime;
            _rb.MovePosition(transform.position + movementVector);
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

    private void Die()
    {
        _alive = false;

        _collider.enabled = false;
        for (int weaponIndex = 0; weaponIndex < _weapons.Length; weaponIndex++)
        {
            _weapons[weaponIndex].enabled = false;
        }

        _deathParticles.Emit(50);
        OnDeath?.Invoke();
        Destroy(gameObject, 0.5f);
    }
}
