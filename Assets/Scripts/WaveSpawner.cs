using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject walkerPrefab;
    public GameObject shooterPrefab;
    
    [Header("Collectibles & Limits")]
    public GameObject starPrefab;
    public int maxEnemies = 12; 
    [Header("UI")]
    public TextMeshProUGUI waveText;

    private float elapsedTime = 0f; 

    void Start()
    {
        
        StartCoroutine(SpawnLoop(walkerPrefab, 4f, 0f));   
        StartCoroutine(SpawnLoop(shooterPrefab, 6f, 10f)); 
        
        StartCoroutine(SpawnStarLoop());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (waveText != null)
        {
            if (elapsedTime >= 30f) waveText.text = "WAVE 3";
            else if (elapsedTime >= 15f) waveText.text = "WAVE 2";
            else waveText.text = "WAVE 1";
        }
    }

    IEnumerator SpawnLoop(GameObject prefab, float interval, float unlockTime)
    {
        while (true)
        {
            GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (elapsedTime >= unlockTime && currentEnemies.Length < maxEnemies)
            {
                SpawnEnemy(prefab);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        
        
        float spawnX = Mathf.Cos(randomAngle) * 18f;
        float spawnZ = Mathf.Sin(randomAngle) * 18f;
        
        Vector3 spawnPos = new Vector3(Mathf.Round(spawnX), 0.35f, Mathf.Round(spawnZ));
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    IEnumerator SpawnStarLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(7f); 

            
            int randomX = Random.Range(-15, 15);
            int randomZ = Random.Range(-15, 15);
            Vector3 starPos = new Vector3(randomX, 0.35f, randomZ);

            Instantiate(starPrefab, starPos, Quaternion.identity);
        }
    }
}