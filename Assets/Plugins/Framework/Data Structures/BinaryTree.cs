using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    /// <summary>
    /// A tree data structure where each node has a maximum of two child nodes. The nodes are not ordered in any way, that would be a binary search tree or something else.
    /// </summary>
    /// <typeparam name="T">The type of item to be stored in the nodes</typeparam>
    public class BinaryTree<T> : ICollection<T>
    {
        protected BinaryTreeNode<T> _root;

        /// <summary>
        /// The root node of this tree. Everything happens through this node.
        /// </summary>
        public BinaryTreeNode<T> Root
        {
            get => _root;
            set => _root = value;
        }
        /// <summary>
        /// The number of nodes in the entire tree.
        /// </summary>
        public int Count => _root == null ? 0 : _root.CountNodesBelow() + 1;

        /// <summary>
        /// The depth of the deepest branch in the tree. 
        /// </summary>
        public int Depth => CalculateDepth(_root);

        /// <summary>
        /// Whether or not the collection is read-only. Spoiler: Nope.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Creates an empty binary tree. It has no nodes.
        /// </summary>
        public BinaryTree()
        {
            _root = null;
        }

        /// <summary>
        /// Creates a new binary tree with a single node: the root.
        /// </summary>
        /// <param name="root">The value to store in the root</param>
        public BinaryTree(T root)
        {
            _root = new BinaryTreeNode<T>(root);
        }

        /// <summary>
        /// Creates a new binary tree from an existing one. Just be aware that nodes are mutable reference types.
        /// </summary>
        /// <param name="root">The node of the other tree that will form the root of this one.</param>
        public BinaryTree(BinaryTreeNode<T> root)
        {
            _root = root;
        }

        /// <summary>
        /// Finds all the nodes in the tree using a breadth first search.
        /// </summary>
        /// <returns>An array of the tree's nodes</returns>
        public BinaryTreeNode<T>[] GetNodesBreadthFirst()
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                list.Add(node);
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the nodes in the tree using a depth first search.
        /// </summary>
        /// <returns>An array of the tree's nodes</returns>
        public BinaryTreeNode<T>[] GetNodesDepthFirst()
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                BinaryTreeNode<T> node = stack.Pop();
                list.Add(node);
                if (node.HasRightChild) stack.Push(node.RightChild);
                if (node.HasLeftChild) stack.Push(node.LeftChild);

            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the nodes in the tree using an in-order traversal.
        /// </summary>
        /// <returns>An array of the tree's nodes</returns>
        public BinaryTreeNode<T>[] GetNodesInOrder()
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    list.Add(current);
                    current = current.RightChild;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Finds all the nodes in the tree that have no children. 
        /// </summary>
        /// <returns>An array of the tree's leaf nodes. In breadth first order.</returns>
        public BinaryTreeNode<T>[] GetLeafNodes()
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();

                if (node.IsLeaf)
                {
                    list.Add(node);
                }
                else
                {
                    if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                    if (node.HasRightChild) queue.Enqueue(node.RightChild);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the items stored in nodes that have no children. 
        /// </summary>
        /// <returns>An array of the items stored in the tree's leaf nodes. In breadth first order</returns>
        public T[] GetLeafValues()
        {
            if (_root == null) return new T[0];

            List<T> list = new List<T>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();

                if (node.IsLeaf)
                {
                    list.Add(node.Value);
                }
                else
                {
                    if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                    if (node.HasRightChild) queue.Enqueue(node.RightChild);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the items stored in the tree's nodes using a breadth first search.
        /// </summary>
        /// <returns>An array of the items stored in the tree's nodes</returns>
        public T[] GetValuesBreadthFirst()
        {
            if (_root == null) return new T[0];

            List<T> list = new List<T>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                list.Add(node.Value);

                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the items stored in the tree's nodes using a depth first search.
        /// </summary>
        /// <returns>An array of the items stored in the tree's nodes</returns>
        public T[] GetValuesDepthFirst()
        {
            if (_root == null) return new T[0];

            List<T> list = new List<T>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                BinaryTreeNode<T> node = stack.Pop();
                list.Add(node.Value);
                if (node.HasRightChild) stack.Push(node.RightChild);
                if (node.HasLeftChild) stack.Push(node.LeftChild);

            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the items stored in the tree's nodes using an in-order traversal.
        /// </summary>
        /// <returns>An array of the items stored in the tree's nodes</returns>
        public T[] GetValuesInOrder()
        {
            if (_root == null) return new T[0];

            List<T> list = new List<T>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    list.Add(current.Value);
                    current = current.RightChild;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Performs a breadth first search and adds a node containing the item in the first open spot that is found.
        /// </summary>
        /// <param name="item">The item to add</param>
        public virtual void Add(T item)
        {
            if (_root == null)
            {
                _root = new BinaryTreeNode<T>(item);
                return;
            }

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();

            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();

                if (!node.HasLeftChild)
                {
                    node.LeftChild = new BinaryTreeNode<T>(item);
                    return;
                }
                if (!node.HasRightChild)
                {
                    node.RightChild = new BinaryTreeNode<T>(item);
                    return;
                }

                queue.Enqueue(node.LeftChild);
                queue.Enqueue(node.RightChild);

            }


        }

        /// <summary>
        /// Adds each value of a collection to the tree, one at a time.
        /// </summary>
        /// <param name="values">The values to add</param>
        public void AddRange(ICollection<T> values)
        {
            foreach (T value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Clears the tree. It will have no nodes after this.
        /// </summary>
        public void Clear()
        {
            _root = null;
        }

        /// <summary>
        /// Checks whether or not the tree has a node containing a specfic item.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if a node containing the item is found</returns>
        public virtual bool Contains(T item)
        {
            if (_root == null) return false;

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                if (node.Value.Equals(item)) return true;
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return false;
        }

        /// <summary>
        /// Checks whether or not a node is part of this tree.
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <returns>True if the node is part of this tree</returns>
        public bool ContainsNode(BinaryTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (_root == null) return false;

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> currentNode = queue.Dequeue();
                if (currentNode.Equals(node)) return true;
                if (currentNode.HasLeftChild) queue.Enqueue(currentNode.LeftChild);
                if (currentNode.HasRightChild) queue.Enqueue(currentNode.RightChild);
            }

            return false;
        }

        /// <summary>
        /// Finds the number of nodes in the tree that contain a certain item.
        /// </summary>
        /// <param name="item">The item to count</param>
        /// <returns>The number of nodes in the tree that contain the item</returns>
        public int CountItems(T item)
        {
            if (_root == null) return 0;

            int count = 0;
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                if (node.Value.Equals(item)) count++;
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return count;
        }

        /// <summary>
        /// Copies the contents of tree's nodes (in breadth first order) to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The target array</param>
        /// <param name="arrayIndex">The starting index</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            array.CopyTo(GetNodesBreadthFirst(), arrayIndex);
        }

        /// <summary>
        /// Finds the first node in the tree that contains a specific item, by using a breadth first search.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The first node that contains the item. Null if no such node is found</returns>
        public BinaryTreeNode<T> FindBreathFirst(T item)
        {
            if (_root == null) return null;

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                if (node.Value.Equals(item)) return node;
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return null;
        }

        /// <summary>
        /// Finds the first node in the tree that contains a specific item, by using a depth first search.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The first node that contains the item. Null if no such node is found</returns>
        public BinaryTreeNode<T> FindDepthFirst(T item)
        {
            if (_root == null) return null;

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                BinaryTreeNode<T> node = stack.Pop();
                if (node.Value.Equals(item)) return node;
                if (node.HasRightChild) stack.Push(node.RightChild);
                if (node.HasLeftChild) stack.Push(node.LeftChild);

            }

            return null;
        }

        /// <summary>
        /// Finds the first node in the tree that contains a specific item, by using an in-order traversal.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The first node that contains the item. Null if no such node is found</returns>
        public BinaryTreeNode<T> FindInOrder(T item)
        {
            if (_root == null) return null;

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    if (current.Value.Equals(item)) return current;
                    current = current.RightChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all the nodes in the tree that contain a specfic item, by using a breadth first search.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>An array of nodes that contain the item, in breadth first order</returns>
        public BinaryTreeNode<T>[] FindAllBreathFirst(T item)
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                if (node.Value.Equals(item)) list.Add(node);
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the nodes in the tree that contain a specfic item, by using a depth first search.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>An array of nodes that contain the item, in depth first order</returns>
        public BinaryTreeNode<T>[] FindAllDepthFirst(T item)
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                BinaryTreeNode<T> node = stack.Pop();
                if (node.Value.Equals(item)) list.Add(node);
                if (node.HasRightChild) stack.Push(node.RightChild);
                if (node.HasLeftChild) stack.Push(node.LeftChild);

            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds all the nodes in the tree that contain a specfic item, by using an in-order traversal.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>An array of nodes that contain the item, in depth first order</returns>
        public BinaryTreeNode<T>[] FindAllInOrder(T item)
        {
            if (_root == null) return new BinaryTreeNode<T>[0];

            List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    if (current.Value.Equals(item)) list.Add(current);
                    current = current.RightChild;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Removes the first node that contains a specific item, uses a breadth first seach.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if a node was found and removed</returns>
        public virtual bool Remove(T item)
        {
            if (_root == null) return false;


            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> parent = queue.Dequeue();

                if (parent.LeftChild.Value.Equals(item))
                {
                    RemoveNode(parent.LeftChild, parent);
                    return true;
                }

                if (parent.RightChild.Value.Equals(item))
                {
                    RemoveNode(parent.RightChild, parent);
                    return true;
                }

                if (parent.HasLeftChild) queue.Enqueue(parent.LeftChild);
                if (parent.HasRightChild) queue.Enqueue(parent.RightChild);
            }

            return false;
        }

        /// <summary>
        /// Removes a specific node from the tree.
        /// </summary>
        /// <param name="node">The node to be removed</param>
        /// <returns>True if the node was found and removed</returns>
        public virtual bool RemoveNode(BinaryTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (_root == null) return false;

            if (node == _root)
            {
                RemoveNode(_root);
                return true;
            }

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> parent = queue.Dequeue();

                if (parent.LeftChild == node)
                {
                    RemoveNode(parent.LeftChild, parent);
                    return true;
                }

                if (parent.RightChild == node)
                {
                    RemoveNode(parent.RightChild, parent);
                    return true;
                }

                if (parent.HasLeftChild) queue.Enqueue(parent.LeftChild);
                if (parent.HasRightChild) queue.Enqueue(parent.RightChild);
            }

            return false;
        }

        /// <summary>
        /// Removes all nodes in the tree that contains a specific item.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if at least one node was found and removed</returns>
        public bool RemoveAll(T item)
        {
            if (_root == null) return false;

            bool somethingRemoved = false;
            while (Remove(item))
            {
                somethingRemoved = true;
            }

            return somethingRemoved;
        }


        /// <summary>
        /// Gets an enumerator that returns all of the items in the tree's nodes. Uses an in-order traversal.
        /// </summary>
        /// <returns>An enumerator of the tree's items</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (_root == null) yield break;

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    yield return current.Value;
                    current = current.RightChild;
                }
            }

        }

        /// <summary>
        /// Gets an enumerator that returns all of the tree's nodes. Uses an in-order traversal.
        /// </summary>
        /// <returns>An enumerator of the tree's nodes</returns>
        public IEnumerator<BinaryTreeNode<T>> GetNodeEnumerator()
        {
            if (_root == null) yield break;

            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            BinaryTreeNode<T> current = _root;

            while (current != null || stack.Count > 0)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                    yield return current;
                    current = current.RightChild;
                }
            }

        }

        /// <summary>
        /// Returns the tree in a multiline string, by using a breadth first search.
        /// </summary>
        /// <returns>The tree, one node per line</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Tree: (");
            builder.Append(Count);
            builder.Append(")");
            BinaryTreeNode<T>[] nodes = GetNodesBreadthFirst();
            for (int i = 0; i < nodes.Length; i++)
            {
                builder.Append("\n");
                builder.Append(nodes[i]);
            }

            return builder.ToString();
        }

        protected virtual void RemoveNode(BinaryTreeNode<T> node, BinaryTreeNode<T> parent)
        {
            BinaryTreeNode<T> child;

            // If node has ANY children, transform it into its replacement node and delete THAT node. The node adjustments will always be from wherever the parent reference ends up. 
            if (node.IsLeaf)
            {
                if (parent == null)
                {
                    _root = null;
                }
                else if (parent.LeftChild == node)
                {
                    parent.LeftChild = null;
                }
                else
                {
                    parent.RightChild = null;
                }
                return;
            }

            // Three cases for interior nodes:
            if (node.LeftChild == null) // Half-full to the right
            {
                child = node.RightChild;
                SwapValues(node, child); // Swap data fields 
                node.LeftChild = child.LeftChild; // Bring along the children, 
                node.RightChild = child.RightChild; // both left and right 

            }
            else if (node.RightChild == null) // Half-full to the left
            {
                child = node.LeftChild;
                SwapValues(node, child); // Swap data fields 
                node.LeftChild = child.LeftChild; // Bring along the children, 
                node.RightChild = child.RightChild; // both left and right 


            }
            else // Full: find predecessor
            {
                child = node.LeftChild; // Go one step LEFT 
                parent = null;
                while (child.RightChild != null) // Traverse to the RIGHT 
                {
                    parent = child;
                    child = parent.RightChild;
                }
                if (parent == null) // did NOT traverse into right: 
                {
                    SwapValues(node, child); // node.left.right == null 
                    node.LeftChild = child.LeftChild; // Link in child's left tree 

                }
                else // we DID traverse right 
                {
                    SwapValues(node, child);
                    parent.RightChild = child.LeftChild; // Link in child's left tree 

                }
            }

        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected int CalculateDepth(BinaryTreeNode<T> node)
        {

            if (node == null) return 0;

            Queue<BinaryTreeNode<T>> nodes = new Queue<BinaryTreeNode<T>>();
            Queue<int> depths = new Queue<int>();

            int maxDepth = 1;

            nodes.Enqueue(node);
            depths.Enqueue(1);

            while (nodes.Count > 0)
            {
                BinaryTreeNode<T> current = nodes.Dequeue();
                int depth = depths.Dequeue();

                if (depth > maxDepth)
                {
                    maxDepth = depth;
                }

                if (current.HasLeftChild)
                {
                    nodes.Enqueue(current.LeftChild);
                    depths.Enqueue(depth + 1);
                }
                if (current.HasRightChild)
                {
                    nodes.Enqueue(current.RightChild);
                    depths.Enqueue(depth + 1);
                }

            }

            return maxDepth;
        }

        protected void SwapValues(BinaryTreeNode<T> nodeA, BinaryTreeNode<T> nodeB)
        {
            T temp = nodeA.Value;
            nodeA.Value = nodeB.Value;
            nodeB.Value = temp;
        }

    }

    /// <summary>
    /// A node in a binary tree. Contains a link to its two children, but not a link back up to its parent.
    /// </summary>
    /// <typeparam name="T">The type of item to be stored in the node</typeparam>
    public class BinaryTreeNode<T>
    {
        private T _value;
        private BinaryTreeNode<T> _left;
        private BinaryTreeNode<T> _right;

        /// <summary>
        /// Whether or not this node has a left child.
        /// </summary>
        public bool HasLeftChild => _left != null;

        /// <summary>
        /// Whether or not this node has a right child.
        /// </summary>
        public bool HasRightChild => _right != null;

        /// <summary>
        /// Whether or not this node has is a leaf node, ie. it has no children.
        /// </summary>
        public bool IsLeaf => _left == null && _right == null;

        /// <summary>
        /// The item actually stored in this node.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        /// <summary>
        /// The left child of this node, may be null.
        /// </summary>
        public BinaryTreeNode<T> LeftChild
        {
            get => _left;
            set => _left = value;
        }
        /// <summary>
        /// The right child of this node, may be null.
        /// </summary>
        public BinaryTreeNode<T> RightChild
        {
            get => _right;
            set => _right = value;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="value">The item to be stored in the node</param>
        public BinaryTreeNode(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Counts the number of nodes that are descended from this node.
        /// </summary>
        /// <returns>The number of nodes that descend from this one</returns>
        public int CountNodesBelow()
        {
            int count = 0;
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            if (HasLeftChild) queue.Enqueue(LeftChild);
            if (HasRightChild) queue.Enqueue(RightChild);

            while (queue.Count > 0)
            {
                count++;
                BinaryTreeNode<T> node = queue.Dequeue();
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            return count;
        }

        /// <summary>
        /// Resturns a string containing the value of the node, and the value of its child nodes.
        /// </summary>
        /// <returns>Node value and child values</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Node: ");

            builder.Append(Value);
            if (HasLeftChild)
            {
                builder.Append(" - L: ");
                builder.Append(LeftChild.Value);
            }
            if (HasRightChild)
            {
                builder.Append(" - R: ");
                builder.Append(RightChild.Value);
            }
            return builder.ToString();
        }


    }
}