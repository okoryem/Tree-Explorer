using Cave;
using System;

class TreeStructure {
    Node root;
    Random random;
    public TreeStructure() {
        //
        random = new Random();
    }

    public TreeStructure(Node root) {
        this.root = root;
    }

    public void insert(Node parent, Cave cave) {
        if (parent == null) {
            Node node = new Node(cave);
            parent = node;
            return;
        }

        int rand = random.Next();

        if (rand % 2 == 0) {
            insert(parent.left, cave);
        } else {
            insert(parent.right, cave);
        }
        return;
    }

    public void inOrder() {
        inOrder(root);
    }

    private void inOrder(Node node) {
        if (node != null) {
            inOrder(node.left);
            Console.WriteLine("" + node.cave.test);
            inOrder(node.right);
        }
    }


    class Node() {
        protected Cave cave;
        Node left, right;
        public Node(Cave cave) {
            this.cave = cave;
            left = null;
            right = null;
        }
    }
}

