using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvTag : ITag
    {
        static readonly Dictionary<string, Func<MvTag>> MvTags = new Dictionary<string, Func<MvTag>>();

        MvTag() { }

        MvTag(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            Name = value;
        }
        public string Name { get; private set; }

        public static MvTag Development = new MvTag("Development");

        public static implicit operator string(MvTag current)
        {
            return current.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static MvTag Parse(string tag)
        {
            tag = tag?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(tag) == true)
                tag = string.Empty;

            if (MvTags.TryGetValue(tag, out Func<MvTag> foundTag))
            {
                return foundTag();
            }

            throw new NotSupportedException($"Tag not supported {tag}");
        }

        static void SetupClinets()
        {
            if (MvTags.Count == 0)
            {
                var properties = typeof(MvTag).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvTag));
                var tags = properties.Select(x => x.GetValue(null) as MvTag).ToList();

                foreach (var tag in tags)
                {
                    MvTags.Add(tag.Name.ToLower(), () => tag);
                }
            }
        }
    }
}
