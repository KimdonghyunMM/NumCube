using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObjPool : MonoBehaviour
{
    //싱글턴 선언
    public static CubeObjPool instance;

    private Queue<CubeController> _cubesQueue = new Queue<CubeController>();
    [SerializeField] private int _cubeQueueCapacity = 20;
    [SerializeField] private bool _autoQueueGrow = true;

    [SerializeField] private GameObject _cubePref;
    [SerializeField] private Color[] _cubeColors;

    [HideInInspector] public int _maxCubeNum;                   //최대 4096까지 넣을예정임. 4096 = 2^12 이므로 최대 12까지 설정함

    private int _maxPower = 12;                                  //↑↑↑↑↑↑↑ 참조 ↑↑↑↑↑↑↑↑

    private Vector3 _defaultSpawnPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _defaultSpawnPos = transform.position;
        //Pow(x,y) -> x의 y제곱
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
        //풀에 남은 큐브가 없으면 큐브를 생성한다
        if(_cubesQueue.Count == 0)
        {
            if (_autoQueueGrow)
            {
                _cubeQueueCapacity++;
                AddCubeToQueue();
            }
            else
            {
                Debug.LogError("풀에서 사용할 수 있는 큐브들이 없읍니다.");
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
        //큐브가 파괴되는 것이 아닌 풀로 돌려보냄
        cube._cubeRb.velocity = Vector3.zero;
        cube._cubeRb.angularVelocity = Vector3.zero;
        cube.transform.rotation = Quaternion.identity;
        cube.isMainCube = false;
        cube.gameObject.SetActive(false);
        _cubesQueue.Enqueue(cube);
    }

    public int GenerateRndNum()
    {
        //스폰이 되는 숫자는 2부터 32 중 랜덤 (2^1, 2^2, 2^3 ... 2^6)
        return (int)Mathf.Pow(2, Random.Range(1, 6));           
    }

    private Color GetColor(int num)
    {
        //숫자가 몇 제곱수인지 판별하여 큐브색을 결정함
        return _cubeColors[(int)(Mathf.Log(num) / Mathf.Log(2)) - 1];
    }
}
