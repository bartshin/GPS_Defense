
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
public class _MonoBehaviour: SerializedMonoBehaviour {}
public class _ScriptableObject: SerializedScriptableObject {}
#else
public class _MonoBehaviour: MonoBehaviour {}
public class _ScriptableObject: ScriptableObject {}
#endif
