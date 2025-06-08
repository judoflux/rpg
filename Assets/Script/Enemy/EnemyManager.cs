using UnityEngine;
using EnemyEnum.Enums;
using System.Collections.Generic;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    // You no longer need a single generic 'public Enemy enemy;' field
    // if you intend to find specific types on demand.
    // If you want the manager to hold references to specific known enemies,
    // you could have fields like:
    public Enemy frogneoidEnemy { get; private set; }
    public Enemy skeletonEnemy { get; private set; }


    private void Awake()
    {
        Debug.Log("who calls first awake from enemymanager");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        { 
            // If an instance already exists, and it's not this one, destroy this new one.
            Debug.LogWarning("EnemyManager: Duplicate instance found. Destroying this one.");
            Destroy(gameObject);
            return; // Exit Awake early to prevent further initialization of the duplicate
        }
    }

    public Enemy GetEnemy(EnemyType typeToFind)
        {
            if (typeToFind == EnemyType.None)
            {
                Debug.LogWarning("EnemyManager: GetEnemy called with EnemyType.None. Please specify a valid enemy type.");
                return null;
            }

            // FindObjectsByType is generally preferred in modern Unity versions.
            // For older versions (before Unity 2020.1 approx.), use FindObjectsOfType<Enemy>().
            Enemy[] allEnemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            foreach (Enemy currentEnemy in allEnemiesInScene)
            {
                // Check if the enemy is active in the hierarchy and matches the requested type
                if (currentEnemy != null && currentEnemy.gameObject.activeInHierarchy && currentEnemy.enemyType == typeToFind)
                {
                    return currentEnemy; // Return the first matching enemy
                }
            }

            Debug.LogWarning($"EnemyManager: No active enemy of type '{typeToFind}' found in the scene.");
            return null;
        }
    public System.Collections.Generic.List<Enemy> GetEnemies(EnemyType typeToFind)
    {
        System.Collections.Generic.List<Enemy> foundEnemies = new System.Collections.Generic.List<Enemy>();
        if (typeToFind == EnemyType.None) return foundEnemies;

        Enemy[] allEnemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy currentEnemy in allEnemiesInScene)
        {
            if (currentEnemy != null && currentEnemy.gameObject.activeInHierarchy && currentEnemy.enemyType == typeToFind)
            {
                foundEnemies.Add(currentEnemy);
            }
        }
        if (foundEnemies.Count == 0)
        {
            Debug.LogWarning($"EnemyManager: No active enemies of type '{typeToFind}' found in the scene.");
        }
        return foundEnemies;
    }
    public List<Enemy> GetAllActiveEnemies()
    {
        List<Enemy> activeEnemies = new List<Enemy>();
        Enemy[] allEnemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in allEnemiesInScene)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                activeEnemies.Add(enemy);
            }
        }
        
        if (activeEnemies.Count == 0)
        {
            Debug.LogWarning("EnemyManager: No active enemies found in the scene.");
        }
        return activeEnemies;
    }
}
