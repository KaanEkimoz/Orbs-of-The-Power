using UnityEngine;
using TMPro;
using com.game.generics.entities;

public class AltarSystemController : MonoBehaviour
{
    //[Header("Altar System Settings")]
    //public float safeZoneTimer = 120f;
    //public int maxSafeZoneEntries = 4;
    //public float difficultyIncreaseTime = 30f;  
    //public int maxEnemyIncrease = 10;

    //[Header("Current System Values")]
    //[SerializeField]
    //private int currentDifficultyLevel = 0;
    //public int safeZoneEntryCount = 0;
    //private float timer;

    //[Header("References")]
    //public Door m_safeZoneWalls;
    //public GameObject safeZoneArea;
    //public EnemySpawner enemySpawner;
    //public CheckSafeZone checkSafeZone;

    //[Header("Bools")]
    //[SerializeField] private bool isPlayerInSafeZone = true;
    //[SerializeField] private bool canEnterSafeZone = true;
    //[SerializeField] private bool isTimerActive = false;
    
    //[Header("UI")]
    //public TextMeshProUGUI timerText;

    //private void Start()
    //{
    //    m_safeZoneWalls.Open();
    //    ResetTimer();

    //}

    //private void Update()
    //{
    //    if (!isTimerActive) return;

    //    timer += Time.deltaTime;
    //    UpdateTimerUI();

    //    if (timer < safeZoneTimer) return;

    //    canEnterSafeZone = true;
    //    m_safeZoneWalls.Open();

    //    int difficultyLevel = (int)((timer - safeZoneTimer) / difficultyIncreaseTime);

    //    SetDifficulty(difficultyLevel);

    //}

    //public void SetDifficulty(int difficulty)
    //{
    //    if (difficulty > currentDifficultyLevel)
    //    {
    //        int difficultyIncrease = difficulty - currentDifficultyLevel;
    //        enemySpawner.maxEnemyCount = Mathf.Min(enemySpawner.maxEnemyCount + (maxEnemyIncrease * difficultyIncrease), 100);

    //        currentDifficultyLevel = difficulty;
    //    }


    //    switch (difficulty) 
    //    {
    //        case 0:
                
    //        case 1:
    //            //todo : x dusmani arttir 
    //            break;

    //        case 2:                
    //            //todo : dusmanlari hizlandir
    //            break;

    //        case 3:               
    //            //todo : dusmanlari kuvvetlendir
    //            break;

    //        default:
    //            //idk
    //            break;
    //    }
    //}
    //public void PlayerEnteredSafeZone()
    //{
    //    enemySpawner.ClearEnemies();
    //    enemySpawner.StopSpawning();
    //    safeZoneEntryCount++;
    //    ResetTimer();
    //    UpdateTimerUI();

    //    if (safeZoneEntryCount >= maxSafeZoneEntries)
    //    {
    //        Debug.Log("Boss geliyor!");
    //    }

    //}

    //public void PlayerExitedSafeZone()
    //{
    //    canEnterSafeZone = false;

    //    if (!isTimerActive)
    //    {
    //        StartTimer();
    //        m_safeZoneWalls.Close();
    //        enemySpawner.StartSpawning();
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("girdi");

    //    if (other.CompareTag("Player") && other.gameObject == safeZoneArea)
    //    {          
    //        PlayerEnteredSafeZone();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("cikti");

    //    if (other.CompareTag("Player") && other.gameObject == safeZoneArea)
    //    {         
    //        PlayerExitedSafeZone();  
    //    }
    //}

    //private void UpdateTimerUI()
    //{
    //    int minutes = Mathf.FloorToInt(timer / 60);
    //    int seconds = Mathf.FloorToInt(timer % 60);
    //    timerText.text = minutes + " : " + seconds;
    //}

    //private void ResetTimer()
    //{
    //    timer = 0f;
    //    isTimerActive = false;
    //}

    //private void StartTimer()
    //{
    //    isTimerActive = true;
    //}
}
