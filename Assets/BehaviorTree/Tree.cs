using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        // The root node that in turn has all nodes as children.
        private Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if(root != null)
            {
                // checks evaluate all the time to implement.
                root.Evaluate();
            }

        }

        protected abstract Node SetupTree();

    }
}

