using System.Collections.Generic;
using System.Collections;
using UnityEngine;

class WaveController : MonoBehaviour
{
    [SerializeField] WavesConfig _config;

    private Spawner[] _spawners;
    private ObjectCache _objCache;
    private UpgradesController _upgradesController;
    private HUDController _hudController;
    private WaveNumberDisplay _waveNumberDisplay;
    private SceneController _sceneController;
    private Animator _camerasAnimator;
    private readonly HashSet<EnemyStatus> _enemies = new();
    private int _waveNumber = 1;
    private PlayerStatus _player;
    private bool isPaused;


    void Awake()
    {
        _camerasAnimator = GetComponent<Animator>();
        _spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        _objCache = FindFirstObjectByType<ObjectCache>();
        _player = FindFirstObjectByType<PlayerStatus>();
        _upgradesController = FindFirstObjectByType<UpgradesController>();
        _waveNumberDisplay = FindFirstObjectByType<WaveNumberDisplay>();
        _hudController = FindFirstObjectByType<HUDController>();
        _sceneController = FindFirstObjectByType<SceneController>();
    }

    void Start()
    {
        _hudController.Show();
        NextWave();
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

    // TODO: Also account for entities
    private void Pause()
    {
        _player.IsControllable = false;
        foreach (var enemy in _enemies)
        {
            enemy.IsControllable = false;
        }
    }

    private void UnPause()
    {
        _player.IsControllable = true;
        foreach (var enemy in _enemies)
        {
            enemy.IsControllable = true;
        }
    }

    public void NextWave()
    {
        if (_enemies.Count > 0)
            return;

        StartCoroutine(HandleWaveStart());
    }

    private void OnSpawn(EnemyStatus enemy)
    {
        if (_sceneController.IsDemoMode)
            enemy.AddEffect(new DemoEffect());
        _enemies.Add(enemy);
    }

    private int Split(int total, int n, int curr)
    {
        int amount = total / n;
        int overlap = total % n;
        return amount + ((curr < overlap) ? 1 : 0);
    }

    public void OnDeath(EnemyStatus enemy)
    {
        _enemies.Remove(enemy);
        _objCache.ReturnObject(enemy.Type, enemy);

        if (_enemies.Count == 0)
            StartCoroutine(HandleWaveEnd());
    }

    public void OnDeath(PlayerStatus player)
    {
        // Do something nice
        _sceneController.GoToMainMenu();
    }

    private IEnumerator HandleWaveStart()
    {
        _player.transform.position = Vector3.zero;
        _camerasAnimator.SetTrigger("Start Wave");

        yield return _waveNumberDisplay.DisplayWave(_waveNumber);
        _player.IsControllable = true;
    
        var (num_slimes, num_skeletons, num_orcs) = _config.NumEnemies(_waveNumber);

        for (int i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].Spawn(ObjectType.Orc, Split(num_orcs, _spawners.Length, i), OnSpawn);
            _spawners[i].Spawn(ObjectType.Skeleton, Split(num_skeletons, _spawners.Length, i), OnSpawn);
            _spawners[i].Spawn(ObjectType.Slime, Split(num_slimes, _spawners.Length, i), OnSpawn);
        }
    }

    private IEnumerator HandleWaveEnd()
    {
        _waveNumber += 1;

        if (_waveNumber % 5 == 0)
            _player.UnlockAbilitySlot();

        yield return new WaitForSeconds(1f);
        _upgradesController.Show(1); // FIXME: This should change based on the mode
        _player.IsControllable = false;
        _camerasAnimator.SetTrigger("End Wave");
    }
}