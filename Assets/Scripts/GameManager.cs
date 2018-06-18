﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;

    public List<GameObject> activeEnemise;
    public Transform[] spawnPositions;

    public GameObject currentLevelBackground;

    public GameObject robotPrefab;

    void Start()
    {
        //cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(currentLevelData));
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
        GameObject enemyObj = Instantiate(robotPrefab);

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
        hasRemainingEvents = currentLevelData.battleData.Count > nextEventIndex;
    }

    private IEnumerator LoadLevelData(LevelData data) {
        cameraFollows = false;
        currentLevelData = data;

        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemise = new List<GameObject>();

        yield return null;
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);
        yield return new WaitForSeconds(0.1f);

        cameraFollows = true;
    }
}
