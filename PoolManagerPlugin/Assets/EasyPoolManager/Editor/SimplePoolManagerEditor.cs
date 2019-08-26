using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static SimplePoolManager;
using System;

[CustomEditor(typeof(SimplePoolManager))]
[CanEditMultipleObjects]
public class SimplePoolManagerEditor : Editor
{
    private SimplePoolManager _poolMan;

    private SerializedObject _getTarget;
    private SerializedProperty _thisList;
    private SerializedProperty _expandBool;
    private SerializedProperty _activeObjects;
    private SerializedProperty _inactiveObjects;
    private SerializedProperty _totalPooledObjects;

    private List<bool> _showPool;
    private List<bool> _showPools;
    private bool _expandPoolToggle;

    private int _listSize;

    private float slider1;
    private float slider2;
    private float slider3;

    private void OnEnable()
    {
        _poolMan = (SimplePoolManager)target;
        _getTarget = new SerializedObject(_poolMan);
        _thisList = _getTarget.FindProperty("Pools");
        _expandBool = _getTarget.FindProperty("ExpandIfEmpty");
        _activeObjects = _getTarget.FindProperty("ActiveObjects");
        _inactiveObjects = _getTarget.FindProperty("InactiveObjects");
        _totalPooledObjects = _getTarget.FindProperty("TotalPooledObjects");

        _showPool = new List<bool>();
        _showPools = new List<bool>();
    }

    public override void OnInspectorGUI()
    {
        _getTarget.Update();
        _listSize = _thisList.arraySize;

        while (_showPool.Count < _thisList.arraySize)
        {
            _showPool.Add(false); 
        }
        while(_showPools.Count < Enum.GetValues(typeof(ObjectPool.PoolType)).Length)
        {
            _showPools.Add(true); 
        }

        foreach (var type in Enum.GetValues(typeof(ObjectPool.PoolType)))
        {
            //DrawDefaultInspector(); //for debugs
            var num = Convert.ToInt32(type);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(GUI.skin.window);
            EditorGUILayout.BeginVertical(GUI.skin.button);
            EditorGUI.indentLevel++;


            _showPools[num] = EditorGUILayout.Foldout(_showPools[num], type + " Pool's"/*" | Amount of Pools: " + _listSize*/);

            EditorGUILayout.BeginHorizontal(GUI.skin.window);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Active: " + _activeObjects.intValue.ToString(), GUILayout.Width(65));
            EditorGUILayout.LabelField("Inactive: " + _inactiveObjects.intValue.ToString(), GUILayout.Width(213));
            EditorGUILayout.LabelField("Total Pooled: " + _totalPooledObjects.intValue.ToString(), GUILayout.Width(100));

            EditorGUI.indentLevel++;
            EditorGUILayout.EndHorizontal();

            if (_showPools[num])
            {
                for (int i = 0; i < _thisList.arraySize; i++)
                {
                    var listRef = _thisList.GetArrayElementAtIndex(i);
                    var poolType = listRef.FindPropertyRelative("poolType");
                    var poolName = listRef.FindPropertyRelative("PoolName");
                    var size = listRef.FindPropertyRelative("PoolSize");
                    var obj = listRef.FindPropertyRelative("ObjectToPool");

                    if (poolType.enumValueIndex == num)
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.BeginVertical(GUI.skin.button);
                        _showPool[i] = EditorGUILayout.Foldout(_showPool[i], poolName.stringValue + "Pool | Type - " + poolType.enumNames[poolType.enumValueIndex] + " | Size: " + size.intValue);

                        EditorGUILayout.EndHorizontal();

                        if (!_showPool[i] && GUILayout.Button("Remove", GUILayout.Width(57)))
                        {
                            _thisList.DeleteArrayElementAtIndex(i);
                        }

                        EditorGUILayout.EndVertical();
                    }

                    if (_showPool[i] && poolType.enumValueIndex == num) //must add information here for pool info to show up
                    {
                        if (poolType.enumValueIndex == 0) //add a new if block for the next enum to draw the data in the inspector
                        {
                            if(Enum.GetValues(typeof(ObjectPool.PoolType)).Length >= 2) //show type dropdown only if more then 1 exist
                                EditorGUILayout.PropertyField(poolType);

                            EditorGUILayout.PropertyField(poolName);
                            EditorGUILayout.PropertyField(size);
                            EditorGUILayout.PropertyField(obj);
                        }

                        EditorGUILayout.Space();

                        //Remove this index from the List
                        if (GUILayout.Button("Remove This Index (" + i.ToString() + ")"))
                        {
                            _thisList.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
            }

            var lastIndex = _thisList.arraySize;

            if (_showPools[num] && GUILayout.Button("Add New", GUILayout.Width(100)))
            {
                _thisList.arraySize++;
                var lastPool = _thisList.GetArrayElementAtIndex(lastIndex);
                var poolType = lastPool.FindPropertyRelative("poolType");
                var poolName = lastPool.FindPropertyRelative("PoolName");
                var size = lastPool.FindPropertyRelative("PoolSize");
                var obj = lastPool.FindPropertyRelative("ObjectToPool");

                poolType.enumValueIndex = num;
                poolName.stringValue = string.Empty;
                size.intValue = 0;
                obj.objectReferenceValue = null;
            }
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(1));
            EditorGUI.indentLevel--;

            _expandBool.boolValue = EditorGUILayout.Toggle("Expand Pool's If Empty", _expandBool.boolValue);

            EditorGUI.indentLevel++;
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.HelpBox("For information on how to use this plugin, please refer to the ReadMe in the root folder!", MessageType.Info);

            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal(GUI.skin.box); //for debugs 

            //slider1 = EditorGUILayout.Slider(slider1, 0, 1000);
            //slider2 = EditorGUILayout.Slider(slider2, 0, 1000);
            //slider3 = EditorGUILayout.Slider(slider3, 0, 1000);

            //EditorGUILayout.EndHorizontal();
        }

        //Apply the changes to our list
        _getTarget.ApplyModifiedProperties();
    }
}
