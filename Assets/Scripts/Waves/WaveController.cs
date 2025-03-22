using System.Collections.Generic;
using UnityEngine;

class WaveController : MonoBehaviour
{
    private Spawner[] _spawners;
    private ObjectCache _objCache;
    private UpgradesController _upgradesController;
    private readonly HashSet<EnemyStatus> _enemies = new();
    private int _waveCount;
    private PlayerStatus _player;
    private bool isPaused;


    void Awake()
    {
        _spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        _objCache = FindFirstObjectByType<ObjectCache>();
        _upgradesController = FindFirstObjectByType<UpgradesController>(FindObjectsInactive.Include);
        _player = FindFirstObjectByType<PlayerStatus>();
    }

    void Start()
    {
        _player.AddUpgrade(Upgrade.ShieldAbility);
        NextWave();
    }

    public void TogglePause()
    {
        // Show Pause Screen if game paused
        if (isPaused)
        {
            isPaused = false;
            UnPause();
        }
        else
        {
            isPaused = true;
            Pause();
        }
    }

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

        _player.IsControllable = true;
        foreach (var spawner in _spawners)
        {
            // TODO: Give spawner a list of enemies to spawn instead of doing this
            for (int i = 0; i < (_waveCount + 1) * 2; i++)
            {
                Debug.Log($"Spawning Enemy for Wave {_waveCount + 1}");
                var enemy = spawner.Spawn(ObjectType.Slime);
                _enemies.Add(enemy);
            }
        }
    }

    public void OnDeath(EnemyStatus enemy)
    {
        Debug.Log("Enemy Died");
        _enemies.Remove(enemy);
        _objCache.ReturnObject(enemy.Type, enemy);

        if (_enemies.Count == 0)
        {
            // TODO: Add delays between wave start and end
            _waveCount += 1;
            _player.IsControllable = false;
            // FIXME: Currently player is only able to upgrade abilities on waves with multiple 5. Make it so they don't get fucked if they missed upgrading.
            _upgradesController.Show(1, _waveCount % 5 == 0); // FIXME: This should change based on the mode
        }
    }
}