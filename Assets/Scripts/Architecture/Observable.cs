using System;
using UnityEngine;

namespace Architecture
{
  public class ObservableValue<T>
  {
    public T Value {
      get => this.innerValue;
      set {
        if (value == null) {
          if (this.innerValue == null) {
            return;
          }
          else {
            this.innerValue = default(T);
            this.OnChanged?.Invoke(this.innerValue);
          }
        }
        else {
          if (this.innerValue == null) {
            this.innerValue = value;
            this.OnChanged?.Invoke(this.innerValue);
          }
          else if (!this.innerValue.Equals(value)) {
            this.innerValue = value;
            this.OnChanged?.Invoke(this.innerValue);
          }
        }
      }
    }

    public ObservableValue(T initialValue = default)
    {
      this.innerValue = initialValue;
    }

    public void DestorySelf()
    {
      if (this.OnDestory != null) {
        this.OnDestory.Invoke();
      }
    }

    public Action<T> OnChanged;
    public Action OnDestory;

    [SerializeField]
    T innerValue;
  }
}
