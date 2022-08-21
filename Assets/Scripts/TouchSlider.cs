using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityAction _onPointerDownEvent;
    public UnityAction<float> _onPointerDragEvent;
    public UnityAction _onPointerUpEvent;

    private Slider _uiSlider;

    private void Awake()
    {
        _uiSlider = GetComponent<Slider>();
        _uiSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //마우스 클릭(다운) 이벤트 콜백
        //마우스 포인터를 클릭할 때 드래그 콜백도 함께 실행
        _onPointerDownEvent?.Invoke();
        _onPointerDragEvent?.Invoke(_uiSlider.value);
    }
    public void OnSliderValueChanged(float value)
    {
        //드래그 이벤트 콜백
        _onPointerDragEvent?.Invoke(value);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //마우스 클릭(업) 이벤트 콜백
        _onPointerUpEvent?.Invoke();
        //마우스를 때면 슬라이더는 가운데로 돌아옴
        _uiSlider.value = 0.0f;
    }

    private void OnDestroy()
    {
        _uiSlider.onValueChanged.RemoveAllListeners();
    }
}
