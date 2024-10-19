using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// A self-balancing variation of the binary search tree that satisfies the condition that the depth of the two child subtrees can differ by at most one. This ensures that insertion, removal and lookup all take O(log n) in both the average and worst case.
    /// </summary>
    /// <typeparam name="T">The type of item to store in each node</typeparam>
    public class AVLTree<T> : BinarySearchTree<T> where T : IComparable<T>
    {

        /// <summary>
        /// Creates an empty binary search tree. It will have no nodes.
        /// </summary>
        public AVLTree()
        {
            _root = null;
        }

        /// <summary>
        /// Creates a new binary search tree with a single node: the root.
        /// </summary>
        /// <param name="root">The value to store in the root</param>
        public AVLTree(T root)
        {
            _root = new BinaryTreeNode<T>(root);
        }

        /// <summary>
        /// Creates a new binary search tree from an existing one. Just be aware that nodes are mutable reference types.
        /// </summary>
        /// <param name="root">The node of the other tree that will form the root of this one.</param>
        public AVLTree(BinaryTreeNode<T> root)
        {
            _root = root;
        }

        /// <summary>
        /// Inserts a node containing the item in it's rightful position in the tree.
        /// </summary>
        /// <param name="item">The item to add</param>
        public override void Add(T item)
        {

            if (_root == null)
            {
                _root = new BinaryTreeNode<T>(item);
                return;
            }

            BinaryTreeNode<T> current = _root;
            Stack<BinaryTreeNode<T>> parents = new Stack<BinaryTreeNode<T>>();

            //Add the node, recording parents as we go
            while (current != null)
            {
                if (item.CompareTo(current.Value) < 1)
                {
                    if (current.HasLeftChild)
                    {
                        parents.Push(current);
                        current = current.LeftChild;
                    }
                    else
                    {
                        current.LeftChild = new BinaryTreeNode<T>(item);
                        break;
                    }
                }
                else
                {
                    if (current.HasRightChild)
                    {
                        parents.Push(current);
                        current = current.RightChild;
                    }
                    else
                    {
                        current.RightChild = new BinaryTreeNode<T>(item);
                        break;
                    }
                }
            }

            BinaryTreeNode<T> node = parents.Count > 0 ? parents.Pop() : null;
            BinaryTreeNode<T> parent = parents.Count > 0 ? parents.Pop() : null;

            //Balance going up the tree
            while (node != null)
            {

                int balance = GetBalance(node);

                if (Math.Abs(balance) == 2) //2, -2
                {
                    //Rebalance tree
                    Balance(node, parent, balance);
                }

                node = parent;
                parent = parents.Count > 0 ? parents.Pop() : null;

            }

        }

        /// <summary>
        /// Removes the first node that contains a specific item. Uses a binary search.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if a node was found and removed</returns>
        public override bool Remove(T item)
        {
            if (_root == null) return false;

            BinaryTreeNode<T> current = _root;
            Stack<BinaryTreeNode<T>> parents = new Stack<BinaryTreeNode<T>>();

            //Find the node to remove and all the nodes above it
            while (current != null && !current.Value.Equals(item))
            {

                if (item.CompareTo(current.Value) < 1)
                {
                    parents.Push(current);
                    current = current.LeftChild;
                }
                else
                {
                    parents.Push(current);
                    current = current.RightChild;
                }
            }

            //We haven't found a node with the right value
            if (current == null) return false;

            BinaryTreeNode<T> node = parents.Count > 0 ? parents.Pop() : null;
            BinaryTreeNode<T> parent = parents.Count > 0 ? parents.Pop() : null;

            //Remove the node
            base.RemoveNode(current, node);

            //Balance going up the tree
            while (node != null)
            {
                int balance = GetBalance(node);

                //height hasn't changed, can stop
                if (Math.Abs(balance) == 1) break;


                if (Math.Abs(balance) == 2) //2, -2
                {
                    //Rebalance tree
                    Balance(node, parent, balance);

                }

                node = parent;
                parent = parents.Count > 0 ? parents.Pop() : null;

            }

            return true;

        }

        /// <summary>
        /// Removes a specific node from the tree.
        /// </summary>
        /// <param name="node">The node to be removed</param>
        /// <returns>True if the node was found and removed</returns>
        public override bool RemoveNode(BinaryTreeNode<T> node)
        {
            if (_root == null) return false;

            BinaryTreeNode<T> current = _root;
            Stack<BinaryTreeNode<T>> parents = new Stack<BinaryTreeNode<T>>();

            //Find the node to remove and all the nodes above it
            while (current != null && !current.Equals(node))
            {

                if (node.Value.CompareTo(current.Value) < 1)
                {
                    parents.Push(current);
                    current = current.LeftChild;
                }
                else
                {
                    parents.Push(current);
                    current = current.RightChild;
                }
            }

            //We haven't found a node with the right value
            if (current == null) return false;

            node = parents.Count > 0 ? parents.Pop() : null;
            BinaryTreeNode<T> parent = parents.Count > 0 ? parents.Pop() : null;

            //Remove the node
            RemoveNode(current, node);

            //Balance going up the tree
            while (node != null)
            {
                //  Debug.Log(node.Value + " " + parent.Value);
                int balance = GetBalance(node);

                //height hasn't changed, can stop
                if (Math.Abs(balance) == 1) break;


                if (Math.Abs(balance) == 2) //2, -2
                {
                    //Rebalance tree
                    Balance(node, parent, balance);
                }

                node = parent;
                parent = parents.Count > 0 ? parents.Pop() : null;

            }

            return true;
        }

        /// <summary>
        /// Ensures that the tree is properly ordered and balanced. It is necessary to call this if you manually alter nodes in the tree, which is something you should avoid doing. Note that this is potentially expensive as it basically rebuilds the tree from scratch, rather try not to get the tree in an invalid state.
        /// </summary>
        public override void Validate()
        {
            if (_root == null) return;

            AVLTree<T> newTree = new AVLTree<T>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                newTree.Add(node.Value);
                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }

            _root = newTree._root;

        }

        protected override bool CheckValidity()
        {
            if (_root == null) return true;

            return Math.Abs(GetBalance(_root)) < 2;
        }


        protected virtual void Balance(BinaryTreeNode<T> node, BinaryTreeNode<T> parent, int balance)
        {

            if (balance == 2) //right outweighs
            {
                int rightBalance = GetBalance(node.RightChild);

                if (rightBalance == 1 || rightBalance == 0)
                {
                    //Left rotation needed

                    RotateLeft(node, parent);
                }
                else if (rightBalance == -1)
                {

                    //Right rotation needed
                    RotateRight(node.RightChild, node);

                    //Left rotation needed
                    RotateLeft(node, parent);
                }
            }
            else if (balance == -2) //left outweighs
            {
                int leftBalance = GetBalance(node.LeftChild);

                if (leftBalance == 1)
                {
                    //Left rotation needed
                    RotateLeft(node.LeftChild, node);

                    //Right rotation needed
                    RotateRight(node, parent);
                }
                else if (leftBalance == -1 || leftBalance == 0)
                {
                    //Right rotation needed
                    RotateRight(node, parent);
                }
            }
        }


        protected virtual int GetBalance(BinaryTreeNode<T> node)
        {
            return CalculateDepth(node.RightChild) - CalculateDepth(node.LeftChild);
        }


        protected virtual void RotateLeft(BinaryTreeNode<T> node, BinaryTreeNode<T> parent)
        {
            if (node == null) return;

            BinaryTreeNode<T> pivot = node.RightChild;

            if (pivot == null) return;

            //Rotate
            node.RightChild = pivot.LeftChild;
            pivot.LeftChild = node;

            if (parent == null)
            {
                _root = pivot;
                return;
            }

            if (parent.LeftChild == node)
            {
                parent.LeftChild = pivot;
            }
            else
            {
                parent.RightChild = pivot;
            }
        }

        protected virtual void RotateRight(BinaryTreeNode<T> node, BinaryTreeNode<T> parent)
        {
            if (node == null) return;

            BinaryTreeNode<T> pivot = node.LeftChild;

            if (pivot == null) return;

            //Rotate
            node.LeftChild = pivot.RightChild;
            pivot.RightChild = node;

            if (parent == null)
            {
                _root = pivot;
                return;
            }

            //Update the original parent's child node
            if (parent.LeftChild == node)
            {
                parent.LeftChild = pivot;
            }
            else
            {
                parent.RightChild = pivot;
            }

        }

        protected override void RemoveNode(BinaryTreeNode<T> node, BinaryTreeNode<T> parent)
        {
            RemoveNode(node);
        }


    }
}
