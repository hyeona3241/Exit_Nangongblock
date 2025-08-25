using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TetrisGame;
using System.Xml.Serialization;

public class TetrisManager : MonoBehaviour
{
    public static TetrisManager Instance;

    public Vector3Int tetrisTowerSize = new Vector3Int(4, 8, 4);

    public float fallInterval = 0.5f;

    public TetrisTower tower;

    public TetrisSpawner spawner;

    public TetrisController controller;

    private int[] typeBlockCount = new int[(int)BlockType.None];

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPos = tower.GetSpawnPosition();
        spawner.SetTowerSpawnPosition(spawnPos);
        SpawnNextBlock();

        for (int i = 0; i < typeBlockCount.Length; i++)
        { typeBlockCount[i] = 0; }

    }


    //Ÿ�Ժ� �� ���� ī��Ʈ
    public void IncreaseTypeBlockCount(BlockType type)
    {
        typeBlockCount[(int)type]++;
    }

    public void DecreaseTypeBlockCount(BlockType type)
    {
        typeBlockCount[(int)type]--;
    }

    public void SpawnNextBlock()
    {
        spawner.SpawnBlock();
        controller.SetCurrentBlock(spawner.GetTetriminoBlock());
    }

    public void CheckTower()
    {
        tower.CheckAndDeleteFullLines();       //Ÿ�� �˻�
    }

    public void GameOver()
    {
        Debug.Log("[Tetris] GAME OVER");
        // TODO: ���ӿ��� UI, �Է� ����, ����� ó�� ��
    }

}
