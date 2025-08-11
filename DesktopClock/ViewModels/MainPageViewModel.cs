using Reactive.Bindings;
using Reactive.Bindings.TinyLinq;
using System.Reactive.Subjects;
using System.Reflection;

namespace DesktopClock.ViewModels;

public class MainPageViewModel
{
    private readonly ReactiveTimer _clock = new(TimeSpan.FromSeconds(0.1));
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    private readonly StartupRegister _startupRegister;

    public string? WindowTitle => _assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;

    private ReactiveProperty<DateTimeOffset> Current { get; }
    public ReactiveProperty<double> HourAngle { get; }
    public ReactiveProperty<double> MinuteAngle { get; }
    public ReactiveProperty<double> SecondAngle { get; }

    public MainPageViewModel(StartupRegister startupRegister)
    {
        _startupRegister = startupRegister;

        this.Current = _clock
            .Select(_ => DateTimeOffset.Now)
            .ToReactiveProperty(DateTimeOffset.Now);

        this.HourAngle = this.Current
            .Select(s => ((s.Hour % 12) * 30) + (s.Minute * 0.5))
            .ToReactiveProperty();

        this.MinuteAngle = this.Current
            .Select(s => s.Minute * 6 + s.Second * 0.1)
            .ToReactiveProperty();

        this.SecondAngle = this.Current
            .Select(s => 360d / 60d * (s.Second + (s.Millisecond / 1000d)))
            .ToReactiveProperty();

        _clock.Start();
    }

    public void RegisterStartup()
    {
        _startupRegister.Register();
    }

    public void UnregisterStartup()
    {
        _startupRegister.Unregister();
    }

    public bool IsStartupRegistered()
    {
        return _startupRegister.IsRegistered();
    }
}
