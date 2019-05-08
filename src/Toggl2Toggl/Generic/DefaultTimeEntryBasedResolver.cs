using Toggl;

namespace Toggl2Toggl
{
    public class DefaultTimeEntryBasedResolver<T> : ITimeEntryBasedResolver<T>
    {
        readonly T defaultEntry;

        public DefaultTimeEntryBasedResolver(T defaultEntry, int weight)
        {
            this.defaultEntry = defaultEntry;
            Weight = weight;
        }

        public int Weight { get; private set; }

        public bool TryResolve(TimeEntry entry, out T result)
        {
            result = defaultEntry;
            return true;
        }
    }
}
