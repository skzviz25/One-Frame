using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    void Update()
    {
        
        transform.Rotate(new Vector3(15f, 30f, 45f) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            GameManager.Instance.AddStar();
            SoundManager.Instance.PlayStarCollectSound();
            Destroy(gameObject);
        }
    }
}