using System.Collections;
using System.Collections.Generic;
using TetrisGame;
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
            // �⺻ ���� ��ġ
            Vector3 spawnPos = towerSpawnPosition;

            // I ����̸� �� ĭ �� ���� ����
            if (newBlock.shapeType == BlockShapes.I) 
            {
                spawnPos += Vector3.up;
            }

            newBlock.transform.position = spawnPos;
            newBlock.SetIsSelet(true);
        }

        // ���� ��ġ�� Ÿ�� ��ġ �ٸ��ϱ� ���� �� Ÿ�� ��ġ�� �̵���Ű��
        nextBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
    }

    public TetriminoBlock GetTetriminoBlock()
    {
        return currentBlock;
    }

    public bool TrySwapWithNext(Vector3 targetWorldPos)
    {
        if (currentBlock == null || nextBlock == null) return false;

        // next�� ���� ��ġ�� �ű� �� �ִ��� �˻� (Ÿ�� ���/�浹 ����)
        Vector3 delta = targetWorldPos - nextBlock.transform.position;
        if (!nextBlock.CanMove(delta)) return false;  // �� ������ ���� �� ��

        // ����
        var oldCurrent = currentBlock;

        oldCurrent.SetIsSelet(false);                  // ���� current ��Ȱ��
        nextBlock.transform.position = targetWorldPos; // next�� ���� ��ġ��
        nextBlock.SetIsSelet(true);                    // ���� ��� ����
        currentBlock = nextBlock;                      // ���� �� ��ü

        // ���� current�� ��ġ(�ؽ�Ʈ �ڸ�)��
        oldCurrent.transform.position = spawnPosition;
        oldCurrent.SetIsSelet(false);
        nextBlock = oldCurrent;

        return true;
    }
}
