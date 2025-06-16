
namespace Unit
{
  public abstract class Trigger : _ScriptableObject
  {
    public abstract bool IsTriggerToReact(BaseUnit unit);
  }

}
