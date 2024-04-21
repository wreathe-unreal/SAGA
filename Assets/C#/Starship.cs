using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EComponentSize
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
    PowerSupply,
    ShieldGenerator,
    CargoHold,
    PortalDrive,
    KineticWeapon,
    EnergyWeapon,
    Engine,
    Thrusters,
}

public class StarshipComponent
{
    public EComponentSize Size;
    public EComponentType Type;

    public StarshipComponent(EComponentSize size, EComponentType type)
    {
        this.Size = size;
        this.Type = type;
    }
}

public class Starship : MonoBehaviour
{
    

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

    private Dictionary<string, EComponentType> EComponentTypeLookup = new Dictionary<string, EComponentType>
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
        if (Components[EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type]] == null)
        {
            Components[EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type]] = StarshipComponentDB[c.ID];
            return true;
        }
        return false;
    }
    
    public void ReplacePart(Card c)
    {
        //add card to starship deck
        Components[EComponentTypeLookup[CardDB.CardDataLookup[c.ID].Type]] = StarshipComponentDB[c.ID];
        //send back the old part as an object that must be crafted again
    }
    
    public float GetShipPower()
    {
        float powerSum = 0f;
        int greatest = 0;
        int least = Int32.MaxValue;
        
        foreach (StarshipComponent ssc in Components.Values)
        {
            powerSum += (int)ssc.Size;

            if (greatest < (int)ssc.Size)
            {
                greatest = (int)ssc.Size;
            }
            if (least > (int)ssc.Size)
            {
                least = (int)ssc.Size;
            }
        }

        //ignore the greatest and least values
        //and divide by 6 to get the remaining average
        //multiply by 4/3 to get back to a 8 spaceship component basis average
        powerSum -= greatest;
        powerSum -= least;
        powerSum /= 6;
        powerSum *= 4f / 3f;

        return powerSum;

    }
    
    static public Dictionary<string, StarshipComponent> StarshipComponentDB = new Dictionary<string, StarshipComponent>
    {
        // Shuttle components normalized to power of 8
        {"Jolt Battery", new StarshipComponent(EComponentSize.Shuttle, EComponentType.PowerSupply)},
        {"Cracked Capacitor", new StarshipComponent(EComponentSize.Shuttle, EComponentType.ShieldGenerator)},
        {"Microvault", new StarshipComponent(EComponentSize.Shuttle, EComponentType.CargoHold)},
        {"Spark Jump", new StarshipComponent(EComponentSize.Shuttle, EComponentType.PortalDrive)},
        {"Rattler", new StarshipComponent(EComponentSize.Shuttle, EComponentType.KineticWeapon)},
        {"Glitchray", new StarshipComponent(EComponentSize.Shuttle, EComponentType.EnergyWeapon)},
        {"Junked Materia Engine", new StarshipComponent(EComponentSize.Shuttle, EComponentType.Engine)},
        {"Sputterjets", new StarshipComponent(EComponentSize.Shuttle, EComponentType.Thrusters)},

        // Cruiser components 16
        {"Quintessence Tubes", new StarshipComponent(EComponentSize.Cruiser, EComponentType.PowerSupply)},
        {"CruiserShieldGenerator", new StarshipComponent(EComponentSize.Cruiser, EComponentType.ShieldGenerator)},
        {"Storage Lockers", new StarshipComponent(EComponentSize.Cruiser, EComponentType.CargoHold)},
        {"Blitz Drive", new StarshipComponent(EComponentSize.Cruiser, EComponentType.PortalDrive)},
        {"CruiserKineticWeapon", new StarshipComponent(EComponentSize.Cruiser, EComponentType.KineticWeapon)},
        {"CruiserEnergyWeapon", new StarshipComponent(EComponentSize.Cruiser, EComponentType.EnergyWeapon)},
        {"CruiserEngine", new StarshipComponent(EComponentSize.Cruiser, EComponentType.Engine)},
        {"CruiserThrusters", new StarshipComponent(EComponentSize.Cruiser, EComponentType.Thrusters)},

        // Frigate components 24
        {"Quintessence Sphere", new StarshipComponent(EComponentSize.Frigate, EComponentType.PowerSupply)},
        {"FrigateShieldGenerator", new StarshipComponent(EComponentSize.Frigate, EComponentType.ShieldGenerator)},
        {"Storage Vault", new StarshipComponent(EComponentSize.Frigate, EComponentType.CargoHold)},
        {"Voidweaver", new StarshipComponent(EComponentSize.Frigate, EComponentType.PortalDrive)},
        {"FrigateKineticWeapon", new StarshipComponent(EComponentSize.Frigate, EComponentType.KineticWeapon)},
        {"FrigateEnergyWeapon", new StarshipComponent(EComponentSize.Frigate, EComponentType.EnergyWeapon)},
        {"FrigateEngine", new StarshipComponent(EComponentSize.Frigate, EComponentType.Engine)},
        {"FrigateThrusters", new StarshipComponent(EComponentSize.Frigate, EComponentType.Thrusters)},

        // Destroyer components 32
        {"Quintessence Nexus", new StarshipComponent(EComponentSize.Destroyer, EComponentType.PowerSupply)},
        {"DestroyerShieldGenerator", new StarshipComponent(EComponentSize.Destroyer, EComponentType.ShieldGenerator)},
        {"Freight Bay", new StarshipComponent(EComponentSize.Destroyer, EComponentType.CargoHold)},
        {"DestroyerPortalDrive", new StarshipComponent(EComponentSize.Destroyer, EComponentType.PortalDrive)},
        {"DestroyerKineticWeapon", new StarshipComponent(EComponentSize.Destroyer, EComponentType.KineticWeapon)},
        {"DestroyerEnergyWeapon", new StarshipComponent(EComponentSize.Destroyer, EComponentType.EnergyWeapon)},
        {"DestroyerEngine", new StarshipComponent(EComponentSize.Destroyer, EComponentType.Engine)},
        {"DestroyerThrusters", new StarshipComponent(EComponentSize.Destroyer, EComponentType.Thrusters)},

        // Battleship components 40
        {"Quintessence Core", new StarshipComponent(EComponentSize.Battleship, EComponentType.PowerSupply)},
        {"BattleshipShieldGenerator", new StarshipComponent(EComponentSize.Battleship, EComponentType.ShieldGenerator)},
        {"Storage Complex", new StarshipComponent(EComponentSize.Battleship, EComponentType.CargoHold)},
        {"BattleshipPortalDrive", new StarshipComponent(EComponentSize.Battleship, EComponentType.PortalDrive)},
        {"BattleshipKineticWeapon", new StarshipComponent(EComponentSize.Battleship, EComponentType.KineticWeapon)},
        {"BattleshipEnergyWeapon", new StarshipComponent(EComponentSize.Battleship, EComponentType.EnergyWeapon)},
        {"Dragon's Breath", new StarshipComponent(EComponentSize.Battleship, EComponentType.Engine)},
        {"BattleshipThrusters", new StarshipComponent(EComponentSize.Battleship, EComponentType.Thrusters)},

        // Titan components 48
        {"Quintessence Cradle", new StarshipComponent(EComponentSize.Titan, EComponentType.PowerSupply)},
        {"Aegis", new StarshipComponent(EComponentSize.Titan, EComponentType.ShieldGenerator)},
        {"Cargo Deck", new StarshipComponent(EComponentSize.Titan, EComponentType.CargoHold)},
        {"TitanPortalDrive", new StarshipComponent(EComponentSize.Titan, EComponentType.PortalDrive)},
        {"TitanKineticWeapon", new StarshipComponent(EComponentSize.Titan, EComponentType.KineticWeapon)},
        {"TitanEnergyWeapon", new StarshipComponent(EComponentSize.Titan, EComponentType.EnergyWeapon)},
        {"TitanEngine", new StarshipComponent(EComponentSize.Titan, EComponentType.Engine)},
        {"TitanThrusters", new StarshipComponent(EComponentSize.Titan, EComponentType.Thrusters)},

        // Worldship components 56
        {"Quintessence Reactor", new StarshipComponent(EComponentSize.Worldship, EComponentType.PowerSupply)},
        {"WorldshipShieldGenerator", new StarshipComponent(EComponentSize.Worldship, EComponentType.ShieldGenerator)},
        {"Worldvault", new StarshipComponent(EComponentSize.Worldship, EComponentType.CargoHold)},
        {"Stargate", new StarshipComponent(EComponentSize.Worldship, EComponentType.PortalDrive)},
        {"WorldshipKineticWeapon", new StarshipComponent(EComponentSize.Worldship, EComponentType.KineticWeapon)},
        {"WorldshipEnergyWeapon", new StarshipComponent(EComponentSize.Worldship, EComponentType.EnergyWeapon)},
        {"WorldshipEngine", new StarshipComponent(EComponentSize.Worldship, EComponentType.Engine)},
        {"WorldshipThrusters", new StarshipComponent(EComponentSize.Worldship, EComponentType.Thrusters)},

        // Excalibur components 64
        {"Quintessence Divijector", new StarshipComponent(EComponentSize.Excalibur, EComponentType.PowerSupply)},
        {"Unity Veil", new StarshipComponent(EComponentSize.Excalibur, EComponentType.ShieldGenerator)},
        {"Sheathe of Excalibur", new StarshipComponent(EComponentSize.Excalibur, EComponentType.CargoHold)},
        {"Reality Rift", new StarshipComponent(EComponentSize.Excalibur, EComponentType.PortalDrive)},
        {"Peacemaker", new StarshipComponent(EComponentSize.Excalibur, EComponentType.KineticWeapon)},
        {"Solaris", new StarshipComponent(EComponentSize.Excalibur, EComponentType.EnergyWeapon)},
        {"ExcaliburEngine", new StarshipComponent(EComponentSize.Excalibur, EComponentType.Engine)},
        {"ExcaliburThrusters", new StarshipComponent(EComponentSize.Excalibur, EComponentType.Thrusters)},
    };
        
        
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}








