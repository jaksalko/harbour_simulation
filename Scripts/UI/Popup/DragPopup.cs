using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragPopup : MonoBehaviour , IDragHandler , IBeginDragHandler , IEndDragHandler
{

    public RectTransform targetTransform;
  

    /// <summary>Should you be able to drag horizontally?</summary>
    public bool Horizontal { set { horizontal = value; } get { return horizontal; } }
    [SerializeField] private bool horizontal = true;

    /// <summary>Should the horizontal position value be clamped?</summary>
    public bool HorizontalClamp { set { horizontalClamp = value; } get { return horizontalClamp; } }
    [SerializeField] private bool horizontalClamp;

    /// <summary>The minimum position value.</summary>
    public float HorizontalMin { set { horizontalMin = value; } get { return horizontalMin; } }
    [SerializeField] private float horizontalMin;

    /// <summary>The maximum position value.</summary>
    public float HorizontalMax { set { horizontalMax = value; } get { return horizontalMax; } }
    [SerializeField] private float horizontalMax;

    /// <summary>If you want the position to be magnetized toward the min/max value, then this allows you to set the speed.
    /// -1 = no magnet.</summary>
    public float HorizontalMagnet { set { horizontalMagnet = value; } get { return horizontalMagnet; } }
    [SerializeField] private float horizontalMagnet = -1.0f;

    /// <summary>Should you be able to drag vertically?</summary>
    public bool Vertical { set { vertical = value; } get { return vertical; } }
    [SerializeField] private bool vertical = true;

    /// <summary>Should the vertical position value be clamped?</summary>
    public bool VerticalClamp { set { verticalClamp = value; } get { return verticalClamp; } }
    [SerializeField] private bool verticalClamp;

    /// <summary>The minimum position value.</summary>
    public float VerticalMin { set { verticalMin = value; } get { return verticalMin; } }
    [SerializeField] private float verticalMin;

    /// <summary>The maximum position value.</summary>
    public float VerticalMax { set { verticalMax = value; } get { return verticalMax; } }
    [SerializeField] private float verticalMax;

    /// <summary>If you want the position to be magnetized toward the min/max value, then this allows you to set the speed.
    /// -1 = no magnet.</summary>
    public float VerticalMagnet { set { verticalMagnet = value; } get { return verticalMagnet; } }
    [SerializeField] private float verticalMagnet = -1.0f;



    // Is this element currently being dragged?
    [System.NonSerialized]
    protected bool dragging;

    [System.NonSerialized]
    private Vector2 startOffset;

    [System.NonSerialized]
    private Vector2 currentPosition;

    [System.NonSerialized]
    private RectTransform cachedRectTransform;

    [System.NonSerialized]
    private bool cachedRectTransformSet;


    public void OnBeginDrag(PointerEventData eventData)
    {
        var vector = default(Vector2);
        var target = targetTransform;

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(target , eventData.position , eventData.pressEventCamera , out vector))
        {
            dragging = true;

            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging)
        {
            var oldVector = default(Vector2);
            var target = targetTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position - eventData.delta, eventData.pressEventCamera, out oldVector) == true)
            {
                var newVector = default(Vector2);

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out newVector) == true)
                {
                    var anchoredPosition = target.anchoredPosition;

                    currentPosition += (Vector2)(target.localRotation * (newVector - oldVector));

                    if (horizontal == true)
                    {
                        anchoredPosition.x = currentPosition.x;
                    }

                    if (vertical == true)
                    {
                        anchoredPosition.y = currentPosition.y;
                    }

                    //ClampPosition(ref anchoredPosition);

                    // Offset the anchored position by the difference
                    target.anchoredPosition = anchoredPosition;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
