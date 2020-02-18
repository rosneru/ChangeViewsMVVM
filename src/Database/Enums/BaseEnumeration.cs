using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Database.Enums
{
  public abstract class BaseEnumeration : IComparable
  {
    public int Id { get; private set; }
    public string Name { get; private set; }

    protected BaseEnumeration(int id, string name)
    {
      Name = name;
      Id = id;
    }

    public override string ToString()
    {
      return Name;
    }

    public static IEnumerable<T> GetAll<T>() where T : BaseEnumeration
    {
      var fields = typeof(T).GetFields(BindingFlags.Public |
                                       BindingFlags.Static |
                                       BindingFlags.DeclaredOnly);

      return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(Object obj)
    {
      if (!(obj is BaseEnumeration otherValue))
      {
        return false;
      }

      var typeMatches = GetType().Equals(obj.GetType());
      var valueMatches = Id.Equals(otherValue.Id);

      return typeMatches && valueMatches;
    }

    public int CompareTo(object obj)
    {
      return Id.CompareTo(((BaseEnumeration) obj).Id);
    }

    public override int GetHashCode()
    {
      var hashCode = -1919740922;
      hashCode = hashCode * -1521134295 + Id.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
      return hashCode;
    }
  }
}
