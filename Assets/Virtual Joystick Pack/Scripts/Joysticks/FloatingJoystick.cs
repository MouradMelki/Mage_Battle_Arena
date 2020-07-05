using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

public class FloatingJoystick : Joystick
{
    private int m_noAimZoneCount;

    public PlayerController PlayerController { get; set; }
    public float NoAimZoneRadius { get; set; }
    public float FireRate { get; set; }
    private float nextFire;

    Vector2 joystickCenter = Vector2.zero;
    RectTransform rectTransform;

    private void Awake()
    {
        background.gameObject.SetActive(false);
        rectTransform = gameObject.GetComponent<RectTransform>();
        Controlls.SetSize(rectTransform, new Vector2(Screen.width / 2, Screen.height));

        if (gameObject.CompareTag("Left_Joystick"))
            Controlls.SetPositionOfPivot(rectTransform, new Vector2(- Screen.width / 2, - Screen.height / 2));
        else if(gameObject.CompareTag("Right_Joystick"))
            Controlls.SetPositionOfPivot(rectTransform, new Vector2(Screen.width / 2, - Screen.height / 2));
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (PlayerController != null)
        {
            // in the future change PlayerController component into main component of player
            if (gameObject.CompareTag("Right_Joystick") && !PlayerController.Player.IsDead)
            {
                Vector3 movement = new Vector3(Horizontal, 0f, Vertical);
                if (movement.magnitude > NoAimZoneRadius)
                {
                    PlayerController.Player.RJoyStickAct = true;
                }
                else if (movement.magnitude <= NoAimZoneRadius)
                {
                    if (m_noAimZoneCount == 0)
                        m_noAimZoneCount++;

                    if (PlayerController.Player.RJoyStickAct && m_noAimZoneCount != 0)
                    {
                        Vibration.Vibrate(50);
                    }

                    PlayerController.Player.RJoyStickAct = false;
                }
            }

            if (gameObject.CompareTag("Left_Joystick") && !PlayerController.Player.IsDead)
            {
                PlayerController.Player.LJoyStickAct = Math.Abs(Horizontal) > 0f || Math.Abs(Vertical) > 0f;
            }
        }

        Vector2 direction = eventData.position - joystickCenter;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        joystickCenter = eventData.position;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerController != null)
        {
            if (gameObject.CompareTag("Right_Joystick") && !PlayerController.Player.IsDead)
            {
                Vector3 movement = new Vector3(Horizontal, 0f, Vertical);
                if (m_noAimZoneCount == 0 || movement.magnitude > NoAimZoneRadius)
                {
                    if (Time.time > nextFire && PlayerController.shoot == false)
                    {
                        nextFire = Time.time + FireRate;
                        PlayerController.Player.NormalAttack.Direction = movement;
                        PlayerController.shoot = true;
                    }
                    m_noAimZoneCount = 0;
                }
                else
                {
                    m_noAimZoneCount = 0;
                }
                PlayerController.Player.RJoyStickAct = false;
            }

            PlayerController.Player.LJoyStickAct &= !gameObject.CompareTag("Left_Joystick");
        }

        background.gameObject.SetActive(false);
        inputVector = Vector2.zero;
    }
}