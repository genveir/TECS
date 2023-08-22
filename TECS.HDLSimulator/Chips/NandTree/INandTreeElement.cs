using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

public interface INandTreeElement
{
    // Clone the node and the entire parent tree
    INandTreeElement Clone(long cloneId);
    
    // Get or set the value of this element
    bool Value { get; set; }

    // Minimize tree size by fusing all pins upwards. Leaves output and inputs unchanged
    INandTreeElement Fuse(long fuseId);
    
    // Count how many pins and nand nodes are in this tree
    (int pins, int nands) CountNodes(int countId);
    
    // Check if all parents are set except on inputs, if there are no cycles, etc
    void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun);
    
    // Check if a node is a valid input
    void SetAsInputForValidation(List<ValidationError> errors, long validationRun);

    // Check if an element was validated (otherwise it is not connected to the network)
    bool IsValidatedInRun(long validationRun);

}