using DG.Tweening;
using UnityEngine;

public class CubeDispenser : MonoBehaviour, IInteractable
{
    [Header("Cube")]
    [Tooltip("The cube prefab for the dispenser to create new cubes.")]
    [SerializeField] private DispensableCube _cube;
    [Tooltip("The spawn point of the cube.")]
    [SerializeField] private Transform _cubeSpawn;
    [Tooltip("The idle position of the cube before it is launched by the player.")]
    [SerializeField] private Transform _cubeIdle;
    [Tooltip("The time it takes to load a cube.")]
    [SerializeField] private float _cubeLoadTime;

    [Header("Interact HUD")]
    [Tooltip("The interact hud tooltip for this game object.")]
    [SerializeField] private InteractHUD _interactHUD;

    private DispensableCube _loadedCube;

    private bool _isLaunchReady;

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
        _isLaunchReady = true;
    }

    #region IInteractable interface methods

    public void ShowInteractHUD()
    {
        _interactHUD.Show();
    }

    public void HideInteractHUD()
    {
        _interactHUD.Hide();
    }

    #endregion
}
