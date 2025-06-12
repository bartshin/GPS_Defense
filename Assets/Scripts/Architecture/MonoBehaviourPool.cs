using UnityEngine;

namespace Architecture
{
  public class MonoBehaviourPool<T>: ObjectPool<T> where T: MonoBehaviour, IPooledObject
  {

    GameObject prefab;
    System.Action<T> initPooledObject;

    public MonoBehaviourPool(
      int poolSize,
      int? maxPoolSize = null,
      GameObject prefab = null,
      System.Action<T> initPooledObject = null
      ): base(poolSize, maxPoolSize)
    { 
      this.prefab = prefab;
      this.initPooledObject = initPooledObject;
    }

    protected override T CreatePooledObject()
    {
      var gameObject = this.prefab != null ?
        Object.Instantiate(this.prefab):
        new GameObject(nameof(T));
      T monoBehaviour = gameObject.GetComponent<T>();
      if (monoBehaviour == null) {
        monoBehaviour = gameObject.AddComponent<T>();
      }
      if (this.initPooledObject != null) {
        this.initPooledObject(monoBehaviour);
      }
      return (monoBehaviour);
    }

    protected override void OnTakeFromPool(T obj) 
    {
      obj.gameObject.SetActive(true);
    }

    protected override void OnReturnedToPool(T obj)
    {
      if (obj.gameObject.activeSelf) {
        obj.gameObject.SetActive(false);
      }
    }
  }
}
