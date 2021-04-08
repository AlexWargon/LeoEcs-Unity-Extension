using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Leopotam.Ecs;
namespace Wargon.LeoEcsExtention.Unity {

    [DisallowMultipleComponent]
    public class MonoEntity : MonoBehaviour {
        public EcsEntity Entity;
        private EcsWorld world;
        [HideInInspector] public int lastIndex = 0;
        [SerializeReference] public List<object> Components = new List<object>();
        public int ComponentsCount => runTime ? Entity.GetComponentsCount() : Components.Count;
        public bool runTime;
        public bool destroyObject;
        public bool destroyComponent;
        private bool converted;
        public int id;
        private void Start()
        {
            ConvertToEntity();
        }
        public void ConvertToEntity() {
            if(converted) return;
            world = MonoConverter.GetWorld();
            Entity = world.NewEntity();
            
            MonoConverter.Execute(ref Entity, Components);
            id = Entity.GetInternalId();
            converted = true;
            if (destroyComponent) Destroy(this);
            if (destroyObject) Destroy(gameObject);
            runTime = true;
            gameObject.name = $"{gameObject.name} ID:{Entity.GetInternalId().ToString()}";
        }

        public void Add<T>(T component) where T : struct
        {
            Entity.Get<T>();
        }
        private void OnDestroy() {
            if(!destroyObject)
                if(world.IsAlive())
                    Entity.Destroy();
        }


        public void DestroyWithoutEntity()
        {
            destroyObject = true;
            Destroy(this.gameObject);
        }
    }

}
