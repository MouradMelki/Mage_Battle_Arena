using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

public class FloatingJoystick : Joystick
{
    private const string LeftJoystickTag = "Left_Joystick";
    private const string RightJoystickTag = "Right_Joystick";

    private int noAimZoneCount;
    private Vector2 joystickCenter = Vector2.zero;
    private RectTransform rectTransform;
    private bool isLeftJoystick;
    private bool isRightJoystick;

    public PlayerController PlayerController { get; set; }

    private void Awake()
    {
        background.gameObject.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        Controlls.SetSize(rectTransform, new Vector2(Screen.width / 2, Screen.height));

        isLeftJoystick = CompareTag(LeftJoystickTag);
        isRightJoystick = CompareTag(RightJoystickTag);

        if (isLeftJoystick)
        {
            Controlls.SetPositionOfPivot(rectTransform, new Vector2(-Screen.width / 2, -Screen.height / 2));
        }
        else if (isRightJoystick)
        {
            Controlls.SetPositionOfPivot(rectTransform, new Vector2(Screen.width / 2, -Screen.height / 2));
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickCenter;
        inputVector = direction.magnitude > background.sizeDelta.x / 2f
            ? direction.normalized
            : direction / (background.sizeDelta.x / 2f);

        ClampJoystick();
        handle.anchoredPosition = inputVector * background.sizeDelta.x / 2f * handleLimit;

        UpdatePlayerInputState();
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
            if (isRightJoystick && !PlayerController.IsDead)
            {
                Vector3 movement = new Vector3(Horizontal, 0f, Vertical);
                float noAimZoneSqr = PlayerController.NoAimZoneRadius * PlayerController.NoAimZoneRadius;
                if (noAimZoneCount == 0 || movement.sqrMagnitude > noAimZoneSqr)
                {
                    PlayerController.QueueShot(movement);
                }

                noAimZoneCount = 0;
                PlayerController.SetRightJoystickActive(false);
            }

            if (isLeftJoystick)
            {
                PlayerController.SetLeftJoystickActive(false);
            }
        }

        background.gameObject.SetActive(false);
        inputVector = Vector2.zero;
    }

    private void UpdatePlayerInputState()
    {
        if (PlayerController == null || PlayerController.IsDead)
        {
            return;
        }

        if (isRightJoystick)
        {
            Vector3 movement = new Vector3(Horizontal, 0f, Vertical);
            float noAimZoneSqr = PlayerController.NoAimZoneRadius * PlayerController.NoAimZoneRadius;
            bool aiming = movement.sqrMagnitude > noAimZoneSqr;
            if (!aiming && noAimZoneCount == 0)
            {
                noAimZoneCount++;
            }

            if (!aiming && noAimZoneCount != 0)
            {
                Vibration.Vibrate(50);
            }

            PlayerController.SetRightJoystickActive(aiming);
        }

        if (isLeftJoystick)
        {
            PlayerController.SetLeftJoystickActive(Horizontal != 0f || Vertical != 0f);
        }
    }
}
