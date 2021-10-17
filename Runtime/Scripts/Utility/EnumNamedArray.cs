using System;

namespace Chroma.Utility
{
    // https://stackoverflow.com/questions/55583071/see-enumerated-indices-of-array-in-unity-inspector
    // https://stackoverflow.com/questions/24892935/custom-property-drawers-for-generic-classes-c-sharp-unity

    [Serializable]
    public abstract class EnumNamedArray { }

    [Serializable]
    public class EnumNamedArray<T> : EnumNamedArray
    {
        public string[] Names;
        public T[] Values;

        Type _enumType;

        public EnumNamedArray(Type enumType)
        {
            this._enumType = enumType;
            this.Names = Enum.GetNames(enumType);
            this.Values = new T[Names.Length];
        }

        public T this[Enum enumValue]
        {
            get
            {
                Type type = enumValue.GetType();
                if (!type.IsEnum && type != _enumType)
                    throw new ArgumentException("EnumValue must be of correct Enum type", "enumValue");
                string name = Enum.GetName(type, enumValue);
                int idx = Array.IndexOf(Names, name);
                return Values[idx];
            }
            set
            {
                Type type = enumValue.GetType();
                if (!type.IsEnum && type != _enumType)
                    throw new ArgumentException("EnumValue must be of correct Enum type", "enumValue");
                string name = Enum.GetName(type, enumValue);
                int idx = Array.IndexOf(Names, name);
                Values[idx] = value;
            }
        }
    }
}
