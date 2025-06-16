using UnityEngine;

namespace Architecture
{
  public interface ISingleton<T>
  {
     static T Shared { get; private set; } 
  }

  public abstract class Singleton<T>: ISingleton<T> where T: class, new()
  {
    static T instance;
    public static T Shared 
    {
      get {
        if (Singleton<T>.instance != null ) {
          return (Singleton<T>.instance);
        }
        Singleton<T>.instance = new ();
        return (Singleton<T>.instance);
      }
    }
  }

  public class SingletonBehaviour<T>: _MonoBehaviour where T: _MonoBehaviour
  {
    static T instance;

    public static T Shared => SingletonBehaviour<T>.instance;
  
    public static void Destroy()
    {
      if (SingletonBehaviour<T>.instance != null) {
        Destroy(SingletonBehaviour<T>.Shared.gameObject);
        SingletonBehaviour<T>.instance = null;
      }
    }

    protected static T CreateInstance() 
    {
      var gameObject = new GameObject(typeof(T).Name);
      DontDestroyOnLoad(gameObject);
      return (gameObject.AddComponent<T>());
    }

    protected virtual void Awake()
    {
      if (SingletonBehaviour<T>.instance == null) {
        SingletonBehaviour<T>.instance = this as T;
      }
      else if (SingletonBehaviour<T>.instance != this) {
        Destroy(this.gameObject);
      }
    }

    protected void OnDestroyed()
    {
      SingletonBehaviour<T>.instance = null;
    }
  }
}
