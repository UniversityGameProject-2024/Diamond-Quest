using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    [Tooltip("Which fruits will appear")]
    [SerializeField]
    private GameObject[] fruitPrefabs;// fruits array
    [SerializeField]
    [Tooltip("Bomb prefab")]
    private GameObject bombPrefab;
    [Range(0f, 1f)]
    [Tooltip("Bomb chance spawn")]
    [SerializeField]
    private float bombChance = 0.05f;

    //Fruit time spawning
    [Tooltip("Fruit spawn min time in seconds")]
    [SerializeField]
    private float minSpawnDelay = 0.25f;
    [Tooltip("Fruit spawn max time in seconds")]
    [SerializeField]
    private float maxSpawnDelay = 1.0f;

    //Angle spawning
    [Tooltip("Fruit min angle spawning")]
    [SerializeField]
    private float minAngle = -15f;
    [Tooltip("Fruit max angle spawning")]
    [SerializeField]
    private float maxAngle = 15f;

    //Force for throwing fruits
    [Tooltip("Min force applied to throw fruits")]
    [SerializeField]
    private float minForce = 18f;
    [Tooltip("Max force applied to throw fruits")]
    [SerializeField]
    private float maxForce = 22f;

    //Fruit living time
    [Tooltip("Fruit living time in seconds")]
    [SerializeField]
    private float maxLifetime = 5f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawm());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawm()
    {
        yield return new WaitForSeconds(2f);//How long to start the first spawning

        while (enabled)
        {
            //Take a random index from fruit array
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            //Randomize Bomb spawn
            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            //Where fruit will spawn
            Vector3 position = new Vector3();
            position.x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            position.y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            position.z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

            // Create a random rotation for the fruit(spinning effect)
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);//fruit spawn
            Destroy(fruit, maxLifetime);//Fruit living time

            //Force for throwing fruits
            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            // How long take to spawn a new fruit
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }

    }
}
