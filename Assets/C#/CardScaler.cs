using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScaler : MonoBehaviour
{
    public Camera MainCamera;
    public Card CardBeingScaled;
    public ActionGUI ActionPanel;
    public Coroutine ScaleCoroutine;

    public Coroutine PositionCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GetComponent<Camera>();

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
         
        //find a normalized position of the mouse in regards to the screen and prevent the below code from running if it's within 15%

        Vector3 mousePosition = MainCamera.ScreenToViewportPoint(Input.mousePosition);
        if ((mousePosition.x < .05f && mousePosition.x > 95f) || (mousePosition.y > .05f && mousePosition.y > .95f))
        {
            return;
        }

        if (ScaleCoroutine != null)
        {
            StopCoroutine(ScaleCoroutine);
        }

        Vector3 targetScale = new Vector3(2.0f, 2.0f, 1f);
        CardBeingScaled.transform.localScale = Vector3.Lerp(CardBeingScaled.transform.localScale, targetScale * (MainCamera.orthographicSize / 240), Time.deltaTime * 10);

        
        // Clamp to screen edges
        Vector3 viewportPosition = MainCamera.WorldToViewportPoint(CardBeingScaled.transform.position);
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f); // Adjust these values based on your needs
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.15f, 0.85f);
        viewportPosition = MainCamera.ViewportToWorldPoint(viewportPosition);
        viewportPosition.z = CardBeingScaled.transform.position.z;
        CardBeingScaled.transform.position = viewportPosition;
        
        if (CardBeingScaled.GetDeckType() != "System" 
            && CardBeingScaled.GetDeckType() != "Cargo" 
            && CardBeingScaled.GetDeckType() != "Crafting" 
            && CardBeingScaled.GetDeckType() != "Action" 
            && CardBeingScaled.GetDeckType() != "Enemy")
        {
            CardBeingScaled.TMP_MouseOverName.text = CardBeingScaled.Name;
            CardBeingScaled.TMP_MouseOverFlavor.text = CardDB.CardDataLookup[CardBeingScaled.ID].FlavorText;
        }
        else
        {
            CardBeingScaled.TMP_MouseOverNameLeft.text = CardBeingScaled.Name;
            CardBeingScaled.TMP_MouseOverFlavorLeft.text = CardDB.CardDataLookup[CardBeingScaled.ID].FlavorText;
        }
    }
    
    void MouseExit()
    {
        
        CardBeingScaled.TMP_MouseOverName.text = "";
        CardBeingScaled.TMP_MouseOverFlavor.text = "";
        CardBeingScaled.TMP_MouseOverNameLeft.text = "";
        CardBeingScaled.TMP_MouseOverFlavorLeft.text = "";

        if (ScaleCoroutine != null)
        {
            StopCoroutine(ScaleCoroutine);
        }

        if (PositionCoroutine != null)
        {
            StopCoroutine(PositionCoroutine);
        }

        ScaleCoroutine = StartCoroutine(CardBeingScaled.ScaleToSize(Vector3.one, .15f));
        PositionCoroutine = StartCoroutine(CardBeingScaled.MoveToPosition(CardBeingScaled.OriginalPosition, .15f));  // Lerp position back to original
        CardBeingScaled = null;
    }
    

}
