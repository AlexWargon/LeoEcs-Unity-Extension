# LeoEcs Unity Extension
# How to use:
0) Init EcsWorld from Awake method

1) After instatiating EcsWorld, Call MonoConvertor.Init(link to world)
```C#
world = new EcsWorld();
MonoConverter.Init(world);
```

2) Add to your component attribute [EcsComponent]
```C#
[EcscComponent]
public struct Health 
{ 
    public int value;
}
```
Must be something like that

![image](https://user-images.githubusercontent.com/37613162/115751740-24a8be00-a3a2-11eb-8f63-4788e3c3770d.png)

All components will be added automatically, no need to build entities manually from code
