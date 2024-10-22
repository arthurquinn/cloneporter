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

    private bool _canLaunch;
    private bool _isLoaded;

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
    }

    private void SetLoadComplete()
    {
        _isLoaded = true;
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

        // Animate the cube
        _loadedCube.MoveCube(_cubeIdle.position, _cubeLaunchTime, Ease.OutBounce, SetDeactivateComplete);
    }

    private void SetDeactivateComplete()
    {

    }

    private void Launch()
    {
        _loadedCube.Launch(_collider);
    }

    #region IInteractable interface methods

    public void Interact()
    {
        Launch();
    }

    public void ShowInteractHUD()
    {
        InitiateLaunch();
    }

    public void HideInteractHUD()
    {
        DeactivateLaunch();
    }

    #endregion
}
