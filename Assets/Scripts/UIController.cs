using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    // Configuration fields
    [SerializeField] private Gradient _ammoChargeGradient;

    [SerializeField] private float _uiDownStartYPosition;
    [SerializeField] private float _uiUpTimeSeconds;


    // Game references
    [SerializeField] PlayerController _player;
    PlayerWeapon _playerWeapon;
    [SerializeField] PowerLeverController _powerLeverController;

    // UI references
    [SerializeField] private Image _healthBar;
    [SerializeField] private Animator _healthBarAnimator;
    [SerializeField] private Image _shieldBar;
    [SerializeField] private RectTransform _respiteUI;
    [SerializeField] private GameObject[] _ammoSlots;
    [SerializeField] private GameObject _ammoRechargeContainer;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private UpgradeGauge _upgradeGauge;
    [SerializeField] private TextMeshProUGUI _upgradeText;
    [SerializeField] private TextMeshProUGUI _shieldDepletedText;
    [SerializeField] private TextMeshProUGUI _ammoDepletedText;
    [SerializeField] private TextMeshProUGUI _gameScoreText;
    [SerializeField] private Button _replayButton;
    [SerializeField] private TextMeshProUGUI _gameMaxScoreText;
    [SerializeField] private TextMeshProUGUI _mainMenuMaxScoreText;
    [SerializeField] private GameObject _replayMenuHighScore;

    private bool _shieldDepletedTextFlashing = false;
    private bool _ammoDepletedTextFlashing = false;

    // UI Events
    public event Action OnUpgradeGaugeFull;
    public event Action OnReplayButtonPushed;

    // Start is called before the first frame update
    void Start()
    {
        _playerWeapon = _player.GetComponent<PlayerWeapon>();
        
        _respiteUI.gameObject.SetActive(false);
        
        ResetGame();
        

        _ammoDepletedTextFlashing = false;
        _shieldDepletedTextFlashing = false;
        
        _player.onHealthChange += UpdateHealthBar;
        _player.onShieldChange += UpdateShieldBar;
        _player.onShieldDepleted += ShieldDepletedHandler;
        _playerWeapon.onAmmoDepleted += AmmoDepletedHandler;

        _playerWeapon.onAmmoCountChange += UpdateAmmo;

        BulletRecharge[] _bulletRecharges = _ammoRechargeContainer.GetComponentsInChildren<BulletRecharge>();
        foreach (BulletRecharge bulletRecharge in _bulletRecharges)
        {
            bulletRecharge.BeginDragProxyEvent += BulletRechargeBeginDragHandler;
            bulletRecharge.EndDragProxyEvent += BulletRechargeEndDragHandler;
        }

        foreach (GameObject ammoSlot in _ammoSlots)
        {
            BulletRechargeSlot slot = ammoSlot.GetComponentInChildren<BulletRechargeSlot>(true);
            slot.onRechargeSlotFilled += RechargeSlotFilledHandler;
        }

        _powerLeverController.OnShieldGaugeFull += ShieldGaugeFullHandler;
        _upgradeGauge.OnGaugeFull += UpgradeGaugeFullHandler;
        _upgradeText.text = "";

        SetTimer("");
    }

    private void ShieldDepletedHandler()
    {
        if (!_shieldDepletedTextFlashing)
        {
            StartCoroutine(FlashShieldText(3));
        }
    }
    
    private void AmmoDepletedHandler()
    {
        if (!_ammoDepletedTextFlashing)
        {
            StartCoroutine(FlashAmmoText(3));
        }
    }

    private IEnumerator FlashShieldText(float duration)
    {
        if (!_shieldDepletedTextFlashing)
        {
            _shieldDepletedTextFlashing = true;
        }

        float elapsed = 0;
        float interval = 0.5f;

        while(elapsed <= duration && _shieldDepletedTextFlashing)
        {
            _shieldDepletedText.enabled = !_shieldDepletedText.enabled;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        _shieldDepletedTextFlashing = false;
        _shieldDepletedText.enabled = false;
    }

    private IEnumerator FlashAmmoText(float duration)
    {
        if (!_ammoDepletedTextFlashing)
        {
            _ammoDepletedTextFlashing = true;
        }

        float elapsed = 0;
        float interval = 0.5f;

        while (elapsed <= duration && _ammoDepletedTextFlashing)
        {
            _ammoDepletedText.enabled = !_ammoDepletedText.enabled;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        _ammoDepletedTextFlashing = false;
        _ammoDepletedText.enabled = false;
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        float percentHealth = Mathf.Clamp01(health / maxHealth);
        _healthBar.fillAmount = percentHealth;
        _healthBarAnimator.SetBool("isInDanger", percentHealth < 0.3f);
    }

    public void UpdateShieldBar(float shield, float maxShield)
    {
        float percentShield = Mathf.Clamp01(shield / maxShield);
        _shieldBar.fillAmount = percentShield;
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        int activeSlots = Mathf.CeilToInt(maxAmmo / 10);
        int fullSlots = Mathf.CeilToInt(currentAmmo / 10);

        for (int slot = 0; slot < _ammoSlots.Length; slot++)
        {
            Image ammoChargeImage = _ammoSlots[slot].GetComponentsInChildren<Image>(true)[1];
            if (slot < fullSlots)
            {
                ammoChargeImage.fillAmount = 1;
                ammoChargeImage.color = _ammoChargeGradient.Evaluate(1);

            }
            else if (slot == fullSlots)
            {
                ammoChargeImage.fillAmount = (currentAmmo % 10) / 10f;
                ammoChargeImage.color = _ammoChargeGradient.Evaluate((currentAmmo % 10) / 10f);
            }
            else
            {
                ammoChargeImage.fillAmount = 0;
                ammoChargeImage.color = _ammoChargeGradient.Evaluate(0);
            }

            if (slot >= activeSlots)
            {
                _ammoSlots[slot].SetActive(false);
            }
        }
    }

    public void StartDown()
    {
        transform.position = new Vector3(transform.position.x, _uiDownStartYPosition, transform.position.z);
    }

    public IEnumerator ShiftUp()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -_uiDownStartYPosition, 0);

        while ((Time.time - startTime) < _uiUpTimeSeconds)
        {
            transform.position = Vector3.Slerp(startPosition, endPosition, (Time.time - startTime) / _uiUpTimeSeconds);
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;
    }

    public void BulletRechargeBeginDragHandler()
    {
        ShowEmptyAmmoSlots();
    }

    public void BulletRechargeEndDragHandler()
    {
        HideEmptyAmmoSlots();
    }

    private void ShowEmptyAmmoSlots()
    {
        int currentAmmo = _playerWeapon.GetAmmo();
        int fullAmmoSlots = Mathf.FloorToInt(currentAmmo / 10f);

        for (int slot = fullAmmoSlots; slot < _ammoSlots.Length; slot++)
        {
            Image[] slotImages = _ammoSlots[slot].GetComponentsInChildren<Image>(true);


            slotImages[0].gameObject.SetActive(false);
            slotImages[1].gameObject.SetActive(false);
            slotImages[2].gameObject.SetActive(true);
        }
    }

    private void HideEmptyAmmoSlots()
    {
        for (int slot = 0; slot < _ammoSlots.Length; slot++)
        {
            Image[] slotImages = _ammoSlots[slot].GetComponentsInChildren<Image>(true);

            slotImages[0].gameObject.SetActive(true);
            slotImages[1].gameObject.SetActive(true);
            slotImages[2].gameObject.SetActive(false);
        }
    }

    private void RechargeSlotFilledHandler()
    {
        _playerWeapon.Recharge();
        ShowEmptyAmmoSlots();
    }

    private void ShieldGaugeFullHandler()
    {
        _player.RechargeShield();
    }

    public IEnumerator ActivateRespiteUI(float activateDuration, float respiteDuration)
    {
        _respiteUI.gameObject.SetActive(true);
        _upgradeText.text = "";
        SetTimer(respiteDuration.ToString("0.0"));

        yield return StartCoroutine(EnterRespiteUI(activateDuration));
        StartCoroutine(UpdateRespiteTimer(respiteDuration));
    }

    private IEnumerator EnterRespiteUI(float duration)
    {
        float startTime = Time.time;

        _respiteUI.localPosition = new Vector3(0, 0, -450);
        while (Time.time - startTime < duration)
        {
            _respiteUI.localPosition = new Vector3(0, 0, Mathf.Lerp(-450, 0, Time.time-startTime));
            yield return new WaitForEndOfFrame();
        }
        _respiteUI.localPosition = new Vector3(0, 0, 0);
    }

    public void DisableRespiteUI()
    {
        StartCoroutine(ExitRespiteUI());
    }

    private IEnumerator ExitRespiteUI()
    {
        BulletRecharge[] recharges = _ammoRechargeContainer.GetComponentsInChildren<BulletRecharge>(true);
        foreach (BulletRecharge recharge in recharges)
        {
            recharge.Release();
        }

        float startTime = Time.time;

        _respiteUI.localPosition = new Vector3(0, 0, 0);
        while (Time.time - startTime < 1)
        {
            _respiteUI.localPosition = new Vector3(0, 0, Mathf.Lerp(0, -450, Time.time - startTime));
            yield return new WaitForEndOfFrame();
        }
        _respiteUI.localPosition = new Vector3(0, 0, -450);
        _respiteUI.gameObject.SetActive(false);
        SetTimer("");

    }

    public void HealthButtonPushed()
    {
        _player.Heal();
    }

    private IEnumerator UpdateRespiteTimer(float respiteDurationSeconds)
    {
        float startTime = Time.time;

        SetTimer(respiteDurationSeconds.ToString("0.0"));
        float lastDisplayedTick = respiteDurationSeconds;
        float remaining = respiteDurationSeconds;

        while (remaining > 0)
        {
            remaining = (startTime + respiteDurationSeconds) - Time.time;

            if (remaining.ToString("0.0") != lastDisplayedTick.ToString("0.0"))
            {
                SetTimer(remaining.ToString("0.0"));
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetTimer(string timerText)
    {
        _timer.text = timerText;
    }

    public void UpgradeGaugeFullHandler()
    {
        OnUpgradeGaugeFull?.Invoke();
    }

    public IEnumerator DisplayUpgradeText(string upgradeText)
    {
        _upgradeText.text = upgradeText;
        StartCoroutine(AnimateUpgradeText());
        yield return StartCoroutine(AnimateUpgradeText());
        _upgradeText.text = "";
    }

    private IEnumerator AnimateUpgradeText()
    {
        float endTime = Time.time + 0.6f;
        float lerpSpeed = 0.5f;
        float animationDistance = 20;

        _upgradeText.rectTransform.anchoredPosition = new Vector2(0, animationDistance);
        while (Time.time < endTime)
        {
            float newY = Mathf.Lerp(_upgradeText.rectTransform.anchoredPosition.y, 0, lerpSpeed);
            _upgradeText.rectTransform.anchoredPosition = new Vector2(0, newY);
            yield return new WaitForEndOfFrame();
        }

        _upgradeText.rectTransform.anchoredPosition = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
    }

    public void SetScoreText(int score)
    {
        _gameScoreText.text = score.ToString();
    }

    public void SetMaxScoreText(int score)
    {
        _gameMaxScoreText.text = score.ToString();
        _mainMenuMaxScoreText.text = score.ToString();
    }

    public void ActivateReplayMenu()
    {
        _replayButton.gameObject.SetActive(true);
        _replayMenuHighScore.gameObject.SetActive(true);
    }

    public void ReplayButtonPushed()
    {
        OnReplayButtonPushed?.Invoke();
    }

    public void ResetGame()
    {
        _replayButton.gameObject.SetActive(false);
        _replayMenuHighScore.gameObject.SetActive(false);
    }
}
