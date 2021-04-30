using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{

    private Dictionary<Projectile, ProjectileSettings> _projectiles;
    [SerializeField] Projectile _baseProjectile;

    private void OnEnable()
    {
        _projectiles = new Dictionary<Projectile, ProjectileSettings>();
    }

    public  void InstanceCreateProjectile(ProjectileSettings projectileSettings)
    {
        Projectile projectileInstance = Instantiate(_baseProjectile);

        projectileInstance.SetSpeed(projectileSettings.speed);

        _projectiles[projectileInstance] = projectileSettings;
    }
}
