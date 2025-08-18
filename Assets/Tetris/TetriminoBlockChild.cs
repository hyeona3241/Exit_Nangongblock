using System.Collections;
using System.Collections.Generic;
using TetrisGame;
using Unity.VisualScripting;
using UnityEngine;

public class TetriminoBlockChild : MonoBehaviour
{
    [SerializeField]
    private BlockType blockType;    //�� ����

    void Awake()
    {
        blockType = BlockType.None;
    }

    // �� Ÿ�� ����
    public void SetBlockType(BlockType blockType)
    {
        this.blockType = blockType;
    }

    // �� ��Ƽ���� ����
    public void SetBlockMaterial(Material material)
    {
        Renderer rend = this.GetComponent<Renderer>();
        if (rend != null && material != null)
            rend.material = material;
    }

    public void BlockLock()
    {
        //Ÿ�� �� ī��Ʈ
        TetrisManager.Instance.IncreaseTypeBlockCount(blockType);


        //Ÿ�Կ� ���� �����̴��� ����
    }

    public void DeletBlock()
    {
        // ��ȣ���� �����̴��� ����

        TetrisManager.Instance.DecreaseTypeBlockCount(blockType);

        Destroy(gameObject);
    }
}
