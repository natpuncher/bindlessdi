![](https://img.shields.io/badge/unity-2019.3%20or%20later-green)
[![](https://img.shields.io/github/license/natpuncher/bindlessdi)](https://github.com/natpuncher/bindlessdi/blob/master/LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg?style=flat-square)](https://makeapullrequest.com)

bindlessdi
===

Lightweight dependency injection framework for Unity almost free of bindings.

* [Installation](#installation)
* [Usage](#usage)

## Installation
* In **Package Manager** press `+`, select `Add package from git URL` and paste `https://github.com/natpuncher/bindlessdi.git` 
* Or find the `manifest.json` file in the `Packages` folder of your project and add the following line to dependencies section:
```json
{
 "dependencies": {
    "com.npg.bindlessdi": "https://github.com/natpuncher/bindlessdi.git",
 },
}
```

## Usage

* [Initializing](#initializing-the-container) 
* [Injecting](#injecting)
* [Bind Instance](#bind-instance)
* [Instantiation Policy](#instantiation-policy)
* [Factory](#factory)
* [Unity Objects](#working-with-unity-objects)
* [Unity Events](#unity-events)
* [Tests](#tests)

### Initializing the Container
To initialize **Bindlessdi** call `Container.Initialize()` from entry point of the game.
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

### Injecting
**Bindlessdi** only supports **constructor injection**.
```c#
public class MyGame
{
    private readonly MyController _myController;

    public MyGame(MyController controller)
    {
        _myController = controller;
    }
}
```

In most cases **Bindlessdi** will guess implementation by itself without any bindings.
```c#
public class MyController : IController
{
}
```
```c#
public class MyGame
{
    private readonly IController _controller;

    public MyGame(IController controller)
    {
        _controller = controller;
    }
}
```

If there are several of implementations for specific interface, intended implementation should be defined by calling `container.BindImplementation<IInterface, Implementation>()`.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();
        container.BindImplementation<IBar, Qux>();

        var myGame = container.Resolve<MyGame>();
        myGame.Play();
    }
}
```
```c#
public class Foo : IFoo
{
}

public class Bar : IBar
{
}

public class Qux : IBar
{
}
```
```c#
public class MyGame
{
    public MyGame(IFoo foo, IBar bar)
    {
        // bar is Qux
    }
}
```

### Bind Instance
Already instanced objects could be added to container by calling `container.BindInstance(instance)`.
Its also possible to add a bunch of them using `container.BindInstances(instances)`.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();

        var myController = new MyController();
        container.BindInstance(myController);

        var guns = new IGun[] { new FireGun(), new ColdGun() };
        container.BindInstances(guns);

        var myGame = container.Resolve<MyGame>();
        myGame.Play();
    }
}
```
```c#
public class MyGame
{
    public MyGame(MyController myController, FireGun fireGun, ColdGun coldGun)
    {
    }
}
```

### Instantiation Policy
**Bindlessdi** resolves everything **as single instance** by default, but it can be changed.

* By changing **default initialization policy** to `InstantiationPolicy.Transient`, so every resolve will create a new instance.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();
        container.DefaultInstantiationPolicy = InstantiationPolicy.Transient;

        var myGame = container.Resolve<MyGame>();
        myGame.Play();

        var myGame2 = container.Resolve<MyGame>();
        myGame2.Play();

        // myGame != myGame2
    }
}
```

* By **registering instantiation policy** for concrete type, so every resolve of this type will create a new instance.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();
        container.RegisterInstantiationPolicy<MyGame>(InstantiationPolicy.Transient);

        var myGame = container.Resolve<MyGame>();
        var myGame2 = container.Resolve<MyGame>();
        // myGame != myGame2
    }
}
```

* By **registering instantiation policy** for an interface or a base type, so every resolve of a type implementing it will get this policy override.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();
    
        container.DefaultInstantiationPolicy = InstantiationPolicy.Single;
        container.RegisterInstantiationPolicy<ITrigger>(InstantiationPolicy.Transient);
        container.RegisterInstantiationPolicy<TriggerA>(InstantiationPolicy.Single);
		
        var a1 = container.Resolve<TriggerA>();
        var a2 = container.Resolve<TriggerA>();
        // a1 == a2
		
        var b1 = container.Resolve<TriggerB>();
        var b2 = container.Resolve<TriggerB>();
        // b1 != b2
    }
}
```

* By passing **instantiation policy** to `container.Resolve()` method, so only this call will create a new instance.
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize();

        var myGame = container.Resolve<MyGame>();
        var myGame2 = container.Resolve<MyGame>(InstantiationPolicy.Transient);
        var myGame3 = container.Resolve<MyGame>(InstantiationPolicy.Transient);
        var myGame4 = container.Resolve<MyGame>();
        // myGame != myGame2 != myGame3
        // myGame == myGame4
    }
}
```

### Factory
To create instances on demand use **Factories** for a specific interface.
Just add `IFactory<MyInterface>` as a constructor argument and call `Resolve` with concrete type when needed.
It is also possible to override instantiation policy on resolve.
There is no need to bind **Factories**, they will be resolved automatically.
```c#
public interface IBullet
{
}

public class FireBullet : IBullet
{
}

public class ColdBullet : IBullet
{
}
```
```c#
public class Gun
{
    private readonly IFactory<IBullet> _factory;

    public Gun(IFactory<IBullet> factory)
    {
        _factory = factory;
    }

    public IBullet Fire()
    {
        return _factory.Resolve<FireBullet>(InstantiationPolicy.Transient);
    }

    public IBullet Cold()
    {
        return _factory.Resolve<ColdBullet>(InstantiationPolicy.Transient);
    }
}
```

### Working with Unity Objects

* Create an implementation of `SceneContext` class
```c#
public class MyGameSceneContext : SceneContext
{
    public override IEnumerable<Object> GetObjects()
    {
        return new Object[] { };
    }
}
```

* Create a **GameObject** in the root of the scene and attach the `SceneContext` implementation script to it

* Add `[SerializeField]` private fields for links to **Components** from scene, **Prefabs** or **Scriptable Object** assets and return it from `GetObjects()` method

```c#
public class MyGameSceneContext : SceneContext
{
    [SerializeField] private Camera _camera;
    [SerializeField] private MyHudView _hudView;
    [SerializeField] private BulletView _bulletPrefab;
    [SerializeField] private MyScriptableObjectConfig _config;

    public override IEnumerable<Object> GetObjects()
    {
        return new Object[] { _camera, _hudView, _bulletPrefab, _config };
    }
}
```

* Get `UnityObjectContainer` class as a constructor argument and receive objects by calling `unityObjectContainer.TryGetObject(out TObject object)` method
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
        if (!_unityObjectContainer.TryGetObject(out MyHudView hudView) ||
            !_unityObjectContainer.TryGetObject(out MyScriptableObjectConfig config))
        {
            return;
        }

        hudView.Show(config);
    }
}
```

### Unity Events

Implement `ITickable`, `IFixedTickable`, `ILateTickable`, `IPausable`, `IDisposable` to handle **Unity Events**. There is no need to bind these interfaces to a class, once instance will be resolved - **Unity Events** will be passed to it.

> Unity Update => ITickable<br>
Unity FixedUpdate => IFixedTickable<br>
Unity LateUpdate => ILateTickable<br>
Unity Pause => IPausable<br>
Unity Application.Quit => IDisposable

```c#
public class MyGame : ITickable, IDisposable
{
    public void Tick()
    {
        // called on Unity Update
    }

    public void Dispose()
    {
        // called on Application.Quit
    }
}
```

This functionality could be turned off by passing `false` to **Container** initialization
```c#
public class EntryPoint : MonoBehaviour
{
    private void Start()
    {
        var container = Container.Initialize(false);

        var myGame = container.Resolve<MyGame>();
        myGame.Play();
    }
}
```
```c#
public class MyGame : ITickable, IDisposable
{
    public void Tick()
    {
        // not called
    }

    public void Dispose()
    {
        // not called
    }
}
```

### Tests

To use **Bindlessdi** in tests call `Container.Initialize(false)` in the begining of a test and `container.Dispose` in the end.

```c#
public class MyTests
{
    [Test]
    public void TestResolve()
    {
            var container = Container.Initialize(false);

            var a = container.Resolve<A>();
            var b = container.Resolve<B>();
            var c = container.Resolve<C>();
            var d = container.Resolve<D>();

            Assert.True(a.B == b);
            Assert.True(a.C == b.C);
            Assert.True(a.C == c);
            Assert.True(b.C == c);
            Assert.True(b.D == c.D);
            Assert.True(b.D == d);

            container.Dispose();
    }
}
```
