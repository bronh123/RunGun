using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class ScriptMapping
{
    public GameObject targetObject;
    public string[] scriptNames;
}
public class ScriptManager : MonoBehaviour
{

    [Header("Script Management")]
    [Tooltip("Scripts to disable during certain UI events")]
    public MonoBehaviour[] scriptsToDisable;
    [SerializeField] private List<ScriptMapping> scriptMappings = new();
    [SerializeField] private bool setTimeScale = true;
    private List<MonoBehaviour> disabledScripts = new();

    [SerializeField] private GameObject[] objectsToDisable;
    private List<GameObject> disabledObjects = new();
    private void Start()
    {
        CollectScripts();
    }

    private void CollectScripts()
    {
        List<MonoBehaviour> tempScriptList = new List<MonoBehaviour>();
        foreach (var mapping in scriptMappings)
        {
            if (mapping.targetObject != null)
            {
                MonoBehaviour[] scripts = mapping.targetObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (mapping.scriptNames.Contains(script.GetType().Name))
                    {
                        tempScriptList.Add(script);
                    }

                }
            }

            tempScriptList.AddRange(scriptsToDisable);
            scriptsToDisable = tempScriptList.ToArray();
        }
    }


    public void DisablePlayerScripts()
    {
        disabledScripts.Clear();

        if (scriptsToDisable != null)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null && script.enabled)
                {
                    script.enabled = false;
                    disabledScripts.Add(script);
                    //Debug.Log($"Disabled script: {script.GetType().Name}");
                }
            }
        }

        if (setTimeScale)
        {
            Time.timeScale = 0f;
        }
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ReenablePlayerScripts()
    {
        foreach (MonoBehaviour script in disabledScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                //Debug.Log($"Re-enabled script: {script.GetType().Name}");
            }
        }

        disabledScripts.Clear();

        if (setTimeScale)
        {
            Time.timeScale = 1f;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableObjects()
    {
        disabledObjects.Clear();

        if (objectsToDisable != null)
        {
            foreach (GameObject objectToDisable in objectsToDisable)
            {
                if (objectToDisable != null)
                {
                    objectToDisable.SetActive(false);
                    disabledObjects.Add(objectToDisable);
                    //Debug.Log($"Disabled script: {objectToDisable.GetType().Name}");
                }
            }
        }
    }

}
