using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenu : MonoBehaviour
{
    public GameObject StatPanel;
    public Camera Camera;

    public void MoveStatsMenuTo(Vector3 point)
    {
        print("herE");
        var rectTransform = StatPanel.GetComponent<RectTransform>();
        var screenPoint = Camera.main.WorldToScreenPoint(point);

        var screenRect = Camera.main.pixelRect;

        var rectTransPoint = new Vector3(
            screenPoint.x - screenRect.width / 2.0f,
            screenPoint.y - screenRect.height / 2.0f,
            screenPoint.z
            );


        rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);
    }


}
