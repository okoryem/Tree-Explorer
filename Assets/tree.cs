using System;

class TreeStructure {
    private Node root;
    private Random random;

    public TreeStructure() {
        random = new Random();
        root = null;
    }

    /*
    public TreeStructure(Node root) {
        this.root = root;
        random = new Random();
    }
    */

    public void Insert(CaveClass cave) {
        root = Insert(root, cave);
    }

    private Node Insert(Node parent, CaveClass cave) {
        if (parent == null) {
            return new Node(cave);
        }

        if (random.Next() % 2 == 0) {
            parent.left = Insert(parent.left, cave);
        } else {
            parent.right = Insert(parent.right, cave);
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

    private class Node {
        public CaveClass cave;
        public Node left, right;

        public Node(CaveClass cave) {
            this.cave = cave;
            left = null;
            right = null;
        }
    }
}