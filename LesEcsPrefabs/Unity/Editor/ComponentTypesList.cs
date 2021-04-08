using System.Collections.Generic;
using System;
using System.Reflection;

namespace Wargon.LeoEcsExtention.Unity
{
    public static class ComponentTypesList {
        private static List<string> Types = new List<string>() { "Add" };
        private static string[] TypesArray;
        public static int Count => Types.Count;
        public static string Get(int index) 
        {
            return Types[index];
        }

        public static List<string> GetAll() 
        {
            return Types;
        }

        public static string[] GetAllInArray()
        {
            return TypesArray;
        }
        public static void Init() {
            var assembly = Assembly.GetAssembly(typeof(EcsComponentAttribute));
            var types = GetTypesWithAttribute(typeof(EcsComponentAttribute), assembly);
            foreach (var type in types)
                Add($"{type}");
            Types.Sort();
            var first = Types[0];
            var addIndex = Types.IndexOf("Add");
            Types[addIndex] = first;
            Types[0] = "Add";
            TypesArray = Types.ToArray();
        }
        private static void Add(string name) {
            if (!Types.Contains(name))
                Types.Add(name);
        }
        private static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, Assembly assembly) {
            foreach (Type type in assembly.GetTypes()) {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                    yield return type;
            }
        }
    }
}
    


