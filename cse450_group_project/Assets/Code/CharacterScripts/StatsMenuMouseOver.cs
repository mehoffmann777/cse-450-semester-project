using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenuMouseOver : MonoBehaviour
{

    public GameObject statMenu;
    private Vector3 originalPosition;

    public void Start()
    {
        statMenu = GameObject.Find("StatMenu");
        originalPosition = statMenu.transform.position;
        
    }

    public void OnMouseOver()
    {
        CharacterStats stats = this.GetComponent<CharacterStats>();
        TextMeshProUGUI statText = statMenu.GetComponent < TextMeshProUGUI > ();
        var rectTransform = statMenu.GetComponent<RectTransform>();

        if (!Camera.main) { return; } 

        var screenPoint = Camera.main.WorldToScreenPoint(new Vector3(-3, 3, 0));
        var screenRect = Camera.main.pixelRect;

        var rectTransPoint = new Vector3(
            screenPoint.x - screenRect.width / 2.0f,
            screenPoint.y - screenRect.height / 2.0f,
            screenPoint.z);

        
        statText.text = "HP: " + stats.health
                        + "\nStr: " + stats.strength
                        + "\nDef: " + stats.defense
                        + "\nMov: " + stats.movement;

        rectTransform.SetLocalPositionAndRotation(rectTransPoint, Quaternion.identity);
        //statMenu.transform.position = new Vector3(0, 0, 2);
       // statMenu.transform.Translate(new Vector3(20, 0, 0), null);


    }

    public void OnMouseExit()
    {
        statMenu.transform.position = originalPosition;
    }
}
