using System.Collections;
using UnityEngine;

class GameController : MonoBehaviour
{
    [SerializeField] bool _demoMode;

    private UpgradesController _upgradesController;
    private HUDController _hudController;
    private Animator _camerasAnimator;
    private WaveController _waveController;
    private WaveNumberDisplay _waveNumberDisplay;
    private PauseScreenController _pauseScreenController;
    private PlayerStatus _player;
    private bool isPaused;

    public bool IsDemoMode => SceneController.Instance.IsDemoMode || _demoMode;

    void Awake()
    {
        _upgradesController = FindFirstObjectByType<UpgradesController>();
        _hudController = FindFirstObjectByType<HUDController>();
        _waveController = FindFirstObjectByType<WaveController>();
        _player = FindFirstObjectByType<PlayerStatus>();
        _camerasAnimator = GetComponent<Animator>();
        _waveNumberDisplay = FindFirstObjectByType<WaveNumberDisplay>();
        _pauseScreenController = FindFirstObjectByType<PauseScreenController>();
    }

    void Start()
    {
        Time.timeScale = 1f; // Fix game frozen when reseting after pausing
        _hudController.Show();

        if (IsDemoMode)
        {
            _player.UnlockAbilitySlot(4);
            _upgradesController.Show(4);
        }
        else
        {
            OnFinishedUpgrading();
        }
    }

    public void OnWaveEnd() => StartCoroutine(HandleWaveEnd());

    private IEnumerator HandleWaveEnd()
    {
        yield return new WaitForSeconds(1f);
        _camerasAnimator.SetTrigger("End Wave");
        _player.IsControllable = false;
        if (IsDemoMode)
        {
            _upgradesController.Show(4);
        }
        else if (_waveController.CurrentWave % 5 == 0)
        {
            _player.UnlockAbilitySlot();
            _upgradesController.Show(2);
        }
        else    
        {
            _upgradesController.Show(1);
        }
    }

    public void OnFinishedUpgrading() => StartCoroutine(HandleWaveStart());

    private IEnumerator HandleWaveStart()
    {
        _player.transform.position = Vector3.zero;
        _camerasAnimator.SetTrigger("Start Wave");
        _player.IsControllable = true;
        yield return _waveNumberDisplay.DisplayWave(_waveController.CurrentWave + 1);
        _waveController.NextWave();
    }

    public void OnDeath(PlayerStatus player) =>
        StartCoroutine(HandlePlayerDeath());

    private IEnumerator HandlePlayerDeath()
    {
        _camerasAnimator.SetTrigger("PlayerDied");
        yield return _waveNumberDisplay.DisplayPlayerDead();
        yield return new WaitForSeconds(4f);
        SceneController.Instance.GoToMainMenu();
    }

    public void TogglePause()
    {
        // Show Pause Screen if game paused
        if (isPaused)
            UnPause();
        else
            Pause();
        isPaused = !isPaused;
    }

    private void Pause()
    {
        _pauseScreenController.Show();
        Time.timeScale = 0f;
    }

    private void UnPause()
    {
        _pauseScreenController.Hide();
        Time.timeScale = 1f;
    }


}