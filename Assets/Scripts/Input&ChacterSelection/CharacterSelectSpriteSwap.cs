using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSpriteSwap : MonoBehaviour {

    //Images and sprites to swap
    [Header("Image that indicates how to go back to previous screen.")]
    [SerializeField] private Image backImage;
    [SerializeField] private Image backImageGhost;
    [SerializeField] private Sprite backSpriteKeyboard;
    [SerializeField] private Sprite backSpriteController;

    [Header("Images that indicate how players should select their character.")]
    [SerializeField] private Image selectImageP1;
    [SerializeField] private Image cancelImageP1;
    
    [SerializeField] private Sprite selectSpriteKeyboard;
    [SerializeField] private Sprite cancelSpriteKeyboard;

    //controller checking stuff
    private CheckControllers cc;
    private bool backImageSwapping = false;
    private float swapCounter = 0;
    private bool swapped = false;

    private void Start()
    {
        cc = GameObject.Find("InputManager").GetComponent<CheckControllers>();

        //Note - p2 images will always be controllers.

        //Set player 1 images - they are controller by default, only need to swap if both controllers aren't being used
        if (!(cc.GetControllerOneState() && cc.GetControllerTwoState()))
        {
            backImageSwapping = true;

            selectImageP1.sprite = selectSpriteKeyboard;
            cancelImageP1.sprite = cancelSpriteKeyboard;
        }
        
    }

    private void Update()
    {
        if(backImageSwapping)
        {
            FadeBackImage();
        }
    }

    private void FadeBackImage()
    {
        swapCounter += 0.007f;
        backImage.color = new Color(backImage.color.r, backImage.color.g, backImage.color.b, Mathf.PingPong(swapCounter, 1));

        backImageGhost.color = new Color(backImageGhost.color.r, backImageGhost.color.g, backImageGhost.color.b, Mathf.PingPong(swapCounter, 1));

        if (backImage.color.a <= 0.025f && !swapped)
        {
            SwapSprite();
            swapped = true;
        }
        if(backImage.color.a >= 0.9)
        {
            swapped = false;
        }
    }

    private void SwapSprite()
    {
        if(backImage.sprite == backSpriteKeyboard)
        {
            backImage.sprite = backSpriteController;
            backImageGhost.sprite = backSpriteController;
        }
        else
        {
            backImage.sprite = backSpriteKeyboard;
            backImageGhost.sprite = backSpriteKeyboard;
        }
    }
    
}
