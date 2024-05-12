using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {

    }
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
        {"Cargo Hold", EComponentType.CargoHold},
        {"Power Supply", EComponentType.PowerSupply},
        {"Shield Generator", EComponentType.ShieldGenerator},
        {"Portal Drive", EComponentType.PortalDrive},
        {"Kinetic Weapon", EComponentType.KineticWeapon},
        {"Energy Weapon", EComponentType.EnergyWeapon},
        {"Engine", EComponentType.Engine},
        {"Thrusters", EComponentType.Thrusters}
    };

    public bool AutoEquip(Card c)
    {
        EComponentType ect = EComponentTypeLookup[c.Data.Type];

        if (Components[ect] == null ||Components[ect].Power < StarshipComponentDB[c.Name].Power)
        {
            Board.Decks["Starship"].Cards[(int)ect - 1] = c; // ect is 1 based
            Components[ect] = StarshipComponentDB[c.Name];
            ModifyCurrentHealth(((int)Components[ect].Power * 110));
            return true;
        }
        else
        {
            Board.State.AddCard(c.ID, 1, false);
            return false;
        }
    }
    
    public void UpdateFleetCard()
    {
        //if we have the 4 core ship components
        if (Components[EComponentType.PowerSupply] != null && Components[EComponentType.Engine] != null &&
            Components[EComponentType.Thrusters] != null && Components[EComponentType.PortalDrive] != null)
        {
            if (Board.Decks["Fleet"].Cards.Count != 0 && GetShipClass() == Board.Decks["Fleet"].Cards[0].ID)
            {
                return;
            }

            if (Board.Decks["Fleet"].Cards.Count != 0)
            {
                Board.DestroyCard(Board.Decks["Fleet"].Cards[0]);
                return;
            }
            
            Board.Decks["Fleet"].Cards = Board.Decks["Fleet"].Cards.Prepend(Board.State.AddCard(GetShipClass(), 1, true)).ToList();
        }
        else
        {
            return;
        }
    }
    
    public static EComponentType GetCardECT(Card c)
    {
        EComponentType ect = EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type];
        return ect;
    }
    
    public int GetMaxHealth()
    {
        return (int) (110 * (1 + GetShipPower()));
    }

    public int ModifyCurrentHealth(int modifier)
    {
        // Add the modifier to the current health
        CurrentHealth += modifier;

        // Clamp CurrentHealth between 0 and MaxHealth
        CurrentHealth = Math.Max(0, Math.Min(CurrentHealth, (int)GetMaxHealth()));

        // Return the modified and clamped CurrentHealth
        return CurrentHealth;
    }
    
    public float GetShipPower()
    {
        float powerSum = 0f;
        
        foreach (StarshipComponent ssc in Components.Values)
        {
            if (ssc != null)
            {
                powerSum += (int)ssc.Power;
            }
        }

        return powerSum;
    }

    public string GetShipClass()
    {
        float power = GetShipPower();
        
        return power switch
        { 
            <= 4  => "shuttle",
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
            ModifyCurrentHealth((int)(-1 * results * 220)); //arbitrary damage for losing
            Player.bLastBattleWasWin = false;
            return false;
        }
        else
        {
            ModifyCurrentHealth((int)(results * 110));
            Player.bLastBattleWasWin = true;
            return true;
        }
    }

    public int GetShipUpkeep()
    {
        return (int)GetShipPower() * 2;
    }
    
}








