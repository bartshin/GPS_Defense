using System;

namespace Architecture
{
  public interface IPooledObject 
  {
    public Action<IPooledObject> OnDisabled { get; set; }
  }
}
