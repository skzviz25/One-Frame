using UnityEngine;
public class EnemyBasic : MonoBehaviour, ITurnTaker
{
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }


        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.Register(this);
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
        if (player == null) return;

        Vector3 distanceToPlayer = player.position - transform.position;
        Vector3 snappedDir = Vector3.zero;

        if (Mathf.Abs(distanceToPlayer.x) > Mathf.Abs(distanceToPlayer.z))
            snappedDir.x = Mathf.Sign(distanceToPlayer.x);
        else
            snappedDir.z = Mathf.Sign(distanceToPlayer.z);


        Vector3 targetPos = transform.position + (snappedDir * 1f);

        
        Vector3 enemyMoveDirection = targetPos - transform.position;
        enemyMoveDirection.y = 0;

        if (enemyMoveDirection != Vector3.zero)
        {
            transform.forward = enemyMoveDirection.normalized;
        }

        Collider[] colliders = Physics.OverlapSphere(targetPos, 0.3f);
        foreach (Collider col in colliders)
        {

            if (col.CompareTag("Wall") || col.CompareTag("Obstacle")) return;
        }

        transform.position = targetPos;
    }
}