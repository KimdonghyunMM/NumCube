using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _pushForce;
    [SerializeField] private float _cubeMaxPosX;    //큐브의 최대 행동범위
    [Space]
    [SerializeField] private TouchSlider _touchSlider;

    private CubeController _mainCube;
    private bool _isPointerDown;
    private bool _canClick;
    private Vector3 _cubePos;

    private void Start()
    {
        //새 큐브 소환
        SpawnCube();
        _canClick = true;

        //이벤트 리스너에 액션 등록
        _touchSlider._onPointerDownEvent += OnPointerDown;
        _touchSlider._onPointerDragEvent += OnPointerDrag;
        _touchSlider._onPointerUpEvent += OnPointerUp;
    }

    private void Update()
    {
        if (_isPointerDown)
            _mainCube.transform.position = Vector3.Lerp(_mainCube.transform.position, _cubePos, _moveSpd * Time.deltaTime);
    }

    private void OnDestroy()
    {
        //액션 할당 해제
        _touchSlider._onPointerDownEvent -= OnPointerDown;
        _touchSlider._onPointerDragEvent -= OnPointerDrag;
        _touchSlider._onPointerUpEvent -= OnPointerUp;
    }

    private void OnPointerDown()
    {
        _isPointerDown = true;
    }
    private void OnPointerDrag(float xMoveValue)
    {
        if (_isPointerDown)
        {
            _cubePos = _mainCube.transform.position;
            _cubePos.x = xMoveValue * _cubeMaxPosX;
        }
    }

    private void OnPointerUp()
    {
        if (_isPointerDown && _canClick)
        {
            _isPointerDown = false;
            _canClick = false;

            //큐브 날리기
            _mainCube._cubeRb.AddForce(Vector3.forward * _pushForce, ForceMode.Impulse);

            //큐브를 날리고 0.3초 후 새로운 큐브를 꺼낸다
            StartCoroutine(SpawnNewCube());
        }
    }

    private void SpawnCube()
    {
        _mainCube = CubeObjPool.instance.SpawnRnd();
        _mainCube.isMainCube = true;

        //큐브 재정렬
        _cubePos = _mainCube.transform.position;
    }

    private IEnumerator SpawnNewCube()
    {
        yield return new WaitForSeconds(0.3f);
        _mainCube.isMainCube = false;
        _canClick = true;
        SpawnCube();
    }
}
