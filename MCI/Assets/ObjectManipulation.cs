using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;
using UnityEngine.UI;

public class ObjectManipulation : MonoBehaviour {

    public UI ui_script;

    public GameObject templateDecalPlane;
    public float defaultObjectScale = 0.1f;
        
    private Material materialWhenSelected;
    private GameObject selectedObj;

    private Stack changes;                          //Allows Undo
    private bool changedColorOrMat = false;

    private GameObject lastObjectInstance;
    
    void Start()
    {
        changes = new Stack();                      //Stack for Undo steps  
    }

    //Will be called if clicked on a valid object
    public void init(GameObject selectedObject)
    {
        selectedObj = selectedObject;
        Material mat = selectedObj.GetComponent<MeshRenderer>().material;
        materialWhenSelected = mat;

        changes.Clear();                        //Empty the Stack
        changedColorOrMat = false;

        if (!selectedObj.GetComponent<Wall>().isInDefaultState())
            ui_script.setResetButtonActive(true);
        else
        {
            ui_script.setResetButtonActive(false);
            mat.color = Color.yellow;               //"Highlight the selected Object"
        }
    }

    //Will be called if "Tab" pressed or "Accept" button clicked
    public void shutdown()
    {
        if (!changedColorOrMat)
            selectedObj.GetComponent<MeshRenderer>().material.color = Color.white;   //Reset the "Highlight" Wall-Color
    }
    
    //Change Material from the selected object
    public void changeMaterial(Material mat)
    {
        if (selectedObj.GetComponent<MeshRenderer>().sharedMaterial.mainTexture == mat.mainTexture)
            return;     //Object has already this Material, so return
        changedColorOrMat = true;
        changes.Push(new Change(selectedObj.GetComponent<MeshRenderer>().material, selectedObj, ChangeType.changedMaterial));

        selectedObj.GetComponent<Wall>().changeMaterial(mat);

        ui_script.setUndoButtonActive(true);
        ui_script.setResetButtonActive(true);
    }


    //Change color for this object
    public void changeColor(Image img)
    {
        Color col = img.color;
        Material mat = selectedObj.GetComponent<MeshRenderer>().material;
        if (mat.color == col)
            return;     //Object has already this color, so return
        changedColorOrMat = true;
        changes.Push(new Change(mat, selectedObj, ChangeType.changedColor));

        selectedObj.GetComponent<Wall>().changeColor(col);

        ui_script.setUndoButtonActive(true);
        ui_script.setResetButtonActive(true);
    }
    
    //add the given object to the wall
    public void addObject(Material mat)
    {
        //Set Material
        templateDecalPlane.GetComponent<MeshRenderer>().material = mat;
        float aspectRatio = (float) mat.mainTexture.width / (float) mat.mainTexture.height;
        templateDecalPlane.transform.localScale = new Vector3(aspectRatio * defaultObjectScale, 1 * defaultObjectScale, 1 * defaultObjectScale);

        //Set Rotation
        Vector3 rot = selectedObj.transform.rotation.eulerAngles;
        rot.x += 90;

        //Instantiate Prefab
        GameObject instance = (GameObject)Instantiate(templateDecalPlane, transform.position, Quaternion.Euler(rot));
        instance.GetComponent<TemplateDecalScript>().init(selectedObj, aspectRatio, ui_script);

        //Set lastObjectInstance to this Object
        lastObjectInstance = instance;

        //Add Change to Stack
        changes.Push(new Change(instance, selectedObj, ChangeType.attachedObject));

        //Add Object To Wall
        selectedObj.GetComponent<Wall>().addObject(instance);

        //Disable Scroll-View and enable Undo-Button
        ui_script.disableScrollView();
        ui_script.setPapierkorbButtonActive(true);
        ui_script.setResetButtonActive(true);          
    }

    //Remove everything from the selected Object (pillar, floor, wall etc.)
    public void reset()
    {
        changedColorOrMat = false;
        selectedObj.GetComponent<Wall>().reset();

        changes.Push(new Change(selectedObj, ChangeType.resetObject));
        
        ui_script.setUndoButtonActive(true);           //Disable Undo-Button
        ui_script.setResetButtonActive(false);          //Disable Reset-Button
    }

    //Undo the last change
    public void undo()
    {
        if (changes.Count == 0)
            return;             //Stack is Empty
        Change lastChange = (Change) changes.Pop();
        lastChange.undo();
        
        if (changes.Count == 0 || lastChange.changeType == ChangeType.resetObject)
        {
            ui_script.setUndoButtonActive(false);
            changes.Clear();
        }

        if (!selectedObj.GetComponent<Wall>().isInDefaultState())
        {
            ui_script.setResetButtonActive(true);
            changedColorOrMat = true;
        }
            
    }

    public void deleteCurrentDraggedObject()
    {
        lastObjectInstance.GetComponent<TemplateDecalScript>().DestroyObject();
        ui_script.setPapierkorbButtonActive(false);
        changes.Pop();           
    }


    //Represents one change 
    private class Change
    {
        public Material material;       //The material
        public GameObject gameObject;   //The object attached to the wall 
        public GameObject selectedObj;  //the selected object (e.g. wall)
        public Color color;
        public ChangeType changeType;

        public Change(Material mat, GameObject selectedObject, ChangeType changeType)
        {
            material = mat;
            color = mat.color;
            selectedObj = selectedObject;
            this.changeType = changeType;
        }

        public Change(GameObject go, GameObject selectedObject, ChangeType changeType)
        {
            gameObject = go;
            selectedObj = selectedObject;
            this.changeType = changeType;
        }

        public Change(GameObject selectedObject, ChangeType changeType)
        {
            selectedObj = selectedObject;
            this.changeType = changeType;
        }

        public void undo()
        {
            if (changeType == ChangeType.changedMaterial)
            {
                selectedObj.GetComponent<Wall>().changeMaterial(this.material);
            }
            else if(changeType == ChangeType.attachedObject)
            {
                selectedObj.GetComponent<Wall>().removeObject(gameObject);
            }
            else if(changeType == ChangeType.resetObject)
            {
                selectedObj.GetComponent<Wall>().restoreLastState();
            }
            else if(changeType == ChangeType.changedColor)
            {
                selectedObj.GetComponent<Wall>().changeMaterial(this.material);
                selectedObj.GetComponent<Wall>().changeColor(color);
            }
        }
    }

    enum ChangeType
    {
        changedMaterial,
        attachedObject,
        resetObject,
        changedColor
    }

    
}
