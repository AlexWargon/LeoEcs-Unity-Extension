using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Leopotam.Ecs;

namespace Leopotam.Ecs
{
    public static class Extensions
    {
    
        public static bool HasType(this IList list, Type whatHas)
        {
            var i = 0;
            var count = list.Count;
            for (i = 0; i < count; i++)
                if (list[i].GetType().Equals(whatHas))
                    return true;
            return false;
        }


        public static void Add<T>(this EcsEntity entity, T component) where T : struct
        {
            entity.Get<T>() = component;
        }

    }
}

