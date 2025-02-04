using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SimpleEditorTools
{
    [MenuItem("GameObject/Simple Editor Tools/Sort Children")]
    public static void SortChildren(MenuCommand command) 
    {
        Transform parent = ((GameObject)command.context).transform;
        List<Transform> children = new List<Transform>(parent.childCount);
        foreach(Transform child in parent) 
        {
            children.Add(child);
        }
        Undo.RecordObjects(children.ToArray(), "Sort Children");
        children.Sort((x, y) => { return x.name.CompareTo(y.name); });
        foreach (Transform child in children) 
        {
            child.SetAsLastSibling();
        }
    }

    [MenuItem("GameObject/Simple Editor Tools/Sort Children", validate = true)]
    public static bool SortChildrenValidate() 
    {
        return Selection.gameObjects.Length > 0 && Selection.gameObjects.Length == Selection.objects.Length;
    }
}
