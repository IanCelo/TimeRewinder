using System;


public class Timer
{
    private bool _isBound;
    private bool _started;
    
    public bool HasTimerEnded { get; private set; }
    public double MaxTime { get; private set; }
    public double CurrentTime { get; private set; }

    public double RemainingTime => Math.Max(MaxTime - CurrentTime, 0.0);

    internal Action OnTimerStart;
    internal Action OnTimerUpdate;
    internal Action OnTimerEnded;

    // Requires AutomaticUpdate. Setting to true will also set AutomaticUpdate
    private bool _isTimerLooping;
    internal bool LoopTimer
    {
        get => _isTimerLooping;

        set
        {
            _isTimerLooping = value;

            if (value)
                automaticUpdate = true;
        }
    }
    
    private bool _isUpdateAutomatic;
    internal bool automaticUpdate
    {
        get => _isUpdateAutomatic;

        set
        {
            _isUpdateAutomatic = value;
            BindUpdate(value);
        }
    }

    private void BindUpdate(bool bind)
    {
        if (bind)
        {
            if (_isBound) return;

            TimerManager.OnUpdate += OnUpdate;
            _isBound = true;

            return;
        }

        if (!_isBound) return;

        TimerManager.OnUpdate -= OnUpdate;
        _isBound = false;
    }

    public Timer()
    {
        OnTimerEnded += () => HasTimerEnded = !LoopTimer;
    }
    
    public Timer(float timer_duration)
    {
        Set(timer_duration);
        OnTimerEnded += () => HasTimerEnded = !LoopTimer;
    }

    public void Set(float timer_duration)
    {
        MaxTime = timer_duration;
    }

    public void Start()
    {
        _started = true;
        HasTimerEnded = false;

        OnTimerStart?.Invoke();    
    }

    public void Stop()
    {
        if (!_isUpdateAutomatic) return;
        _started = false;
        CurrentTime = 0f;
    }

    public void StartOrReset()
    {
        if (_started) Stop();
        Start();
    }

    public void Pause()
    {
        if (!_isUpdateAutomatic) return;
        _started = !_started;
    }

    // Won't do anything if AutomaticUpdate is on
    public void Update(float delta_time)
    {
        if (_isUpdateAutomatic) return;

        if (!_started)
            StartOrReset();
        
        OnUpdate(delta_time);
    }

    private void OnUpdate(float delta_time)
    {
        if (!_started) return;
        
        CurrentTime += delta_time;
        OnTimerUpdate?.Invoke();

        if (CurrentTime <= MaxTime) return;

        CurrentTime -= MaxTime;
        OnTimerEnded?.Invoke();

        _started = _isUpdateAutomatic && _isTimerLooping;
        if (_started)
            Start();
    }
}
