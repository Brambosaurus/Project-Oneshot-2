using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        foreach (Transform point in spawnPoints)
        {
            Instantiate(enemyPrefab, point.position, point.rotation);
        }
    }
}
