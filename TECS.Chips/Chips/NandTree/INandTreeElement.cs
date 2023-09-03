using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

internal interface INandTreeElement
{
    // Clone the node and the entire parent tree
    INandTreeElement Clone(long cloneId);
    
    // Get the value of this element
    bool GetValue(long clock);

    // Minimize tree size by fusing all pins upwards. Leaves output and inputs unchanged
    INandTreeElement Fuse(long fuseId);

    // Check if all parents are set except on inputs, if there are no cycles, etc
    void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun);

    // Check if an element was validated (otherwise it is not connected to the network)
    bool IsValidatedInRun(long validationRun);
}

internal interface ISettableElement : INandTreeElement
{
    // Check if a node is a valid input
    void SetAsInputForValidation(List<ValidationError> errors, long validationRun);
    
    // Set the value of this node
    void SetValue(bool value);
}

