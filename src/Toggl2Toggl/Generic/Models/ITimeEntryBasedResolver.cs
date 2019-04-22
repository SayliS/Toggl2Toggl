using Toggl;

namespace Toggl2Toggl
{
    public interface ITimeEntryBasedResolver<T>
    {
        bool TryResolve(TimeEntry entry, out T result);

        int Weight { get; }
    }
}
