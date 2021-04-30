using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField] BulletGeneratorSettings _settings;
    [SerializeField] Projectile _bullet;

    [SerializeField] Transform _target;

    private float _nextShoot;

    void OnEnable()
    {
        UpdateNextShoot();
        _nextShoot += _settings.delay;
    }

    private void Update()
    {
        if (Time.time > _nextShoot)
        {
            GenerateWave();
        }

        if (_target != null)
        {
            transform.up = _target.position - transform.position;
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, _settings.angularVelocity * Time.deltaTime));
        }

    }

    private void UpdateNextShoot()
    {
        _nextShoot = Time.time + (1 / _settings.bulletsPerSecond);
    }

    private void GenerateWave()
    {
        for (int i = 0; i < _settings.bulletsPerWave; i++)
        {
            float angle = (-0.5f + i / (_settings.bulletsPerWave-1f)) * _settings.coneAperture;
            Projectile instancedBullet = Instantiate(_bullet, transform.position, transform.rotation);
            //Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + angle)
        }

        if (_settings.repeat)
        {
            UpdateNextShoot();
        }
    }
}
