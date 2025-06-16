using System.Linq;
using UnityEngine;

public static class Utils
{
  public static Transform RecursiveFindChild(Transform parent, string childName)
  {
    foreach (Transform child in parent)
    {
      if(child.name == childName)
      {
        return child;
      }
      else
      {
        Transform found = RecursiveFindChild(child, childName);
        if (found != null)
        {
          return found;
        }
      }
    }
    return null;
  }

  public static T[] ConcatArrays<T>(params T[][] list)
  {
    var result = new T[list.Sum(a => a.Length)];
    int offset = 0;
    for (int x = 0; x < list.Length; x++)
    {
      list[x].CopyTo(result, offset);
      offset += list[x].Length;
    }
    return result;
  }
}
