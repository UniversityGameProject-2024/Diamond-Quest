using UnityEngine;
using TMPro;
using System.Collections;

public class Blade : MonoBehaviour
{
    public Color bigDiamondColor;
    private TMP_Text textScore;
    private int score;
    private int scoreBad;
    private int GoodDiamonds;
    private GameObject spawner;

    public void Start()
    {
        score = 0;
        spawner = GameObject.Find("RandomSpawner");
    }
    private void Update()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("❌ GameManager.Instance is NULL in Blade.cs!");
            return;
        }
        if (Time.timeScale == 0f)
        {
            return;
        }
        if (!GameManager.Instance.IsGameActive && !GameManager.Instance.IsTutorialActive)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseWorldPosition, Camera.main.transform.forward, out hit))
            {
                Debug.Log("🎯 Hit object: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.tag == "Big Diamond")
                {
                    return; //  לא ניתן לחתוך את היהלום הגדול
                }
                SliceableObject sliceable = hit.collider.GetComponent<SliceableObject>();
                if (sliceable != null)
                {
                    sliceable.Slice(hit.point, transform.up);
                    Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
                    if (hitRenderer != null)
                    {
                        if (hit.collider.gameObject.tag == "Small Diamond")
                        {
                            GameObject boss = GameObject.FindWithTag("Boss");
                            if (boss != null)
                            {
                                GameManager.Instance.SetWasDiamondSlicedWhenBossAskedToStop(true);
                                GameManager.Instance.AddToCountDiamondsCutWhenBossNotAllowed(1);
                                GameManager.Instance.PlaySound(false); //  צליל כישלון
                            }
                            else
                            {
                                Color diamondColor = hitRenderer.material.color;
                                if (GameManager.Instance.ColorsMatch(diamondColor, bigDiamondColor))
                                {
                                    GoodDiamonds++;
                                    Debug.Log("GoodDiamonds" + GoodDiamonds);

                                    GameManager.Instance.AddScore(1);
                                    GameManager.Instance.AddToCountGoodDiamondsCut(1);
                                    GameManager.Instance.PlaySound(true); //  צליל הצלחה
                                    GameManager.Instance.ResetMistakeCount();
                                }
                                else
                                {
                                    GameManager.Instance.AddScore(-1);
                                    scoreBad++;
                                    GameManager.Instance.AddToCountBadDiamondsCut(1);
                                    GameManager.Instance.HandleMistake();
                                    GameManager.Instance.PlaySound(false); //  צליל כישלון
                                }
                            }
                            hit.collider.gameObject.tag = "Diamond Sliced";
                        }
                    }
                    if (GameManager.Instance.GetScore() > 0 && GameManager.Instance.GetScore() % 5 == 0)
                    {
                        GameManager.Instance.SetGameLevelActive(false);
                        GameObject[] smallDiamonds = GameObject.FindGameObjectsWithTag("Small Diamond");
                        foreach (GameObject smallDiamond in smallDiamonds)
                        {
                            Destroy(smallDiamond);
                        }
                        StartCoroutine(WaitAndStartNewGameLevel());
                    }
                    Debug.Log("✂ Slicing successful: " + hit.collider.gameObject.name);
                }
            }
        }
    }

    private IEnumerator WaitAndStartNewGameLevel()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.SetGameLevelActive(false);
        StartCoroutine(spawner.GetComponent<RandomSpawner>().ShowBigDiamond());
    }

    public void SetBigDiamondColor(Color color)
    {
        bigDiamondColor = color;
        Debug.Log("🎨 Big diamond color set to: " + bigDiamondColor);
    }
}
