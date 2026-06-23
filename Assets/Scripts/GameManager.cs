using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int hp = 3;
    public int steps = 0;

    [Header("Economy System")]
    public int starsCollected = 0;
    public int starsNeededForAbility = 3;
    public int abilityCharges = 3;

    [Header("UI References")]
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI stepResultText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI notificationText;
    public GameObject gameOverPanel;
    public Image[] heartImages;

    [Header("Juice & Effects")]
    public GameObject explosionParticlePrefab;

    [Header("Trade-off System")]

    public int turnsBetweenSpawns = 10; 
    public int minimumSpawnLimit = 2;   


    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (notificationText != null) notificationText.text = "";
        UpdateUI();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {

        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void AddStep()
    {
        if (isGameOver) return;
        steps++;
        UpdateUI();
    }

    public void AddStar()
    {
        if (isGameOver) return;

        starsCollected++;

        if (starsCollected >= starsNeededForAbility)
        {
            starsCollected -= starsNeededForAbility;
            abilityCharges++;
            ShowNotification("<color=#FFD700>ABILITY CHARGE ACQUIRED!</color>");
        }

        UpdateUI();
    }

    public bool SpendCharge()
    {
        if (abilityCharges >= 1)
        {
            abilityCharges--;
            UpdateUI();
            return true;
        }

        ShowNotification("NOT ENOUGH CHARGES! COLLECT MORE STARS!");
        return false;
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            StopCoroutine("FadeNotification");
            StartCoroutine(FadeNotification(message));
        }
    }

    private IEnumerator FadeNotification(string message)
    {
        notificationText.text = message;
        yield return new WaitForSeconds(2f);
        notificationText.text = "";
    }

    public void TakeHit()
    {
        if (isGameOver) return;
        hp--;
        UpdateUI();
        if (hp <= 0) GameOver();
    }

    private void UpdateUI()
    {
        stepText.text = "STEPS: " + steps;

        if (starText != null)
        {
            starText.text = "STARS: " + starsCollected + " / " + starsNeededForAbility + "\nCHARGES: " + abilityCharges;
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < hp);
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        stepResultText.text = "You survived " + steps + " steps";
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TriggerExplosionEffect(Vector3 position)
    {

        if (explosionParticlePrefab != null)
        {
            Vector3 spawnPos = new Vector3(position.x, 1.5f, position.z); 
            Instantiate(explosionParticlePrefab, spawnPos, Quaternion.identity);

        }


        SoundManager.Instance.PlayExplosionSound();
    }

    public void IncreaseEMPDifficulty()
    {
        
        turnsBetweenSpawns--;
        
        if (turnsBetweenSpawns < minimumSpawnLimit) 
        {
            turnsBetweenSpawns = minimumSpawnLimit;
        }
    }
}