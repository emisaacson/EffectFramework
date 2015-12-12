﻿using EffectFramework.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// Interval structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct Interval<T> where T : IComparable<T>
    {

        public T Start;
        public T End;

        public Interval(T start, T end)
            : this()
        {
            if (start.CompareTo(end) >= 0)
            {
                throw new ValidationFailedException(Strings.Start_Must_Be_Less_Than_End);
            }

            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Determines if two intervals overlap (i.e. if this interval starts before the other ends and it finishes after the other starts)
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if the specified other is overlapping; otherwise, <c>false</c>.
        /// </returns>
        public bool OverlapsWith(Interval<T> other)
        {
            return this.Start.CompareTo(other.End) < 0 && this.End.CompareTo(other.Start) > 0;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", this.Start.ToString(), this.End.ToString());
        }

    }

    /// <summary>
    /// Interval Tree class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IntervalTree<T, TypeValue> where T : IComparable<T> where TypeValue : class
    {        

        private int Count;
        private IntervalNode Root;
        private IComparer<T> comparer;
        private KeyValueComparer<T, TypeValue> keyvalueComparer;


        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;"/> class.
        /// </summary>
        public IntervalTree()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;"/> class.
        /// </summary>
        /// <param name="elems">The elems.</param>
        public IntervalTree(IEnumerable<KeyValuePair<Interval<T>, TypeValue>> elems)
        {
            if (elems != null)
            {
                foreach (var elem in elems)
                {
                    Add(elem.Key, elem.Value);
                }
            }
            this.comparer = ComparerUtil.GetComparer();
            this.keyvalueComparer = new KeyValueComparer<T, TypeValue>(this.comparer);
        }


        /// <summary>
        /// visitor delegate
        /// </summary>
        /// <typeparam name="TNode">The type of the node.</typeparam>
        /// <param name="node">The node.</param>
        /// <param name="level">The level.</param>
        private delegate void VisitNodeHandler<TNode>(TNode node, int level);

        /// <summary>
        /// Adds the specified interval.
        /// If there is more than one interval starting at the same time/value, the intervalnode.Interval stores the start time and the maximum end time of all intervals starting at the same value.
        /// All end values (except the maximum end time/value which is stored in the interval node itself) are stored in the Range list in decreasing order.
        /// Note: this is okay for problems where intervals starting at the same time /value is not a frequent occurrence, however you can use other data structure for better performance depending on your problem needs
        /// </summary>
        /// <param name="arg">The arg.</param>
        public void Add(T x, T y, TypeValue value)
        {
            Add(new Interval<T>(x, y), value);
        }

        /// <summary>
        /// Adds the specified interval.
        /// If there is more than one interval starting at the same time/value, the intervalnode.Interval stores the start time and the maximum end time of all intervals starting at the same value.
        /// All end values (except the maximum end time/value which is stored in the interval node itself) are stored in the Range list in decreasing order.
        /// Note: this is okay for problems where intervals starting at the same time /value is not a frequent occurrence, however you can use other data structure for better performance depending on your problem needs
        /// </summary>
        /// <param name="arg">The arg.</param>
        public bool Add(Interval<T> interval, TypeValue value)
        {
            bool wasAdded = false;
            bool wasSuccessful = false;

            this.Root = IntervalNode.Add(this.Root, interval, value, ref wasAdded, ref wasSuccessful);
            if (this.Root != null)
            {
                IntervalNode.ComputeMax(this.Root);
            }

            if (wasSuccessful)
            {
                this.Count++;
            }

            return wasSuccessful;
        }

        /// <summary>
        /// Deletes the specified interval.
        /// If the interval tree is used with unique intervals, this method removes the interval specified as an argument.
        /// If multiple identical intervals (starting at the same time and also ending at the same time) are allowed, this function will delete one of them( see procedure DeleteIntervalFromNodeWithRange for details)
        /// In this case, it is easy enough to either specify the (interval, value) pair to be deleted or enforce uniqueness by changing the Add procedure.
        /// </summary>
        /// <param name="arg">The arg.</param>
        public bool Delete(Interval<T> arg)
        {
            return Delete(arg, null);
        }

        public bool Delete(Interval<T> arg, TypeValue val)
        {
            if (this.Root != null)
            {
                bool wasDeleted = false;
                bool wasSuccessful = false;

                this.Root = IntervalNode.Delete(this.Root, arg, ref wasDeleted, ref wasSuccessful, val);
                if (this.Root != null)
                {                    
                    IntervalNode.ComputeMax(this.Root);
                }

                if (wasSuccessful)
                {
                    this.Count--;
                }
                return wasSuccessful;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Searches for all intervals overlapping the one specified.
        /// If multiple intervals starting at the same time/value are found to overlap the specified interval, they are returned in decreasing order of their End values.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <param name="list">The list.</param>
        public void GetIntervalsOverlappingWith(Interval<T> toFind, ref List<KeyValuePair<Interval<T>, TypeValue>> list)
        {
            if (this.Root != null)
            {
                this.Root.GetIntervalsOverlappingWith(toFind, ref list);
            }
        }

        /// <summary>
        /// Searches for all intervals overlapping the one specified.
        /// If multiple intervals starting at the same time/value are found to overlap the specified interval, they are returned in decreasing order of their End values.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsOverlappingWith(Interval<T> toFind)
        {
            return (this.Root != null) ? this.Root.GetIntervalsOverlappingWith(toFind) : null;
        }

        /// <summary>
        /// Returns all intervals beginning at the specified start value. 
        /// The multiple intervals start at the specified value, they are sorted based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns></returns>
        public List<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsStartingAt(T arg)
        {
            return IntervalNode.GetIntervalsStartingAt(this.Root, arg);
        }

        /// <summary>
        /// Gets the collection of intervals (in ascending order of their Start values).
        /// Those intervals starting at the same time/value are sorted further based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        public IEnumerable<Interval<T>> Intervals
        {
            get
            {
                if (this.Root == null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.Root);
                while (p != null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode.Key;
                    }

                    yield return p.Interval;
                    p = p.Successor();
                }
            }
        }

        /// <summary>
        /// Gets the collection of values (ascending order)
        /// Those intervals starting at the same time/value are sorted further based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        public IEnumerable<TypeValue> Values
        {
            get
            {
                if (this.Root == null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.Root);
                while (p != null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode.Value;
                    }

                    yield return p.Value;
                    p = p.Successor();
                }
            }
        }

        /// <summary>
        /// Gets the interval value pairs.
        /// Those intervals starting at the same time/value are sorted further based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> IntervalValuePairs
        {
            get
            {
                if (this.Root == null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.Root);
                while (p != null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode;
                    }

                    yield return new KeyValuePair<Interval<T>, TypeValue>(p.Interval, p.Value);
                    p = p.Successor();
                }
            }
        }

        /// <summary>
        /// Tries to the get the value associated with the interval.
        /// </summary>
        /// <param name="subtree">The subtree.</param>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetInterval(Interval<T> data, out TypeValue value)
        {
            return this.TryGetIntervalImpl(this.Root, data, out value);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.Root = null;
            this.Count = 0;
        }

        /// <summary>
        /// Prints this instance (to console).
        /// </summary>
        public void Print()
        {
            this.Visit((node, level) =>
            {
                Console.Write(new string(' ', 2 * level));
                Console.Write(string.Format("{0}.{1}", node.Interval.ToString(), node.Max));

                if (node.Range != null)
                {                    
                    Console.Write(" ... ");
                    foreach (var rangeNode in node.GetRange())
                    {
                        Console.Write(string.Format("{0}  ", rangeNode.Key));
                    }
                }

                Console.WriteLine();
            });
        }        

        /// <summary>
        /// Searches for interval starting at.
        /// </summary>
        /// <param name="subtree">The subtree.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private bool TryGetIntervalImpl(IntervalNode subtree, Interval<T> data, out TypeValue value)
        {
            if (subtree != null)
            {
                int compareResult = data.Start.CompareTo(subtree.Interval.Start);

                if (compareResult < 0)
                {
                    return this.TryGetIntervalImpl(subtree.Left, data, out value);
                }
                else if (compareResult > 0)
                {
                    return this.TryGetIntervalImpl(subtree.Right, data, out value);
                }
                else
                {
                    if (data.End.CompareTo(subtree.Interval.End) == 0)
                    {
                        value = subtree.Value;
                        return true;
                    }
                    else if (subtree.Range != null)
                    {
                        int kthIndex = subtree.Range.BinarySearch(new KeyValuePair<T, TypeValue>(data.End, default(TypeValue)), this.keyvalueComparer);
                        if (kthIndex >= 0)
                        {
                            value = subtree.Range[kthIndex].Value;
                            return true;
                        }
                    }
                }
            }
            value = default(TypeValue);
            return false;
        }

        /// <summary>
        /// Visit_inorders the specified visitor. Defined for debugging purposes only
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        private void Visit(VisitNodeHandler<IntervalNode> visitor)
        {
            if (this.Root != null)
            {
                this.Root.Visit(visitor, 0);
            }
        }        


        /// <summary>
        /// IntervalNode class. 
        /// </summary>
        /// <typeparam name="TElem">The type of the elem.</typeparam>
        [Serializable]
        private class IntervalNode
        {

            private IntervalNode Parent;


            public int Balance { get; private set; }
            public IntervalNode Left  { get; private set; }
            public IntervalNode Right { get; private set; }
            public Interval<T> Interval { get; private set; }
            public TypeValue Value { get; private set; }
            public List<KeyValuePair<T, TypeValue>> Range { get; private set; }
            public T Max { get;  private set; }


            public IntervalNode(Interval<T> interval, TypeValue value)
            {
                this.Left = null;
                this.Right = null;
                this.Balance = 0;
                this.Interval = interval;
                this.Value = value;
                this.Max = interval.End;
            }



            /// <summary>
            /// Adds the specified elem.
            /// </summary>
            /// <param name="elem">The elem.</param>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            public static IntervalNode Add(IntervalNode elem, Interval<T> interval, TypeValue value, ref bool wasAdded, ref bool wasSuccessful)
            {
                if (elem == null)
                {
                    elem = new IntervalNode(interval, value);
                    wasAdded = true;
                    wasSuccessful = true;
                }
                else
                {
                    int compareResult = interval.Start.CompareTo(elem.Interval.Start);
                    IntervalNode newChild = null;
                    if (compareResult < 0)
                    {
                        newChild = Add(elem.Left, interval, value, ref wasAdded, ref wasSuccessful);
                        if (elem.Left != newChild)
                        {
                            elem.Left = newChild;
                            newChild.Parent = elem;
                        }

                        if (wasAdded)
                        {
                            elem.Balance--;

                            if (elem.Balance == 0)
                            {
                                wasAdded = false;
                            }
                            else if (elem.Balance == -2)
                            {
                                if (elem.Left.Balance == 1)
                                {
                                    int elemLeftRightBalance = elem.Left.Right.Balance;

                                    elem.Left = RotateLeft(elem.Left);
                                    elem = RotateRight(elem);

                                    elem.Balance = 0;
                                    elem.Left.Balance = elemLeftRightBalance == 1 ? -1 : 0;
                                    elem.Right.Balance = elemLeftRightBalance == -1 ? 1 : 0;
                                }
                                else if (elem.Left.Balance == -1)
                                {
                                    elem = RotateRight(elem);
                                    elem.Balance = 0;
                                    elem.Right.Balance = 0;
                                }
                                wasAdded = false;
                            }
                        }
                    }
                    else if (compareResult > 0)
                    {
                        newChild = Add(elem.Right, interval, value, ref wasAdded, ref wasSuccessful);
                        if (elem.Right != newChild)
                        {
                            elem.Right = newChild;
                            newChild.Parent = elem;
                        }

                        if (wasAdded)
                        {
                            elem.Balance++;
                            if (elem.Balance == 0)
                            {
                                wasAdded = false;
                            }
                            else if (elem.Balance == 2)
                            {
                                if (elem.Right.Balance == -1)
                                {
                                    int elemRightLeftBalance = elem.Right.Left.Balance;

                                    elem.Right = RotateRight(elem.Right);
                                    elem = RotateLeft(elem);

                                    elem.Balance = 0;
                                    elem.Left.Balance = elemRightLeftBalance == 1 ? -1 : 0;
                                    elem.Right.Balance = elemRightLeftBalance == -1 ? 1 : 0;
                                }

                                else if (elem.Right.Balance == 1)
                                {
                                    elem = RotateLeft(elem);

                                    elem.Balance = 0;
                                    elem.Left.Balance = 0;
                                }
                                wasAdded = false;
                            }
                        }
                    }
                    else
                    {
                        //// if there are more than one interval starting at the same time/value, the intervalnode.Interval stores the start time and the maximum end time of all intervals starting at the same value.
                        //// all end values (except the maximum end time/value which is stored in the interval node itself) are stored in the Range list in decreasing order.
                        //// note: this is ok for problems where intervals starting at the same time /value is not a frequent occurrence, however you can use other data structure for better performance depending on your problem needs

                        elem.AddIntervalValuePair(interval, value);

                        wasSuccessful = true;
                    }
                    ComputeMax(elem);
                }

                return elem;
            }

            /// <summary>
            /// Computes the max.
            /// </summary>
            /// <param name="node">The node.</param>
            public static void ComputeMax(IntervalNode node)
            {
                T maxRange = node.Interval.End;

                if (node.Left == null && node.Right == null)
                {
                    node.Max = maxRange;
                }
                else if (node.Left == null)
                {
                    node.Max = (maxRange.CompareTo(node.Right.Max) >= 0) ? maxRange : node.Right.Max;
                }
                else if (node.Right == null)
                {
                    node.Max = (maxRange.CompareTo(node.Left.Max) >= 0) ? maxRange : node.Left.Max;
                }
                else
                {
                    T leftMax = node.Left.Max;
                    T rightMax = node.Right.Max;

                    if (leftMax.CompareTo(rightMax) >= 0)
                    {
                        node.Max = maxRange.CompareTo(leftMax) >= 0 ? maxRange : leftMax;
                    }
                    else
                    {
                        node.Max = maxRange.CompareTo(rightMax) >= 0 ? maxRange : rightMax;
                    }
                }
            }            

            /// <summary>
            /// Finds the min.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns></returns>
            public static IntervalNode FindMin(IntervalNode node)
            {
                while (node != null && node.Left != null)
                {
                    node = node.Left;
                }
                return node;
            }

            /// <summary>
            /// Finds the max.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns></returns>
            public static IntervalNode FindMax(IntervalNode node)
            {
                while (node != null && node.Right != null)
                {
                    node = node.Right;
                }
                return node;
            }

            /// <summary>
            /// Gets the range of intervals stored in this.Range (i.e. starting at the same value 'this.Interval.Start' as the interval stored in the node itself)
            /// The range intervals are sorted in the descending order of their End interval values
            /// </summary>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetRange()
            {
                if (this.Range != null)
                {
                    foreach (var value in this.Range)
                    {
                        var kth = new Interval<T>(this.Interval.Start, value.Key);
                        yield return new KeyValuePair<Interval<T>, TypeValue>(kth, value.Value);
                    }
                }
                else
                {
                    yield break;
                }
            }

            /// <summary>
            /// Gets the range of intervals stored in this.Range (i.e. starting at the same value 'this.Interval.Start' as the interval stored in the node itself).
            /// The range intervals are sorted in the ascending order of their End interval values
            /// </summary>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetRangeReverse()
            {
                if (this.Range != null && this.Range.Count > 0)
                {
                    int rangeCount = this.Range.Count;
                    for (int k = rangeCount - 1; k >= 0; k--)
                    {
                        var kth = new Interval<T>(this.Interval.Start, this.Range[k].Key);
                        yield return new KeyValuePair<Interval<T>, TypeValue>(kth, this.Range[k].Value);
                    }
                }
                else
                {
                    yield break;
                }
            }


            /// <summary>
            /// Succeeds this instance.
            /// </summary>
            /// <returns></returns>
            public IntervalNode Successor()
            {
                if (this.Right != null)
                    return FindMin(this.Right);
                else
                {
                    var p = this;
                    while (p.Parent != null && p.Parent.Right == p)
                    {
                        p = p.Parent;
                    }
                    return p.Parent;
                }
            }

            /// <summary>
            /// Precedes this instance.
            /// </summary>
            /// <returns></returns>
            public IntervalNode Predecesor()
            {
                if (this.Left != null)
                {
                    return FindMax(this.Left);
                }
                else
                {
                    var p = this;
                    while (p.Parent != null && p.Parent.Left == p)
                    {
                        p = p.Parent;
                    }
                    return p.Parent;
                }
            }

            /// <summary>
            /// Deletes the specified node.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="arg">The arg.</param>
            /// <returns></returns>
            public static IntervalNode Delete(IntervalNode node, Interval<T> arg, ref bool wasDeleted, ref bool wasSuccessful, TypeValue val)
            {
                int cmp = arg.Start.CompareTo(node.Interval.Start);
                IntervalNode newChild = null;

                if (cmp < 0)
                {
                    if (node.Left != null)
                    {
                        newChild = Delete(node.Left, arg, ref wasDeleted, ref wasSuccessful, val);
                        if (node.Left != newChild)
                        {
                            node.Left = newChild;
                        }

                        if (wasDeleted)
                        {
                            node.Balance++;
                        }
                    }
                }
                else if (cmp == 0)
                {
                    if (arg.End.CompareTo(node.Interval.End) == 0 && node.Range == null && (val == null || node.Value == val))
                    {
                        if (node.Left != null && node.Right != null)
                        {
                            var min = FindMin(node.Right);

                            var interval = node.Interval;
                            node.Swap(min);

                            wasDeleted = false;

                            newChild = Delete(node.Right, interval, ref wasDeleted, ref wasSuccessful, val);
                            if (node.Right != newChild)
                            {
                                node.Right = newChild;
                            }

                            if (wasDeleted)
                            {
                                node.Balance--;
                            }
                        }
                        else if (node.Left == null)
                        {
                            wasDeleted = true;
                            wasSuccessful = true;

                            if (node.Right != null)
                            {
                                node.Right.Parent = node.Parent;
                            }
                            return node.Right;
                        }
                        else
                        {
                            wasDeleted = true;
                            wasSuccessful = true;
                            if (node.Left != null)
                            {
                                node.Left.Parent = node.Parent;
                            }
                            return node.Left;
                        }
                    }
                    else
                    {
                        wasSuccessful = node.DeleteIntervalFromNodeWithRange(arg, val);
                    }
                }
                else
                {
                    if (node.Right != null)
                    {
                        newChild = Delete(node.Right, arg, ref wasDeleted, ref wasSuccessful, val);
                        if (node.Right != newChild)
                        {
                            node.Right = newChild;
                        }

                        if (wasDeleted)
                        {
                            node.Balance--;
                        }
                    }
                }

                ComputeMax(node);

                if (wasDeleted)
                {
                    if (node.Balance == 1 || node.Balance == -1)
                    {
                        wasDeleted = false;
                        return node;
                    }
                    else if (node.Balance == -2)
                    {
                        if (node.Left.Balance == 1)
                        {
                            int leftRightBalance = node.Left.Right.Balance;

                            node.Left = RotateLeft(node.Left);
                            node = RotateRight(node);

                            node.Balance = 0;
                            node.Left.Balance = (leftRightBalance == 1) ? -1 : 0;
                            node.Right.Balance = (leftRightBalance == -1) ? 1 : 0;
                        }
                        else if (node.Left.Balance == -1)
                        {
                            node = RotateRight(node);
                            node.Balance = 0;
                            node.Right.Balance = 0;
                        }
                        else if (node.Left.Balance == 0)
                        {
                            node = RotateRight(node);
                            node.Balance = 1;
                            node.Right.Balance = -1;

                            wasDeleted = false;
                        }
                    }
                    else if (node.Balance == 2)
                    {
                        if (node.Right.Balance == -1)
                        {
                            int rightLeftBalance = node.Right.Left.Balance;

                            node.Right = RotateRight(node.Right);
                            node = RotateLeft(node);

                            node.Balance = 0;
                            node.Left.Balance = (rightLeftBalance == 1) ? -1 : 0;
                            node.Right.Balance = (rightLeftBalance == -1) ? 1 : 0;
                        }
                        else if (node.Right.Balance == 1)
                        {
                            node = RotateLeft(node);
                            node.Balance = 0;
                            node.Left.Balance = 0;
                        }
                        else if (node.Right.Balance == 0)
                        {
                            node = RotateLeft(node);
                            node.Balance = -1;
                            node.Left.Balance = 1;

                            wasDeleted = false;
                        }
                    }
                }

                return node;
            }

            /// <summary>
            /// Returns all intervals beginning at the specified start value. The intervals are sorted based on their End value (i.e. returned in ascending order of their End values)
            /// </summary>
            /// <param name="subtree">The subtree.</param>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            public static List<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsStartingAt(IntervalNode subtree, T start)
            {
                if (subtree != null)
                {
                    int compareResult = start.CompareTo(subtree.Interval.Start);
                    if (compareResult < 0)
                    {
                        return GetIntervalsStartingAt(subtree.Left, start);
                    }
                    else if (compareResult > 0)
                    {
                        return GetIntervalsStartingAt(subtree.Right, start);
                    }
                    else
                    {
                        var result = new List<KeyValuePair<Interval<T>, TypeValue>>();
                        if (subtree.Range != null)
                        {
                            foreach (var kvp in subtree.GetRangeReverse())
                            {
                                result.Add(kvp);
                            }
                        }
                        result.Add(new KeyValuePair<Interval<T>, TypeValue>(subtree.Interval, subtree.Value));
                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Searches for all intervals in this subtree that are overlapping the argument interval.
            /// If multiple intervals starting at the same time/value are found to overlap, they are returned in decreasing order of their End values.
            /// </summary>
            /// <param name="toFind">To find.</param>
            /// <param name="list">The list.</param>
            public void GetIntervalsOverlappingWith(Interval<T> toFind, ref List<KeyValuePair<Interval<T>, TypeValue>> list)
            {
                if (toFind.End.CompareTo(this.Interval.Start) <= 0)
                {
                    ////toFind ends before subtree.Data begins, prune the right subtree
                    if (this.Left != null)
                    {
                        this.Left.GetIntervalsOverlappingWith(toFind, ref list);
                    }
                }
                else if (toFind.Start.CompareTo(this.Max) >= 0)
                {
                    ////toFind begins after the subtree.Max ends, prune the left subtree
                    if (this.Right != null)
                    {
                        this.Right.GetIntervalsOverlappingWith(toFind, ref list);
                    }
                }
                else
                {
                    //// search the left subtree
                    if (this.Left != null)
                    {
                        this.Left.GetIntervalsOverlappingWith(toFind, ref list);
                    }

                    if (this.Interval.OverlapsWith(toFind))
                    {
                        if (list == null)
                        {
                            list = new List<KeyValuePair<Interval<T>, TypeValue>>();
                        }

                        list.Add(new KeyValuePair<Interval<T>, TypeValue>(this.Interval, this.Value));

                        ////the max value is stored in the node, if the node doesn't overlap then neither are the nodes in its range 
                        if (this.Range != null && this.Range.Count > 0)
                        {
                            int rangeCount = this.Range.Count;
                            foreach (var kvp in this.GetRange())
                            {
                                if (kvp.Key.OverlapsWith(toFind))
                                {
                                    if (list == null)
                                    {
                                        list = new List<KeyValuePair<Interval<T>, TypeValue>>();
                                    }
                                    list.Add(kvp);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    //// search the right subtree
                    if (this.Right != null)
                    {
                        this.Right.GetIntervalsOverlappingWith(toFind, ref list);
                    }
                }
            }

            /// <summary>
            /// Gets all intervals in this subtree that are overlapping the argument interval. 
            /// If multiple intervals starting at the same time/value are found to overlap, they are returned in decreasing order of their End values.
            /// </summary>
            /// <param name="toFind">To find.</param>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsOverlappingWith(Interval<T> toFind)
            {
                if (toFind.End.CompareTo(this.Interval.Start) <= 0)
                {
                    ////toFind ends before subtree.Data begins, prune the right subtree
                    if (this.Left != null)
                    {
                        foreach (var value in this.Left.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
                else if (toFind.Start.CompareTo(this.Max) >= 0)
                {
                    ////toFind begins after the subtree.Max ends, prune the left subtree
                    if (this.Right != null)
                    {
                        foreach (var value in this.Right.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
                else
                {
                    if (this.Left != null)
                    {
                        foreach (var value in this.Left.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }

                    if (this.Interval.OverlapsWith(toFind))
                    {
                        yield return new KeyValuePair<Interval<T>, TypeValue>(this.Interval, this.Value);

                        if (this.Range != null && this.Range.Count > 0)
                        {
                            foreach (var kvp in this.GetRange())
                            {
                                if (kvp.Key.OverlapsWith(toFind))
                                {
                                    yield return kvp;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (this.Right != null)
                    {
                        foreach (var value in this.Right.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
            }

            public void Visit(VisitNodeHandler<IntervalNode> visitor, int level)
            {
                if (this.Left != null)
                {
                    this.Left.Visit(visitor, level + 1);
                }

                visitor(this, level);

                if (this.Right != null)
                {
                    this.Right.Visit(visitor, level + 1);
                }
            }

            /// <summary>
            /// Rotates lefts this instance.
            /// Assumes that this.Right != null
            /// </summary>
            /// <returns></returns>
            private static IntervalNode RotateLeft(IntervalNode node)
            {
                var right = node.Right;
                Debug.Assert(node.Right != null);

                var rightLeft = right.Left;

                node.Right = rightLeft;
                ComputeMax(node);

                var parent = node.Parent;
                if (rightLeft != null)
                {
                    rightLeft.Parent = node;
                }
                right.Left = node;
                ComputeMax(right);

                node.Parent = right;
                if (parent != null)
                {
                    if (parent.Left == node)
                    {
                        parent.Left = right;
                    }
                    else
                    {
                        parent.Right = right;
                    }
                }
                right.Parent = parent;
                return right;
            }

            /// <summary>
            /// Rotates right this instance.
            /// Assumes that (this.Left != null)
            /// </summary>
            /// <returns></returns>
            private static IntervalNode RotateRight(IntervalNode node)
            {
                var left = node.Left;
                Debug.Assert(node.Left != null);

                var leftRight = left.Right;
                node.Left = leftRight;
                ComputeMax(node);

                var parent = node.Parent;
                if (leftRight != null)
                {
                    leftRight.Parent = node;
                }
                left.Right = node;
                ComputeMax(left);

                node.Parent = left;
                if (parent != null)
                {
                    if (parent.Left == node)
                    {
                        parent.Left = left;
                    }
                    else
                    {
                        parent.Right = left;
                    }
                }
                left.Parent = parent;
                return left;
            }

            /// <summary>
            /// Deletes the specified interval from this node. 
            /// If the interval tree is used with unique intervals, this method removes the interval specified as an argument.
            /// If multiple identical intervals (starting at the same time and also ending at the same time) are allowed, this function will delete one of them. 
            /// In this case, it is easy enough to either specify the (interval, value) pair to be deleted or enforce uniqueness by changing the Add procedure.
            /// </summary>
            /// <param name="interval">The interval to be deleted.</param>
            /// <param name="val">optional value to delete, or null</param>
            /// <returns></returns>
            private bool DeleteIntervalFromNodeWithRange(Interval<T> interval, TypeValue val)
            {
                bool Found = false;
                if (this.Range != null && this.Range.Count > 0)
                {
                    int rangeCount = this.Range.Count;
                    int intervalPosition = -1;

                    // find the exact interval to delete based on its End value.
                    if (interval.End.CompareTo(this.Interval.End) == 0)
                    {
                        intervalPosition = 0;
                    }
                    else if (rangeCount > 12)
                    {
                        var keyvalueComparer = new KeyValueComparer<T, TypeValue>(ComparerUtil.GetComparer());
                        int k = this.Range.BinarySearch(new KeyValuePair<T, TypeValue>(interval.End, default(TypeValue)), keyvalueComparer);
                        if (k >= 0)
                        {
                            intervalPosition = k + 1;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < rangeCount; k++)
                        {
                            if (interval.End.CompareTo(this.Range[k].Key) == 0)
                            {
                                intervalPosition = k + 1;
                                break;
                            }
                        }
                    }

                    if (intervalPosition < 0)
                    {
                        return false;
                    }

                    if (val == null)
                    {
                        if (intervalPosition == 0)
                        {
                            this.Interval = new Interval<T>(this.Interval.Start, this.Range[0].Key);
                            this.Value = this.Range[0].Value;
                            this.Range.RemoveAt(0);
                        }
                        else if (intervalPosition > 0)
                        {
                            this.Range.RemoveAt(intervalPosition - 1);
                        }
                        Found = true;
                    }
                    else
                    {
                        if (intervalPosition == 0)
                        {
                            if (this.Value == val)
                            {
                                this.Interval = new Interval<T>(this.Interval.Start, this.Range[0].Key);
                                this.Value = this.Range[0].Value;
                                this.Range.RemoveAt(0);
                                Found = true;
                            }
                            else
                            {
                                if (this.Range[0].Key.CompareTo(interval.End) == 0)
                                {
                                    intervalPosition = 1;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }

                        if (!Found)
                        {
                            // rewind and check for particular val
                            while (intervalPosition > 1 && this.Range[intervalPosition].Key.CompareTo(interval.End) == 0)
                            {
                                intervalPosition--;
                            }
                            for (int i = intervalPosition; i < this.Range.Count && this.Range[i].Key.CompareTo(interval.End) == 0; i++)
                            {
                                if (this.Range[i].Value == val)
                                {
                                    this.Range.RemoveAt(i);
                                    Found = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (this.Range.Count == 0)
                    {
                        this.Range = null;
                    }

                    return Found;
                }
                else
                {
                    ////if interval end was not found in the range (or the node itself) or if the node doesnt have a range, return false
                    return false;
                }
            }            

            private void Swap(IntervalNode node)
            {
                var dataInterval = this.Interval;
                var dataValue = this.Value;
                var dataRange = this.Range;

                this.Interval = node.Interval;
                this.Value = node.Value;
                this.Range = node.Range;

                node.Interval = dataInterval;
                node.Value = dataValue;
                node.Range = dataRange;
            }

            private void AddIntervalValuePair(Interval<T> interval, TypeValue value)
            {
                if (this.Range == null)
                {
                    this.Range = new List<KeyValuePair<T, TypeValue>>();
                }

                ////always store the max End value in the node.Data itself .. store the Range list in decreasing order
                if (interval.End.CompareTo(this.Interval.End) > 0)
                {
                    this.Range.Insert(0, new KeyValuePair<T, TypeValue>(this.Interval.End, this.Value));
                    this.Interval = interval;
                    this.Value = value;
                }
                else
                {
                    bool wasAdded = false;
                    for (int i = 0; i < this.Range.Count; i++)
                    {
                        if (interval.End.CompareTo(this.Range[i].Key) >= 0)
                        {
                            this.Range.Insert(i, new KeyValuePair<T, TypeValue>(interval.End, value));
                            wasAdded = true;
                            break;
                        }
                    }
                    if (!wasAdded)
                    {
                        this.Range.Add(new KeyValuePair<T, TypeValue>(interval.End, value));
                    }
                }
            }

        }

        [Serializable]
        private class KeyValueComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        {
            private IComparer<TKey> keyComparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;.KeyValueComparer&lt;TKey, TValue&gt;"/> class.
            /// </summary>
            /// <param name="keyComparer">The key comparer.</param>
            public KeyValueComparer(IComparer<TKey> keyComparer)
            {
                this.keyComparer = keyComparer;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero is less than y.Zerox equals y.Greater than zero is greater than y.
            /// </returns>
            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return (-1) * this.keyComparer.Compare(x.Key, y.Key);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj is KeyValueComparer<TKey, TValue>)
                {
                    return object.Equals(this.keyComparer, ((KeyValueComparer<TKey, TValue>)obj).keyComparer);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return this.keyComparer.GetHashCode();
            }
        }

        public static class ComparerUtil
        {
            public static IComparer<T> GetComparer()
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(System.IComparable).IsAssignableFrom(typeof(T)))
                {
                    return Comparer<T>.Default;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("The type {0} cannot be compared. It must implement IComparable<T> or IComparable", typeof(T).FullName));
                }
            }
        }

    }
}
