using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SkyboxMaterial
{
    public string Location;
    public Material M_Skybox;
}
public class Location : MonoBehaviour
{
    public string Name;
    public string System;
    public string Habitat;

    public List<SkyboxMaterial> SkyboxMap;
    
    void Start()
    {
        
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
        

        //update system and habitat based deck card positions
        foreach (Deck d in Board.Decks.Values)
        {
            if (d.Name == "Habitat" || d.Name == "Character" || d.Name == "Ambition" || d.Name == "Enemy" || d.Name == "Quest")
            {
                d.SetCardPositions();
            }
        }
        UpdateSkyBox();
    }
    
    private void UpdateSkyBox()
    {
        //update post process volumes and scene
        Transform Skyboxes = GameObject.Find("Skybox").transform;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // Disable the child GameObject
        }
        
        switch (Name)
        {
            case "Deepmine":
                Skyboxes.Find("Deepmine").gameObject.SetActive(true);
                
                break;
            case "Wreckage Bay":
                Skyboxes.Find("WreckageBay").gameObject.SetActive(true);
                
                break;
        }
    
        //update skybox tint
        foreach (SkyboxMaterial map in SkyboxMap)
        {
            if (map.Location == Name)
            {
                RenderSettings.skybox = map.M_Skybox;
                DynamicGI.UpdateEnvironment(); // Update global illumination to reflect changes
                return;
            }
        }
        
        //search again for a more generic case if none found
        foreach (SkyboxMaterial map in SkyboxMap)
        {
            if (map.Location == System)
            {
                RenderSettings.skybox = map.M_Skybox;
                DynamicGI.UpdateEnvironment(); // Update global illumination to reflect changes
                return;
            }
        }
    }

    public void UpdateCardGlow()
    {
        
        
        foreach (Card c in Board.Decks["Habitat"].Cards)
        {
            if (c != null && c.Name == Habitat)
            {
                c.LocationGlow.fillAmount = 1;
                break;
            }
            else
            {
                c.LocationGlow.fillAmount = 0;
            }
            
        }
        
        foreach (Card c in Board.Decks["System"].Cards)
        {
            if (c != null && c.Name == System)
            {
                c.LocationGlow.fillAmount = 1;
                break;

            }
            else
            {
                c.LocationGlow.fillAmount = 0;

            }
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

    public void UpdatePlayerSystem()
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
    }
}
