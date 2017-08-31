using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SwipeOnUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 mStart, mEnd, swipeVector;
    public float thresholdTime = 0.3f;
    public float time = 0f;
    public float minSwipeDist = 200f;

    private void Awake()
    {
        SwipeExt.ui.Add(gameObject.name.ToLower());
        SwipeExt.Up.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Down.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Left.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Right.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Horizontal.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Vertical.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
        SwipeExt.Click.Add(new KeyValuePair<string, SwipeEvent>(gameObject.name.ToLower(), new SwipeEvent()));
    }

    public void OnDrag(PointerEventData eventData)
    {
        time += Time.deltaTime;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        mStart = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mEnd = eventData.position;
        swipeVector = mEnd - mStart;
        if(time >= thresholdTime)
        {
            swipeVector.Normalize();
            float angleOfSwipe = Vector2.Dot(swipeVector, new Vector2(1, 0));
            angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
            if(angleOfSwipe > 120)
            {
                Swipe.Left.Invoke(gameObject.name.ToLower());
            }
            else if(angleOfSwipe < 60)
            {
                Swipe.Right.Invoke(gameObject.name.ToLower());
            }
            else
            {
                angleOfSwipe = Vector2.Dot(swipeVector, new Vector2(0, 1));
                angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                if(angleOfSwipe < 30)
                {
                    Swipe.Up.Invoke(gameObject.name.ToLower());
                }
                else if((180f - angleOfSwipe) < 30)
                {
                    Swipe.Down.Invoke(gameObject.name.ToLower());
                }
            }
        }
        else if(Vector2.Distance(mStart, mEnd) < 90)
        {
            Swipe.Click.Invoke(gameObject.name.ToLower());
        }
        else
        {
            Debug.Log(Vector2.Distance(mStart, mEnd));
        }

        time = 0f;
    }
}
