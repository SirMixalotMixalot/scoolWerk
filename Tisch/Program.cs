using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
namespace Tisch 
{
    //A couple constants to help construct the table 
    /*
          ┌━━━━━━━━━━┬━━━━━━━┐ 
          │   Ages   │ Names │
          ├━━━━━━━━━━┼━━━━━━━┤
          │    14    │ David │
          ├━━━━━━━━━━┼━━━━━━━┤
          |    20    | Mark  |
          └━━━━━━━━━━┴━━━━━━━┘
    */
          
    public static class TABLE_CHARS {
    
        public const char LEFT_TOP_EDGE     = '┌';
        public const char RIGHT_TOP_EDGE    = '┐';

        public const char LEFT_BOTTOM_EDGE  = '└';
        public const char RIGHT_BOTTOM_EDGE = '┘';

        public const char TOP_COL_DIV       = '┬';
        public const char BOTTOM_COL_DIV    = '┴';

        public const char MID_ROW_COL_DIV   = '┼';

        public const char LEFT_MID_EDGE     = '├';
        public const char RIGHT_MID_EDGE    = '┤';
        public const char ROW_LINE          = '━';
        public const char COL_LINE          = '│';
    }

    public class Table {
        //Need to represent
        //  1. Padding that should be added to each column
        //  2. Maximum length of individual cell
        //  3. The number of rows
        //All of this info will be encapsulated in the TableInfo struct
        private struct TableInfo {
            public int lenOfRow;
            public int[] maxLenCell;
            public int rowNums;
            public readonly string[] colHeadings;

            public TableInfo(string[] cols) {
                colHeadings = cols;
                lenOfRow    = 0;
                //All members initialized to 0 so I don't have to
                maxLenCell = new int[cols.Length];
                rowNums     = 0;
                //Set the initial cell length to that of the column heading
                for (int i = 0; i < cols.Length;i++) {
                    maxLenCell[i] = colHeadings[i].Length;
                }
            }

        }   
        private TableInfo _tableInfo;
        //Stores the columns 
        //so 0th index is first column
        private List<String>[] _internalTable;
        private int _currentCol;
        private bool[] _colHasData;
        //Creates a table with the given headings, and appropriate number of columns
        public Table(params string[] colHeadings) {
            var len = colHeadings.Length;
            _currentCol  = 0;
            _internalTable = new List<String>[len];
            _tableInfo = new TableInfo(colHeadings);
            _colHasData = new bool[colHeadings.Length];

        }
        private void PadColsToLength(int newColLength) {
            _tableInfo.rowNums = newColLength;
            for (int i = 0; i < _internalTable.Length; i++)
            {
                if(_colHasData[i]) {
                    for(int j = 0; j < newColLength - _internalTable[i].Count; j++)
                        _internalTable[i].Append(" "); 
                }

            }

        }
        //Adds data to the specified column, returns false if columnName could not be found
        public bool AddColData(string colunmName,string[] col) {
            _currentCol = Array.IndexOf(_tableInfo.colHeadings, colunmName);
            if(_currentCol < 0) {
                return false;
            }
            //Console.WriteLine($"{colunmName} was found at index {_currentCol}");
            //If we change the number of rows, we should do that for every column before this new
            //one
            var mustAddMore = false;
            if(_tableInfo.rowNums < col.Length) {
                PadColsToLength(col.Length);
            }else {
                mustAddMore = true;
            }
            
            _internalTable[_currentCol] = (new List<String>(_tableInfo.rowNums));

            for (int i = 0; i < col.Length; i++) {
                string strRepr = col[i];
                
                //Updating the max length of a column for formatting purposes later on
                _tableInfo.maxLenCell[_currentCol] = Math.Max(_tableInfo.maxLenCell[_currentCol], strRepr.Length);
                _internalTable[_currentCol].Add(strRepr);
            }
            //If we have to add more elements to the current column, add empty strings
            if(mustAddMore) {
                for (int i = 0; i < (_tableInfo.rowNums - col.Length); i++) {
                    _internalTable[_currentCol].Add(" ");
                }
            }
            _colHasData[_currentCol++] = true;
            return true;
        }
        //TODO:Add other types of Padding, such as Left, Right
        //  Idea, map each column to a padding function
        private string PadCentred(string item, int col, char padChar) {
            
            var diff = _tableInfo.maxLenCell[col] - item.Length;
          
            if(diff == 0) 
                return item;
            var n = diff / 2;
          
            
            var padStrFront = new String(padChar,n);
            var padStrBack  = diff % 2 == 0? padStrFront : new String(padChar, _tableInfo.maxLenCell[col] - (n + item.Length)) ;
            return $"{padStrFront}{item}{padStrBack}"; 

        }
        private StringBuilder AppendRecord(int row, bool areheadings) {
            var table = new StringBuilder();
            char topLeft = TABLE_CHARS.LEFT_MID_EDGE;
            char topRight = TABLE_CHARS.RIGHT_MID_EDGE;
            char divider = TABLE_CHARS.MID_ROW_COL_DIV;
            
            if(areheadings) {
                topLeft = TABLE_CHARS.LEFT_TOP_EDGE;
                topRight = TABLE_CHARS.RIGHT_TOP_EDGE;
                divider  = TABLE_CHARS.TOP_COL_DIV;
            }
            

            // Setup top of record
            table.Append(topLeft);
            for (int i = 0; i < _tableInfo.colHeadings.Length; i++) {
                table.Append(PadCentred(new String(TABLE_CHARS.ROW_LINE,1),i,TABLE_CHARS.ROW_LINE));
                if (i == _tableInfo.colHeadings.Length - 1) 
                    table.Append(topRight);
                else
                    table.Append(divider);
            }
            table.AppendLine();
            //Actually add data
            table.Append(TABLE_CHARS.COL_LINE);
            for (int i = 0; i < _tableInfo.colHeadings.Length; i++) {
                var current_data = !areheadings? _internalTable[i][row] : _tableInfo.colHeadings[i];
                var padded_col = PadCentred(current_data,i, ' ');
                table.Append(padded_col);
                table.Append(TABLE_CHARS.COL_LINE);
            }
            table.AppendLine();
            //Last row so we should draw bottom
            if (row == _tableInfo.rowNums - 1) {
                table.Append(TABLE_CHARS.LEFT_BOTTOM_EDGE);
                for (int i = 0 ; i < _tableInfo.colHeadings.Length; i++) {
                    table.Append(PadCentred(new String(TABLE_CHARS.ROW_LINE,1),i, TABLE_CHARS.ROW_LINE));
                    if(i == _tableInfo.colHeadings.Length - 1) 
                        table.Append(TABLE_CHARS.RIGHT_BOTTOM_EDGE);
                    else 
                        table.Append(TABLE_CHARS.BOTTOM_COL_DIV);
                }
                table.AppendLine();
            }
            return table;
        }
        //Only way of viewing the table, all you would have to do is call `table.ToString()`
        public override string ToString() {
            var table = new StringBuilder();
            table.Append(AppendRecord(0,true));
            for(int row = 0; row < _tableInfo.rowNums; row++) {
                table.Append(AppendRecord(row,false));
            }
            return table.ToString();
        }
    }

}
