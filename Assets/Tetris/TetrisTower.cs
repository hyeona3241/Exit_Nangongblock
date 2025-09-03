using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisTower : MonoBehaviour
{
    private int[,,] towerGrid;
    private Vector3Int towerSize;
    // Start is called before the first frame update

    private void Awake()
    {
        towerSize = TetrisManager.Instance.tetrisTowerSize;
    }

    void Start()
    {
        towerGrid = new int[towerSize.x, towerSize.y, towerSize.z];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Ư�� ��ġ�� ä�� ���·� ǥ��
    public void AddBlockToTower(Vector3Int blockPos)
    {
        if (IsInsideTower(blockPos))
        {
            towerGrid[blockPos.x, blockPos.y, blockPos.z] = 1;
        }
    }

    // Ÿ�� �ȿ� �����ϴ°�
    public bool IsInsideTower(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < towerSize.x &&
               pos.y >= 0 && pos.y < towerSize.y &&
               pos.z >= 0 && pos.z < towerSize.z;
    }

    // �ش� ��ǥ�� ���� �ִ°�
    public bool IsFilled(Vector3Int pos)
    {
        return towerGrid[pos.x, pos.y, pos.z] != 0;
    }

    //�ش� ���� �� ä�������� �˻�
    public bool IsLineFull(int y)
    {
        if (y > towerSize.y) return false;

        for (int x = 0; x < towerSize.x; x++)
        {
            for (int z = 0; z < towerSize.z; z++)
            {
                if (towerGrid[x, y, z] == 0)
                    return false;
            }
        }
        return true;
    }

    // �ش� ���� ����
    public void DeleteLine(int y)
    {
        if (y > towerSize.y) return;

        Debug.Log("Delete Line");

        // y������ ������ �� ���� �Ʒ��� �̵�
        for (int currY = y; currY < towerSize.y - 1; currY++)
        {
            for (int x = 0; x < towerSize.x; x++)
            {
                for (int z = 0; z < towerSize.z; z++)
                {
                    towerGrid[x, currY, z] = towerGrid[x, currY + 1, z];
                }
            }
        }

        // �ֻ�� ������ ���
        for (int x = 0; x < towerSize.x; x++)
        {
            for (int z = 0; z < towerSize.z; z++)
            {
                towerGrid[x, towerSize.y - 1, z] = 0;
            }
        }

        var blocks = FindObjectsOfType<TetriminoBlock>();
        foreach (var b in blocks)
        {
            b.ApplyLineClear(y);
            b.CleanupIfEmpty();
        }
    }

    // ��ü Ÿ�� �˻� (�� �� �ɶ����� �Ŵ������� �� ȣ��ǰ�)
    public void CheckAndDeleteFullLines()
    {
        Debug.Log("check Delete Full Line");

        for (int y = 0; y < towerSize.y; y++)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                y--; // �� ���� ���������� ���� y�� �ٽ� �˻�
            }
        }
    }

    // ���� ��ġ ��ȯ
    public Vector3 GetSpawnPosition()
    {
        int spawnX = (towerSize.x - 1) / 2;
        int spawnZ = (towerSize.z - 1) / 2;
        int spawnY = towerSize.y - 1;

        Vector3 spawnPos = transform.position + new Vector3(spawnX, spawnY, spawnZ);
        Debug.Log("[TetrisTower] Spawn Position: " + spawnPos);
        return spawnPos;
    }
}
