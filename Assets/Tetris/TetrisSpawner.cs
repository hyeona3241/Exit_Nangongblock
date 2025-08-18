using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSpawner : MonoBehaviour
{
    [SerializeField]
    private TetriminoBlock blockPrefab;

    [SerializeField]
    private TetriminoBlock nextBlock;
    [SerializeField]
    private TetriminoBlock currentBlock;

    [SerializeField]
    Vector3 spawnPosition;

    [SerializeField]
    Vector3 towerSpawnPosition;

    private void Awake()
    {
        nextBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
    }

    public void SetTowerSpawnPosition(Vector3 pos)
    {
        towerSpawnPosition = pos;
    }

    public void SpawnBlock()
    {
        currentBlock = nextBlock;

        TetriminoBlock newBlock = currentBlock.GetComponent<TetriminoBlock>();

        if (newBlock != null)
        {
            newBlock.transform.position = towerSpawnPosition;
            newBlock.SetIsSelet(true);
        }

        // 스폰 위치랑 타워 위치 다르니까 현재 블럭 타워 위치로 이동시키기
        nextBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
    }

    public TetriminoBlock GetTetriminoBlock()
    {
        return currentBlock;
    }
}
