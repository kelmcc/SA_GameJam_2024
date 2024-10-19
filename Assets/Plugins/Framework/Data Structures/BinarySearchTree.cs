using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// A special variation of a binary tree that satisfies the condition that every node in a node's left subtree must be less than or equal to the node itself, and all nodes in the right subtree must be greater. This makes searching the tree for a specific item very quick.
    /// </summary>
    /// <typeparam name="T">The type of item to store in each node</typeparam>
    public class BinarySearchTree<T> : BinaryTree<T> where T : IComparable<T>
    {

        /// <summary>
        /// Whether or not the tree satisfies its ordering rules. If it does not, it should be validated.
        /// </summary>
        public bool IsValidTree => CheckValidity();

        /// <summary>
        /// Creates an empty binary search tree. It will have no nodes.
        /// </summary>
        public BinarySearchTree()
        {
            _root = null;
        }

        /// <summary>
        /// Creates a new binary search tree with a single node: the root.
        /// </summary>
        /// <param name="root">The value to store in the root</param>
        public BinarySearchTree(T root)
        {
            _root = new BinaryTreeNode<T>(root);
        }

        /// <summary>
        /// Creates a new binary search tree from an existing one. Just be aware that nodes are mutable reference types.
        /// </summary>
        /// <param name="root">The node of the other tree that will form the root of this one.</param>
        public BinarySearchTree(BinaryTreeNode<T> root)
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

            BinaryTreeNode<T> node = _root;
            while (node != null)
            {
                if (item.CompareTo(node.Value) < 1)
                {
                    if (node.HasLeftChild)
                    {
                        node = node.LeftChild;
                    }
                    else
                    {
                        node.LeftChild = new BinaryTreeNode<T>(item);
                        return;
                    }
                }
                else
                {
                    if (node.HasRightChild)
                    {
                        node = node.RightChild;
                    }
                    else
                    {
                        node.RightChild = new BinaryTreeNode<T>(item);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a node containing the item in it's rightful position in the tree.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Insert(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Checks whether or not the tree has a node containing a specfic item.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if a node containing the item is found</returns>
        public override bool Contains(T item)
        {
            BinaryTreeNode<T> node = _root;
            while (node != null)
            {

                if (node.Value.Equals(item)) return true;

                if (item.CompareTo(node.Value) < 1)
                {
                    node = node.LeftChild;
                }
                else
                {
                    node = node.RightChild;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the first node in the tree that contains a specific item, by using a binary search.
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The first node that contains the item. Null if no such node is found</returns>
        public virtual BinaryTreeNode<T> Find(T item)
        {
            BinaryTreeNode<T> node = _root;
            while (node != null)
            {

                if (node.Value.Equals(item)) return node;

                if (item.CompareTo(node.Value) < 1)
                {
                    node = node.LeftChild;
                }
                else
                {
                    node = node.RightChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the first node that contains a specific item. Uses a binary search.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if a node was found and removed</returns>
        public override bool Remove(T item)
        {
            if (_root == null) return false;

            BinaryTreeNode<T> node = _root;
            BinaryTreeNode<T> parent = null;

            while (node != null)
            {

                if (node.Value.Equals(item)) break;

                if (item.CompareTo(node.Value) < 1)
                {
                    parent = node;
                    node = node.LeftChild;
                }
                else
                {
                    parent = node;
                    node = node.RightChild;
                }
            }

            if (node != null)
            {
                RemoveNode(node, parent);
                return true;
            }
            return false;

        }

        /// <summary>
        /// Ensures that the tree is properly ordered. It is necessary to call this if you manually alter nodes in the tree, which is something you should avoid doing. Note that this is potentially expensive as it basically rebuilds the tree from scratch, rather try not to get the tree in an invalid state.
        /// </summary>
        public virtual void Validate()
        {
            if (_root == null) return;

            BinarySearchTree<T> newTree = new BinarySearchTree<T>();
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

        protected virtual bool CheckValidity()
        {
            if (_root == null) return true;

            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();


                if (node.HasLeftChild && node.LeftChild.Value.CompareTo(node.Value) > 0)
                {
                    return false;
                }

                if (node.HasRightChild && node.RightChild.Value.CompareTo(node.Value) < 1)
                {
                    return false;
                }

                if (node.HasLeftChild) queue.Enqueue(node.LeftChild);
                if (node.HasRightChild) queue.Enqueue(node.RightChild);
            }
            return true;
        }


    }
}



