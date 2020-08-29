using System.Collections.Generic;
using UnityEngine;

using Constants;

namespace GameCore {
    /// <summary>
    /// The <see cref="GridData"/> class maintains the current state of the grid.
    /// 
    /// The grid is the two-dimensional equivalent of a Turing machine tape. As
    /// such, Turing machines operating on the grid can read a symbol off of the
    /// grid cell they are on, can write a new symbol to the grid cell, and can
    /// move in one of the four cardinal directions. This class exists to model
    /// the grid in simulation, and has no connection with actually displaying
    /// the grid to the user. For that, see 
    /// <see cref="MonoBehaviours.TilemapMonobehavior"/>.
    /// </summary>
    /// <seealso cref="MonoBehaviours.TilemapMonobehavior"/>
    /// <seealso cref="TuringMachines.TuringMachine"/>
    /// <seealso cref="GameMaster"/>
    public class GridData {
        /// <value>
        /// The default symbol every grid cell is initialized with.
        /// </value>
        protected const TM_Symbol DEFAULT_SYMBOL = TM_Symbol.OFF;

        /// <value>
        /// Holds the mapping between pairs of (row, col) grid coordinates and
        /// symbols. Each (row, col) coordinate has at most one corresponding
        /// symbol.
        /// </value>
        protected Dictionary<Vector3Int, TM_Symbol> CellDictionary;

        /// <summary>
        /// The default constructor. Creates a grid populated with cells
        /// containing the <see cref="DEFAULT_SYMBOL"/>.
        /// </summary>
        public GridData() {
            CellDictionary = new Dictionary<Vector3Int, TM_Symbol>();
        }

        /// <summary>
        /// Resets the grid by calling the <see cref="Dictionary{TKey, TValue}.Clear"/>
        /// method.
        /// </summary>
        public void ResetGrid() {
            CellDictionary.Clear();
        }

        /// <summary>
        /// Retrieve the symbol written to a specific cell.
        /// </summary>
        /// <param name="loc">
        /// The (row, col) coordinate of the cell to get the symbol of.
        /// </param>
        /// <returns>
        /// The <see cref="TM_Symbol"/> written to the cell at <c>loc</c>.
        /// </returns>
        public TM_Symbol GetCellSymbol(Vector3Int loc) {
            _EnsureCellSymbol(loc);
            return CellDictionary[loc];
        }

        /// <summary>
        /// Sets the symbol of a cell to the given symbol. Completely overwrites
        /// the previous symbol.
        /// </summary>
        /// <param name="loc">The (row, col) coordinate of the cell to write to.</param>
        /// <param name="symbol">The new <see cref="TM_Symbol"/> the cell should contain.</param>
        public void SetCellSymbol(Vector3Int loc, TM_Symbol symbol) {
            CellDictionary[loc] = symbol;
        }

        /// <summary>
        /// Ensures that the given cell has a symbol written to it.
        /// 
        /// This is used prior to retrieving the value of an arbitrary cell,
        /// and allows us to use previously unaccessed dictionary slots as if
        /// they always were the <see cref="DEFAULT_SYMBOL"/>.
        /// </summary>
        /// <param name="loc">The (row, col) coordinate of the cell to ensure.</param>
        private void _EnsureCellSymbol(Vector3Int loc) {
            if (!CellDictionary.ContainsKey(loc)) {
                CellDictionary[loc] = DEFAULT_SYMBOL;
            }
        }
    }
}