using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloneporter.UI
{
    public class StartMenuTitle : MonoBehaviour
    {
        [SerializeField] private StartMenuEventChannel _events;

        private Animator _animator;
        private int _didStartID;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _didStartID = Animator.StringToHash("didStart");
        }

        private void Start()
        {
            _animator.SetBool(_didStartID, true);
        }

        #region Animator Animation Event Handlers

        public void OnTitleAnimationStop()
        {
            _events.OnAnimationEvent.Raise(new StartMenuAnimationEvent(
                StartMenuAnimationEventType.TitleAnimationComplete));
        }

        #endregion
    }
}

