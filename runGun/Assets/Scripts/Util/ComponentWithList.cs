using System.Collections.Generic;
using UnityEngine;

public class ComponentWithList : MonoBehaviour
{
    public List<Component> componentList = new List<Component>();
    [HideInInspector] public GameObject targetObject;
}