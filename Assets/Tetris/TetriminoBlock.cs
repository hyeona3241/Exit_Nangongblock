using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetrisGame;
using Unity.VisualScripting;

public class TetriminoBlock : MonoBehaviour
{
    [SerializeField]
    public BlockType blockType;    //�� ����

    private Material blockMaterial; //�� ��Ƽ����


    // ��Ʈ���� �׺������
    [SerializeField] private TetrisNavigator navigator;
    [SerializeField] private Material navigatorGhostMaterial;

    [SerializeField]
    private Vector3Int localPosition;   //���� Ÿ���� ��ġ

    [SerializeField]
    private bool isSelect;   //���� ���� ���� ���ΰ�

    private bool isLock; // ���� ��ġ�Ǿ� ���������ΰ�

    
    public BlockShapes shapeType; //�� ���

    private Vector3[] shapeData;    //�� ��翡 ���� ����

    [SerializeField]
    private TetriminoBlockChild tetriminoBlock;


    [SerializeField]
    private float fallInterval = 0.5f;  //�������� �ӵ�
    private float fallTimer = 0f;


    private float inputWhileLandedTimer = 0f;
    private float lockDelay = 1f;   // 1�ʰ� �Է� ������ ��
    private bool isLanded = false; // �ٴڿ� ��Ҵ��� ����

    // �ڽ� ��
    private TetriminoBlockChild[] tetriminoBlockChild;

    void Awake()
    {
        isSelect = false;

        //�� ��� ��������
        shapeType = GetRandomShapeType();
        shapeData = BlockData.Shapes[shapeType];


        //�� Ÿ�� ó���� None ǥ��
        blockType = BlockType.None;

        //�� ��Ƽ���� ����
        blockType = GetRandomBlockType();
        blockMaterial = BlockMaterialBinder.Materials[blockType];
    }

    // Start is called before the first frame update
    void Start()
    {
        // �� ���� ���� �� ��Ƽ���� ����
        SpawnBlockVisual();

        fallInterval = TetrisManager.Instance.fallInterval;


        if (navigator == null)
        {
            // �� �ڽ����� Navigator�� �ϳ� ���δ� (�ν����Ϳ� �̸� ��ġ�ص� OK)
            var go = new GameObject("TetrisNavigator");
            go.transform.SetParent(this.transform, false);
            navigator = go.AddComponent<TetrisNavigator>();
        }
        navigator.Initialize(this, navigatorGhostMaterial);
        navigator.gameObject.SetActive(isSelect);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSelect) return;


        if (blockType == BlockType.None)
        {
            //None�� ��� �߸��� �� ó�� ���
        }

        if (isLock)
        {
            // ���� ���� ���� ó��
        }

        if (!CanMove(Vector3.down))
        {
            if (!isLanded)
            {
                isLanded = true;
                inputWhileLandedTimer = 0f;
            }

            inputWhileLandedTimer += Time.deltaTime;

            // �Է��� ���� ���¿��� ���� �ð� ������ ����
            if (inputWhileLandedTimer >= lockDelay)
            {
                BlockLock();
                isLanded = false;
            }
        }
        else
        {
            // �ٴڿ��� ���������� �ʱ�ȭ
            isLanded = false;

            // �ڵ� ����
            fallTimer += Time.deltaTime;
            if (fallTimer >= fallInterval)
            {
                Move(Vector3.down);
                fallTimer = 0f;
            }
        }
    }

    public void SetIsSelet(bool selet)
    {
        isSelect = selet;

        if (navigator != null)
            navigator.gameObject.SetActive(isSelect);
    }

    public void BlockLock()
    {
        isSelect = false;
        isLock = true;

        int towerHeight = 8; // y �ִ� ����
        foreach (var child in tetriminoBlockChild)
        {
            if (child == null) continue;
            Vector3Int pos = WorldToTowerOffset(child.transform.position); // ��ġ��Ÿ���� ��ǥ ��ȯ
            if (pos.y >= towerHeight)   // �����(��ȿ���� ��)�� ���ӿ���
            {
                TetrisManager.Instance.GameOver();
                return; // �� �������� ���� (Ÿ�� ���/���� ���� X)
            }
        }

        foreach (var child in tetriminoBlockChild)
        {
            if (child != null)
            {
                // �ڽ� ���� ���� ��ǥ �� Ÿ�� ��ǥ
                Vector3Int towerPos = WorldToTowerOffset(child.transform.position);

                // Ÿ���� ���
                TetrisManager.Instance.tower.AddBlockToTower(towerPos);

                child.BlockLock();
            }
        }

        if (navigator != null)
        {
            Destroy(navigator.gameObject);
            navigator = null;
        }

        TetrisManager.Instance.CheckTower();
        TetrisManager.Instance.SpawnNextBlock();
    }

    public void DeletBlock()
    {
        foreach (var child in tetriminoBlockChild)
        {
            child?.DeletBlock();
        }

        Destroy(gameObject);
    }

    // �� ��� ����
    private BlockShapes GetRandomShapeType()
    {
        BlockShapes[] values = (BlockShapes[])System.Enum.GetValues(typeof(BlockShapes));
        return values[Random.Range(0, values.Length)];
    }

    // �� Ÿ�� �������� �����ֱ�
    private BlockType GetRandomBlockType()
    {
        BlockType[] values = (BlockType[])System.Enum.GetValues(typeof(BlockType));

        // None�� ����
        List<BlockType> valid = new(values);
        valid.Remove(BlockType.None);

        return valid[Random.Range(0, valid.Count)];
    }

    // �� ��� ���� && ��Ƽ���� �ҷ�����
    private void SpawnBlockVisual()
    {
        if (shapeData == null || shapeData.Length == 0 || tetriminoBlock == null)
            return;

        tetriminoBlockChild = new TetriminoBlockChild[shapeData.Length];

        for (int i = 0; i < shapeData.Length; i++)
        {
            Vector3 pos = shapeData[i];

            // �ڽ� ��� ������ �ν��Ͻ�ȭ
            GameObject childObj = Instantiate(tetriminoBlock.gameObject, this.transform);
            childObj.transform.localPosition = pos;

            // ������Ʈ ���� �� ����
            TetriminoBlockChild child = childObj.GetComponent<TetriminoBlockChild>();
            if (child != null)
            {
                child.SetBlockType(blockType);
                child.SetBlockMaterial(blockMaterial);
                tetriminoBlockChild[i] = child;
            }
        }
    }

    // �̵�
    public void Move(Vector3 direction)
    {
        if (isSelect)
        {
            transform.position += direction;
            Vector3Int delta = WorldToTowerOffset(direction);
            localPosition += delta;

            inputWhileLandedTimer = 0f;
        }
    }

    // Ÿ�� ��ǥ�� ����
    private Vector3Int WorldToTowerOffset(Vector3 worldDir)
    {
        Vector3 localToTower = worldDir - TetrisManager.Instance.tower.transform.position;

        return new Vector3Int(
            Mathf.FloorToInt(localToTower.x + 0.0001f),
            Mathf.FloorToInt(localToTower.y + 0.0001f),
            Mathf.FloorToInt(localToTower.z + 0.0001f)
        );
    }


    public bool CanMove(Vector3 direction)
    {
        foreach (var child in tetriminoBlockChild)
        {
            if (child == null) continue;

            Vector3 targetPos = child.transform.position + direction;
            Vector3Int towerPos = WorldToTowerOffset(targetPos);

            if (!TetrisManager.Instance.tower.IsInsideTower(towerPos)) return false;
            if (TetrisManager.Instance.tower.IsFilled(towerPos)) return false;
        }

        return true;
    }


    //���� ������ �� ��ġ ����
    public void ApplyLineClear(int clearedY)
    {
        if (tetriminoBlockChild == null)
        {
            CleanupIfEmpty();
            return;
        }

        foreach (var child in tetriminoBlockChild)
        {
            if (child == null) continue;

            Vector3Int towerPos = WorldToTowerOffset(child.transform.position);

            if (towerPos.y == clearedY)
            {
                child.DeletBlock(); // �ش� ������ ����̸� ����
            }
            else if (towerPos.y > clearedY)
            {
                // �� ���� ������ �� ĭ �Ʒ���
                child.transform.position += Vector3.down;
            }
        }
    }

    // �ڽĵ��� ����ٸ� ������ ����
    public void CleanupIfEmpty()
    {
        int alive = 0;
        if (tetriminoBlockChild != null)
        {
            foreach (var c in tetriminoBlockChild)
            {
                if (c != null) alive++;
            }
        }

        if (alive == 0)
            Destroy(gameObject);
    }

    public void SetLocalPosition(Vector3Int position)
    {
        localPosition = position;
    }

    public void RotateX()
    {
        // ȸ�� �� ��ȿ ��ġ���� Ȯ��
        transform.Rotate(Vector3.right * 90f, Space.World);

        if (!CanMove(Vector3.zero)) // ȸ�� �� �浹 üũ
        {
            // �Ұ����ϸ� �ǵ�����
            transform.Rotate(Vector3.right * -90f, Space.World);
        }

        // �ڽ� ȸ�� �߰��ؼ� ��Ƽ���� �ȵ��ư����� �߰��ϱ�
    }

    public void RotateY()
    {
        transform.Rotate(Vector3.up * 90f, Space.World);

        if (!CanMove(Vector3.zero)) // ȸ�� �� �浹 üũ
        {
            transform.Rotate(Vector3.up * -90f, Space.World);
        }
    }

    public void RotateZ()
    {
        transform.Rotate(Vector3.forward * 90f, Space.World);

        if (!CanMove(Vector3.zero)) // ȸ�� �� �浹 üũ
        {
            transform.Rotate(Vector3.forward * -90f, Space.World);
        }
    }
}
