using System;
using System.Collections.Generic;

namespace AdapterExampleCSh
{
  class Program
  {
    static void Main( string[] args )
    {
      TrafficLightManagerSingleton traffic_light_man = TrafficLightManagerSingleton.GetInstance();
      traffic_light_man.AddLight("red", new CtrALight("red"));
      traffic_light_man.AddLight( "green", new CtrBLight( "green" ) );
      traffic_light_man.ManageLights();
    }
  }

  ///  SDK EXAMPLE - START ///
  class VendorALight
  {
    public static void TurnOn()
    { }

    public static void TurnOff()
    { }
  }

  class VendorBLight
  {
    public static void TurnLight( bool isOn )
    { }
  }
  ///  SDK EXAMPLE - END ///

  interface ILight
  {
    void On();
    void Off();
  }

  public abstract class CtrLight : ILight
  {
    private string _colorName;

    protected CtrLight( string colorName)
    {
      _colorName = colorName;
    }
    
    public string GetColorName()
    {
      return _colorName;
    }

    public abstract void On();
    public abstract void Off();
  }

  class CtrALight : CtrLight
  {
    public CtrALight( string colorName ) : base( colorName ) { }

    public override void Off()
    {
      VendorALight.TurnOff();
      Console.WriteLine( "Vendor A's ({0}) light is off", GetColorName() );
    }

    public override void On()
    {
      VendorALight.TurnOn();
      Console.WriteLine( "Vendor A's ({0}) light is on", GetColorName() );
    }
  }

  class CtrBLight : CtrLight
  {
    public CtrBLight( string colorName ) : base( colorName ) { }

    public override void Off()
    {
      VendorBLight.TurnLight( false );
      Console.WriteLine( "Vendor B's ({0}) light is off", GetColorName() );
    }

    public override void On()
    {
      VendorBLight.TurnLight( true );
      Console.WriteLine( "Vendor B's ({0}) light is on", GetColorName() );
    }
  }

  class TrafficLightManagerSingleton
  {
    private static TrafficLightManagerSingleton _uniqueInstance;
    private Dictionary<string, CtrLight> _lights;
    private List<string> _light_names;
    private CtrLight _currentLight = null;
    private CtrLight _nextLight = null;

    private TrafficLightManagerSingleton() {
      _lights = new Dictionary<string, CtrLight>();

      _light_names = new List<string>();
      _light_names.Add( "red" );
      _light_names.Add( "green" );
    }

    public static TrafficLightManagerSingleton GetInstance()
    {
      if( _uniqueInstance == null )
        _uniqueInstance = new TrafficLightManagerSingleton();

      return _uniqueInstance;
    }

    private CtrLight GetNextLight()
    {
      Random rnd = new Random();
      int rnd_int = rnd.Next( _light_names.Count );
      
      return _lights[ (_light_names[ rnd_int ]) ];
    }

    private void SwitchLight()
    {
      if ( _currentLight != null)
        _currentLight.Off();

      _nextLight.On();

      _currentLight = _nextLight;
    }

    public void ManageLights()
    {
      while (true)
      {
        _nextLight = GetNextLight();
        SwitchLight();

        Console.WriteLine("---- delay ----");
        System.Threading.Thread.Sleep(5000);
      }
    }

    public void AddLight(string colorName, CtrLight newLight)
    {
      _lights.Add( colorName, newLight );
    }
  }
}
