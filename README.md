# UnityUtilities



### EnumeratorExtension

```c#
public static IEnumerable<T> GetEnumerator<T>(this IEnumerator<T> enumerator)
```



### Singleton

1. Normal Singleton
2. PersistentSingleton (destroy any new singleton)
3. RegulatorSingleton (destory any old singleton)



### TransformExtensions

```c#
public static void DestroyChildren(this Transform parent)
public static void EnableChildren(this Transform parent)
public static void DisableChildren(this Transform parent)
```



### Vector3Extensions

```c#
public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
```



### GameObjectExtensions

```c#
public static T GetOrAdd<T>(this GameObject gameObject)
public static T OrNull<T> (this T obj)
public static void DestroyChildren(this GameObject gameObject)
```



### TypeFilter

Auto get all subtype from interface or abstract class

usage:

```c#
public class TestTypeFilter : MonoBehaviour
    {
        [Header("State Machine")] 
        [TypeFilter(typeof(IState))] [SerializeField]
        private SerializableType staringState;

        private void Start()
        {
            Debug.Log(staringState.Type);
            object instance = Activator.CreateInstance(staringState.Type);
            
            IState currentState = (IState)instance;
            currentState.OnEnter();
        }
    }

    public interface IState
    {
        void OnEnter();
        void OnExit();
    }

    public abstract class baseState : IState
    {
        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }

    public class IdleState : baseState
    {
        public override void OnEnter()
        {
            Debug.Log("Idle");
        }
    }

    public class RunState : baseState
    {
        public override void OnEnter()
        {
            Debug.Log("Run");
        }
    }
```

