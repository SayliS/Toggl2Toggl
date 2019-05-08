using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoIntegration
{
    public class WorkItemType
    {
        static Dictionary<string, Func<WorkItemType>> WorkItemTypes = new Dictionary<string, Func<WorkItemType>>();

        WorkItemType(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public string Value { get; private set; }

        public static WorkItemType ProductBacklogItem = new WorkItemType("Product Backlog Item");

        public static WorkItemType Support = new WorkItemType("Support");

        public static WorkItemType Bug = new WorkItemType("Bug");

        public static implicit operator string(WorkItemType current)
        {
            return current.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static WorkItemType Parse(string workItemType)
        {
            workItemType = workItemType?.ToLower();
            SetupWorkItems();

            if (string.IsNullOrWhiteSpace(workItemType) == true)
                workItemType = string.Empty;

            if (WorkItemTypes.TryGetValue(workItemType, out Func<WorkItemType> foundWorkItemType))
            {
                return foundWorkItemType();
            }

            throw new NotSupportedException($"Unable to find work item type for - {workItemType}");
        }

        static void SetupWorkItems()
        {
            if (WorkItemTypes.Count == 0)
            {
                var properties = typeof(WorkItemType).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(WorkItemType));
                var workItemTypes = properties.Select(x => x.GetValue(null) as WorkItemType).ToList();

                foreach (var workItemType in workItemTypes)
                {
                    WorkItemTypes.Add(workItemType.Value.ToLower(), () => workItemType);
                }
            }
        }
    }
}
