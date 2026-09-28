using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class bl_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    public float Radio = 50f; // Increase this to 50 for better movement range
    public float SmoothTime = 0.5f;
    public float OnPressScale = 1.2f;
    public Color NormalColor = Color.white;
    public Color PressColor = Color.gray;
    public float Duration = 0.1f;

    [Header("Reference")]
    public RectTransform StickRect;
    public RectTransform CenterReference;

    private Vector3 DeathArea;
    private Vector3 currentVelocity;
    private bool isFree = false;
    private int lastId = -2;
    private Canvas m_Canvas;
    private Image stickImage;
    private Image backImage;

    void Start()
    {
        m_Canvas = GetComponentInParent<Canvas>();
        
        // Setup initial positions
        if(CenterReference != null) 
            DeathArea = CenterReference.position;
        else
            DeathArea = transform.position;

        backImage = GetComponent<Image>();
        if(StickRect != null) stickImage = StickRect.GetComponent<Image>();
        
        // Reset stick to center
        StickRect.position = DeathArea;
    }

    void Update()
    {
        if (!isFree) return;

        // Smoothly return to center when released
        StickRect.position = Vector3.SmoothDamp(StickRect.position, DeathArea, ref currentVelocity, (1 - SmoothTime));
        
        if (Vector3.Distance(StickRect.position, DeathArea) < .1f)
        {
            isFree = false;
            StickRect.position = DeathArea;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (lastId == -2)
        {
            lastId = data.pointerId;
            StopAllCoroutines();
            OnDrag(data);
            if(backImage != null) backImage.color = PressColor;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == lastId)
        {
            isFree = false;
            Vector3 position;
            // Correct way to get touch position on UI
            RectTransformUtility.ScreenPointToWorldPointInRectangle(StickRect, data.position, data.pressEventCamera, out position);

            float dist = Vector3.Distance(DeathArea, position);
            if (dist < Radio)
                StickRect.position = position;
            else
                StickRect.position = DeathArea + (position - DeathArea).normalized * Radio;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == lastId)
        {
            isFree = true;
            lastId = -2;
            if(backImage != null) backImage.color = NormalColor;
        }
    }

    // These are the values your PlayerMovement script reads
    public float Horizontal => (StickRect.position.x - DeathArea.x) / Radio;
    public float Vertical => (StickRect.position.y - DeathArea.y) / Radio;
}