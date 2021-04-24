using UnityEditor;
using UnityEngine;
using System;
using Leopotam.Ecs;
using UnityEditor.Compilation;

namespace Wargon.LeoEcsExtention.Unity {

    [CustomEditor(typeof(MonoEntity))]
    public class MonoEntityEditor : Editor 
    {
        private MonoEntity entity;

        private bool flowed;

        private void Awake() 
        {
            EntityGUI.Init();
            ComponentTypesList.Init();
        }
        
        public override void OnInspectorGUI() 
        {
            //DrawDefaultInspector();
            entity = (MonoEntity)target;
            if(!entity.runTime)
                EditorGUI.BeginChangeCheck();

            if (entity.runTime)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Kill Entity"),GUILayout.Width(154), GUILayout.Height(24)))
                    entity.Entity.Destroy();
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                entity.destroyComponent = EditorGUILayout.Toggle("Destroy MonoBeh", entity.destroyComponent);
                entity.destroyObject = EditorGUILayout.Toggle("Destroy GO", entity.destroyObject);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Run Time", entity.runTime ? "✔" : "✘", EditorStyles.largeLabel);
            EditorGUILayout.LabelField($"ID:{entity.Entity.GetInternalId().ToString()}");
            EditorGUILayout.EndHorizontal();
            EntityGUI.Vertical(GUI.skin.box, () =>
            {
                if (entity.runTime)
                    if (!entity.Entity.IsAlive())
                    {
                        EditorGUILayout.LabelField("ENTITY DEAD", EditorStyles.whiteLargeLabel);
                        return;
                    }
                EntityGUI.Horizontal(() =>
                {
                    EditorGUILayout.LabelField($"ECS Components [{entity.ComponentsCount.ToString()}]", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Clear", "Remove All Components")))
                        RemoveAll();
                });
                
                EntityGUI.Horizontal(() =>
                {
                    EditorGUILayout.LabelField("Add Component");
                    if (GUILayout.Button(new GUIContent("▼"), GUILayout.Width(21), GUILayout.Height(21)))
                        flowed = !flowed;
                });

                if (ComponentTypesList.Count > 1)
                    entity.lastIndex = EditorGUILayout.Popup(entity.lastIndex, ComponentTypesList.GetAllInArray());
                else
                    ComponentTypesList.Init();

                if (entity.lastIndex != 0)
                    AddComponent(entity.lastIndex);
                
                EditorGUILayout.BeginVertical(GUI.skin.box);
                if (!flowed)
                    DrawComponents();
                EditorGUILayout.EndVertical();
            });

            if (entity.runTime) return;
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private object NewObject(Type type) {
            return Activator.CreateInstance(type);
        }

        private Type GetComponentType(string type) {
            return Type.GetType(type + ",Assembly-CSharp", true);
        }

        private void RemoveAll() {
            entity.Components.Clear();
            entity.lastIndex = 0;
        }
        private void AddComponent(int index) {
            if (ComponentTypesList.Get(index) == "Add") return;
            // if (entity.runTime)
            //     AddComponentRuntime(index);
            // else
                AddComponentEditor(index);
        }

        private void AddComponentEditor(int index)
        {
            var type = GetComponentType(ComponentTypesList.Get(index));
            if (entity.Components.HasType(type)) {
                entity.lastIndex = 0;
                return;
            }

            var resolver = NewObject(type);
            entity.Components.Add(resolver);
            entity.lastIndex = 0;
        }
        // private void AddComponentRuntime(int index)
        // {
        //     var type = GetComponentType(ComponentTypesList.Get(index));
        //     ref var data =  ref entity.Entity.GetInternalWorld().GetEntityData(entity.Entity);
        //     
        //     
        //     if (data.Components.Contains(ComponentTypeMap.GetID(type))) {
        //         entity.lastIndex = 0;
        //         Debug.LogError($"ERROR! ENTITY ALREADY HAS '{type}' COMPONENT");
        //         return;
        //     }
        //
        //     var resolver = NewObject(type);
        //     entity.Entity.AddBoxed(resolver);
        //     //MonoConverter.AddComponent(ref entity.Entity, resolver);
        //     entity.Components.Add(resolver);
        //     entity.lastIndex = 0;
        // }
        private void DrawComponents() {
            var components = serializedObject.FindProperty("Components");
            for (int i = 0; i < components.arraySize; i++)
            {
                var dataProperty = components.GetArrayElementAtIndex(i);
                ComponentInspector.DrawComponentBox(entity, i, dataProperty);
            }
        }
    }
}

