using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float _shootsPerSecond;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] public float _bulletSpeedBonus = 0;

    [SerializeField] private bool _shootStraight = true;

    private float _nextShoot; 
    private int _ammo;

    public event Action<int, int> onAmmoCountChange;

    private void Awake()
    {
        _ammo = _maxAmmo;
    }

    private void OnEnable()
    {
        UpdateNextShoot();
    }

    public void Shoot(Vector2 mousePos)
    {
        if (_nextShoot <= Time.time)
        {
            Projectile projectileInstance = Instantiate(_projectilePrefab);

            Vector3 target = Camera.main.ScreenToWorldPoint(mousePos);
            target.z = 0;
            if (!_shootStraight)
            {
                projectileInstance.transform.up = target - transform.position;
            }
            projectileInstance.transform.position = transform.position;
            projectileInstance.SetSpeed(_bulletSpeed + _bulletSpeedBonus);

            _ammo -= 1;
            onAmmoCountChange?.Invoke(_ammo, _maxAmmo);

            UpdateNextShoot();
        }
    }

    private void UpdateNextShoot()
    {
        _nextShoot = Time.time + (1 / _shootsPerSecond);
    }

    public int GetAmmo()
    {
        return _ammo;
    }

    public void Recharge()
    {
        _ammo += 10;
        onAmmoCountChange?.Invoke(_ammo, _maxAmmo);
        print(_ammo);
    }
}
