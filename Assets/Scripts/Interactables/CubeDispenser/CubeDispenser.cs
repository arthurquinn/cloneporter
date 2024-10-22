using DG.Tweening;
using UnityEngine;

public class CubeDispenser : MonoBehaviour, IInteractable
{
    [Header("Cube")]
    [Tooltip("The cube prefab for the dispenser to create new cubes.")]
    [SerializeField] private DispensableCube _cube;
    [Tooltip("The spawn point of the cube.")]
    [SerializeField] private Transform _cubeSpawn;
    [Tooltip("The idle position of the cube when it is idle.")]
    [SerializeField] private Transform _cubeIdle;
    [Tooltip("The launch position of the cube.")]
    [SerializeField] private Transform _cubeLaunch;
    [Tooltip("The time it takes to load a cube.")]
    [SerializeField] private float _cubeLoadTime;
    [Tooltip("The time it takes to launch a cube.")]
    [SerializeField] private float _cubeLaunchTime;

    [Header("Interact HUD")]
    [Tooltip("The interact hud tooltip for this game object.")]
    [SerializeField] private InteractHUD _interactHUD;

    private Collider2D _collider;

    private DispensableCube _loadedCube;
    private bool _canInteract;
    private bool _canLaunch;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        // Load a new cube into the dispenser when we start
        LoadCube();
    }

    private void LoadCube()
    {
        // Create the cube
        _loadedCube = Instantiate(_cube, _cubeSpawn.position, Quaternion.identity);

        // Animate the cube
        _loadedCube.MoveCube(_cubeIdle.position, _cubeLoadTime, Ease.OutBounce, SetLoadComplete);

        // Listen for the cube destroyed event
        CubeDestructable destructable =  _loadedCube.GetComponent<CubeDestructable>();
        destructable.OnCubeDestroyed += HandleCubeDestroyed;
    }

    private void SetLoadComplete()
    {
        _canInteract = true;

        // Toggle our collider on and off
        // If a player was waiting by the dispenser while the cube was loading
        //   this will allow them to interact instantly
        _collider.enabled = false;
        _collider.enabled = true;
    }

    private void InitiateLaunch()
    {
        // Animate the cube
        _loadedCube.MoveCube(_cubeLaunch.position, _cubeLaunchTime, Ease.OutQuad, SetInitiateComplete);
    }

    private void SetInitiateComplete()
    {
        _interactHUD.Show();
        _canLaunch = true;
    }

    private void DeactivateLaunch()
    {
        // Hide the hud
        _interactHUD.Hide();

        // Cannot launch
        _canLaunch = false;

        // Animate the cube
        _loadedCube.MoveCube(_cubeIdle.position, _cubeLaunchTime, Ease.OutBounce, SetDeactivateComplete);
    }

    private void SetDeactivateComplete()
    {

    }

    private void Launch()
    {
        if (_canLaunch)
        {
            _loadedCube.Launch(_collider);
            _interactHUD.Hide();
            _canInteract = false;
            _canLaunch = false;
        }
    }

    private void HandleCubeDestroyed()
    {
        // Load a new cube if the one we dispensed gets destroyed
        LoadCube();
    }

    #region IInteractable interface methods

    public void Interact()
    {
        if (_canInteract)
        {
            Launch();
        }
    }

    public void ShowInteractHUD()
    {
        if (_canInteract)
        {
            InitiateLaunch();
        }
    }

    public void HideInteractHUD()
    {
        if (_canInteract)
        {
            DeactivateLaunch();
        }
    }

    #endregion
}
