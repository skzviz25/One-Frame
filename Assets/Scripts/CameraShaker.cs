using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    
    public static CameraShaker Instance;

    private Vector3 originalLocalPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       
        originalLocalPosition = transform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        
        StartCoroutine(ProcessShake(duration, magnitude));
    }

    private IEnumerator ProcessShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float zOffset = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalLocalPosition.x + xOffset, originalLocalPosition.y, originalLocalPosition.z + zOffset);

            elapsed += Time.unscaledDeltaTime; 
            yield return null; 
        }

        transform.localPosition = originalLocalPosition;
    }
}