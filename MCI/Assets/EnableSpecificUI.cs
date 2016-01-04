using UnityEngine;
using System.Collections;

public class EnableSpecificUI : MonoBehaviour {

    public Canvas UI;

    public GameObject ui_wall;
    public GameObject ui_pillar;

    void Start()
    {
        UI.enabled = false;
        ui_wall.SetActive(false);
        ui_pillar.SetActive(false);
    }

    public void enableUI(string tag)
    {
        UI.enabled = true;
        switch (tag)
        {
            case "Wall":
                ui_wall.SetActive(true);
                break;
            case "Tiled":
                ui_pillar.SetActive(true);
                break;
        }
    }

    public void disableUI()
    {
        UI.enabled = false;
        ui_wall.SetActive(false);
        ui_pillar.SetActive(false);
    }
}
