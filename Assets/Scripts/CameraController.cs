using UnityEngine;
using UnityEngine.EventSystems;
using ZeroByterGames.BlockBuilder.TransformSystem;
using ZeroByterGames.BlockBuilder.UI;
using ZeroByterGames.BlockBuilder.UndoSystem;
using static ZeroByterGames.BlockBuilder.UI.ToolbarController;

namespace ZeroByterGames.BlockBuilder
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController Singleton;

        public static Camera GetCamera()
        {
            if (Singleton == null) return null;

            return Singleton.camera;
        }

        new private Camera camera;

        private float movementSpeed = 0.175f;
        private bool isLooking = false;

        private float lastRapidTool;

        private void Awake()
        {
            Singleton = this;

            camera = GetComponent<Camera>();

            ToolbarController.NewToolSelected += OnNewToolSelected;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var currentTool = GetCurrentTool();

            if (currentTool == Tool.Select)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                        if (ModelManager.GetCube(x, y, z) && !CubeSelectionController.DoesCubeExist(x, y, z) && TransformController.GetSelectedTranformComponents() == 0)
                        {
                            int color = ModelManager.GetCubeColor(x, y, z);

                            //CubeSelectionController.ResetCubePositions();
                            CubeSelectionController.AddCube(x, y, z, color);

                            ModelManager.RemoveCube(x, y, z);

                            TransformController.UpdateVisibleTools();
                        }
                    }
                    else
                    {
                        var controllerPosition = CubeSelectionController.GetPosition();
                        foreach (var cube in CubeSelectionController.GetAllCubes())
                        {
                            ModelManager.AddCube(cube.x + (int)controllerPosition.x, cube.y + (int)controllerPosition.y, cube.z + (int)controllerPosition.z, cube.color);
                        }

                        CubeSelectionController.Clear();

                        CubeSelectionController.ResetPosition();

                        TransformController.UpdateVisibleTools();
                    }
                }
            }
            else if (currentTool == Tool.Create)
            {
                if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift) && Time.time - lastRapidTool > 0.15f)
                {
                    lastRapidTool = Time.time;

                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / 2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / 2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / 2);

                        ModelManager.AddCube(x, y, z);

                        UndoManager.AddAction(new CreateBlockAction(x, y, z, ColorpickerController.GetClosestColor().GetAsIndex()));
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / 2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / 2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / 2);

                        ModelManager.AddCube(x, y, z);

                        UndoManager.AddAction(new CreateBlockAction(x, y, z, ColorpickerController.GetClosestColor().GetAsIndex()));
                    }
                    else
                    {
                        var point = ray.GetPoint(5);
                        int x = Mathf.FloorToInt(point.x);
                        int y = Mathf.FloorToInt(point.y);
                        int z = Mathf.FloorToInt(point.z);

                        ModelManager.AddCube(x, y, z);

                        UndoManager.AddAction(new CreateBlockAction(x, y, z, ColorpickerController.GetClosestColor().GetAsIndex()));
                    }
                }
            }
            else if (currentTool == Tool.Destroy)
            {
                if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift) && Time.time - lastRapidTool > 0.15f)
                {
                    lastRapidTool = Time.time;

                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                        int color = ModelManager.GetCubeColor(x, y, z);
                        ModelManager.RemoveCube(x, y, z);

                        UndoManager.AddAction(new RemoveBlockAction(x, y, z, color));

                    }
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                        int color = ModelManager.GetCubeColor(x, y, z);
                        ModelManager.RemoveCube(x, y, z);

                        UndoManager.AddAction(new RemoveBlockAction(x, y, z, color));

                    }
                }
            }
            else if (currentTool == Tool.Paint)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                        int color = ModelManager.GetCubeColor(x, y, z);
                        int colorpickerValue = ColorpickerController.GetClosestColor().GetAsIndex();

                        if (color != colorpickerValue)
                        {
                            UndoManager.AddAction(new PaintBlockAction(x, y, z, color, ColorpickerController.GetClosestColor().GetAsIndex()));

                            ModelManager.AddCube(x, y, z);
                        }
                    }
                }
            }
            else if (currentTool == Tool.Colorpicker)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    var ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        int x = Mathf.FloorToInt(hit.point.x + hit.normal.x / -2);
                        int y = Mathf.FloorToInt(hit.point.y + hit.normal.y / -2);
                        int z = Mathf.FloorToInt(hit.point.z + hit.normal.z / -2);

                        int color = ModelManager.GetCubeColor(x, y, z);
                        int paletteWidth = ColorPaletteManager.GetPaletteWidth();
                        ColorpickerController.SetColor(color % paletteWidth, Mathf.FloorToInt(color / paletteWidth));
                    }
                }
            }

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
                movementSpeed = 0.35f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                movementSpeed = 0.175f;
            }
        }

        public void FixedUpdate()
        {
            if (isLooking) DoLooking();
            DoMovement();
        }

        private void OnNewToolSelected(Tool newTool)
        {
            if(newTool != Tool.Select)
            {
                //Remove selected cubes when not in selection tool
                foreach (var cube in CubeSelectionController.GetAllCubes())
                {
                    ModelManager.AddCube(cube.x, cube.y, cube.z, cube.color);
                }

                CubeSelectionController.Clear();
            }
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
