using UnityEngine;
using UnityEngine.UI;

public class SubtitleSystem : MonoBehaviour
{
    private Text _text;
    private float _timer;

    public void Init(Text text)
    {
        _text = text;
        _text.text = "";
    }

    public void Show(string msg, float seconds = 3f)
    {
        if (_text == null) return;
        _text.text = msg;
        _timer = seconds;
    }

    private void Update()
    {
        if (_text == null) return;
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f) _text.text = "";
        }
    }
}
