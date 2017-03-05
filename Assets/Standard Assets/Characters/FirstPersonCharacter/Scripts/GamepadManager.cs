using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XInputDotNetPure;
using System;

[ExecuteInEditMode]

public class GamepadManager : MonoBehaviour
{

    bool playerIndexSet = false;

    static public PlayerIndex playerIndex;

    static public GamePadState prevState;

    static public GamePadState state;



    //  static public booliables for

    static public float h1 = 0.0f;

    static public float v1 = 0.0f;



    static public float h2 = 0.0f;

    static public float v2 = 0.0f;




    static public bool buttonA = false;

    static public bool buttonB = false;

    static public bool buttonX = false;

    static public bool buttonY = false;



    static public bool buttonADown = false;

    static public bool buttonBDown = false;

    static public bool buttonXDown = false;

    static public bool buttonYDown = false;



    static public bool buttonAUp = false;

    static public bool buttonBUp = false;

    static public bool buttonXUp = false;

    static public bool buttonYUp = false;



    static public bool dpadUp = false;

    static public bool dpadDown = false;

    static public bool dpadLeft = false;

    static public bool dpadRight = false;



    static public bool dpadUpDown = false;

    static public bool dpadDownDown = false;

    static public bool dpadLeftDown = false;

    static public bool dpadRightDown = false;



    static public bool dpadUpUp = false;

    static public bool dpadDownUp = false;

    static public bool dpadLeftUp = false;

    static public bool dpadRightUp = false;



    static public bool buttonStart = false;

    static public bool buttonBack = false;



    static public bool buttonStartDown = false;

    static public bool buttonBackDown = false;



    static public bool buttonStartUp = false;

    static public bool buttonBackUp = false;



    static public bool shoulderL = false;

    static public bool shoulderR = false;



    static public bool shoulderLDown = false;

    static public bool shoulderRDown = false;



    static public bool shoulderLUp = false;

    static public bool shoulderRUp = false;



    static public bool stickL = false;

    static public bool stickR = false;



    static public bool stickLDown = false;

    static public bool stickRDown = false;



    static public bool stickLUp = false;

    static public bool stickRUp = false;



    static public float triggerL = 0.0f;

    static public float triggerR = 0.0f;


    ////Display
    //public bool Display = false;
    //public bool displayed = false;
    //public Image controllerDisplay;

    //public SpriteRenderer X;
    //public SpriteRenderer Y;
    //public SpriteRenderer A;
    //public SpriteRenderer B;

    //public SpriteRenderer LB;
    //public SpriteRenderer RB;
    //public SpriteRenderer LT;
    //public SpriteRenderer RT;

    //public SpriteRenderer LeftStick;
    //public SpriteRenderer RightStick;
    //public SpriteRenderer Dpad;

    //public Sprite[] buttons;
    //public Sprite[] sticks;
    //public Sprite[] pad;



    void Update()
    {
        // Find a PlayerIndex, for a single player game

        if (!playerIndexSet)
        {

            for (int i = 0; i < 4; ++i)
            {

                PlayerIndex testPlayerIndex = new PlayerIndex();

                switch (i)
                {
                    case 0:

                        testPlayerIndex = PlayerIndex.One;

                        break;

                    case 1:

                        testPlayerIndex = PlayerIndex.Two;

                        break;

                    case 2:

                        testPlayerIndex = PlayerIndex.Three;

                        break;

                    case 3:

                        testPlayerIndex = PlayerIndex.Four;

                        break;

                }

                GamePadState testState = GamePad.GetState(testPlayerIndex);

                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;

                }

            }

        }



        prevState = state;

        state = GamePad.GetState(playerIndex);



        h1 = state.ThumbSticks.Left.X;

        v1 = state.ThumbSticks.Left.Y;

        h2 = state.ThumbSticks.Right.X;

        v2 = state.ThumbSticks.Right.Y;



        buttonA = (state.Buttons.A == ButtonState.Pressed);

        buttonB = (state.Buttons.B == ButtonState.Pressed);

        buttonX = (state.Buttons.X == ButtonState.Pressed);

        buttonY = (state.Buttons.Y == ButtonState.Pressed);



        buttonADown = (buttonA && prevState.Buttons.A != ButtonState.Pressed);

        buttonBDown = (buttonB && prevState.Buttons.B != ButtonState.Pressed);

        buttonXDown = (buttonX && prevState.Buttons.X != ButtonState.Pressed);

        buttonYDown = (buttonY && prevState.Buttons.Y != ButtonState.Pressed);



        buttonAUp = (!buttonA && prevState.Buttons.A == ButtonState.Pressed);

        buttonBUp = (!buttonB && prevState.Buttons.B == ButtonState.Pressed);

        buttonXUp = (!buttonX && prevState.Buttons.X == ButtonState.Pressed);

        buttonYUp = (!buttonY && prevState.Buttons.Y == ButtonState.Pressed);



        dpadUp = (state.DPad.Up == ButtonState.Pressed);

        dpadDown = (state.DPad.Down == ButtonState.Pressed);

        dpadLeft = (state.DPad.Left == ButtonState.Pressed);

        dpadRight = (state.DPad.Right == ButtonState.Pressed);

        dpadUpDown = (dpadUp && prevState.DPad.Up != ButtonState.Pressed);

        dpadDownDown = (dpadDown && prevState.DPad.Down != ButtonState.Pressed);

        dpadLeftDown = (dpadLeft && prevState.DPad.Left != ButtonState.Pressed);

        dpadRightDown = (dpadRight && prevState.DPad.Right != ButtonState.Pressed);



        dpadUpUp = (!dpadUp && prevState.DPad.Up == ButtonState.Pressed);

        dpadDownUp = (!dpadDown && prevState.DPad.Down == ButtonState.Pressed);

        dpadLeftUp = (!dpadLeft && prevState.DPad.Left == ButtonState.Pressed);

        dpadRightUp = (!dpadRight && prevState.DPad.Right == ButtonState.Pressed);



        buttonStart = (state.Buttons.Start == ButtonState.Pressed);

        buttonBack = (state.Buttons.Back == ButtonState.Pressed);



        buttonStartDown = (buttonStart && prevState.Buttons.Start != ButtonState.Pressed);

        buttonBackDown = (buttonBack && prevState.Buttons.Back != ButtonState.Pressed);



        buttonStartUp = (!buttonStart && prevState.Buttons.Start == ButtonState.Pressed);

        buttonBackUp = (!buttonBack && prevState.Buttons.Back == ButtonState.Pressed);



        shoulderL = (state.Buttons.LeftShoulder == ButtonState.Pressed);

        shoulderR = (state.Buttons.RightShoulder == ButtonState.Pressed);


        shoulderLDown = (shoulderL && prevState.Buttons.LeftShoulder != ButtonState.Pressed);

        shoulderRDown = (shoulderR && prevState.Buttons.RightShoulder != ButtonState.Pressed);



        shoulderLUp = (!shoulderL && prevState.Buttons.LeftShoulder == ButtonState.Pressed);

        shoulderRUp = (!shoulderR && prevState.Buttons.RightShoulder == ButtonState.Pressed);



        stickL = (state.Buttons.LeftStick == ButtonState.Pressed);

        stickR = (state.Buttons.RightStick == ButtonState.Pressed);



        stickLDown = (stickL && prevState.Buttons.LeftStick != ButtonState.Pressed);

        stickRDown = (stickR && prevState.Buttons.RightStick != ButtonState.Pressed);



        stickLUp = (!stickL && prevState.Buttons.LeftStick == ButtonState.Pressed);

        stickRUp = (!stickR && prevState.Buttons.RightStick == ButtonState.Pressed);



        triggerL = state.Triggers.Left;

        triggerR = state.Triggers.Right;



        //if(Display)
        //{
        //        if(stickRDown)
        //        {
        //            controllerDisplay.gameObject.SetActive(true);
        //            if(displayed)
        //            {
        //                displayed = false;
        //            }
        //            else
        //            {
        //                displayed = true;
        //            }
        //        }
        //        if(displayed)
        //        {

        //            controllerDisplay.rectTransform.anchoredPosition = Vector2.Lerp (controllerDisplay.rectTransform.anchoredPosition,new Vector2(0,100),Time.deltaTime * 3);
        //            //controllerDisplay.rectTransform.localPosition = Vector3.Lerp(controllerDisplay.transform.localPosition,new Vector3(0,60,0),Time.deltaTime * 3);
        //        }
        //        else
        //        {
        //            controllerDisplay.rectTransform.anchoredPosition = Vector2.Lerp (controllerDisplay.rectTransform.anchoredPosition,new Vector2(0,-100),Time.deltaTime * 3);

        //            //controllerDisplay.rectTransform.localPosition = Vector3.Lerp(controllerDisplay.transform.localPosition,new Vector3(0,-50,0),Time.deltaTime * 3);
        //        }
        //        if(v1 >= 0.5)
        //        {
        //            LeftStick.sprite = sticks[1];
        //        }
        //        else if(v1 <= -0.5)
        //        {
        //            LeftStick.sprite = sticks[2];
        //        }
        //        else if(h1 <= -0.5)
        //        {
        //            LeftStick.sprite = sticks[3];
        //        }
        //        else if(h1 >= 0.5)
        //        {
        //            LeftStick.sprite = sticks[4];
        //        }
        //        else
        //        {
        //            LeftStick.sprite = sticks[0];
        //        }
        //        if(v2 >= 0.5)
        //        {
        //            RightStick.sprite = sticks[1];
        //        }
        //        else if(v2 <= -0.5)
        //        {
        //            RightStick.sprite = sticks[2];
        //        }
        //        else if(h2 <= -0.5)
        //        {
        //            RightStick.sprite = sticks[3];
        //        }
        //        else if(h2 >= 0.5)
        //        {
        //            RightStick.sprite = sticks[4];
        //        }
        //        else
        //        {
        //            RightStick.sprite = sticks[0];
        //        }
        //        if(buttonX)
        //        {
        //            X.sprite = buttons[1];
        //        }
        //        else
        //        {
        //            X.sprite = buttons[0];
        //        }
        //        if(buttonY)
        //        {
        //            Y.sprite = buttons[3];
        //        }
        //        else
        //        {
        //            Y.sprite = buttons[2];
        //        }
        //        if(buttonA)
        //        {
        //            A.sprite = buttons[5];       
        //        }
        //        else
        //        {
        //            A.sprite = buttons[4];
        //        }
        //        if(buttonB)
        //        {
        //            B.sprite = buttons[7];       
        //        }
        //        else
        //        {
        //            B.sprite = buttons[6];
        //        }
        //        if(dpadUp)
        //        {
        //            Dpad.sprite = pad[1];
        //        }
        //        else if(dpadDown)
        //        {
        //            Dpad.sprite = pad[2];
        //        }
        //        else if(dpadLeft)
        //        {
        //            Dpad.sprite = pad[3];
        //        }
        //        else if(dpadRight)
        //        {
        //            Dpad.sprite = pad[4];
        //        }
        //        else
        //        {
        //            Dpad.sprite = pad[0];
        //        }
        //        if(shoulderL)
        //        {
        //            LB.sprite = buttons[9];
        //        }
        //        else
        //        {
        //            LB.sprite = buttons[8];
        //        }
        //        if(shoulderR)
        //        {
        //            RB.sprite = buttons[9];
        //        }
        //        else
        //        {
        //            RB.sprite = buttons[8];
        //        }

        //        if(triggerL >= 0.5)
        //        {
        //            LT.sprite = buttons[11];
        //        }
        //        else
        //        {
        //            LT.sprite = buttons[10];
        //        }
        //        if(triggerR >= 0.5)
        //        {
        //            RT.sprite = buttons[11];
        //        }
        //        else
        //        {
        //            RT.sprite = buttons[10];
        //        }
        //}
    }

    public static bool AnyButtonPressed()
    {
        if (buttonXDown || buttonADown || buttonBDown || buttonYDown || dpadUpDown || dpadLeftDown || dpadDownDown || dpadRightDown || shoulderLDown || shoulderRDown || buttonStartDown || buttonBackDown)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static public void padVibration(float big, float small)
    {
        GamePad.SetVibration(GamepadManager.playerIndex, big, small);
    }



    static public void stopPadVibration()
    {
        GamePad.SetVibration(GamepadManager.playerIndex, 0, 0);
    }
}