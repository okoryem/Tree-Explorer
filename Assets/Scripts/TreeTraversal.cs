using System;
using System.Collections.Generic;

public static class TreeTraversal {
    public static void BFS(TreeStructure tree) {
        if (tree == null || tree.GetRoot() == null) return;

        Queue<TreeStructure.Node> queue = new Queue<TreeStructure.Node>();
        queue.Enqueue(tree.GetRoot());

        while (queue.Count > 0) {
            TreeStructure.Node current = queue.Dequeue();
            Console.WriteLine(current.cave.cave.name); // Process the node

            if (current.left != null) queue.Enqueue(current.left);
            if (current.right != null) queue.Enqueue(current.right);
        }
    }

    public static void DFS(TreeStructure tree) {
        if (tree == null || tree.GetRoot() == null) return;

        Stack<TreeStructure.Node> stack = new Stack<TreeStructure.Node>();
        stack.Push(tree.GetRoot());

        while (stack.Count > 0) {
            TreeStructure.Node current = stack.Pop();
            Console.WriteLine(current.cave.cave.name); // Process the node

            // Push right child first so that left child is processed first
            if (current.right != null) stack.Push(current.right);
            if (current.left != null) stack.Push(current.left);
        }
    }
}