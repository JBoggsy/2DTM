using System.Collections.Generic;
using UnityEngine;

using Constants;

namespace GameCore {
    public class GridData {
        protected const TM_Symbol DEFAULT_SYMBOL = TM_Symbol.OFF;

        protected Dictionary<Vector3Int, TM_Symbol> CellDictionary;

        public GridData() {
            CellDictionary = new Dictionary<Vector3Int, TM_Symbol>();
        }

        public void ResetGrid() {
            CellDictionary.Clear();
        }

        public TM_Symbol GetCellSymbol(Vector3Int loc) {
            _EnsureCellSymbol(loc);
            return CellDictionary[loc];
        }

        public void SetCellSymbol(Vector3Int loc, TM_Symbol symbol) {
            CellDictionary[loc] = symbol;
        }

        private void _EnsureCellSymbol(Vector3Int loc) {
            if (!CellDictionary.ContainsKey(loc)) {
                CellDictionary[loc] = DEFAULT_SYMBOL;
            }
        }
    }
}