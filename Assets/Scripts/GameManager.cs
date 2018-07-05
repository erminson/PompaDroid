﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public LifeBar enemyLifeBar;
    public GameObject goIndicator;

    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    public LevelData[] levels;
    public static int CurrenLevel = 0;

    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;

    public List<GameObject> activeEnemise;
    public Transform[] spawnPositions;

    public GameObject currentLevelBackground;

    public GameObject robotPrefab;
    public GameObject bossPrefab;

    public GameObject levelNamePrefab;
    public GameObject gameOverPrefab;

    public RectTransform uiTransform;

    public Transform walkInStartTarget;
    public Transform walkInTarget;
    public Transform walkOutTarget;

    public GameObject loadingScreen;

    void Awake()
    {
        loadingScreen.SetActive(true);    
    }

    void Start()
    {
        //cameraBounds.SetXPosition(cameraBounds.minVisibleX);
        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(levels[CurrenLevel]));
    }

    void Update()
    {
        if (currentBattleEvent == null && hasRemainingEvents) {
            if (Mathf.Abs(currentLevelData.battleData[nextEventIndex].column - cameraBounds.activeCamera.transform.position.x) < 0.2f) {
                PlayBattleEvent(currentLevelData.battleData[nextEventIndex]);
            }
        }

        if (currentBattleEvent != null) {
            if (Robot.TotalEnemies == 0) {
                CompleteCurrentEvent();
            }
        }

        if (cameraFollows) {
            cameraBounds.SetXPosition(actor.transform.position.x);
        }
    }

    private GameObject SpawnEnemy(EnemyData data)
    {
        GameObject enemyObj;
        if (data.type == EnemyType.Boss)
        {
            enemyObj = Instantiate(bossPrefab);
        }
        else
        {
            enemyObj = Instantiate(robotPrefab);
        }

        Vector3 position = spawnPositions[data.row].position;
        position.x = cameraBounds.activeCamera.transform.position.x + (data.offset * (cameraBounds.cameraHalfWidth + 1));
        enemyObj.transform.position = position;

        if (data.type == EnemyType.Robot) {
            enemyObj.GetComponent<Robot>().SetColor(data.color);
        }

        enemyObj.GetComponent<Enemy>().RegisterEnemy();

        return enemyObj;
    }

    private void PlayBattleEvent(BattleEvent battleEventData)
    {
        currentBattleEvent = battleEventData;
        nextEventIndex++;

        cameraFollows = false;
        cameraBounds.SetXPosition(battleEventData.column);

        foreach (GameObject enemy in activeEnemise) {
            Destroy(enemy);
        }

        activeEnemise.Clear();
        Enemy.TotalEnemies = 0;

        foreach (EnemyData enemyData in currentBattleEvent.enemies) {
            activeEnemise.Add(SpawnEnemy(enemyData));
        }
    }

    private void CompleteCurrentEvent()
    {
        currentBattleEvent = null;

        cameraFollows = true;
        cameraBounds.CalculateOffset(actor.transform.position.x);
        hasRemainingEvents = currentLevelData.battleData.Count > nextEventIndex;

        enemyLifeBar.EnableLifeBar(false);

        if (!hasRemainingEvents) {
            StartCoroutine(HeroWalkout());
        } else {
            ShowGoIndicator();
        }
    }

    private IEnumerator LoadLevelData(LevelData data) {
        cameraFollows = false;
        currentLevelData = data;

        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemise = new List<GameObject>();

        yield return null;
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        if (currentLevelBackground != null) {
            Destroy(currentLevelBackground);
        }
        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);

        cameraBounds.EnableBounds(false);
        actor.transform.position = walkInStartTarget.transform.position;
        actor.UseAutopilot(true);
        actor.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

        yield return new WaitForSeconds(0.1f);

        cameraFollows = true;
        ShowTextBanner(currentLevelData.levelName);
        loadingScreen.SetActive(false);
    }

    private void DidFinishIntro()
    {
        actor.UseAutopilot(false);
        actor.controllable = true;
        cameraBounds.EnableBounds(true);

        ShowGoIndicator();
    }

    private IEnumerator HeroWalkout() 
    {
        cameraBounds.EnableBounds(false);
        cameraFollows = false;

        actor.UseAutopilot(true);
        actor.controllable = false;
        actor.AnimateTo(walkOutTarget.transform.position, true, DidFinishWalkout);

        yield return null;
    }

    private void DidFinishWalkout()
    {
        CurrenLevel++;
        if (CurrenLevel >= levels.Length) {
            Victory();
        } else {
            StartCoroutine(AnimateNextLevel());
        }

        cameraBounds.EnableBounds(true);
        cameraFollows = false;
        actor.UseAutopilot(false);
        actor.controllable = false;
    }

    private IEnumerator AnimateNextLevel()
    {
        ShowTextBanner(currentLevelData.levelName + " COMPLETED");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Game");
    }

    private void ShowGoIndicator()
    {
        StartCoroutine(FlickerGoIndicator(4));
    }

    private IEnumerator FlickerGoIndicator(int count = 4)
    {
        while (count > 0)
        {
            goIndicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            goIndicator.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            count--;
        }
    }

    private void ShowBanner(string bannerText, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.GetComponent<Text>().text = bannerText;
        RectTransform rectTransform = obj.transform as RectTransform;
        rectTransform.SetParent(uiTransform);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void GameOver()
    {
        ShowBanner("GAME OVER", gameOverPrefab);
    }

    public void Victory()
    {
        ShowBanner("YOU WON", gameOverPrefab);
    }

    public void ShowTextBanner(string levelName)
    {
        ShowBanner(levelName, levelNamePrefab);
    }
}
