using System;
using System.Collections;
using System.Collections.Generic;



namespace BehaviorTree
{

    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE

    }


    public class Node
    {
        //protected makes it so if a class derives from Node class they can reach and change this.
        protected NodeState state;

        // we can use parent and children to determine where in the branch we are.
        public Node parent;
        protected List<Node> children = new List<Node>();

        //constructor of node
        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children) //if you want to create a node with children
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node child)
        {
            //sets the nodes childs and parent
            child.parent = this;
            children.Add(child);
        }

        // Evaluate will be used differently by every node to define unique action
        // that's the reason its a virtual. This code ensures if no custom Evaluate is 
        // issued nodestate is failure by default.
        // Virtual also makes sure inhariting classes also run it.
        public virtual NodeState Evaluate() => NodeState.FAILURE;


        // This dictionary will take care of our shared data. We use Object type so its pretty flexible.
        // It has getters and setters.
        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        // Get data will check all nodes so it has recursiveness to it.
        public object GetData(string key)
        {
            object value = null;
            if(dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node node = parent;

            return parent.GetData(key);
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            if(parent != null)
            {
                parent.ClearData(key);
            }
            return false;

        }

    }
}
