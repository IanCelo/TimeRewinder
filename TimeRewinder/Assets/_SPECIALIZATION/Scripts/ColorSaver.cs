using CVS_Time;
using UnityEngine;


[TimeSaved]
internal class ColorSaver : MonoBehaviour
{
	private bool _isPaused;

	private Material _material;

	private Timer _colorSwapTimer;

	[TimeSaved] private Color MaterialColor
	{
		get => _material.color;
		set => _material.color = value;
	}
	
	private void Awake()
	{
		_material ??= GetComponent<MeshRenderer>().material;
	}

	private void Start()
	{
		_colorSwapTimer = new Timer(0.5f);
		_colorSwapTimer.LoopTimer = true;

		_colorSwapTimer.OnTimerEnded += ChangeColor;
		
		_colorSwapTimer.Start();

		TimeManager.Instance.OnSaveStart += Pause;
		TimeManager.Instance.OnRewindStart += Resume;
	}

	private void ChangeColor()
	{
		if (!_isPaused)
			MaterialColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));
	}

	private void Pause() => _isPaused = true;
	private void Resume() => _isPaused = false;
	
	private void OnDestroy()
	{
		_colorSwapTimer.OnTimerEnded -= ChangeColor;

		TimeManager.Instance.OnSaveStart -= Pause;
		TimeManager.Instance.OnRewindStart -= Resume;
	}
}
