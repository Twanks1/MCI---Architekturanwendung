﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class UI : MonoBehaviour {

    enum Mode
    {
        TypÄndern,
        ObjekteHinzufügen
    }

    //The scroll views from the individual ui's
    public GameObject ui_wall;
    public GameObject ui_tiled;
    public GameObject ui_objects;

    public GameObject objektHinzufügen_button;

    public GameObject undo_button;

    public GameObject player;                               //Player for acces to the "Change Type" - script

    public GameObject[] buttons;                            //All Buttons in the Menu

    private Canvas canvas;
    private Mode currentMode = Mode.TypÄndern;
    private Color buttonColorWhenInMode = Color.cyan;
    private string currentTag = "";                          //The tag from the current selected Object

    void Start()
    {
        canvas = GetComponent<Canvas>();        
        canvas.enabled = false;
        ui_wall.SetActive(false);
        ui_tiled.SetActive(false);
        ui_objects.SetActive(false);
        setUndoButtonActive(false);
        disableCursor();                           
    }

    //Enable the whole UI based on which Object were selected
    public void enableUI(GameObject selectedObject)
    {
        canvas.enabled = true;
        currentTag = selectedObject.tag;
        switch (currentTag)
        {
            case "Wall":
                ui_wall.SetActive(true);
                objektHinzufügen_button.GetComponent<Button>().interactable = true;
                break;
            case "Tiled":
                ui_tiled.SetActive(true);
                objektHinzufügen_button.GetComponent<Button>().interactable = false;
                break;
        }
        changeMode(Mode.TypÄndern);
        player.GetComponent<ObjectManipulation>().init(selectedObject);
        enableCursor();
    }

    //Disable the whole UI
    public void disableUI()
    {
        canvas.enabled = false;
        disableScrollView();
        setUndoButtonActive(false);
        player.GetComponent<ObjectManipulation>().shutdown();
        disableCursor();
    }

    //Called if Menu Button "Typ ändern" was clicked
    public void onClickTypÄndern(GameObject button)
    {
        changeMode(Mode.TypÄndern);
        changeButtonColor(button, buttonColorWhenInMode);
    }

    //Called if Menu Button "Objekt Hinzufügen" was clicked
    public void onClickObjekteHinzufügen(GameObject button)
    {
        changeMode(Mode.ObjekteHinzufügen);
        changeButtonColor(button, buttonColorWhenInMode);
    }


    public void disableScrollView()
    {
        ui_tiled.SetActive(false);
        ui_objects.SetActive(false);
        ui_wall.SetActive(false);
    }

   

    //Change the Mode (changes button color and enables/disables specific UI)
    private void changeMode(Mode mode)
    {
        currentMode = mode;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == (int)currentMode)
            {
                buttons[i].GetComponent<Image>().color = buttonColorWhenInMode;
            }
            else
            {
                buttons[i].GetComponent<Image>().color = Color.white;
            }
        }
        enableScrollView();
    }

    public void enableScrollView()
    {
        switch (currentMode)
        {
            case Mode.TypÄndern:
                if (currentTag == "Wall")
                {
                    ui_tiled.SetActive(false);
                    ui_objects.SetActive(false);
                    ui_wall.SetActive(true);
                }
                else if (currentTag == "Tiled")
                {
                    ui_objects.SetActive(false);
                    ui_wall.SetActive(false);
                    ui_tiled.SetActive(true);
                }
                break;
            case Mode.ObjekteHinzufügen:
                ui_wall.SetActive(false);
                ui_tiled.SetActive(false);
                ui_objects.SetActive(true);
                break;
        }
    }


    public void setUndoButtonActive(bool b)
    {
        undo_button.GetComponent<Button>().interactable = b;
    }

    private void changeButtonColor(GameObject button, Color col)
    {
        button.GetComponent<Image>().color = col;
    }

    //Disables cursor and lock the rotation
    private void disableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.GetComponent<FirstPersonController>().lockRotation = false;
    }

    //Enables cursor and rotation with mouse
    private void enableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.GetComponent<FirstPersonController>().lockRotation = true;
    }
}
