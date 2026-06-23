using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Action Wheel Setup")]
    public TextMeshProUGUI actionWheelUiText;
    public GameObject minePrefab; 
    private int selectedActionIndex = 0; 
    
    private bool dashQueued = false;
    public bool riftActive = false;
    private int riftStepsLeft = 0;

    [Header("Juice & Feedback Visuals")]
    public Image vignetteImage; 
    private MeshRenderer meshRenderer;
    private Color originalColor = new Color(0f, 1f, 0.81f);
    private Color actionColor = new Color(1f, 0.5f, 0f);


    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateWheelUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            selectedActionIndex++;
            if (selectedActionIndex > 3) selectedActionIndex = 0; 
            UpdateWheelUI();
            SoundManager.Instance.PlayStepSound();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            TryExecuteSelectedAction();
        }

        Vector3 moveDir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveDir = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveDir = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveDir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveDir = Vector3.right;

        moveDir.y = 0;

        if (moveDir != Vector3.zero)
        {
            if (dashQueued) ExecuteCyberDash(moveDir);
            else 
            {
                transform.forward = moveDir.normalized;
                TryMovePlayer(moveDir);
            }
        }
    }

    private void TryExecuteSelectedAction()
    {
        if (!GameManager.Instance.SpendCharge()) return;

        SoundManager.Instance.PlayStepSound();

        switch (selectedActionIndex)
        {
            case 0: 
                Instantiate(minePrefab, transform.position, Quaternion.identity);
                break;
            case 1: 
                dashQueued = true;
                meshRenderer.material.color = actionColor;
                break;
            case 2:
                ExecuteEMPPulse();
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.IncreaseEMPDifficulty();
                }
                break;
            case 3: 
                riftActive = true;
                riftStepsLeft = 3;
                meshRenderer.material.color = Color.yellow;
                break;
        }

        
        TurnManager.Instance.ProcessTurn();
    }

    private void TryMovePlayer(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;
        if (IsTileBlocked(targetPosition)) return;

        transform.position = targetPosition;
        GameManager.Instance.AddStep();
        SoundManager.Instance.PlayStepSound();

        if (riftActive)
        {
            riftStepsLeft--;
            if (riftStepsLeft <= 0)
            {
                riftActive = false;
                meshRenderer.material.color = originalColor;
            }
            
            TurnManager.Instance.EvaluateGridRules();
        }
        else
        {
            TurnManager.Instance.ProcessTurn();
        }
    }

    private void ExecuteCyberDash(Vector3 direction)
    {
        dashQueued = false;
        meshRenderer.material.color = originalColor;
        Vector3 finalPos = transform.position;

        SoundManager.Instance.PlayStepSound();

        for (int i = 1; i <= 3; i++)
        {
            Vector3 checkPos = transform.position + (direction * i);
            if (IsTileBlocked(checkPos)) break;

            finalPos = checkPos;

            
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || !enemy.activeSelf) continue;
                if (Mathf.Abs(checkPos.x - enemy.transform.position.x) < 0.5f && Mathf.Abs(checkPos.z - enemy.transform.position.z) < 0.5f)
                {

                    GameManager.Instance.TriggerExplosionEffect(enemy.transform.position);
                    enemy.SetActive(false); 
                    Destroy(enemy);
                }
            }
        }

        transform.position = finalPos;
        GameManager.Instance.AddStep();
        TurnManager.Instance.ProcessTurn();
    }

    private void ExecuteEMPPulse()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        float blastRadius = 2.5f; 
        
        foreach (GameObject bullet in bullets)
        {
            if (bullet == null || !bullet.activeSelf) continue;
            
            if (Vector3.Distance(transform.position, bullet.transform.position) <= blastRadius)
            {
                GameManager.Instance.TriggerExplosionEffect(bullet.transform.position);
                bullet.SetActive(false);
                Destroy(bullet);
            }
        }

        
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeSelf) continue;
            
            if (Vector3.Distance(transform.position, enemy.transform.position) <= blastRadius)
            {
                GameManager.Instance.TriggerExplosionEffect(enemy.transform.position);
                enemy.SetActive(false);
                Destroy(enemy);
            }
        }
    }

    
    public void CheckGridCollisions(GameObject[] enemies, GameObject[] bullets)
    {
        if (riftActive) return;

        bool tookDamageThisTurn = false;

        
        Bounds playerBounds = new Bounds(transform.position, new Vector3(0.8f, 2f, 0.8f));

        
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeSelf) continue; 
            
            Collider enemyCol = enemy.GetComponent<Collider>();
            bool isHitting = false;

            
            if (enemyCol != null) 
            {
                isHitting = enemyCol.bounds.Intersects(playerBounds);
            }
            else 
            {
                isHitting = Mathf.Abs(transform.position.x - enemy.transform.position.x) < 0.5f && Mathf.Abs(transform.position.z - enemy.transform.position.z) < 0.5f;
            }

            if (isHitting)
            {
                if (!tookDamageThisTurn) 
                {
                    ProcessPlayerHit(enemy);
                    tookDamageThisTurn = true; 
                }
                else
                {
                    SoundManager.Instance.PlayHitSound();
                    enemy.SetActive(false);
                    Destroy(enemy);
                }
            }
        }

        
        foreach (GameObject bullet in bullets)
        {
            if (bullet == null || !bullet.activeSelf) continue; 
            
            Collider bulletCol = bullet.GetComponent<Collider>();
            bool isHitting = false;

           
            if (bulletCol != null) 
            {
                isHitting = bulletCol.bounds.Intersects(playerBounds);
            }
            else
            {
                isHitting = Mathf.Abs(transform.position.x - bullet.transform.position.x) < 0.5f && Mathf.Abs(transform.position.z - bullet.transform.position.z) < 0.5f;
            }

            if (isHitting)
            {
                if (!tookDamageThisTurn) 
                {
                    ProcessPlayerHit(bullet);
                    tookDamageThisTurn = true;
                }
                else
                {
                    SoundManager.Instance.PlayHitSound();
                    bullet.SetActive(false);
                    Destroy(bullet);
                }
            }
        }
    }

    private void ProcessPlayerHit(GameObject hazard)
    {
        GameManager.Instance.TakeHit();
        SoundManager.Instance.PlayHitSound();
        GameManager.Instance.TriggerExplosionEffect(transform.position);
        hazard.SetActive(false); 
        Destroy(hazard); 
        if (CameraShaker.Instance != null) CameraShaker.Instance.TriggerShake(0.25f, 0.25f);
    }

    private bool IsTileBlocked(Vector3 targetPos)
    {
        
        if (Mathf.Abs(targetPos.x) >= 20f || Mathf.Abs(targetPos.z) >= 20f) return true;

        
        Bounds targetTileBounds = new Bounds(targetPos, new Vector3(0.8f, 2f, 0.8f));

        
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obs in obstacles)
        {
            Collider col = obs.GetComponent<Collider>();
            
            
            if (col != null) 
            {
                
                if (col.bounds.Intersects(targetTileBounds))
                {
                    return true; 
                }
            }
        }
        
        return false; 
    }

  

    private void UpdateWheelUI()
    {
        if (actionWheelUiText == null) return;
        string item0 = (selectedActionIndex == 0) ? "<color=#FF8000>[>] 1. DROP PLACEMENT MINE</color>\n" : "    1. DROP PLACEMENT MINE\n";
        string item1 = (selectedActionIndex == 1) ? "<color=#FF5555>[>] 2. CYBER LINE DASH</color>\n" : "    2. CYBER LINE DASH\n";
        string item2 = (selectedActionIndex == 2) ? "<color=#55FF55>[>] 3. EMP PULSE</color>\n" : "    3. EMP PULSE\n";
        string item3 = (selectedActionIndex == 3) ? "<color=#FFFF55>[>] 4. TIME RIFT</color>\n" : "    4. TIME RIFT\n";
        actionWheelUiText.text = "<b>EQUIPPED MATRIX ACTION:</b>\n" + item0 + item1 + item2 + item3 + "\n<size=24>[CTRL] - Cycle Action  |  [SHIFT] - Fire Selection</size>";
    }
}