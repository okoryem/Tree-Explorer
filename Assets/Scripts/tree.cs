using System;

public class TreeStructure {
    private Node root;

    public TreeStructure() {
        root = null;
    }

    public void Insert(CaveClass cave) {
        root = Insert(root, cave);
    }

    private Node Insert(Node parent, CaveClass cave) {
        if (parent == null) {
            return new Node(cave); // Root node has no parent
        }

        if (string.Compare(cave.cave.name, parent.cave.cave.name, StringComparison.Ordinal) < 0) {
            if (parent.left == null) {
                parent.left = new Node(cave, parent); // Set parent reference
            } else {
                Insert(parent.left, cave);
            }
        } else {
            if (parent.right == null) {
                parent.right = new Node(cave, parent); // Set parent reference
            } else {
                Insert(parent.right, cave);
            }
        }

        return parent;
    }

    public void InOrderTraversal() {
        InOrderTraversal(root);
    }

    private void InOrderTraversal(Node node) {
        if (node != null) {
            InOrderTraversal(node.left);
            Console.WriteLine(node.cave.cave.name); // Assuming the GameObject has a name
            InOrderTraversal(node.right);
        }
    }

    public Node GetRoot() {
        return root;
    }

    // Change Node class to public
    public class Node {
        public CaveClass cave;
        public Node left, right, parent; // Make parent public

        public Node(CaveClass cave, Node parent = null) {
            this.cave = cave;
            this.left = null;
            this.right = null;
            this.parent = parent;
        }
    }
}