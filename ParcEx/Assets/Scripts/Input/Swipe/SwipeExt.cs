using System;
using System.Collections.Generic;
using UnityEngine.Events;

public enum Swipe
{
    Up,
    Down,
    Left,
    Right,
    Horizontal,
    Vertical,
    Click
}
public static class SwipeExt
{
    public static List<KeyValuePair<string, SwipeEvent>> Up = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Down = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Left = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Right = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Horizontal = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Vertical = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<KeyValuePair<string, SwipeEvent>> Click = new List<KeyValuePair<string, SwipeEvent>>();
    public static List<string> ui = new List<string>();
    public static int test = 0;
    public static void AddListener(this Swipe s, string uiName, UnityAction<Swipe> method)
    {
        if (ui.Contains(uiName.ToLower()))
        {
            switch (s)
            {
                case Swipe.Up:
                    for (int i = 0; i < Up.Count; i++)
                    {
                        if(Up[i].Key.ToLower() == uiName.ToLower())
                        {
                            Up[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Down:
                    for (int i = 0; i < Down.Count; i++)
                    {
                        if (Down[i].Key.ToLower() == uiName.ToLower())
                        {
                            Down[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Left:
                    for (int i = 0; i < Left.Count; i++)
                    {
                        if (Left[i].Key.ToLower() == uiName.ToLower())
                        {
                            Left[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Right:
                    for (int i = 0; i < Right.Count; i++)
                    {
                        if (Right[i].Key.ToLower() == uiName.ToLower())
                        {
                            Right[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Horizontal:
                    for(int i = 0; i < Horizontal.Count; i++)
                    {
                        if(Horizontal[i].Key.ToLower() == uiName.ToLower())
                        {
                            Horizontal[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Vertical:
                    for (int i = 0; i < Vertical.Count; i++)
                    {
                        if (Vertical[i].Key.ToLower() == uiName.ToLower())
                        {
                            Vertical[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
                case Swipe.Click:
                    for (int i = 0; i < Click.Count; i++)
                    {
                        if (Click[i].Key.ToLower() == uiName.ToLower())
                        {
                            Click[i].Value.AddListener(method);
                            break;
                        }
                    }
                    break;
            }
        }
    }
    public static void Invoke(this Swipe s, string uiName)
    {
        if (ui.Contains(uiName.ToLower()))
        {
            switch (s)
            {
                case Swipe.Up:
                    for (int i = 0; i < Up.Count; i++)
                    {
                        if (Up[i].Key.ToLower() == uiName.ToLower())
                        {
                            Up[i].Value.Invoke(s);
                            Vertical[i].Value.Invoke(s);
                            break;
                        }
                    }
                    break;
                case Swipe.Down:
                    for (int i = 0; i < Down.Count; i++)
                    {
                        if (Down[i].Key.ToLower() == uiName.ToLower())
                        {
                            Down[i].Value.Invoke(s);
                            Vertical[i].Value.Invoke(s);
                            break;
                        }
                    }
                    break;
                case Swipe.Left:
                    for (int i = 0; i < Left.Count; i++)
                    {
                        if (Left[i].Key.ToLower() == uiName.ToLower())
                        {
                            Left[i].Value.Invoke(s);
                            Horizontal[i].Value.Invoke(s);
                            break;
                        }
                    }
                    break;
                case Swipe.Right:
                    for (int i = 0; i < Right.Count; i++)
                    {
                        if (Right[i].Key.ToLower() == uiName.ToLower())
                        {
                            Right[i].Value.Invoke(s);
                            Horizontal[i].Value.Invoke(s);
                            break;
                        }
                    }
                    break;
                case Swipe.Click:
                    for (int i = 0; i < Right.Count; i++)
                    {
                        if (Click[i].Key.ToLower() == uiName.ToLower())
                        {
                            Click[i].Value.Invoke(s);
                            break;
                        }
                    }
                    break;
            }
        }
    }
}

public class SwipeEvent : UnityEvent<Swipe> { }
