using UnityEngine;

public class ExplosiveMine : MonoBehaviour
{
    
    public void ExecuteGridExplosion()
    {

        GameManager.Instance.TriggerExplosionEffect(transform.position);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        float explosionRadius = 1.6f; 

        
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeSelf) continue; 
            
            if (Vector3.Distance(transform.position, enemy.transform.position) <= explosionRadius)
            {
                enemy.SetActive(false); 
                Destroy(enemy);
               
            }
        }

        foreach (GameObject bullet in bullets)
        {
            if (bullet == null || !bullet.activeSelf) continue;
            
            if (Vector3.Distance(transform.position, bullet.transform.position) <= explosionRadius)
            {
                bullet.SetActive(false); 
                Destroy(bullet);
               
            }
        }

        SoundManager.Instance.PlayExplosionSound();
        gameObject.SetActive(false); 
        Destroy(gameObject);
    }
}