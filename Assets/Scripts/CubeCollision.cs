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

        //다른 큐브와 충돌 여부 확인
        if(otherCube != null && _cube._cubeIdx > otherCube._cubeIdx) 
        {
            //내 큐브와 충돌한 큐브가 같은 숫자라면
            if(_cube._cubeNum == otherCube._cubeNum)
            {
                //충돌 지점은 첫번째로 닿은 점 (점과 점의 충돌)
                Vector3 collidedPoint = collision.contacts[0].point;

                //큐브가 가지는 최대숫자보다 충돌한 큐브의 숫자가 작으면
                if (otherCube._cubeNum < CubeObjPool.instance._maxCubeNum)
                {
                    //2배 큰 새로운 큐브를 생성함 (생성 효과를 위해 생성 지점은 충돌 지점보다 조금 더 위에서 생성)
                    var newCube = CubeObjPool.instance.Spawn(_cube._cubeNum * 2, collidedPoint + Vector3.up * 1.6f);

                    //랜덤 방향으로 튀어오르는 효과
                    float pushForce = 2.5f;
                    newCube._cubeRb.AddForce(new Vector3(0, 0.3f, 1f) * pushForce, ForceMode.Impulse);

                    float rndTorqueValue = Random.Range(-20f, 20f);
                    var rndDir = Vector3.one * rndTorqueValue;
                    newCube._cubeRb.AddTorque(rndDir);
                }

                //큐브가 합쳐질때 폭발 효과를 주며, 주변 큐브에도 폭발에 대한 영향이 감
                Collider[] surroundedCubes = Physics.OverlapSphere(collidedPoint, 2f);
                float explosionForce = 400f;
                float explosionRadius = 1.5f;

                for(var i = 0; i < surroundedCubes.Length; i++)
                {
                    if (surroundedCubes[i].attachedRigidbody != null)
                        surroundedCubes[i].attachedRigidbody.AddExplosionForce(explosionForce, collidedPoint, explosionRadius);
                }

                //효과 넣기
                FXManager.instance.PlayCubeExplosionFX(collidedPoint, _cube._cubeColor);

                //충돌한 큐브들 제거
                CubeObjPool.instance.DestroyCube(_cube);
                CubeObjPool.instance.DestroyCube(otherCube);
            }
        }
    }
}
