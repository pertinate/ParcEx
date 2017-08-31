using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObject
{
    public Image icon;
    public GameObject owner;
}

public class MapController : MonoBehaviour {
    public Camera minimapCamera;
    public bool canDraw = true;
    public static List<MapObject> mapObjects = new List<MapObject>();
    private void OnEnable()
    {
        DrawMapIcons();//instantly get draw on enable.
        StartCoroutine(DrawIcons());
    }
    public static void RegisterMapObject(GameObject o, Image i)
    {
        if(i != null)
        {
            Image image = Instantiate(i);
            mapObjects.Add(new MapObject() { owner = o, icon = image });
        }
    }
    public static void RemoveMapObject(GameObject o)
    {
        for(int i = 0; i < mapObjects.Count; i++)
        {
            if(mapObjects[i].owner == o)
            {
                Destroy(mapObjects[i].icon);
                mapObjects.RemoveAt(i);
                continue;
            }
        }
    }
    private void DrawMapIcons()
    {
        foreach(MapObject mo in mapObjects)
        {
            Vector3 screenPos = minimapCamera.WorldToViewportPoint(mo.owner.transform.position);
            mo.icon.transform.SetParent(this.transform);
            RectTransform rt = this.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            screenPos.x = screenPos.x * rt.rect.width + corners[0].x;
            screenPos.y = screenPos.y * rt.rect.height + corners[0].y;
            screenPos.z = 0;
            mo.icon.transform.position = screenPos;
        }
        minimapCamera.targetTexture.DiscardContents();
    }
    private IEnumerator DrawIcons()
    {
        while (canDraw)
        {
            yield return new WaitForSeconds(0.25f);
            DrawMapIcons();
        }
    }
}
