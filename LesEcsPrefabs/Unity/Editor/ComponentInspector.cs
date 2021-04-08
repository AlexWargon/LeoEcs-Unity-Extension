using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEditorInternal;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Wargon.LeoEcsExtention.Unity {
    public static class ComponentInspector {

        private static bool remove;
        
        public static void DrawComponentBox(MonoEntity entity, int index)
        {
            if(entity.ComponentsCount < index) return;
            EntityGUI.Vertical(EntityGUI.GetColorStyle(entity.ComponentsCount, index), () =>
            {
                // if (entity.runTime)
                //     DrawRunTimeMode(entity, index);
                // else
                    DrawEditorMode(entity, index);
            });
            if (remove)
                Remove(entity, index);
        }

        private static void RemoveBtn()
        {
            if (GUILayout.Button(new GUIContent("✘", "Remove"), EditorStyles.miniButton, GUILayout.Width(21),
                GUILayout.Height(14)))
                remove = true;
        }

        private static void DrawTypeField(object component, FieldInfo field)
        {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(UnityEngine.Object) || fieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue = EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as UnityEngine.Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue, field.Name, component));
        }
        private static void DrawTypeFieldRunTime(object component, FieldInfo field)
        {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(UnityEngine.Object) || fieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue = EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as UnityEngine.Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue,field.Name, component));
        }
        private static void SetFieldValue(object fieldValue, string fieldName, object component)
        {
            switch (fieldValue)
            {
                case LayerMask field:
                    LayerMask tempMask = EditorGUILayout.MaskField(fieldName,
                        InternalEditorUtility.LayerMaskToConcatenatedLayersMask(field),
                        InternalEditorUtility.layers);
                    fieldValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                    break;
                case Enum field:
                    fieldValue = EditorGUILayout.EnumFlagsField($"    {fieldName}", field);
                    break;
                case int field:
                    fieldValue = EditorGUILayout.IntField($"    {fieldName}", field);
                    break;
                case float field:
                    fieldValue = EditorGUILayout.FloatField($"    {fieldName}", field);
                    break;
                case bool field:
                    fieldValue = EditorGUILayout.Toggle($"    {fieldName}", field);
                    break;
                case double field:
                    fieldValue = EditorGUILayout.DoubleField($"    {fieldName}", field);
                    break;
                case Vector2 field:
                    fieldValue = EditorGUILayout.Vector2Field($"    {fieldName}", field);
                    break;
                case Vector3 field:
                    fieldValue = EditorGUILayout.Vector3Field($"    {fieldName}", field);
                    break;
                case Vector4 field:
                    fieldValue = EditorGUILayout.Vector4Field($"    {fieldName}", field);
                    break;
                case AnimationCurve field:
                    fieldValue = EditorGUILayout.CurveField($"    {fieldName}", field);
                    break;
                case Quaternion field:
                    var vec = QuaternionToVector4(field);
                    var tempVec =  EditorGUILayout.Vector4Field($"    {fieldName}", vec);
                    fieldValue = Vector4ToQuaternion(tempVec);
                    break;
            }
            component.GetType().GetField(fieldName).SetValue(component, fieldValue);
        }
        private static Vector4 QuaternionToVector4(Quaternion rot)
        {
            return new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        private static Quaternion Vector4ToQuaternion(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
        private static void Remove(MonoEntity entity, int index)
        {
            if (entity.runTime)
            {
                // entity.Entity.RemoveByTypeID(entity.Entity.GetEntityData().componentTypes.ElementAt(index));
                // remove = false;
            }
            else
            {
                entity.Components.RemoveAt(index);
                remove = false;
            }
        }

        private static void DrawEditorMode(MonoEntity entity, int index)
        {
            var component = entity.Components[index];
            if (component == null)
            {
                entity.Components = entity.Components.Where(x => x != null).ToList();
                return;
            }

            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            EntityGUI.Horizontal(() =>
            {
                EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
                RemoveBtn();
            });

            //if (!entity.gui[index].showing) return;
            foreach (var field in fields)
                DrawTypeField(component, field);
        }

        // private static void DrawRunTimeMode(MonoEntity entity, int index)
        // {
        //     if(remove) return;
        //     var componentTypeID = entity.Entity.GetEntityData().componentTypes.ElementAt(index);
        //     var pool = entity.Entity.world.ComponentPools[componentTypeID];
        //     var component = pool.GetItem(entity.Entity.id);
        //     
        //     var type = pool.ItemType;
        //     var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        //     
        //     EditorGUILayout.BeginHorizontal();
        //     EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
        //     RemoveBtn();
        //     EditorGUILayout.EndHorizontal();
        //
        //     for (var i = 0; i < fields.Length; i++)
        //         DrawTypeFieldRunTime(component, fields[i]);
        //
        //     pool.SetItem(component,entity.Entity.id);
        //     if(index >= entity.Components.Count) return;
        //     entity.Components[index] = component;
        // }
    }

}
