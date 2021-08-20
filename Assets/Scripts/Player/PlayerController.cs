using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private GameObject _shield;
    [SerializeField] private PlayerWeapon _weapon;
    [SerializeField] private ParticleSystem _damageParticles;
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private SpriteRenderer _renderer;

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
    [SerializeField] private float _bonusBulletSpeed = 0;
    [SerializeField] private float _bonusDamage = 0;
    [SerializeField] private float _bonusShootSpeed = 0;

    private float BonusBulletSpeed { 
        get => _bonusBulletSpeed;
        set
        {
            _bonusBulletSpeed = value;
            _weapon._bulletSpeedBonus = _bonusBulletSpeed;
        }
    }
    private float BonusDamage
    {
        get => _bonusDamage;
        set
        {
            _bonusDamage = value;
            _weapon._damageBonus = _bonusDamage;
        }
    }
    private float BonusShootSpeed
    {
        get => _bonusShootSpeed;
        set
        {
            _bonusShootSpeed = value;
            _weapon._shootSpeedBonus = _bonusShootSpeed;
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
    private bool _isControllable;


    [Header("Movement Constraints")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    public event Action<float, float> onHealthChange;
    public event Action<float, float> onShieldChange;
    public event Action onShieldDepleted;
    public event Action onAmmoDepleted;
    public event Action onDeath;

    private Vector2 _moveDirection;
    private bool _isShooting;

    private void FixedUpdate()
    {
        if (_isAlive && _isControllable)
        {
            float newX = _rb.position.x + _moveDirection.normalized.x * (_moveSpeed+_bonusMoveSpeed) * Time.fixedDeltaTime;
            float newY = _rb.position.y + _moveDirection.normalized.y * (_moveSpeed+_bonusMoveSpeed) * Time.fixedDeltaTime;
            _rb.MovePosition(new Vector2(Mathf.Clamp(newX, minX, maxX), Mathf.Clamp(newY, minY, maxY)));
        }
    }

    private void Update()
    {
        if (_isShooting && _isAlive && _isControllable)
        {
            Shoot();
        }

        if (_shieldActive && _isAlive && _isControllable)
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
        _isControllable = true;
        _shieldActive = false;
        _isShooting = false;

        _bonusMaxHealth = 0;
        _bonusMoveSpeed = 0;
        _bonusMaxShieldCharge = 0;
        BonusBulletSpeed = 0;
        BonusDamage = 0;
        BonusShootSpeed = 0;

        _weapon.ResetGame();
        Health = _maxHealth;
        ShieldCharge = _maxShieldCharge;
    }

    private void Shoot()
    {
        _weapon.Shoot(Mouse.current.position.ReadValue());
    }

    IEnumerator ActivateShield()
    {
        _shieldActive = true;
        _shield.SetActive(true);

        float endTime = Time.time + _shieldDuration;

        while(ShieldCharge > 0 && Time.time < endTime)
        {
            yield return new WaitForEndOfFrame();
        }

        if (ShieldCharge == 0)
        {
            onShieldDepleted?.Invoke();
        }

        _shield.SetActive(false);
        _shieldActive = false;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
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
            if(_isAlive)
            {
                DisableControl();
                onDeath?.Invoke();
                _isAlive = false;
            }
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
                Health += bonus;
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
                ShieldCharge += bonus;
                break;
            case 3:
                bonus = 4;
                concept = "Missile Speed";
                BonusBulletSpeed += bonus;
                break;
            case 4:
                bonus = 1;
                concept = "Damage";
                BonusDamage += bonus;
                break;
            case 5:
                bonus = 20;
                concept = "% Shooting Speed";
                BonusShootSpeed += bonus;
                break;
            default: 
                return "";

        }

        return string.Format("+{0} {1}", bonus, concept);
    }

    public void ResetGame()
    {
        RestartPosition();
        InitializeState();
    }

    public void DisableControl()
    {
        _isControllable = false;
    }
    
    public void EnableControl()
    {
        _isControllable = true;
    }

    private void RestartPosition()
    {
        transform.position = _startPosition.position;
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        float endTime = Time.time + 2;

        while (Time.time < endTime)
        {
            _renderer.enabled = !_renderer.enabled;
            yield return new WaitForSeconds(0.3f);
        }

        _renderer.enabled = true;
    }
}