namespace Framework
{
    /// <summary>
    /// A simple data structure that associates two objects.
    /// </summary>
    public struct Tuple<T1, T2>
    {

        private T1 _first;
        private T2 _second;

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 First { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Second { get => _second; set => _second = value; }

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 X { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Y { get => _second; set => _second = value; }

        /// <summary>
        /// Creates a new Tuple with two elements.
        /// </summary>
        /// <param name="first">The first element</param>
        /// <param name="second">The second element</param>
        public Tuple(T1 first, T2 second)
        {
            _first = first;
            _second = second;
        }

    }

    /// <summary>
    /// A simple data structure that associates three objects.
    /// </summary>
    public struct Tuple<T1, T2, T3>
    {
        private T1 _first;
        private T2 _second;
        private T3 _third;

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 First { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Second { get => _second; set => _second = value; }

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 X { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Y { get => _second; set => _second = value; }

        /// <summary>
        /// The third element in the Tuple.
        /// </summary>
        public T3 Third { get => _third; set => _third = value; }

        /// <summary>
        /// The third element in the Tuple.
        /// </summary>
        public T3 Z { get => _third; set => _third = value; }

        /// <summary>
        /// Creates a new Tuple with three elements.
        /// </summary>
        /// <param name="first">The first element</param>
        /// <param name="second">The second element</param>
        /// <param name="third">The third element</param>
        public Tuple(T1 first, T2 second, T3 third)
        {
            _first = first;
            _second = second;
            _third = third;
        }
    }

    /// <summary>
    /// A simple data structure that associates four objects.
    /// </summary>
    public struct Tuple<T1, T2, T3, T4>
    {

        private T1 _first;
        private T2 _second;
        private T3 _third;
        private T4 _fourth;

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 First { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Second { get => _second; set => _second = value; }

        /// <summary>
        /// The first element in the Tuple.
        /// </summary>
        public T1 X { get => _first; set => _first = value; }

        /// <summary>
        /// The second element in the Tuple.
        /// </summary>
        public T2 Y { get => _second; set => _second = value; }

        /// <summary>
        /// The third element in the Tuple.
        /// </summary>
        public T3 Third { get => _third; set => _third = value; }

        /// <summary>
        /// The third element in the Tuple.
        /// </summary>
        public T3 Z { get => _third; set => _third = value; }

        /// <summary>
        /// The fourth element in the Tuple.
        /// </summary>
        public T4 Fourth { get => _fourth; set => _fourth = value; }

        /// <summary>
        /// The fourth element in the Tuple.
        /// </summary>
        public T4 W { get => _fourth; set => _fourth = value; }

        /// <summary>
        /// Creates a new Tuple with four elements.
        /// </summary>
        /// <param name="first">The first element</param>
        /// <param name="second">The second element</param>
        /// <param name="third">The third element</param>
        /// <param name="fourth">The fourth element</param>
        public Tuple(T1 first, T2 second, T3 third, T4 fourth)
        {
            _first = first;
            _second = second;
            _third = third;
            _fourth = fourth;
        }
    }
}