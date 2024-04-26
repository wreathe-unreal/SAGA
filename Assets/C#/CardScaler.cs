using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CardScaler : MonoBehaviour
{
    public Camera MainCamera;
    public Card CardBeingScaled;
    private List<string> LeftMouseoverDeckTypes = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GetComponent<Camera>();
        LeftMouseoverDeckTypes = new List<string> { "System", "Cargo", "Crafting", "Action", "Enemy", "Habitat" };
    }

    // Update is called once per frame
    void Update()
    {
        //use raycast since raycasts work regardless of timescale 0
        RaycastHit hit;
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out hit, float.MaxValue, 1<<3))
        {
            if (CardBeingScaled != null)
            {
                MouseExit();
            }
            return;
        }
        
        Card hitCard = hit.transform.gameObject.GetComponent<Card>();
        if (hitCard != null) //it hit a card
        {
            if (CardBeingScaled != null && hitCard != CardBeingScaled)
            {
                MouseExit();
            }
                
            CardBeingScaled = hitCard;
            MouseOver();
        }
    }

    void MouseOver()
    {
         
        //find a normalized position of the mouse in regards to the screen and exit early if it's near the very edges
        Vector3 mousePosition = MainCamera.ScreenToViewportPoint(Input.mousePosition);
        if ((mousePosition.x < .05f && mousePosition.x > 95f) || (mousePosition.y > .05f && mousePosition.y > .95f))
        {
            return;
        }

        Vector3 targetScale = new Vector3(2.5f, 2.5f, 1f);
        //start scaling coroutine
        CardBeingScaled.StartScaling(targetScale, .15f);

        //adjust scaling based on camera zoom level
        CardBeingScaled.transform.localScale = Vector3.Lerp(CardBeingScaled.transform.localScale, targetScale * (MainCamera.orthographicSize / 157), Time.deltaTime * 10);

        
        // Clamp to screen edges
        Vector3 viewportPosition = MainCamera.WorldToViewportPoint(CardBeingScaled.transform.position);
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f); // Adjust these values based on your needs
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.15f, 0.85f);
        viewportPosition = MainCamera.ViewportToWorldPoint(viewportPosition);
        viewportPosition.z = CardBeingScaled.transform.position.z;
        CardBeingScaled.transform.position = viewportPosition;

        
        //display left or right depending on deck type
        if (LeftMouseoverDeckTypes.Contains(CardBeingScaled.GetDeckType()))
        {
            CardBeingScaled.LeftSmoke.gameObject.SetActive(true);
            CardBeingScaled.TMP_MouseOverNameLeft.text = CardBeingScaled.Name;
            CardBeingScaled.TMP_MouseOverFlavorLeft.text = CardBeingScaled.Data.FlavorText;
        }
        else
        {
            CardBeingScaled.RightSmoke.gameObject.SetActive(true);
            CardBeingScaled.TMP_MouseOverName.text = CardBeingScaled.Name;
            CardBeingScaled.TMP_MouseOverFlavor.text = CardBeingScaled.Data.FlavorText;
        }

        string cardPropertyTypeText;
        if (CardBeingScaled.Data.Property == "")
        {
            cardPropertyTypeText = CardBeingScaled.Data.Type;

        }
        else
        {
            cardPropertyTypeText = CardBeingScaled.Data.Property + "\n" + CardBeingScaled.Data.Type;

        }

        CardBeingScaled.BottomSmoke.gameObject.SetActive(true);
        
        cardPropertyTypeText = cardPropertyTypeText.ToLower();
        CardBeingScaled.TMP_MouseOverProperty.text = cardPropertyTypeText;
        CardBeingScaled.TMP_MouseOverProperty.color = CardBeingScaled.TypeColor;
    }
    
    void MouseExit()
    {
        CardBeingScaled.LeftSmoke.gameObject.SetActive(false);
        CardBeingScaled.RightSmoke.gameObject.SetActive(false);
        CardBeingScaled.BottomSmoke.gameObject.SetActive(false);
        CardBeingScaled.TMP_MouseOverName.text = "";
        CardBeingScaled.TMP_MouseOverFlavor.text = "";
        CardBeingScaled.TMP_MouseOverNameLeft.text = "";
        CardBeingScaled.TMP_MouseOverFlavorLeft.text = "";
        CardBeingScaled.TMP_MouseOverProperty.text = "";
        

        CardBeingScaled.RevertScaling();
        CardBeingScaled.RevertPosition();

        CardBeingScaled = null;
    }
    

}
