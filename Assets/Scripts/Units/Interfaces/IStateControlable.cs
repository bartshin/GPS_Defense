using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
  public interface IStateControlable 
  {
    public StateController StateController { get; }
  }
}
