using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _pushForce;
    [SerializeField] private float _cubeMaxPosX;    //ť���� �ִ� �ൿ����
    [Space]
    [SerializeField] private TouchSlider _touchSlider;

    private CubeController _mainCube;
    private bool _isPointerDown;
    private bool _canClick;
    private Vector3 _cubePos;

    private void Start()
    {
        //�� ť�� ��ȯ
        SpawnCube();
        _canClick = true;

        //�̺�Ʈ �����ʿ� �׼� ���
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
        //�׼� �Ҵ� ����
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

            //ť�� ������
            _mainCube._cubeRb.AddForce(Vector3.forward * _pushForce, ForceMode.Impulse);

            //ť�긦 ������ 0.3�� �� ���ο� ť�긦 ������
            StartCoroutine(SpawnNewCube());
        }
    }

    private void SpawnCube()
    {
        _mainCube = CubeObjPool.instance.SpawnRnd();
        _mainCube.isMainCube = true;

        //ť�� ������
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
