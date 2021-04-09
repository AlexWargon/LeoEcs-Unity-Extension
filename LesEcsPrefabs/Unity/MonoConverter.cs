using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Leopotam.Ecs;
using UnityEngine;

namespace Wargon.LeoEcsExtention.Unity {
    public static class MonoConverter {
        private static EcsWorld world;
        public static bool HasWorld => world != null;
        private static MethodInfo addComponent = null;
        public static void Init(EcsWorld ecsWorld) 
        {
            world = ecsWorld;
            addComponent = typeof(Extensions).GetMethod("Add");
        }
        public static EcsWorld GetWorld()
        {
            return world;
        }
        public static void Execute(ref EcsEntity entity, IEnumerable<object> components)
        {
            foreach (var component in components) {
                var addComponentGeneric = addComponent.MakeGenericMethod(component.GetType());
                addComponentGeneric.Invoke(null, new []{ entity, component});
            }
        }
    }

}


