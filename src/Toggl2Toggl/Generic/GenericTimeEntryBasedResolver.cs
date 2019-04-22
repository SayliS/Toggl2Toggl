﻿using Toggl;

namespace Toggl2Toggl
{
    public class GenericTimeEntryBasedResolver<T> : ITimeEntryBasedResolver<T>
    {
        readonly IPhraseMapping<T> phraseMapping;

        public GenericTimeEntryBasedResolver(IPhraseMapping<T> phraseMapping, int weight)
        {
            this.phraseMapping = phraseMapping;
            Weight = weight;
        }

        public int Weight { get; private set; }

        public bool TryResolve(TimeEntry entry, out T result)
        {
            result = default(T);
            if (phraseMapping.TryGet(entry.Description, out result))
                return true;

            return false;
        }
    }
}
