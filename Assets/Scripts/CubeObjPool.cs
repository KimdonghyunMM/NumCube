using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObjPool : MonoBehaviour
{
    //�̱��� ����
    public static CubeObjPool instance;

    private Queue<CubeController> _cubesQueue = new Queue<CubeController>();
    [SerializeField] private int _cubeQueueCapacity = 20;
    [SerializeField] private bool _autoQueueGrow = true;

    [SerializeField] private GameObject _cubePref;
    [SerializeField] private Color[] _cubeColors;

    [HideInInspector] public int _maxCubeNum;                   //�ִ� 4096���� ����������. 4096 = 2^12 �̹Ƿ� �ִ� 12���� ������

    private int _maxPower = 12;                                  //�������� ���� ���������

    private Vector3 _defaultSpawnPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _defaultSpawnPos = transform.position;
        //Pow(x,y) -> x�� y����
        _maxCubeNum = (int)Mathf.Pow(2, _maxPower);             

        InitializeCubesQueue();
    }

    private void InitializeCubesQueue()
    {
        for (var i = 0; i < _cubeQueueCapacity; i++)
        {
            AddCubeToQueue();
        }
    }

    private void AddCubeToQueue()
    {
        var cube = Instantiate(_cubePref, _defaultSpawnPos, Quaternion.identity, transform).GetComponent<CubeController>();

        cube.gameObject.SetActive(false);
        cube.isMainCube = false;
        _cubesQueue.Enqueue(cube);
    }

    public CubeController Spawn(int num, Vector3 pos)
    {
        //Ǯ�� ���� ť�갡 ������ ť�긦 �����Ѵ�
        if(_cubesQueue.Count == 0)
        {
            if (_autoQueueGrow)
            {
                _cubeQueueCapacity++;
                AddCubeToQueue();
            }
            else
            {
                Debug.LogError("Ǯ���� ����� �� �ִ� ť����� �����ϴ�.");
                return null;
            }
        }
     

        var cube = _cubesQueue.Dequeue();
        cube.transform.position = pos;
        cube.SetNum(num); ;
        cube.SetColor(GetColor(num));
        cube.gameObject.SetActive(true);

        return cube;
    }

    public CubeController SpawnRnd()
    {
        return Spawn(GenerateRndNum(), _defaultSpawnPos);
    }

    public void DestroyCube( CubeController cube)
    {
        //ť�갡 �ı��Ǵ� ���� �ƴ� Ǯ�� ��������
        cube._cubeRb.velocity = Vector3.zero;
        cube._cubeRb.angularVelocity = Vector3.zero;
        cube.transform.rotation = Quaternion.identity;
        cube.isMainCube = false;
        cube.gameObject.SetActive(false);
        _cubesQueue.Enqueue(cube);
    }

    public int GenerateRndNum()
    {
        //������ �Ǵ� ���ڴ� 2���� 32 �� ���� (2^1, 2^2, 2^3 ... 2^6)
        return (int)Mathf.Pow(2, Random.Range(1, 6));           
    }

    private Color GetColor(int num)
    {
        //���ڰ� �� ���������� �Ǻ��Ͽ� ť����� ������
        return _cubeColors[(int)(Mathf.Log(num) / Mathf.Log(2)) - 1];
    }
}
