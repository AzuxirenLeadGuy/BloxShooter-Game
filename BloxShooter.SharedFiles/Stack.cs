namespace BloxShooter.SharedFiles
{
    internal class Node<T>
    {
        /// <summary>
        /// Reference to other linked nodes.
        /// </summary>
        internal Node<T> next,prev;
        /// <summary>
        /// The contained item.
        /// </summary>
        internal T item;
        /// <summary>
        /// Initializes the item only.
        /// </summary>
        /// <param name="t">Item to add</param>
        public Node(T t){item=t;next=null;prev = null; }
        /// <summary>
        /// The complete Constructor for initializing all the fields.
        /// </summary>
        /// <param name="t">Item to add</param>
        /// <param name="nex">The next node reference</param>
        /// <param name="pre">The Previous node reference</param>
        public Node(T t, Node<T> nex,Node<T> pre)
        {
            item = t;
            next = nex;
            prev = pre;
            if (next != null) { next.prev = this; }
            if (prev != null) { prev.next = this; }
        }
        /// <summary>
        /// Add Such that the called node is the next node of the added node.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>The reference of the added node</returns>
        public Node<T> InsertBefore(T item) => new Node<T>(item, this, prev);
        /// <summary>
        /// Add such that the called node is the previous node of the added node.
        /// </summary>
        /// <param name="item">Items to Add</param>
        /// <returns>Reference of the added node</returns>
        public Node<T> InsertAfter(T item) => new Node<T>(item, next, this);
        /// <summary>
        /// Deletes the called node and return the reference of its replacement if any.
        /// </summary>
        /// <returns>The reference of the replacement, if any</returns>
        public Node<T> Delete()
        {
            if(prev!=null) prev.next = this.next;
            if (next != null) next.prev = this.prev;
            return this.next;
        }
    }
}