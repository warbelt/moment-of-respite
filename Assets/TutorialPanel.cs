using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TutorialPanel : MonoBehaviour
{
    public event Action onTutorialEnded;
    
    [Header("Tutorial panels")]
    [SerializeField] private GameObject _moveTutorial;
    [SerializeField] private GameObject _shootTutorial;
    [SerializeField] private GameObject _shieldTutorial;
    [SerializeField] private GameObject _respiteTutorial;
    [SerializeField] private GameObject _healTutorial;
    [SerializeField] private GameObject _shieldRechargeTutorial;
    [SerializeField] private GameObject _ammoReloadTutorial;
    [SerializeField] private GameObject _upgradeTutorial;
    [SerializeField] private GameObject _tutorialEndedPanel;


    [Header("UI references")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _shieldBar;
    [SerializeField] private Image _rechargeShieldBar;
    [SerializeField] private PowerLeverController _powerLeverController;
    [SerializeField] private BulletRechargeSlot _emptyBulletSlot;
    [SerializeField] private GameObject _filledBulletSlot;
    [SerializeField] private UpgradeGauge _upgradeGauge;
    [SerializeField] private TextMeshProUGUI _upgradeText;

    private bool _movedUp = false;
    private bool _movedDown = false;
    private bool _movedLeft = false;
    private bool _movedRight = false;

    private bool _shot = false;
    private bool _usedShield = false;

    private bool _respiteOKbuttonPushed = false;
    private bool _rechargedHealth = false;
    private bool _rechargedShield = false;
    private bool _rechargedAmmo = false;
    private bool _upgraded = false;
    private bool _tutorialEndButonPushed = false;


    public void ResetTutorial()
    {
        _movedUp = false;
        _movedDown = false;
        _movedLeft = false;
        _movedRight = false;
        _shot = false;
        _usedShield = false;
        _rechargedHealth = false;
        _rechargedShield = false;
        _rechargedAmmo = false;
        _upgraded = false;

        _moveTutorial.SetActive(false);
        _shootTutorial.SetActive(false);
        _shieldTutorial.SetActive(false);
        _healTutorial.SetActive(false);
        _shieldRechargeTutorial.SetActive(false);
        _ammoReloadTutorial.SetActive(false);
        _upgradeTutorial.SetActive(false);

        try {
            _powerLeverController.OnShieldGaugeFull -= OnShieldRecharged;
        } catch {}

        try {
            _emptyBulletSlot.onRechargeSlotFilled -= OnAmmoRecharge;
        } catch { }

        try {
            _upgradeGauge.OnGaugeFull -= OnUpgradeFull;
        } catch { }

        _powerLeverController.OnShieldGaugeFull += OnShieldRecharged;
        _emptyBulletSlot.onRechargeSlotFilled += OnAmmoRecharge;
        _upgradeGauge.OnGaugeFull += OnUpgradeFull;


    }

    private void OnEnable()
    {
        ResetTutorial();

        StartCoroutine(LaunchTutorialSequence());
    }

    private IEnumerator LaunchTutorialSequence()
    {
        // Movement tutorial
        _moveTutorial.SetActive(true);
        _movedUp = false;
        _movedDown = false;
        _movedLeft = false;
        _movedRight = false;
        yield return new WaitUntil(() => _movedUp && _movedDown && _movedLeft && _movedRight);
        yield return new WaitForSeconds(2);
        _moveTutorial.SetActive(false);
        
        // Shooting tutorial
        _shootTutorial.SetActive(true);
        _shot = false;
        yield return new WaitUntil(() => _shot);
        yield return new WaitForSeconds(2);
        _shootTutorial.SetActive(false);

        // Shield tutorial
        _shieldTutorial.SetActive(true);
        _shieldBar.fillAmount = 1;
        _usedShield = false;
        yield return new WaitUntil(() => _usedShield);
        float elapsed = 0, duration = 2, shieldSpent = 0.4f;
        while (elapsed < duration)
        {
            _shieldBar.fillAmount = 1 - elapsed / duration * shieldSpent;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _shieldBar.fillAmount = 1 - shieldSpent;
        _shieldTutorial.SetActive(false);

        // Respite tutorial
        _respiteTutorial.SetActive(true);
        _respiteOKbuttonPushed = false;
        yield return new WaitUntil(() => _respiteOKbuttonPushed);
        _respiteTutorial.SetActive(false);

        // Health tutorial
        _healTutorial.SetActive(true);
        _healthBar.fillAmount = 0.7f;
        _rechargedHealth = false;
        yield return new WaitUntil(() => _rechargedHealth);
        _healthBar.fillAmount = 0.8f;
        _rechargedHealth = false;
        yield return new WaitUntil(() => _rechargedHealth);
        _healthBar.fillAmount = 0.9f;
        _rechargedHealth = false;
        yield return new WaitUntil(() => _rechargedHealth);
        _healthBar.fillAmount = 1.0f;
        yield return new WaitForSeconds(2);
        _healTutorial.SetActive(false);

        // Shield Recharge Tutorial
        _shieldRechargeTutorial.SetActive(true);
        _rechargeShieldBar.fillAmount = 0.6f;
        _rechargedShield = false;
        yield return new WaitUntil(() => _rechargedShield);
        _rechargeShieldBar.fillAmount = 0.7f;
        _rechargedShield = false;
        yield return new WaitUntil(() => _rechargedShield);
        _rechargeShieldBar.fillAmount = 0.8f;
        _rechargedShield = false;
        yield return new WaitUntil(() => _rechargedShield);
        _rechargeShieldBar.fillAmount = 0.9f;
        _rechargedShield = false;
        yield return new WaitUntil(() => _rechargedShield);
        _rechargeShieldBar.fillAmount = 1.0f;

        yield return new WaitForSeconds(2);
        _shieldRechargeTutorial.SetActive(false);

        // Ammo ReloadTutorial
        _ammoReloadTutorial.SetActive(true);
        _emptyBulletSlot.gameObject.SetActive(true);
        _filledBulletSlot.gameObject.SetActive(false);
        _rechargedAmmo = false;
        yield return new WaitUntil(() => _rechargedAmmo);
        _emptyBulletSlot.gameObject.SetActive(false);
        _filledBulletSlot.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        _ammoReloadTutorial.SetActive(false);

        // Upgrades Tutorial
        _upgradeTutorial.SetActive(true);
        _upgraded = false;
        _upgradeText.text = "";
        yield return new WaitUntil(() => _upgraded);
        _upgradeText.text = "+2 SPEED";

        yield return new WaitForSeconds(2);
        _upgradeTutorial.SetActive(false);

        // Tutorial Ended
        _tutorialEndedPanel.SetActive(true);
        _tutorialEndButonPushed = false;
        yield return new WaitUntil(() => _tutorialEndButonPushed);
        _tutorialEndedPanel.SetActive(false);

        if (onTutorialEnded != null)
        {
            onTutorialEnded.Invoke();
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();

        _movedUp = _movedUp || (move.y > 0);
        _movedDown = _movedDown || (move.y < 0);
        _movedLeft = _movedLeft || (move.x < 0);
        _movedRight = _movedRight || (move.x > 0);
    }

    public void OnFire()
    {
        _shot = true;
    }

    public void OnShield()
    {
        _usedShield = true;
    }

    public void OnRespiteOKButonPushed()
    {
        _respiteOKbuttonPushed = true;
    }

    public void OnHealthPushed()
    {
        _rechargedHealth = true;
    }

    public void OnShieldRecharged()
    {
        _rechargedShield = true;
    }

    public void OnAmmoRecharge()
    {
        _rechargedAmmo = true;
    }

    public void OnUpgradeFull()
    {
        _upgraded = true;
    }

    public void OnTutorialEndButonPushed()
    {
        _tutorialEndButonPushed = true;
    }

    public void OnSkipButtonPushed()
    {
        if (onTutorialEnded != null)
        {
            onTutorialEnded.Invoke();
        }
    }
}
