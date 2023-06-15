using System;

public abstract class ActionNode<T> : ActionNodeBase
{
    public Func<T> operation; // The delegate representing the operation this instance of node performs


    // Function to evaluate this node's operation and produce a result
    public T Evaluate()
    {
        if (operation != null)
            return operation();
        else
            throw new Exception($"No operation for {GetType().Name} defined!");
    }
}
