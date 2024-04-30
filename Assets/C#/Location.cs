using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using ColorParameter = UnityEngine.Rendering.ColorParameter;
using Vignette = UnityEngine.Rendering.Universal.Vignette;

public class Location : MonoBehaviour
{
    public string Name;
    public string System;
    public string Habitat;
    public Vignette PP_Vignette;
    // Start is called before the first frame update
    void Start()
    {
        PP_Vignette = FindObjectOfType<Volume>().GetComponent<Vignette>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsInLocation(string reqLocation)
    {
        
        if (reqLocation == "")
        {
            return true;
        }

        bool bReqSystem = false; //represents true if the required location is a system, false if it's a habitat
        
        List<string> Systems = new List<string> { "Avalon", "Glint", "Merlin", "Nocturne", "Bane", "Macbeth IV" };

        foreach (string s in Systems)
        {
            if (Name == s)
            {
                bReqSystem = true;
            }

        }

        if(bReqSystem)
        {
            if (reqLocation == System)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Habitat == reqLocation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void SetLocation(string newLocation)
    {
        Name = newLocation;
        UpdatePlayerSystem();
        UpdatePlayerHabitat();
        
        foreach (Deck d in Board.Decks.Values)
        {
            if (d.Name == "Habitat")
            {
                d.SetCardPositions();
            }
        }
        
        foreach (Card c in Board.Decks["Habitat"])
        {
            if (c != null && c.Name == Habitat)
            {
                c.LocationGlow.fillAmount = 100;
            }
            else
            {
                c.LocationGlow.fillAmount = 0;
            }
                
        }
        foreach (Card c in Board.Decks["System"])
        {
            if (c != null && c.Name == System)
            {
                c.LocationGlow.fillAmount = 100;
            }
            else
            {
                c.LocationGlow.fillAmount = 0;
            }
        }
    }
    
    private void UpdateSkyBox()
    {
        switch (Name)
        {
            case "Deepmine":
                PP_Vignette.color = new ColorParameter(new Color(0f, 1f, .3f));
                
                break;
                
        }
    }
    
    public void UpdatePlayerHabitat()
    {
        if (IsPlayerInHabitat())
        {
            Habitat = Name;
        }
        else
        {
            Habitat = "";

        }
    }

    public bool IsPlayerInHabitat()
    {
        List<string> Systems = new List<string> { "Avalon", "Glint", "Merlin", "Nocturne", "Bane", "Macbeth IV" };

        foreach (string s in Systems)
        {
            if (Name == s)
            {
                return false;
            }
        }
        return true;
    }

    public string UpdatePlayerSystem()
    {
        string newSystem = "";
        
        switch (Name)
        {
            case "Avalon":
            case "Roundtable Nexus":
            case "Terminus Est":
            case "The Citadel":
            case "The White Lodge":
            case "Dragonroost":
            case "Yggdrasil":
            case "Leviathan Belt":
            case "Earth":
                newSystem = "Avalon";
                break;
            case "Glint":
            case "Boulderhearth":
            case "Forge":
            case "Behemoth":
            case "The Sledge":
            case "Brewhalla":
            case "Deepmine":
            case "Jormungandr":
            case "Ben Nevis":
                newSystem = "Glint";
                break;
            case "Merlin":
            case "Jenasysz":
            case "Nostalgia V":
            case "Undine":
            case "Reverie":
            case "Lore":
            case "Longbow":
            case "Myst":
            case "Veil Lookouts":
                newSystem = "Merlin";
                break;
            case "Nocturne":
            case "Dracule":
            case "Crimsontide":
            case "One-eyed Jacks":
            case "Wreckage Bay":
            case "Smuggler's Nook":
            case "Rakshasa":
            case "Black Sun Campaign":
            case "The Feast":
                newSystem = "Nocturne";
                break;
            case "Bane":
            case "Bameth":
            case "Obsidian Conclave":
            case "Voltage":
            case "Darkwash":
            case "Golem":
            case "Blitzkrieg":
            case "Zweijager":
                newSystem = "Bane";
                break;
            case "Macbeth IV":
            case "Mausoleum":
            case "Umbressa":
            case "Sakura":
            case "Exvulsa":
            case "Triangula":
            case "Blame":
            case "Abyss Abbey":
            case "The Black Mass":
                newSystem = "Macbeth IV";
                break;

        }

        System = newSystem;

        return newSystem;
    }
    
    public List<string> GetSystemHabitats(string System)
    {
        List<string> Habitats = new List<string>();
        
        switch (System)
        {
            case "Avalon":
                Habitats.Add("Roundtable Nexus");
                Habitats.Add("Terminus Est");
                Habitats.Add("The Citadel");
                Habitats.Add("The White Lodge");
                Habitats.Add("Dragonroost");
                Habitats.Add("Yggdrasil");
                Habitats.Add("Leviathan Belt");
                Habitats.Add("Earth");
                break;
            case "Glint":
                Habitats.Add("Boulderhearth");
                Habitats.Add("Forge");
                Habitats.Add("Behemoth");
                Habitats.Add("The Sledge");
                Habitats.Add("Brewhalla");
                Habitats.Add("Deepmine");
                Habitats.Add("Jormungandr");
                Habitats.Add("Ben Nevis");
                break;
            case "Merlin":
                Habitats.Add("Jenasysz");
                Habitats.Add("Nostalgia V");
                Habitats.Add("Undine");
                Habitats.Add("Reverie");
                Habitats.Add("Lore");
                Habitats.Add("Longbow");
                Habitats.Add("Myst");
                Habitats.Add("Veil Lookouts");
                break;
            case "Nocturne":
                Habitats.Add("Dracule");
                Habitats.Add("Crimsontide");
                Habitats.Add("One-eyed Jacks");
                Habitats.Add("Wreckage Bay");
                Habitats.Add("Smuggler's Nook");
                Habitats.Add("Rakshasa");
                Habitats.Add("Black Sun Campaign");
                Habitats.Add("The Feast");
                break;
            case "Bane":
                Habitats.Add("Bameth");
                Habitats.Add("Obsidian Conclave");
                Habitats.Add("Voltage");
                Habitats.Add("Darkwash");
                Habitats.Add("Golem");
                Habitats.Add("Blitzkrieg");
                Habitats.Add("Zweijager");
                break;
            case "Macbeth IV":
                Habitats.Add("Mausoleum");
                Habitats.Add("Umbressa");
                Habitats.Add("Sakura");
                Habitats.Add("Exvulsa");
                Habitats.Add("Triangula");
                Habitats.Add("Blame");
                Habitats.Add("Abyss Abbey");
                Habitats.Add("The Black Mass");
                break;
        }

        return Habitats;
    }
}
