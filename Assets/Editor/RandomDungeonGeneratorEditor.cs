using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    // Provides references to the generator and allows us to call the GenerateDungeon method from the inspector.
    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;

    }

    // Creates a button in the inspector that calls the GenerateDungeon method when clicked.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}
