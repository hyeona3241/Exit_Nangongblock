using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisController : MonoBehaviour
{
    // ���� ���� ī�޶�
    public Camera gameCamera;
    // ���� �������� ��
    public TetriminoBlock currentBlock;

    private InputManager input;

    // �Է� ó�� Ȱ��ȭ
    private void OnEnable()
    {
        input = InputManager.Instance ?? FindObjectOfType<InputManager>();
        if (input == null)
        {
            Debug.LogError("[TetrisController] InputManager�� ���� �����ϴ�.");
            enabled = false;
            return;
        }

        input.OnLeftArrow += MoveLeft;
        input.OnRightArrow += MoveRight;
        input.OnUpArrow += MoveForward;
        input.OnDownArrow += MoveBack;
        input.OnKeyF += SoftDrop;
        input.OnSpace += HardDrop;
        input.OnKeyA += XRotate;
        input.OnKeyS += YRotate;
        input.OnKeyD += ZRotate;
        input.OnKeyC += BlockChange;
    }


    // �Է� ó�� ��Ȱ��ȭ
    private void OnDisable()
    {
        if (input == null) return;
        input.OnLeftArrow -= MoveLeft;
        input.OnRightArrow -= MoveRight;
        input.OnUpArrow -= MoveForward;
        input.OnDownArrow -= MoveBack;
        input.OnKeyF -= SoftDrop;
        input.OnSpace -= HardDrop;
        input.OnKeyA -= XRotate;
        input.OnKeyS -= YRotate;
        input.OnKeyD -= ZRotate;
        input.OnKeyC -= BlockChange;
    }

    private void MoveLeft() 
    {
        if (currentBlock == null) return;
        Vector3 dir = GetCameraRelativeDirection(Vector3.left);
        if (currentBlock.CanMove(dir)) currentBlock.Move(dir);
    }
    private void MoveRight()
    {
        if (currentBlock == null) return;
        Vector3 dir = GetCameraRelativeDirection(Vector3.right);
        if (currentBlock.CanMove(dir)) currentBlock.Move(dir);
    }
    private void MoveForward()
    {
        if (currentBlock == null) return;
        Vector3 dir = GetCameraRelativeDirection(Vector3.back);
        if (currentBlock.CanMove(dir)) currentBlock.Move(dir);
    }
    private void MoveBack()
    {
        if (currentBlock == null) return;
        Vector3 dir = GetCameraRelativeDirection(Vector3.forward);
        if (currentBlock.CanMove(dir)) currentBlock.Move(dir);
    }

    private void SoftDrop() 
    {
        if (currentBlock == null) return;
        Vector3 dir = Vector3.down;
        if (currentBlock.CanMove(dir)) currentBlock.Move(dir);
    }

    private void HardDrop() 
    {
        if (currentBlock == null) return;

        // �ִ� �ϰ� Ƚ�� = Ÿ�� ����
        int guard = TetrisManager.Instance.tetrisTowerSize.y;

        while (guard-- > 0 && currentBlock.CanMove(Vector3.down))
        {
            currentBlock.Move(Vector3.down); // �� ĭ�� ��� ���� (�� �����ӿ� ó���� -> ���� ���̴� �̵� ����)
        }

        currentBlock.BlockLock(); // �ٴ�/���� ������� �ٷ� ��
    }

    private void XRotate()
    {
        if (currentBlock == null) return;
        currentBlock.RotateX();
    }

    private void YRotate()
    {
        if (currentBlock == null) return;
        currentBlock.RotateY();
    }

    private void ZRotate()
    {
        if (currentBlock == null) return;
        currentBlock.RotateZ();
    }

    private void BlockChange() 
    {
        if (currentBlock == null) return;

        var spawner = FindObjectOfType<TetrisSpawner>();
        if (spawner == null) return;

        Vector3 target = currentBlock.transform.position;
        bool ok = spawner.TrySwapWithNext(target);

        if (ok)
        {
            currentBlock = spawner.GetTetriminoBlock();
        }
    }

    private Vector3 GetCameraRelativeDirection(Vector3 inputDir)
    {
        Vector3 forward = gameCamera.transform.forward;
        Vector3 right = gameCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // �Է� ������ ī�޶� �������� ��ȯ
        Vector3 moveDir = inputDir.x * right + inputDir.z * forward;
                
        return moveDir;
    }

    public void SetCurrentBlock(TetriminoBlock block)
    {
        currentBlock = block;
    }

}
