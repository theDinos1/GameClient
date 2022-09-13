using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerManager Player;
    [SerializeField] private float Sensitivity = 100f;
    [SerializeField] private float SlampAngle = 85f;

    private float VerticalRotation;
    private float HorizontalRotation;

    private void Start()
    {
        VerticalRotation = transform.localEulerAngles.x;
        HorizontalRotation = Player.transform.eulerAngles.y;
    }

    private void Update()
    {
        Look();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        VerticalRotation += _mouseVertical * Sensitivity * Time.deltaTime;
        HorizontalRotation += _mouseHorizontal * Sensitivity * Time.deltaTime;

        VerticalRotation = Mathf.Clamp(VerticalRotation, -SlampAngle, SlampAngle);

        transform.localRotation = Quaternion.Euler(VerticalRotation, 0f, 0f);
        Player.transform.rotation = Quaternion.Euler(0f, HorizontalRotation, 0f);
    }
}