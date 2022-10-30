![](https://img.shields.io/badge/unity-2019.3%20or%20later-green)
[![](https://img.shields.io/github/license/no0bsprey/bindlessdi)](https://github.com/mob-sakai/UIEffect/blob/master/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg?style=flat-square)](https://makeapullrequest.com)
![](https://img.shields.io/github/stars/no0bsprey/bindlessdi?style=social)

bindlessdi
===

Lightweight dependency injection framework for Unity almost free of bindings.

[Installation](#installation) | [Usage](#usage)

## Installation

Find the manifest.json file in the Packages folder of your project and add the followng line to dependencies section:
```json
{
 "dependencies": {
    "com.npg.bindlessdi": "https://github.com/no0bsprey/bindlessdi.git",
 },
}
```

## Usage

### Initializing the Container
Call Container.Initialize() once your first scene will be loaded
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();
        var myGame = container.Resolve<MyGame>();
        myGame.Play();
    }
}
```

### Getting links to gameobjects from scene
 - Create a SceneContext class for every scene in your game
 - Add serialized fields for links to MonoBehaviours you need
```c#
public class MyGameSceneContext : SceneContext
{
    [SerializeField] private MyHudView _hudView;
    [SerializeField] private Camera _camera;
    ...

    public override IEnumerable<Object> GetObjects()
    {
        return new Object[] { _hudView, _camera };
    }
}
```
 - Create a gameObject and attach this scene context script to it
 - Get UnityObjectContainer class as a constructor argument
```c#
public class MyGame
{
    private readonly UnityObjectContainer _unityObjectContainer;
    
    public MyGame(UnityObjectContainer unityObjectContainer)
    {
        _unityObjectContainer = unityObjectContainer;
    }
    
    public void Play()
    {
        if (!_unityObjectContainer.TryGetObject(out MyHudView hudView))
        {
            return;
        }
        hudView.Show();
    }
}
````
 - Once a scene will be loaded - every monoBehaviour from SceneContexts will be added to UnityObjectContainer
