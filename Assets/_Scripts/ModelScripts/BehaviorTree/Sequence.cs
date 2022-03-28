using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        // Constructors for sequence that inherint from base aka node class
        public Sequence() : base() { }
        
        public Sequence(List<Node> children) : base(children) { }


        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node child in children)
            {
                // child node success = continue to next
                // any child node is fail = fail
                // any child node is running = returns running
                // this returns success if no child is running or failed so if all is success.

                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;


                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;


        }

    }

}
