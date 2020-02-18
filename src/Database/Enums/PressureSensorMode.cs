namespace Database.Enums
{
  public class PressureSensorMode : BaseEnumeration
  {
    public static PressureSensorMode Relative = new PressureSensorMode(0, "relative");
    public static PressureSensorMode Absolute = new PressureSensorMode(1, "absolute");

    public PressureSensorMode(int id, string name) : base(id, name)
    {
    }
  }
}
