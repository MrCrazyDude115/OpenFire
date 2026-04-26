using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Useful sources (NOT INCLUDING OFFICIAL DOCUMENTATION)
//https://youtu.be/uWI3JEBRMiA?si=8xV7j5Yhil5So-zb
//https://youtu.be/Q16KIxtomeo?si=nLVJ-52LqeMs1KfI

public class ArenaController : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public EnemySpawnPoint spawnPoint;

        public EnemyCoverShooter.EnemyExposureType exposureType;
        public EnemyCoverShooter.EnemyWeaponType weaponType;
        public EnemyCoverShooter.EnemyFireType fireType;
        public bool facePlayerCamera = true;
        public bool facePlayerWhileMoving = true;
        public bool shootWhileMoving = true;
        public bool doSpawnRoll = false;
        public bool moveAfterRoll = false;
    }

    [System.Serializable]
    public class WaveData
    {
        public List<EnemySpawnData> enemiesInWave;
        public int killsRequiredToTriggerNextWave;
    }

    [Header("Arena Start")]
    [SerializeField] private bool startOnSceneLoad = false;

    [Header("Area Cameras")]
    [SerializeField] private GameObject inCoverCamera;
    [SerializeField] private GameObject peekCamera;

    [Header("Waves")]
    [SerializeField] private List<WaveData> waves = new List<WaveData>();

    [Header("Events")]
    [SerializeField] private UnityEvent onArenaCleared;

    private int currentWaveIndex = -1;
    private int totalEnemiesKilled = 0;
    private int killsThisWave = 0;
    private int aliveEnemyCount = 0;

    private bool hasStarted = false;
    private bool hasFinished = false;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        //StartArena();
        //PlayerCoverController.instance.SetProtectedState(true);

        if (startOnSceneLoad)
        {
            StartArena();
        }
    }

    public void StartArena()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;

        if (waves.Count > 0)
        {
            SpawnNextWave();
        }
    }

    public void RegisterEnemyDeath()
    {
        totalEnemiesKilled++;
        killsThisWave++;
        aliveEnemyCount--;

        if (hasFinished)
        {
            return;
        }

        if (currentWaveIndex < waves.Count - 1)
        {
            int requiredKills = waves[currentWaveIndex].killsRequiredToTriggerNextWave;

            if (killsThisWave >= requiredKills)
            {
                SpawnNextWave();
            }
        }

        CheckArenaComplete();
    }

    private void SpawnNextWave()
    {
        currentWaveIndex++;
        killsThisWave = 0;

        if (currentWaveIndex >= waves.Count)
        {
            return;
        }

        WaveData wave = waves[currentWaveIndex];

        for (int i = 0; i < wave.enemiesInWave.Count; i++)
        {
            EnemySpawnData spawnData = wave.enemiesInWave[i];

            if (spawnData.enemyPrefab != null && spawnData.spawnPoint != null)
            {
                GameObject newEnemy = Instantiate(
                    spawnData.enemyPrefab,
                    spawnData.spawnPoint.GetSpawnTransform().position,
                    spawnData.spawnPoint.GetSpawnTransform().rotation
                );

                spawnedEnemies.Add(newEnemy);
                aliveEnemyCount++;

                EnemyCoverShooter enemyCoverShooter = newEnemy.GetComponent<EnemyCoverShooter>();

                if (enemyCoverShooter != null)
                {
                    enemyCoverShooter.SetSpawnSettings(
                        spawnData.exposureType,
                        spawnData.weaponType,
                        spawnData.fireType,
                        spawnData.facePlayerCamera,
                        spawnData.doSpawnRoll,
                        spawnData.moveAfterRoll
                    );

                    enemyCoverShooter.SetMoveTarget(
                        spawnData.spawnPoint.GetMoveTarget(),
                        spawnData.shootWhileMoving,
                        spawnData.facePlayerWhileMoving
                    );

                    enemyCoverShooter.InitEnemy();
                }

                EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.SetArenaController(this);
                }
            }
        }
    }

    private void CheckArenaComplete()
    {
        if (aliveEnemyCount <= 0 && currentWaveIndex == waves.Count - 1)
        {
            hasFinished = true;
            Debug.Log(gameObject.name + " arena complete.");
            onArenaCleared.Invoke();
        }
    }

    public GameObject GetInCoverCamera()
    {
        return inCoverCamera;
    }

    public GameObject GetPeekCamera()
    {
        return peekCamera;
    }
}