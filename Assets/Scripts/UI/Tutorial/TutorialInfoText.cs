using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialInfoText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a);
    }

    private void OnEnable()
    {
        string text = _text.text;
        _text.text = "";
        _text.DOText(text, text.Length / 20f, scrambleMode: ScrambleMode.None)
            .SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
