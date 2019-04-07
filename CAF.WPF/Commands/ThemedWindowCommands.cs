#region Usings

using System.Windows.Input;

#endregion

namespace CompositeApplicationFramework.Commands
{
    public static class ThemedWindowCommands
    {
        public static RoutedUICommand Confirm { get; } = new RoutedUICommand(
            "Confirm",
            "Confirm",
            typeof (ThemedWindowCommands));

        public static RoutedUICommand Cancel { get; } = new RoutedUICommand(
            "Cancel",
            "Cancel",
            typeof (ThemedWindowCommands));
    }
}