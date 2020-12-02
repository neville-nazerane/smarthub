using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Logic.Automations
{
    internal class AutomationsCollection
    {
        internal enum EventTypes {
            BedroomMotion, BedroomNoMotion
        }

        internal readonly Dictionary<string, ICollection<Type>> _automations;

        internal AutomationsCollection()
        {
            _automations = new Dictionary<string, ICollection<Type>>();
        }

        internal AutomationsCollection SetAutomation<TAutomation>(EventTypes eventType) where TAutomation : IAutomation
        {
            if (_automations.TryGetValue(eventType.ToString(), out var types))
                types.Add(typeof(TAutomation));
            else _automations[eventType.ToString()] = new List<Type> { typeof(TAutomation) };
            return this;
        }

        internal IEnumerable<Type> Get(string eventType) => _automations.GetValueOrDefault(eventType);

        internal IEnumerable<Type> Get(EventTypes eventType) => Get(eventType.ToString());

    }
}
