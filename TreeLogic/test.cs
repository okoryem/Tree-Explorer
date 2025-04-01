using System;
using TreeStructure;
using Cave;

class Program {
    static void Main(string[] args) {
        Cave root = new Cave(1);
        TreeStructure tree = new TreeStructure(root);

        for (int i = 2; i < 14; i++) {
            tree.insert(new Cave(i));
        }

        tree.inOrder();
    }
}