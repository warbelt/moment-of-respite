using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private AudioSource _shotAudioSource;

    [SerializeField] private float _shootsPerSecond;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] public float _bulletSpeedBonus = 0;
    [SerializeField] public float _damageBonus = 0;
    [SerializeField] public float _shootSpeedBonus = 0;

    [SerializeField] private bool _shootStraight = true;

    private float _nextShoot;
    private int _ammo;
    private int Ammo
    {
        get { return _ammo;  }
        set
        {
            _ammo = value;
            onAmmoCountChange?.Invoke(Ammo, _maxAmmo);
        }
    }

    public event Action<int, int> onAmmoCountChange;
    public event Action onAmmoDepleted;

    private void Awake()
    {
        Ammo = _maxAmmo;
    }

    private void OnEnable()
    {
        UpdateNextShoot();
    }

    public void Shoot(Vector2 mousePos)
    {
        if (_nextShoot <= Time.time && Ammo > 0)
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
            projectileInstance.SetBonusDamage(projectileInstance.GetBonusDamage() + _damageBonus);

            _shotAudioSource.Play();
            Ammo -= 1;

            UpdateNextShoot();
        }

        if (Ammo <= 0)
        {
            onAmmoDepleted?.Invoke();
        }
    }

    private void UpdateNextShoot()
    {
        _nextShoot = Time.time + (1 / (_shootsPerSecond * (1 + _shootSpeedBonus / 100)));
    }

    public int GetAmmo()
    {
        return Ammo;
    }

    public void Recharge()
    {
        Ammo += 10;
    }

    public void ResetGame()
    {
        Ammo = _maxAmmo;
        UpdateNextShoot();
    }
}
