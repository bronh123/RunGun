# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentWithList))]
public class ComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ComponentWithList myScript = (ComponentWithList)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add Components from GameObject", EditorStyles.boldLabel);
        
        myScript.targetObject = (GameObject)EditorGUILayout.ObjectField(
            "Target Object", 
            myScript.targetObject, 
            typeof(GameObject), 
            true
        );
        
        if (myScript.targetObject != null && GUILayout.Button("Add All Components"))
        {
            Undo.RecordObject(myScript, "Add Components");
            
            Component[] components = myScript.targetObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                // Skip Transform component if you want
                if (component is MonoBehaviour)
                {
                    myScript.componentList.Add(component);
                }
            }
            
            EditorUtility.SetDirty(myScript);
        }
    }
}
#endif