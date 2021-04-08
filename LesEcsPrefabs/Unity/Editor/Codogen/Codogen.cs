using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wargon.LeoEcsExtention.Unity
{
    public static class Codogen {
        private static string MonoConverter => $"{Application.dataPath}/LesEcsPrefabs/Unity/MonoConverter.cs";
        private static string AddComponentTamplate => $"{Application.dataPath}/LesEcsPrefabs/Unity/Editor/Codogen/Tamplates/AddComponent.txt";


        [MenuItem("Entity/Generate Code")]
        public static void AddNewCode() {
            ComponentTypesList.Init();
        
            var fileName = MonoConverter;
            var tamplate = File.ReadAllText(AddComponentTamplate);
            var codeLines = File.ReadAllLines(fileName).ToList();
        
            DeleteOldCode(codeLines);
            AddNewCode(codeLines, tamplate);
            File.WriteAllLines(fileName, codeLines);
        
            Debug.Log($"<color=lime>Code Generated</color>");
        }
        private static string ModifyTemplate(string template, int index) {
            var newTemplate = template.Replace("@type", ComponentTypesList.Get(index));
            return newTemplate;
        }
        private static void AddNewCode(List<string> code, string tamplate) {
            var end = code.FindIndex(line => line.Contains("//END"));
            for (var i = 1; i < ComponentTypesList.Count; i++) {
                code.Insert(end, ModifyTemplate(tamplate, i));
            }
        }
        private static void DeleteOldCode(List<string> code) {
            var start = code.FindIndex(line => line.Contains("//START")) + 1;
            var end = code.FindIndex(line => line.Contains("//END"));
            for (var i = start; i < end; i++) {
                code.RemoveAt(i);
                end--;
                i--;
            }
        }
    }
}


