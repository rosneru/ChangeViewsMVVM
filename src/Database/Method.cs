using System;
using Database.Enums;
using PropertyChanged;

namespace Database
{
  [AddINotifyPropertyChangedInterface]
  public class Method : IEquatable<Method>
  {
    public int Id { get; set; }

    public string Name { get; set; }
    public string ConfigFile { get; set; }

    public long LinearTableSpeed 
    { 
      get; 
      set; 
    }
    public long LinearTableVolume { get; set; }

    public PressureSensorMode PressureSensorMode 
    { 
      get; 
      set; 
    }
    public long PressureSensorInterval { get; set; }

    public long TemperatureSensorInterval { get; set; }

    public Method()
    {
      ConfigFile = "<None selected>";
      LinearTableSpeed = 0;
      LinearTableVolume = 0;
      PressureSensorInterval = 0;
      PressureSensorMode = PressureSensorMode.Relative;
      TemperatureSensorInterval = 0;
    }


    public Method DeepCopy()
    {
      var other = new Method();
      other.Name = string.Copy(Name);
      other.ConfigFile = string.Copy(ConfigFile);
      other.LinearTableSpeed = LinearTableSpeed;
      other.LinearTableVolume = LinearTableVolume;
      other.PressureSensorInterval = PressureSensorInterval;
      other.PressureSensorMode = PressureSensorMode;
      other.TemperatureSensorInterval = TemperatureSensorInterval;

      return other;
    }

    public bool Equals(Method other)
    {
      if(other == null)
      {
        return false;
      }

      return Name.Equals(other.Name) &&
             ConfigFile.Equals(other.ConfigFile) &&
             LinearTableSpeed.Equals(other.LinearTableSpeed) &&
             LinearTableVolume.Equals(other.LinearTableVolume) &&
             PressureSensorInterval.Equals(other.PressureSensorInterval) &&
             PressureSensorMode.Equals(other.PressureSensorMode) &&
             TemperatureSensorInterval.Equals(other.TemperatureSensorInterval);
    }

  }
}
