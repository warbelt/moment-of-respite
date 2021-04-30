using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Settings", menuName = "ProjectileSettings")]
public class ProjectileSettings : ScriptableObject
{
    public Sprite sprite;
    public float speed;
    public float damage;
}
