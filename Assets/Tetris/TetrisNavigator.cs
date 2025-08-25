using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisNavigator : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;   // ȸ�� 60% ���� (�ν����ͷ� �־ ��)
    private TetriminoBlock owner;                      // ���� ����ٴ� ���� ��
    private Transform ghostRoot;                       // ��Ʈ �ڽĵ��� ���� ��Ʈ
    private readonly List<Transform> ghostCubes = new();

    // �ܺο��� ������/��Ƽ���� �Ѱ� �ʱ�ȭ
    public void Initialize(TetriminoBlock owner, Material mat = null)
    {
        this.owner = owner;
        ghostMaterial = mat ?? ghostMaterial ?? CreateDefaultGhostMaterial();
        BuildGhostChildren();
    }

    // ��Ʈ �ڽĵ��� ���� �� �ڽ� ������ŭ ����� (ť�� ������Ƽ��)
    private void BuildGhostChildren()
    {
        // �ʱ�ȭ/����
        if (ghostRoot != null) Destroy(ghostRoot.gameObject);
        ghostRoot = new GameObject("GhostRoot").transform;
        ghostRoot.SetParent(transform, false);
        ghostCubes.Clear();

        // ���� ���� �ڽ� ����ŭ ��Ʈ ť�� ����
        foreach (Transform src in owner.transform)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var col = go.GetComponent<Collider>();
            if (col) col.enabled = false;

            var rend = go.GetComponent<Renderer>();
            if (rend) rend.sharedMaterial = ghostMaterial;

            var t = go.transform;
            t.SetParent(ghostRoot, false);
            t.localScale = src.localScale; // �ʿ�� ���� ������

            ghostCubes.Add(t);
        }
    }

    private void LateUpdate()
    {
        if (owner == null) return;

        // 1) �ִ� ���� �Ÿ� ���
        int fall = ComputeFallDistance();

        // 2) ��Ʈ �ڽĵ��� ��ġ�� ������ �ڽ� ��ġ + (�Ʒ��� fall)�� ����
        int i = 0;
        foreach (Transform src in owner.transform)
        {
            if (i >= ghostCubes.Count) break;
            ghostCubes[i].position = src.position + Vector3.down * fall;
            i++;
        }
    }

    // �ٴ�/��ġ ���� ��� ���������� �Ÿ� ��� (���� ���� �� ������)
    private int ComputeFallDistance()
    {

        // Ÿ�� ���̸�ŭ�� Ž��
        int maxProbe = TetrisManager.Instance.tetrisTowerSize.y + 2; // Height�� �Ʒ����� ������
        for (int d = 1; d <= maxProbe; d++)
        {
            if (WouldCollide(d)) return d - 1;
        }
        return 0;
    }

    // dĭ �Ʒ��� ���� �浹/��� ������ �˻�
    private bool WouldCollide(int d)
    {
        var tower = TetrisManager.Instance.tower;
        foreach (Transform src in owner.transform)
        {
            Vector3 world = src.position + Vector3.down * d;
            Vector3 local = world - tower.transform.position;
            Vector3Int cell = new Vector3Int(
                Mathf.RoundToInt(local.x),
                Mathf.RoundToInt(local.y),
                Mathf.RoundToInt(local.z)
            );

            if (!tower.IsInsideTower(cell)) return true;
            if (tower.IsFilled(cell)) return true;
        }
        return false;
    }

    // ȸ�� ��Ƽ���� ����
    private Material CreateDefaultGhostMaterial()
    {
        var mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        // ���� ������ ��� ����
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        return mat;
    }
}
