using System;
using System.Xml;

namespace OSM
{
  public static class OsmUtility 
  {
    public static T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
    {
      //FIXME: Check attr is exist
      string valueStr = attributes[attrName].Value;
      return ((T)Convert.ChangeType(valueStr, typeof(T)));
    }
  }
}
