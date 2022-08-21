using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    CubeController _cube;
    private void Awake()
    {
        _cube = GetComponent<CubeController>(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        var otherCube = collision.gameObject.GetComponent<CubeController>();

        //�ٸ� ť��� �浹 ���� Ȯ��
        if(otherCube != null && _cube._cubeIdx > otherCube._cubeIdx) 
        {
            //�� ť��� �浹�� ť�갡 ���� ���ڶ��
            if(_cube._cubeNum == otherCube._cubeNum)
            {
                //�浹 ������ ù��°�� ���� �� (���� ���� �浹)
                Vector3 collidedPoint = collision.contacts[0].point;

                //ť�갡 ������ �ִ���ں��� �浹�� ť���� ���ڰ� ������
                if (otherCube._cubeNum < CubeObjPool.instance._maxCubeNum)
                {
                    //2�� ū ���ο� ť�긦 ������ (���� ȿ���� ���� ���� ������ �浹 �������� ���� �� ������ ����)
                    var newCube = CubeObjPool.instance.Spawn(_cube._cubeNum * 2, collidedPoint + Vector3.up * 1.6f);

                    //���� �������� Ƣ������� ȿ��
                    float pushForce = 2.5f;
                    newCube._cubeRb.AddForce(new Vector3(0, 0.3f, 1f) * pushForce, ForceMode.Impulse);

                    float rndTorqueValue = Random.Range(-20f, 20f);
                    var rndDir = Vector3.one * rndTorqueValue;
                    newCube._cubeRb.AddTorque(rndDir);
                }

                //ť�갡 �������� ���� ȿ���� �ָ�, �ֺ� ť�꿡�� ���߿� ���� ������ ��
                Collider[] surroundedCubes = Physics.OverlapSphere(collidedPoint, 2f);
                float explosionForce = 400f;
                float explosionRadius = 1.5f;

                for(var i = 0; i < surroundedCubes.Length; i++)
                {
                    if (surroundedCubes[i].attachedRigidbody != null)
                        surroundedCubes[i].attachedRigidbody.AddExplosionForce(explosionForce, collidedPoint, explosionRadius);
                }

                //ȿ�� �ֱ�
                FXManager.instance.PlayCubeExplosionFX(collidedPoint, _cube._cubeColor);

                //�浹�� ť��� ����
                CubeObjPool.instance.DestroyCube(_cube);
                CubeObjPool.instance.DestroyCube(otherCube);
            }
        }
    }
}
