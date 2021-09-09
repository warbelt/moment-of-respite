using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletGeneratorSettings", menuName = "Bullet Generator Settings")]
public class BulletGeneratorSettings : ScriptableObject
{
    public bool repeat = true;
    public float bulletsPerSecond = 3;
    public float coneAperture = 45;
    public int bulletsPerWave = 1;
    public float angularVelocityDegs = 0;
    public float delay = 0;
}
