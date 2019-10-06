using System;
using System.Collections.Generic;

namespace AdapterExampleCSh
{
  class Program
  {
    static void Main( string[] args )
    {
      TrafficLightManagerSingleton traffic_light_man = TrafficLightManagerSingleton.GetInstance();

      LightFactory lightFactory = new LightFactory();

      System.IO.StreamReader file = new System.IO.StreamReader( @"light_config.txt" );

      int counter = 0;
      string line;

      while( ( line = file.ReadLine() ) != null )
      {
        counter++;
        string [] lightsConfig = line.Trim().Split( '-' );
        
        if( lightsConfig.Length != 2 )
          throw new Exception("Config format. [Light_vendor]-[color_name]. Ex: A-green. Your's " + line);

        traffic_light_man.AddLight( lightFactory.CreateLight( lightsConfig[0], lightsConfig[1] ) );
      }

      if (counter < 1)
        throw new Exception( "File is not found or the file doesnt contain a config.");
      
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
      _colorName = colorName.ToUpper();
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
      Console.WriteLine( "{0} light (A) is off", GetColorName() );
    }

    public override void On()
    {
      VendorALight.TurnOn();
      Console.WriteLine( "{0} light (A) is on", GetColorName() );
    }
  }

  class CtrBLight : CtrLight
  {
    public CtrBLight( string colorName ) : base( colorName ) { }

    public override void Off()
    {
      VendorBLight.TurnLight( false );
      Console.WriteLine( "{0} light (B) is off", GetColorName() );
    }

    public override void On()
    {
      VendorBLight.TurnLight( true );
      Console.WriteLine( "{0} light (B) is on", GetColorName() );
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
        // simulate delay
        System.Threading.Thread.Sleep(5000);
      }
    }

    public void AddLight( CtrLight newLight)
    {
      if( _light_names.IndexOf( newLight.GetColorName() ) == -1 )
        _light_names.Add( newLight.GetColorName() );
      else throw new Exception("The same color has been registered.");

      _lights.Add( newLight.GetColorName(), newLight );
    }
  }

  class LightFactory
  {
    public CtrLight CreateLight(string vendor, string colorName)
    {
      CtrLight newLight = null;

      if( vendor.Equals( "A" ) )
      {
        newLight = new CtrALight( colorName );
      }
      else if( vendor.Equals( "B" ) )
      {
        newLight = new CtrBLight( colorName );
      }
      else
        throw new Exception("Pick vendor A or B. Your's - " + vendor);

      return newLight;
    }
  }

}
