using DG.Tweening;
using TMPro;
using UnityEngine;

public class InteractHUD : MonoBehaviour
{
    [Header("Text")]
    [Tooltip("The text object to be displayed.")]
    [SerializeField] private InteractText _text;

    private TextMeshProUGUI _textBox;

    private Tween _fadeTween;
    private Tween _moveTween;

    private Vector2 _textStartLocal;

    private void Awake()
    {
        _textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetText();
        SetTextEnabled(false);

        _textStartLocal = _textBox.transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private void SetText()
    {
        _textBox.text = _text.Text;
        _textBox.fontSize = _text.FontSize;
    }

    private void SetTextEnabled(bool enabled)
    {
        _textBox.enabled = enabled;
    }

    private void StartAnimations()
    {
        // Start transparent
        Color transparent = new Color(_textBox.color.r, _textBox.color.g, _textBox.color.b, 0);
        _textBox.color = transparent;

        // Fade in
        _fadeTween = _textBox.DOFade(1, 1)
            .SetAutoKill(false);

        // Bounce
        _textBox.transform.localPosition = _textStartLocal;
        _moveTween = _textBox.transform.DOLocalMoveY(_textStartLocal.y - 3f, 1)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetAutoKill(false);
    }

    private void KillTweens()
    {
        if (_fadeTween != null)
        {
            _fadeTween.Kill();
        }
        if (_moveTween != null)
        {
            _moveTween.Kill();
        }
    }

    public void Show()
    {
        SetTextEnabled(true);
        StartAnimations();
    }

    public void Hide()
    {
        SetTextEnabled(false);
        KillTweens();
    }
}
