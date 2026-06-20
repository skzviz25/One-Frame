using UnityEngine;

public class Bullet : MonoBehaviour, ITurnTaker
{

    [HideInInspector] public Vector3 direction;

    private int turnsLived = 0;

    void Start()
    {
        UpdateRotation();
        if (TurnManager.Instance != null)
            TurnManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.Unregister(this);
    }

    public void InitializeBullet(Vector3 shootDirection)
    {
        direction = shootDirection;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (direction != Vector3.zero)
        {
            direction.y = 0; 
            transform.forward = -direction.normalized;
        }
    }

    public void TakeTurn()
    {

        transform.position += direction * 1f;

        turnsLived++;

        if (turnsLived >= 15)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            SoundManager.Instance.PlayEnemyDisappearSound();
            Destroy(gameObject);
        }

        
        if (other.CompareTag("Enemy") && turnsLived > 0)
        {
            SoundManager.Instance.PlayEnemyDisappearSound();
            Destroy(other.gameObject); 
            Destroy(gameObject);       
        }
    }
}