
using UnityEditor;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;
using  System.Collections.Generic;

class Test : MonoBehaviour
{
    private Texture2D mTexture2D;

    private void Start()
    {
        MyFocusControl(mTexture2D);
    }


    public static bool MyFocusControl(Texture2D texture, params GUILayoutOption[] options)
    {
        int id = GUIUtility.GetControlID(FocusType.Keyboard);
        Color color = GUI.color;

        GUI.color = GUIUtility.hotControl == id ? Color.green : Color.red;
        Rect rect = GUILayoutUtility.GetRect(texture.width, texture.height, options);

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = id;
                    Event.current.Use();
                }
                break;
            case EventType.Repaint:
                GUI.DrawTexture(rect, texture);
                break;
        }

        GUI.color = color;

        return GUIUtility.hotControl == id;
    }
}