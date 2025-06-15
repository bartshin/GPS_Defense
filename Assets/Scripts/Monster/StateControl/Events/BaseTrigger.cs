
namespace Monster
{
  public abstract class Trigger : _ScriptableObject
  {
    public abstract bool IsTriggerToReact(Controller controller);
  }

}
