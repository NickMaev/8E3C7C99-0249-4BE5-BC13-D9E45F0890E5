using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Triangle.Core
{        
    /// <summary>
    /// Represents location in the triangle with traveled path.
    /// </summary>
    public class SavedLocation
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        
        /// <summary>
        /// Traveled path.
        /// </summary>
        public List<int> Path { get; set; }
    }
}
