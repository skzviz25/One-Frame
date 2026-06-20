using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private List<ITurnTaker> turnTakers = new List<ITurnTaker>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(ITurnTaker t) { if (!turnTakers.Contains(t)) turnTakers.Add(t); }
    public void Unregister(ITurnTaker t) { if (turnTakers.Contains(t)) turnTakers.Remove(t); }

    public void ProcessTurn()
    {
        
        for (int i = turnTakers.Count - 1; i >= 0; i--)
        {
            if (turnTakers[i] != null) turnTakers[i].TakeTurn();
        }

        
        EvaluateGridRules();
    }

    public void EvaluateGridRules()
    {
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");

        
        for (int m = mines.Length - 1; m >= 0; m--)
        {
            GameObject mine = mines[m];
            if (mine == null) continue;

            bool mineTriggered = false;

            
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;
                if (IsOnSameTile(mine.transform.position, enemy.transform.position))
                {
                    mineTriggered = true;
                    break;
                }
            }

            
            if (!mineTriggered)
            {
                foreach (GameObject bullet in bullets)
                {
                    if (bullet == null) continue;
                    if (IsOnSameTile(mine.transform.position, bullet.transform.position))
                    {
                        mineTriggered = true;
                        break;
                    }
                }
            }

            
            if (mineTriggered)
            {
                ExplosiveMine mineScript = mine.GetComponent<ExplosiveMine>();
                if (mineScript != null) mineScript.ExecuteGridExplosion();
            }
        }

        
        if (player != null)
        {
            
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            bullets = GameObject.FindGameObjectsWithTag("Bullet");

            player.CheckGridCollisions(enemies, bullets);
        }
    }

   
    public bool IsOnSameTile(Vector3 posA, Vector3 posB)
    {
        return Mathf.Abs(posA.x - posB.x) < 0.5f && Mathf.Abs(posA.z - posB.z) < 0.5f;
    }
}