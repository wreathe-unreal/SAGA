using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EComponentPower
{
    None, //0
    Shuttle, 
    Cruiser, 
    Frigate,
    Destroyer,
    Battleship,
    Titan,
    Worldship,
    Excalibur
}

public enum EComponentType
{
    None, //0
    ShieldGenerator,
    PortalDrive,
    KineticWeapon,
    PowerSupply,
    CargoHold,
    EnergyWeapon,
    Engine,
    Thrusters,
}

public class StarshipComponent
{
    public EComponentPower Power;
    public EComponentType Type;

    public StarshipComponent(EComponentPower power, EComponentType type)
    {
        this.Power = power;
        this.Type = type;
    }
}

public class Starship : MonoBehaviour
{

    public int MaxHealth;
    public int CurrentHealth;
    
    public Dictionary<EComponentType, StarshipComponent> Components = new Dictionary<EComponentType, StarshipComponent>
    {
        {EComponentType.PowerSupply, null},
        {EComponentType.ShieldGenerator, null},
        {EComponentType.CargoHold, null},
        {EComponentType.PortalDrive, null},
        {EComponentType.KineticWeapon, null},
        {EComponentType.EnergyWeapon, null},
        {EComponentType.Engine, null},
        {EComponentType.Thrusters, null},
    };

    private static Dictionary<string, EComponentType> EComponentTypeLookup = new Dictionary<string, EComponentType>
    {
        {"Power Supply", EComponentType.PowerSupply},
        {"Shield Generator", EComponentType.ShieldGenerator},
        {"Portal Drive", EComponentType.PortalDrive},
        {"Kinetic Weapon", EComponentType.KineticWeapon},
        {"Energy Weapon", EComponentType.EnergyWeapon},
        {"Engine", EComponentType.Engine},
        {"Thrusters", EComponentType.Thrusters}
    };

    public bool TryAutoEquip(Card c)
    {
        EComponentType ect = EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type];
        
        if (Components[ect] == null)
        {
            ReplacePart(c);
            Components[ect] = StarshipComponentDB[c.ID];
            UpdateMaxHealth();
            return true;
        }
        return false;
    }
    
    public void ReplacePart(Card c)
    {
        ReplaceCard(c);
        EComponentType ect = GetCardECT(c);
        Components[ect] = StarshipComponentDB[c.ID];
        ModifyCurrentHealth((int)Components[ect].Power);
        UpdateMaxHealth();
    }

    public void ReplaceCard(Card c)
    {
        EComponentType ect = GetCardECT(c);
        Board.Decks["Starship"].Cards[(int)ect - 1] = c; //our ECT are 1 based but deck list is 0 based
    }

    public static EComponentType GetCardECT(Card c)
    {
        EComponentType ect = EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type];
        return ect;
    }
    
    public void UpdateMaxHealth()
    {
        MaxHealth = 110 * (int) GetShipPower();
    }

    public int ModifyCurrentHealth(int modifier)
    {
        // Add the modifier to the current health
        CurrentHealth += modifier;

        // Clamp CurrentHealth between 0 and MaxHealth
        CurrentHealth = Math.Max(0, Math.Min(CurrentHealth, MaxHealth));

        // Return the modified and clamped CurrentHealth
        return CurrentHealth;
    }
    
    public float GetShipPower()
    {
        float powerSum = 0f;
        int greatest = 0;
        int least = Int32.MaxValue;
        
        foreach (StarshipComponent ssc in Components.Values)
        {
            powerSum += (int)ssc.Power;

            if (greatest < (int)ssc.Power)
            {
                greatest = (int)ssc.Power;
            }
            if (least > (int)ssc.Power)
            {
                least = (int)ssc.Power;
            }
        }

        //ignore the greatest and least values
        //multiply by 4/3 to get back to a 8 spaceship component basis average
        powerSum -= greatest;
        powerSum -= least;
        powerSum *= 4f / 3f;

        return powerSum;

    }

    public string GetShipClass()
    {
        float power = GetShipPower();
        
        return power switch
        { 
            < 4  => "shuttle",
            < 6  => "lantern",
            < 8  => "cutter",
            < 10 => "lancer",
            < 12 => "caravan",
            < 14 => "eagle",
            < 16 => "caravel",
            < 18 => "voyager",
            < 20 => "galleon",
            < 22 => "frigate",
            < 24 => "destroyer",
            < 26 => "lead_destroyer",
            < 28 => "battle_destroyer",
            < 30 => "light_cruiser", 
            < 32 => "cruiser",
            < 34 => "super_cruiser",
            < 36 => "battle_cruiser",
            < 38 => "commander_cruiser",
            < 40 => "battleship",
            < 42 => "imperium",
            < 44 => "carrier",
            < 46 => "super_carrier",
            < 48 => "titan",
            < 50 => "covenant",
            < 52 => "dreadnought",
            < 54 => "space_station",
            < 56 => "worldship",
            <=64 => "excalibur",
            > 64 => "excalibur",
            _ => throw new ArgumentOutOfRangeException(nameof(power), $"Not expected power value: {power}")
        };
    }

    public int GetShipHealth()
    {
        return (int) GetShipPower() * 110;
    }
    
    static public Dictionary<string, StarshipComponent> StarshipComponentDB = new Dictionary<string, StarshipComponent>
    {
        //shuttle components normalized to power of 8
        {"Jolt Battery", new StarshipComponent(EComponentPower.Shuttle, EComponentType.PowerSupply)},
        {"Cracked Capacitor", new StarshipComponent(EComponentPower.Shuttle, EComponentType.ShieldGenerator)},
        {"Microvault", new StarshipComponent(EComponentPower.Shuttle, EComponentType.CargoHold)},
        {"Spark Jump", new StarshipComponent(EComponentPower.Shuttle, EComponentType.PortalDrive)},
        {"Rattler", new StarshipComponent(EComponentPower.Shuttle, EComponentType.KineticWeapon)},
        {"Glitchray", new StarshipComponent(EComponentPower.Shuttle, EComponentType.EnergyWeapon)},
        {"Junked Materia Engine", new StarshipComponent(EComponentPower.Shuttle, EComponentType.Engine)},
        {"Sputterjets", new StarshipComponent(EComponentPower.Shuttle, EComponentType.Thrusters)},
        
        // Frigate components 16
        {"Quintessence Tubes", new StarshipComponent(EComponentPower.Frigate, EComponentType.PowerSupply)},
        {"Flash Barrier", new StarshipComponent(EComponentPower.Frigate, EComponentType.ShieldGenerator)},
        {"Storage Lockers", new StarshipComponent(EComponentPower.Frigate, EComponentType.CargoHold)},
        {"Blink Drive", new StarshipComponent(EComponentPower.Frigate, EComponentType.PortalDrive)},
        {"Chaingun", new StarshipComponent(EComponentPower.Frigate, EComponentType.KineticWeapon)},
        {"Pulse Laser", new StarshipComponent(EComponentPower.Frigate, EComponentType.EnergyWeapon)},
        {"Furnace Blaster", new StarshipComponent(EComponentPower.Frigate, EComponentType.Engine)},
        {"Dwarven Balancers", new StarshipComponent(EComponentPower.Frigate, EComponentType.Thrusters)},

        // destroyer Components 24
        {"Quintessence Sphere", new StarshipComponent(EComponentPower.Destroyer, EComponentType.PowerSupply)},
        {"Mirage Cloak", new StarshipComponent(EComponentPower.Destroyer, EComponentType.ShieldGenerator)},
        {"Storage Vault", new StarshipComponent(EComponentPower.Destroyer, EComponentType.CargoHold)},
        {"Blitz Drive", new StarshipComponent(EComponentPower.Destroyer, EComponentType.PortalDrive)},
        {"Gatling Emplacement", new StarshipComponent(EComponentPower.Destroyer, EComponentType.KineticWeapon)},
        {"Beam Coils", new StarshipComponent(EComponentPower.Destroyer, EComponentType.EnergyWeapon)},
        {"Bronze Rocket", new StarshipComponent(EComponentPower.Destroyer, EComponentType.Engine)},
        {"Runic Flames", new StarshipComponent(EComponentPower.Destroyer, EComponentType.Thrusters)},

        // Cruiser components 32
        {"Quintessence Nexus", new StarshipComponent(EComponentPower.Cruiser, EComponentType.PowerSupply)},
        {"Prism Ward", new StarshipComponent(EComponentPower.Cruiser, EComponentType.ShieldGenerator)},
        {"Freight Bay", new StarshipComponent(EComponentPower.Cruiser, EComponentType.CargoHold)},
        {"Galaxy Drive", new StarshipComponent(EComponentPower.Cruiser, EComponentType.PortalDrive)},
        {"Heavy Turret", new StarshipComponent(EComponentPower.Cruiser, EComponentType.KineticWeapon)},
        {"Ray Beams", new StarshipComponent(EComponentPower.Cruiser, EComponentType.EnergyWeapon)},
        {"Eclipse Engines", new StarshipComponent(EComponentPower.Cruiser, EComponentType.Engine)},
        {"Dragon's Breath", new StarshipComponent(EComponentPower.Cruiser, EComponentType.Thrusters)},

        // Battleship components 40
        {"Quintessence Core", new StarshipComponent(EComponentPower.Battleship, EComponentType.PowerSupply)},
        {"Spectral Wall", new StarshipComponent(EComponentPower.Battleship, EComponentType.ShieldGenerator)},
        {"Storage Complex", new StarshipComponent(EComponentPower.Battleship, EComponentType.CargoHold)},
        {"Flux Drive", new StarshipComponent(EComponentPower.Battleship, EComponentType.PortalDrive)},
        {"Broadsides", new StarshipComponent(EComponentPower.Battleship, EComponentType.KineticWeapon)},
        {"Scorching Ray", new StarshipComponent(EComponentPower.Battleship, EComponentType.EnergyWeapon)},
        {"Mithril Jets", new StarshipComponent(EComponentPower.Battleship, EComponentType.Engine)},
        {"Leviathan Lift", new StarshipComponent(EComponentPower.Battleship, EComponentType.Thrusters)},

        // Titan components 48
        {"Quintessence Cradle", new StarshipComponent(EComponentPower.Titan, EComponentType.PowerSupply)},
        {"Aegis", new StarshipComponent(EComponentPower.Titan, EComponentType.ShieldGenerator)},
        {"Cargo Deck", new StarshipComponent(EComponentPower.Titan, EComponentType.CargoHold)},
        {"Dimensional Ripper", new StarshipComponent(EComponentPower.Titan, EComponentType.PortalDrive)},
        {"Havoc Array", new StarshipComponent(EComponentPower.Titan, EComponentType.KineticWeapon)},
        {"Laser Matrix", new StarshipComponent(EComponentPower.Titan, EComponentType.EnergyWeapon)},
        {"Titan Bellows", new StarshipComponent(EComponentPower.Titan, EComponentType.Engine)},
        {"Atlas Stabilizers", new StarshipComponent(EComponentPower.Titan, EComponentType.Thrusters)},

        // Worldship components 56
        {"Quintessence Reactor", new StarshipComponent(EComponentPower.Worldship, EComponentType.PowerSupply)},
        {"Planetary Dome", new StarshipComponent(EComponentPower.Worldship, EComponentType.ShieldGenerator)},
        {"Worldvault", new StarshipComponent(EComponentPower.Worldship, EComponentType.CargoHold)},
        {"Stargate", new StarshipComponent(EComponentPower.Worldship, EComponentType.PortalDrive)},
        {"Skypiercers", new StarshipComponent(EComponentPower.Worldship, EComponentType.KineticWeapon)},
        {"Hyperbeam", new StarshipComponent(EComponentPower.Worldship, EComponentType.EnergyWeapon)},
        {"Supermassive Burners", new StarshipComponent(EComponentPower.Worldship, EComponentType.Engine)},
        {"Orbit Shifters", new StarshipComponent(EComponentPower.Worldship, EComponentType.Thrusters)},

        // Excalibur components 64
        {"Quintessence Divijector", new StarshipComponent(EComponentPower.Excalibur, EComponentType.PowerSupply)},
        {"Unity Veil", new StarshipComponent(EComponentPower.Excalibur, EComponentType.ShieldGenerator)},
        {"Sheathe of Excalibur", new StarshipComponent(EComponentPower.Excalibur, EComponentType.CargoHold)},
        {"Reality Rift", new StarshipComponent(EComponentPower.Excalibur, EComponentType.PortalDrive)},
        {"Peacemaker", new StarshipComponent(EComponentPower.Excalibur, EComponentType.KineticWeapon)},
        {"Grail Gun", new StarshipComponent(EComponentPower.Excalibur, EComponentType.EnergyWeapon)},
        {"Solaris", new StarshipComponent(EComponentPower.Excalibur, EComponentType.Engine)},
        {"Galeforce", new StarshipComponent(EComponentPower.Excalibur, EComponentType.Thrusters)},
    };

    public bool GetBattleResults(int enemyPower)
    {
        float results = enemyPower - GetShipPower();

        if (results >= 0)
        {
            ModifyCurrentHealth((int)results * 220);
            return false;
        }
        else
        {
            ModifyCurrentHealth(((int)GetShipPower() - enemyPower) * 110);
            return true;
        }
    }

    public int GetShipUpkeep()
    {
        return (int)GetShipPower() * 2;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}








