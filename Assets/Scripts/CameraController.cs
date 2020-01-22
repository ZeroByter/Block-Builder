using UnityEngine;
using UnityEngine.EventSystems;

namespace ZeroByterGames.BlockBuilder
{
	public class CameraController : MonoBehaviour
	{
        new private Camera camera;

        private float movementSpeed = 0.1f;
        private bool isLooking = false;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit;
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / 2);
                    int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / 2);
                    int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / 2);

                    ModelManager.AddCube(x, y, z);
                }
                else
                {
                    var point = ray.GetPoint(5);
                    int x = Mathf.FloorToInt(point.x);
                    int y = Mathf.FloorToInt(point.y);
                    int z = Mathf.FloorToInt(point.z);

                    ModelManager.AddCube(x, y, z);
                }
            }
            /*else if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse1))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    var cubePosition = hit.point;

                    int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                    int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                    int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                    ModelManager.RemoveCube(x, y, z);
                }
            }*/

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CursorController.AddUser("mainLook");
                isLooking = true;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                CursorController.RemoveUser("mainLook");
                isLooking = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                movementSpeed = 0.2f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                movementSpeed = 0.1f;
            }
        }

        public void FixedUpdate()
        {
            if(isLooking) DoLooking();
            DoMovement();
        }

        private void DoLooking()
        {
            float horizontal = Input.GetAxisRaw("Mouse X") * 1f;
            float vertical = Input.GetAxisRaw("Mouse Y") * 1f;

            var moveObject = transform;

            float rotationX = moveObject.localEulerAngles.x;
            float newRotationY = moveObject.localEulerAngles.y + horizontal;

            // Weird clamping code due to weird Euler angle mapping...
            float newRotationX = (rotationX - vertical);
            if (rotationX <= 90.0f && newRotationX >= 0.0f)
                newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
            if (rotationX >= 270.0f)
                newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

            moveObject.localRotation = Quaternion.Euler(newRotationX, newRotationY, moveObject.localEulerAngles.z);
        }

        private void DoMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal") * movementSpeed;
            float vertical = Input.GetAxisRaw("Vertical") * movementSpeed;
            float upDown = 0;
            if (Input.GetKey(KeyCode.Q)) upDown--;
            if (Input.GetKey(KeyCode.E)) upDown++;
            upDown *= movementSpeed;

            transform.position += transform.right * horizontal + transform.forward * vertical + transform.up * upDown;
        }
	}
}
