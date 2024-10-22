using DG.Tweening;
using UnityEngine;

public class CubeDispenser : MonoBehaviour
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

    private DispensableCube _loadedCube;

    private void Start()
    {
        // Load a new cube into the dispenser when we start
        LoadCube();
    }

    private void LoadCube()
    {
        // Create the cube
        DispensableCube cube = Instantiate(_cube, _cubeSpawn.position, Quaternion.identity);

        // Animate the cube
        cube.MoveCube(_cubeIdle.position, _cubeLoadTime, Ease.OutBounce);

        // Cache the cube
        _loadedCube = cube;
    }
}
