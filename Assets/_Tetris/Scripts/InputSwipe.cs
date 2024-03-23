using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Raises an event when a swipe gesture is detected
/// 
/// This should be a custom InputBinding for swipe input so that it can be used in an InputAsset. However, it doesn't seem possible to have a custom InputBinding.
/// TODO: automatically actuate other InputBindings in the InputAsset with https://forum.unity.com/threads/trigger-input-action-by-code.1145546/
/// 
/// This functions simliar to samyam's but doesn't use InputAsset https://youtu.be/XUx_QlJpd0M https://www.bing.com/videos/riverview/relatedvideo?q=unity+new+input+system+touch+screen+swipe&mid=7E89CC1482C1B30FB2247E89CC1482C1B30FB224&FORM=VIRE
/// </summary>
public class InputSwipe : MonoBehaviour
{
    const float directionThreshold = .9f;

    [SerializeField] private InputAction TouchPressed;
    [SerializeField] private InputAction TouchPosition;

    [Tooltip("The Swipe gesture must be completed in under this many seconds")]
    [SerializeField] private float maxSeconds = 1;

    [Tooltip("The swipe gesture must go over at least this percentage of the screen")]
    [Range(0, 1)]
    [SerializeField] private float minScreenPercentage = .2f;

    [Tooltip("optional UI image to visualize the swipe")]
    [SerializeField] private Image cursorVisual;

    [Tooltip("What to do after a swipe is detected")]
    [SerializeField] private UnityEvent<Vector2Int> OnUp, OnDown, OnLeft, OnRight;
    [SerializeField] private UnityEvent OnTap;

    /// <summary> minimumScreenPercentage converted to pixels </summary>
    private float minimumDistance = 20f;

    private double startTime;
    private Vector2 startPosition;

    private void Awake()
    {
        // init actions. Example on https://youtu.be/RmGgtBjQCIo
        TouchPressed.Enable();
        TouchPosition.Enable();
        TouchPressed.started += TouchStarted;
        TouchPressed.canceled += TouchEnded;
        minimumDistance = (Screen.height + Screen.width) * .5f * minScreenPercentage;
    }

    private void Reset()
    {
        TouchPressed = new InputAction(name: "Touch Pressed", binding: "<Touchscreen>/primaryTouch/press");
        TouchPosition = new InputAction(name: "Touch Position", binding: "<Touchscreen>/primaryTouch/position");

        // Bindings below are needed for using a mouse. i.e. WebGL on desktop
        TouchPressed.AddBinding("<Mouse>/leftButton");
        TouchPosition.AddBinding("<Mouse>/position");
    }

    [ContextMenu("Empty InputBindings to make sure invisible in inspector bindings are removed")]
    private void nullBindings()
    {
        TouchPressed = new InputAction();
        TouchPosition = new InputAction();
    }

    private void TouchStarted(InputAction.CallbackContext obj)
    {
        startTime = obj.time;
        startPosition = TouchPosition.ReadValue<Vector2>();
        // Debug.Log(obj.time + " & " + obj.startTime + "Started at " + startPosition);
    }

    private void TouchEnded(InputAction.CallbackContext obj)
    {
        double secondsOfSwipe = obj.time - startTime;
        Vector2 endPosition = TouchPosition.ReadValue<Vector2>();
        if (secondsOfSwipe < maxSeconds)
        {
            if (Vector2.Distance(startPosition, endPosition) >= minimumDistance)
            {
                CreateLine(startPosition, endPosition);
                Vector2 dir = (endPosition - startPosition).normalized;
                // Debug.Log(seconds + "s d" + Vector2.Distance(startPosition, endPosition) + " swipe to " + dir);

                if (Vector2.Dot(Vector2.up, dir) > directionThreshold)
                {
                    OnUp?.Invoke(Vector2Int.up);
                }
                else if (Vector2.Dot(Vector2.down, dir) > directionThreshold)
                {
                    OnDown?.Invoke(Vector2Int.down);
                }
                else if (Vector2.Dot(Vector2.left, dir) > directionThreshold)
                {
                    OnLeft?.Invoke(Vector2Int.left);
                }
                else if (Vector2.Dot(Vector2.right, dir) > directionThreshold)
                {
                    OnRight?.Invoke(Vector2Int.right);
                }
            }
            else
            {
                OnTap?.Invoke();
            }
        }
    }

    /// <summary>
    /// scales an image until left and right sides match <paramref name="point1"/> & <paramref name="point2"/>
    /// </summary>
    public void CreateLine(Vector2 point1, Vector2 point2)
    {
        if (cursorVisual == null)
        {
            return;
        }

        // Position between points
        var rectTransform = cursorVisual.rectTransform;
        rectTransform.position = (point1 + point2) / 2f; // position center at the lines midpoint

        // Rotate & Scale to reach for both points
        Vector2 dir = point2 - point1; // direction starts at 1 and ends at 2
        rectTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        rectTransform.localScale = new Vector3(dir.magnitude/rectTransform.sizeDelta.x, 1f, 1f);
    }
}
