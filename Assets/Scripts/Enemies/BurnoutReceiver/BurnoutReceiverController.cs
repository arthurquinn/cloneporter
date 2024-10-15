using UnityEngine;

public class BurnoutReceiverController : MonoBehaviour
{
    [Header("References")]

    [Tooltip("The receiver effect to activate when a laser makes contact.")]
    [SerializeField] private GameObject _receiverEnergyField;

    [Tooltip("The trigger area representing laser activation.")]
    [SerializeField] private LaserActivator _laserActivator;

    private void Start()
    {
        _receiverEnergyField.SetActive(false);
    }

    private void OnEnable()
    {
        _laserActivator.OnLaserEnter += HandleLaserEnter;
        _laserActivator.OnLaserExit += HandleLaserExit;
    }

    private void OnDisable()
    {
        _laserActivator.OnLaserEnter -= HandleLaserEnter;
        _laserActivator.OnLaserExit -= HandleLaserExit;
    }

    private void HandleLaserEnter()
    {
        _receiverEnergyField.SetActive(true);
    }

    private void HandleLaserExit()
    {
        _receiverEnergyField.SetActive(false);
    }
}
