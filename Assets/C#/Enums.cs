using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVerb
{
    Work,
    Dream,
    Explore,
    Research,
    Talk,
    Travel,
    Craft,
    Trade,
    Battle
}


public enum EProperty
{
    Magical,
    Cursed,
    Sanctioned,
    Ally,
    Enemy,
    Neutral,
    Illegal,
    Illicit,
    All
}

//what if i end up wanting to make these more meaningful i.e. have their own class?
public enum EType
{
    Action, //verbs
    Object, //most nouns
    Currency,
    Crafting,
    ShipComponent,
    Character, //allies and prisoners
    Ambition,
    Cargo, //licit and illicit
    Fleet, //friendly and enemy
    Habitat,
    System
}

public enum ESystem
{
    Castle, //human system
    Glint, //dwarve system
    Merlin,  //elven system
    Nocturne, //vampire system
    Bane, //dark dwarf system
    MacbethIV //dark elf system
    
}

public enum EHabitat //system in comments
{
    Space,
    Earth, //castle capital
    Avalon, //castle planet
    Yggdrasil, //castle planet
    Boulderhearth, //glint capital
    Forge, //glint planet
    Jenasysz, //merlin capital
    Myst,  //merlin planet
    Dracula, //nocturne capital
    Sakura, //nocturne planet
    Bameth, //dark dwarves capital
    Darkwash, //dark dwarves capital
    TheStorm, //dark dwarves minefield
    Triangula, //dark elves capital
    Umbressa // dark elves planet
}

public enum EAttribute
{
    Health
    
}