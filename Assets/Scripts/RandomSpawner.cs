using UnityEngine;
using System.Collections;


public class RandomSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject[] diamondPrefabs; //  מערך של יהלומים לבחירה

    public float diamondsSpawnDelay = 1f; // זמן בין הופעות יהלומים
    public float screenPadding = 0.1f; // שוליים למניעת הופעה מחוץ למסך
    public float screenVerticalPadding = 0.2f; // שוליים למניעת הופעה מחוץ למסך

    public float destroyDiamondsIntervalInSeconds = 5;

    private Camera mainCamera;

    [SerializeField] public GameObject prefabBoss;

    GameObject boss;

    private void Start()
    {
        mainCamera = Camera.main;
        // StartCoroutine(ShowBigDiamond());
        GameManager.Instance.SetGameLevelActive(false);
        StartCoroutine(SpawnDiamonds()); //  ממשיך לייצר יהלומים רגילים
        StartCoroutine(SpawnBoss());
    }

    public IEnumerator ShowBigDiamond()
    {
        yield return new WaitForSeconds(1f); //  מחכה שנייה נוספת לפני יצירת היהלומים הרגילים

        GameObject diamondPrefab = diamondPrefabs[Random.Range(0, diamondPrefabs.Length)];
        Vector3 centerScreenPosition = new Vector3(0.5f, 0.5f, 10f);
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(centerScreenPosition);

        //  יוצרים את היהלום הגדול דרך המחלקה שלו
        BigDiamond bigDiamond = BigDiamond.Create(diamondPrefab, worldPosition);

        if (bigDiamond != null)
        {
            yield return new WaitForSeconds(3.5f); //  מחכה עד שהיהלום הגדול נעלם
        }

        yield return new WaitForSeconds(1f); //  מחכה שנייה נוספת לפני יצירת היהלומים הרגילים

        GameManager.Instance.SetGameLevelActive(true);
    }

    private IEnumerator SpawnDiamonds()
    {
        int countSpawnedDiamonds = 0;
        while(true)
        {
            if(GameManager.Instance.IsGameActive &&
            GameManager.Instance.IsGameLevelActive)
            {
                countSpawnedDiamonds++;
                SpawnRandomDiamond();
                yield return new WaitForSeconds(diamondsSpawnDelay);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    private IEnumerator SpawnBoss()
    {
        while(true)
        {
            if (GameManager.Instance.IsGameActive && 
                GameManager.Instance.IsGameLevelActive &&
                GameManager.Instance.GetScore() >= 10)
                {
                    Vector3 bossPositon = new Vector3(0.8f, 0.2f, 8f);
                    Vector3 bossWorldPosition = mainCamera.ViewportToWorldPoint(bossPositon);
                    bossWorldPosition = new Vector3(bossWorldPosition.x, bossWorldPosition.y,3f);
                    GameManager.Instance.SetWasDiamondSlicedWhenBossAskedToStop(false);
                    boss = Instantiate(prefabBoss, bossWorldPosition, Quaternion.identity);
                    Invoke("DestroyBoss", 5f);
                    yield return new WaitForSeconds(45f);
                }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
    private void DestroyBoss()
    {
        Destroy(boss);
        if (GameManager.Instance.WasDiamondSlicedWhenBossAskedToStop)
        {
            GameManager.Instance.AddScore(-5);
        }
        else
        {
            GameManager.Instance.AddScore(20);
        }
    }
   public void SpawnRandomDiamond()
   {
    GameObject prefab = diamondPrefabs[Random.Range(0, diamondPrefabs.Length)];
    prefab.tag = "Small Diamond";
    Renderer hitRenderer = prefab.GetComponent<Renderer>();
    Color diamondColor = hitRenderer.sharedMaterial.color;
    //Debug.Log($"Spawned diamond color: {diamondColor}");
    if (diamondColor == GameManager.Instance.BigDiamondColor)
    {
        GameManager.Instance.AddToCountSpawnedGoodDiamonds(1);
        Debug.Log($"Spawned good diamonds: {GameManager.Instance.CountSpawnedGoodDiamonds}");
    }
    UnityEngine.Vector3 worldPosition = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
    const float MIN_DISTANCE_BETWEEN_DIAMONDS = 1.5f;
    Quaternion fixedRotation = Quaternion.Euler(-30f, 1f, 1f);
    for(int i=0; i<10; i++)
    {
        Vector3 diamondPosition = new Vector3(
        Random.Range(screenPadding, 1f - screenPadding),
        Random.Range(screenVerticalPadding, 1f - screenVerticalPadding),
            10f);
            worldPosition = mainCamera.ViewportToWorldPoint(diamondPosition);
            UnityEngine.Collider[] colliders = 
                Physics.OverlapSphere(worldPosition, MIN_DISTANCE_BETWEEN_DIAMONDS);
            if (colliders.Length == 0)
            {
                Debug.Log("V --- Diamond NOT collides");                
                break;
            }
            else
            {
                Debug.Log("X --- Diamond collides with another collider");
            }
    }
    //GameObject spawnedDiamond = Instantiate(prefab, worldPosition, Quaternion.identity);
    GameObject spawnedDiamond = Instantiate(prefab, worldPosition, fixedRotation);
    spawnedDiamond.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
   //GameObject spawnedDiamond = Instantiate(prefab, worldPosition, Quaternion.identity);
    if(GameManager.Instance.GetScore() >= 10)
    {
        if(destroyDiamondsIntervalInSeconds > 4)
        {
            destroyDiamondsIntervalInSeconds--;
        }
        diamondsSpawnDelay = 0.7f;
    }
    Destroy(spawnedDiamond, destroyDiamondsIntervalInSeconds);
  }
   public void SpawnRandomDiamondForTutorial()
   {
    GameObject prefab = diamondPrefabs[Random.Range(0, diamondPrefabs.Length)];
    prefab.tag = "Small Diamond";
    Vector3 worldPosition;
    Quaternion fixedRotation = Quaternion.Euler(-30f, 1f, 1f);
    Vector3 diamondPosition = new Vector3(
        Random.Range(screenPadding, 1f - screenPadding),
        Random.Range(screenPadding, 1f - screenPadding),
        10f);
    worldPosition = mainCamera.ViewportToWorldPoint(diamondPosition);
    // GameObject spawnedDiamond = Instantiate(prefab, worldPosition, Quaternion.identity);
    GameObject spawnedDiamond = Instantiate(prefab, worldPosition, fixedRotation);
    spawnedDiamond.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    Destroy(spawnedDiamond, destroyDiamondsIntervalInSeconds);
  }
}
