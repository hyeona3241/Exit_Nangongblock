using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetrisGame;

[System.Serializable]
public class BlockMaterialEntry
{
    public BlockType type;
    public Material material;
}



//Project Settings > Script Execution Order���� BlockMaterialBinder�� ����ϰ�
//Default Time���� ���� ����ǵ��� �켱������ ���� ������ ����
public class BlockMaterialBinder : MonoBehaviour
{
    [SerializeField]
    private List<BlockMaterialEntry> materialList;

    public static readonly Dictionary<BlockType, Material> Materials = new();

    private void Awake()
    {
        Materials.Clear();
        foreach (var entry in materialList)
        {
            if (!Materials.ContainsKey(entry.type))
            {
                Materials.Add(entry.type, entry.material);
            }
        }
    }
}
