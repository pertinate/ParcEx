using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Pertinate.Player
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public static bool FlipControls = false;
        public static float deadZone = 0.08f;

        public Image backgroundImage;
        public Image right, left, up, down;

        public Sprite activated, normal;

        private Vector3 moveVector;

        private int queues = 0;

        public VectorEvent onValueChange = new VectorEvent();
        public ClickEvent onClick = new ClickEvent();
        private void Start()
        {
            moveVector = Vector3.zero;
        }

        //interface implementation
        public void OnDrag(PointerEventData eventData)
        {
            queues++;
            if (!Drone.instance.stopMotors)
            {
                Vector2 pos = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    backgroundImage.rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out pos))
                {
                    pos.x = (pos.x / backgroundImage.rectTransform.sizeDelta.x);
                    pos.y = (pos.y / backgroundImage.rectTransform.sizeDelta.y);
                    moveVector = new Vector3(pos.x * 2, 0, pos.y * 2);

                    moveVector = (moveVector.magnitude > 1) ? moveVector.normalized : moveVector;

                    /*joystickImage.rectTransform.anchoredPosition =
                        new Vector3(moveVector.x * (backgroundImage.rectTransform.sizeDelta.x / 4f),
                        moveVector.z * (backgroundImage.rectTransform.sizeDelta.y / 4f));*/
                }


                up.sprite = normal;
                down.sprite = normal;
                right.sprite = normal;
                left.sprite = normal;

                if (moveVector.x < -deadZone)
                {
                    left.sprite = activated;
                }
                else if(moveVector.x > deadZone)
                {
                    right.sprite = activated;
                }

                if(moveVector.z < -deadZone)
                {
                    down.sprite = activated;
                }
                else if(moveVector.z > deadZone)
                {
                    up.sprite = activated;
                }

                if (moveVector != Vector3.zero)
                    onValueChange.Invoke(new Vector2(moveVector.x, moveVector.z));
            }
        }
        //interface implementation
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }
        //interface implementation
        public void OnPointerUp(PointerEventData eventData)
        {
            if(queues == 1)
            {
                onClick.Invoke(true);
            }
            queues = 0;
            moveVector = Vector3.zero;
            up.sprite = normal;
            down.sprite = normal;
            right.sprite = normal;
            left.sprite = normal;
            onValueChange.Invoke(moveVector);
            //joystickImage.rectTransform.anchoredPosition = Vector3.zero;
        }
    }

    public class VectorEvent : UnityEvent<Vector3> { }
    public class ClickEvent : UnityEvent<bool> { }
}
