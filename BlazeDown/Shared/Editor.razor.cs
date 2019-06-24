using Microsoft.AspNetCore.Components;
using System.Timers;

namespace BlazeDown.Shared
{
    public class EditorBase : ComponentBase
    {
        Timer debounceTimer = new Timer()
        {
            Interval = 1000,
            AutoReset = false,
        };

        [Parameter] protected string InitialValue { get; set; }

        private object finalValue;

        [Parameter] EventCallback<UIChangeEventArgs> OnChange { get; set; }

        protected void InputChanged(UIChangeEventArgs args) 
        {
            if (!debounceTimer.Enabled)
            {
                StartDebounceTimer();
            }
            else
            {
                ExtendDebounceTimer(args);
            }
        }

        private void ExtendDebounceTimer(UIChangeEventArgs args)
        {
            finalValue = args.Value;
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void StartDebounceTimer()
        {
            debounceTimer.Elapsed += (_, a) =>
                OnChange.InvokeAsync(new UIChangeEventArgs { Type = "change", Value = finalValue });

            debounceTimer.Start();
        }
    }
}
