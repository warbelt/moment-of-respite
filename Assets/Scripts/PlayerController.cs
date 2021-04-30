using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private GameObject _shield;
    [SerializeField] private PlayerWeapon _weapon;
    [SerializeField] private ParticleSystem _damageParticles;
    [SerializeField] private ParticleSystem _deathParticles;

    // Stats
    [Header("Stats")]
    [SerializeField] private float _maxHealth = 12;
    [SerializeField] private float _moveSpeed = 8;
    [SerializeField] private float _shieldDuration;
    [SerializeField] private float _maxShieldCharge = 100;
    [SerializeField] private float _shieldUsePerSecond = 20;
    [SerializeField] private float _healthRegain = 0.3f;
    [SerializeField] private float _shieldRegain = 8;

    [Header("Bonus Stats")]
    [SerializeField] private float _bonusMaxHealth = 0;
    [SerializeField] private float _bonusMoveSpeed = 0;
    [SerializeField] private float _bonusMaxShieldCharge = 0;
    [SerializeField] private float _bulletSpeedBonus = 0;
    [SerializeField] private float _bonusDamage = 0;
    [SerializeField] private float _bonusShootSpeed = 0;

    private float BulletSpeedBonus { 
        get => _bulletSpeedBonus;
        set
        {
            _bulletSpeedBonus = value;
            _weapon._bulletSpeedBonus = _bulletSpeedBonus;
        }
    }



    // State
    private float _health;
    private float Health
    {
        get
        {
            return _health;
        }

        set
        {
            _health = Mathf.Clamp(value, 0, _maxHealth + _bonusMaxHealth);
            onHealthChange?.Invoke(value, _maxHealth + _bonusMaxHealth);
        }
    }
    private float _shieldCharge;
    private float ShieldCharge
    {
        get
        {
            return _shieldCharge;
        }

        set
        {
            _shieldCharge = Mathf.Clamp(value, 0, _maxShieldCharge + _bonusMaxShieldCharge);
            onShieldChange?.Invoke(value, _maxShieldCharge +_bonusMaxShieldCharge);
        }
    }

    private bool _shieldActive;
    private bool _isAlive;

    [Header("Movement Constraints")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    public event Action<float, float> onHealthChange;
    public event Action<float, float> onShieldChange;

    private Vector2 _moveDirection;
    private bool _isShooting;

    private void FixedUpdate()
    {
        float newX = _rb.position.x + _moveDirection.normalized.x * (_moveSpeed+_bonusMoveSpeed) * Time.fixedDeltaTime;
        float newY = _rb.position.y + _moveDirection.normalized.y * (_moveSpeed+_bonusMoveSpeed) * Time.fixedDeltaTime;
        _rb.MovePosition(new Vector2(Mathf.Clamp(newX, minX, maxX), Mathf.Clamp(newY, minY, maxY)));
    }

    private void Update()
    {
        if (_isShooting)
        {
            Shoot();
        }

        if (_shieldActive)
        {
            ShieldCharge -= _shieldUsePerSecond * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        _shield.SetActive(false);
        _collider.enabled = true;

    }

    private void OnDisable()
    {
        _shield.SetActive(false);
        _isShooting = false;
        _shieldActive = false;

        _collider.enabled = false;

    }

    public void InitializeState()
    {
        _isAlive = true;
        _shieldActive = false;
        _isShooting = false;

        Health = _maxHealth;
        ShieldCharge = _maxShieldCharge;

        _bonusMaxHealth = 0;
        _bonusMoveSpeed = 0;
        _bonusMaxShieldCharge = 0;
        BulletSpeedBonus = 0;
        _bonusDamage = 0;
        _bonusShootSpeed = 0;
    }

    private void Shoot()
    {
        _weapon.Shoot(Mouse.current.position.ReadValue());
    }

    IEnumerator ActivateShield()
    {
        _shieldActive = true;
        _shield.SetActive(true);
        yield return new WaitForSeconds(_shieldDuration);
        _shield.SetActive(false);
        _shieldActive = false;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            if (!_shieldActive) { 
                Damage();
            }
            Destroy(collision.gameObject);
        }
    }

    // TODO: Create health property and call event on setter
    private void Damage()
    {
        Health -= 1;
        _damageParticles.Emit(20);

        if (Health <= 0)
        {
            _deathParticles.Emit(50);
            enabled = false;
        }
    }

    public void ShootHandler(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _isShooting = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            _isShooting = false;
        }
    }

    public void ShieldHandler(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            StartCoroutine(ActivateShield());
    }

    public void MoveHandler(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _moveDirection = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Canceled)
            _moveDirection = Vector2.zero;
    }

    public void RechargeShield()
    {
        ShieldCharge += _shieldRegain;
    }

    public void Heal()
    {
        Health += _healthRegain;
    }

    public string ApplyRandomUpgrade()
    {
        int upgrade = UnityEngine.Random.Range(0,5);
        string concept = "";
        float bonus = 0;

        switch (upgrade)
        {
            case 0:
                bonus = 3;
                concept = "Health";
                _bonusMaxHealth += bonus;
                // Invoke health UI update
                Health = Health;
                break;
            case 1:
                bonus = 1;
                concept = "Speed";
                _bonusMoveSpeed += bonus;
                break;
            case 2:
                bonus = 100;
                concept = "Shield";
                _bonusMaxShieldCharge += bonus;
                ShieldCharge = ShieldCharge;
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default: 
                return "";

        }

        return String.Format("+{0} {1}", bonus, concept);
    }
}