using System;
using System.Collections.Generic;

namespace VisualARQMac.Core
{
    /// <summary>Represents a single typed BIM parameter.</summary>
    public class BimParameter
    {
        /// <summary>Parameter name.</summary>
        public string Name { get; set; }

        /// <summary>Parameter value.</summary>
        public object Value { get; set; }

        /// <summary>Parameter description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Parameter type.</summary>
        public ParameterType Type { get; set; } = ParameterType.Generic;

        /// <summary>Is this a type parameter (vs instance parameter)?</summary>
        public bool IsTypeParameter { get; set; } = false;

        /// <summary>IFC property mapping.</summary>
        public string IfcProperty { get; set; } = string.Empty;

        /// <summary>Unit of measurement.</summary>
        public string Unit { get; set; } = string.Empty;

        public BimParameter(string name, object value, string description = "")
        {
            Name = name;
            Value = value;
            Description = description;
            Type = DetectType(value);
        }

        private static ParameterType DetectType(object value)
        {
            switch (value)
            {
                case null: return ParameterType.Generic;
                case int _: return ParameterType.Integer;
                case double _: return ParameterType.Number;
                case float _: return ParameterType.Number;
                case bool _: return ParameterType.Boolean;
                case string _: return ParameterType.Text;
                case Guid _: return ParameterType.Guid;
                default: return ParameterType.Generic;
            }
        }

        /// <summary>Get value as a specific type, converting when possible.</summary>
        public T GetValue<T>()
        {
            if (Value is T valueT)
                return valueT;

            try
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        public BimParameter Clone()
        {
            return new BimParameter(Name, Value, Description)
            {
                Type = Type,
                IsTypeParameter = IsTypeParameter,
                IfcProperty = IfcProperty,
                Unit = Unit
            };
        }

        public override string ToString() => $"{Name}: {Value} ({Type})";
    }

    /// <summary>Parameter type enumeration.</summary>
    public enum ParameterType
    {
        Generic,
        Integer,
        Number,
        Boolean,
        Text,
        Guid,
        Length,
        Area,
        Volume,
        Angle,
        Material,
        Style
    }

    /// <summary>A name-indexed collection of <see cref="BimParameter"/>.</summary>
    public class BimParameterCollection : List<BimParameter>
    {
        /// <summary>Get or set a parameter by (case-insensitive) name.</summary>
        public BimParameter this[string name]
        {
            get
            {
                foreach (var param in this)
                {
                    if (param.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return param;
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Count; i++)
                {
                    if (base[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        base[i] = value;
                        return;
                    }
                }
                Add(value);
            }
        }

        public bool TryGetValue(string name, out BimParameter parameter)
        {
            parameter = this[name];
            return parameter != null;
        }

        public void AddOrUpdate(BimParameter parameter) => this[parameter.Name] = parameter;

        public object GetValue(string name) => this[name]?.Value;

        public T GetValue<T>(string name)
        {
            var param = this[name];
            return param != null ? param.GetValue<T>() : default;
        }
    }
}
