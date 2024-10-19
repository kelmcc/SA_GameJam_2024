using System;
using System.Text;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Serializable object that takes a float value, modifies it in some way, then returns it.
    /// </summary>
    [Serializable]
    public struct FloatModifier
    {

        public enum OperationType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            RaiseToPower
        }

        public enum OperandType
        {
            Constant,
            Percentage
        }

        public enum VariationType
        {
            FixedValue,
            FloatRange,
            IntRange
        }

        /// <summary>
        /// The operation to perform on the input.
        /// </summary>
        public OperationType Operation
        {
            get => _operation;
            set => _operation = value;
        }

        /// <summary>
        /// The type of operand to use in the operation.
        /// </summary>
        public OperandType Operand
        {
            get => _operand;
            set => _operand = value;
        }

        /// <summary>
        /// The operand variation to use.
        /// </summary>
        public VariationType Variation
        {
            get => _variation;
            set => _variation = value;
        }


        [SerializeField]
        private OperationType _operation;

        [SerializeField]
        private OperandType _operand;

        [SerializeField]
        private VariationType _variation;

        [SerializeField]
        private float _fixed;

        [SerializeField]
        private FloatRange _floatRange;

        [SerializeField]
        private IntRange _intRange;

        [SerializeField]
        private bool _alwaysRandomise;

        /// <summary>
        /// Creates a new float modifier with a fixed value.
        /// </summary>
        /// <param name="operation">The operation type</param>
        /// <param name="operand">The operand type</param>
        /// <param name="value">The fixed value</param>
        public FloatModifier(OperationType operation, OperandType operand, float value)
        {
            _operation = operation;
            _operand = operand;
            _fixed = value;

            _variation = VariationType.FixedValue;
            _floatRange = new FloatRange();
            _intRange = new IntRange();
            _alwaysRandomise = false;
        }


        /// <summary>
        /// Takes a float and processes it according to the FloatModifier's specifications.
        /// </summary>
        /// <param name="input">The input value</param>
        /// <returns>The processed value</returns>
        public float Process(float input)
        {

            float value = _fixed;

            switch (_variation)
            {
                case VariationType.FloatRange:
                    value = _alwaysRandomise ? _floatRange.ChooseRandom() : _floatRange.ChosenValue;
                    break;
                case VariationType.IntRange:
                    value = _alwaysRandomise ? _intRange.ChooseRandom() : _floatRange.ChosenValue;
                    break;
            }

            if (_operand == OperandType.Percentage)
            {
                value = input * (value * 0.01f);
            }

            switch (_operation)
            {
                case OperationType.Add:
                    return input + value;
                case OperationType.Subtract:
                    return input - value;
                case OperationType.Multiply:
                    return input * value;
                case OperationType.Divide:
                    return input / value;
                case OperationType.RaiseToPower:
                    return Mathf.Pow(input, value);
            }

            return input;
        }

        /// <summary>
        /// Returns a concise string the describes what the modifier does eg. "X 25%"
        /// </summary>
        /// <returns>A concise description of the modifier</returns>
        public string GetShortLabel()
        {

            StringBuilder label = new StringBuilder();

            switch (_operation)
            {
                case OperationType.Add:
                    label.Append("+ ");
                    break;
                case OperationType.Subtract:
                    label.Append("- ");
                    break;
                case OperationType.Multiply:
                    label.Append("X ");
                    break;
                case OperationType.Divide:
                    label.Append((char)247).Append(" ");
                    break;
                case OperationType.RaiseToPower:
                    label.Append("^ ");
                    break;
            }


            switch (_variation)
            {
                case VariationType.FixedValue:
                    label.Append(FormatValue(_fixed));
                    break;
                case VariationType.IntRange:
                    if (_alwaysRandomise)
                    {
                        label.Append("(");
                        label.Append(FormatValue(_intRange.Min));
                        label.Append(" - ");
                        label.Append(FormatValue(_intRange.Max));
                        label.Append(")");
                    }
                    else
                    {
                        label.Append(FormatValue(_intRange.ChosenValue));
                    }
                    break;
                case VariationType.FloatRange:
                    if (_alwaysRandomise)
                    {
                        label.Append("(");
                        label.Append(FormatValue(_floatRange.Min));
                        label.Append(" - ");
                        label.Append(FormatValue(_floatRange.Max));
                        label.Append(")");
                    }
                    else
                    {
                        label.Append(FormatValue(_floatRange.ChosenValue));
                    }
                    break;
            }

            return label.ToString();
        }

        /// <summary>
        /// Returns a verbose string the describes what the modifier does eg. "Multiply by 25%"
        /// </summary>
        /// <returns>A more verbose description of the modifier</returns>
        public string GetLongLabel()
        {
            StringBuilder label = _operation == OperationType.RaiseToPower ? new StringBuilder("Raise to the power of ") : new StringBuilder(_operation.ToString()).Append(" ");
            if (_operation == OperationType.Multiply || _operation == OperationType.Divide) label.Append("by ");

            switch (_variation)
            {
                case VariationType.FixedValue:
                    label.Append(FormatValue(_fixed));
                    break;
                case VariationType.IntRange:

                    label.Append("(");
                    label.Append(FormatValue(_intRange.Min));
                    label.Append(" - ");
                    label.Append(FormatValue(_intRange.Max));
                    label.Append(")");
                    break;
                case VariationType.FloatRange:
                    label.Append("(");
                    label.Append(FormatValue(_floatRange.Min));
                    label.Append(" - ");
                    label.Append(FormatValue(_floatRange.Max));
                    label.Append(")");
                    break;
            }

            return label.ToString();
        }

        string FormatValue(float value)
        {
            if (value == 0)
            {
                return _operand == OperandType.Constant ? "0" : "0%";
            }
            return _operand == OperandType.Constant ? value.ToString() : value.ToString("#.##") + "%";


        }



    }
}
