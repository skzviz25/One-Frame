using UnityEngine;

public class EnemyShooter : MonoBehaviour, ITurnTaker
{
    [Header("Projectile Setup")]
    public GameObject bulletPrefab;
    private Transform player;
    private int turnCounter = 0;

    void Start()
    {
        
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.Register(this);
        }
        
        
        PlayerController playerScript = FindFirstObjectByType<PlayerController>();
        if (playerScript != null)
        {
            player = playerScript.transform;
        }
    }

    void OnDestroy()
    {
        SoundManager.Instance.PlayEnemyDisappearSound();
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.Unregister(this);
        }
    }

    public void TakeTurn()
    {
        
        if (!gameObject.activeSelf || player == null) return;

        turnCounter++;
        
        
        if (turnCounter >= 7) 
        {
            Shoot();
            turnCounter = 0; 
        }
    }

    private void Shoot()
    {
        
        Vector3 shootDirection = player.position - transform.position;
        
        
        shootDirection.y = 0;
        shootDirection.Normalize();

        if (shootDirection != Vector3.zero)
        {
            transform.forward = shootDirection;
        }

        
        Vector3 bulletSpawnPosition = transform.position + (shootDirection * 0.6f);
        
        GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            
            bulletScript.direction = shootDirection;
        }
    }
}