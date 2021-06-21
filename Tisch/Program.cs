using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
namespace Tisch 
{
    //A couple constants to help construct the table 
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
    }

    public class Table {
        //Need to represent
        //  1. Padding that should be added to each column
        //  2. Maximum length of individual cell
        //  3. The number of rows
        //All of this info will be encapsulated in the TableInfo struct
        private struct TableInfo {
            public int lenOfRow;
            public int[] maxLenCol;
            public int rowNums;
            public readonly string[] colHeadings;

            public TableInfo(string[] cols) {
                colHeadings = cols;
                lenOfRow    = 0;
                //All members initialized to 0 so I don't have to
                maxLenCol = new int[cols.Length];
                rowNums     = 0;
                //Set the initial column length to that of the column heading
                for (int i = 0; i < cols.Length;i++) {
                    maxLenCol[i] = colHeadings[i].Length;
                }
            }

        }   
        private TableInfo _tableInfo;
        //Stores the columns 
        //so 0th index is first column
        private List<List<String>> _internalTable;
        private int _currentCol;
        //Creates a table with the given headings, and appropriate number of columns
        public Table(params string[] colHeadings) {
            var len = colHeadings.Length;
            _currentCol  = 0;
            _internalTable = new List<List<String>>(len);
            _tableInfo = new TableInfo(colHeadings);

        }
        //Adds data to the specified column, returns false if columnName could not be found
        public bool AddColData(string colunmName,string[] col) {
            _currentCol = Array.IndexOf(_tableInfo.colHeadings, colunmName);
            if(_currentCol < 0) {
                return false;
            }
            //If we change the number of rows, we should do that for every column before this new
            //one
            var mustAddMore = false;
            if(_tableInfo.rowNums < col.Length) {
                _tableInfo.rowNums = col.Length;
                for (int i = 0; i < _internalTable.Count; i++)
                {
                    for (int j = 0; j < col.Length; j++) {
                        _internalTable[i].Add(" ");
                    }
                }

            }else {
                mustAddMore = true;
            }
            
            _internalTable.Add(new List<String>(_tableInfo.rowNums));

            for (int i = 0; i < col.Length; i++) {
                string strRepr = col[i];
                
                //Updating the max length of a column for formatting purposes later on
                _tableInfo.maxLenCol[_currentCol] = Math.Max(_tableInfo.maxLenCol[_currentCol], strRepr.Length);
                _internalTable[_currentCol].Add(strRepr);
            }
            //If we have to add more elements to the current column, add empty strings
            if(mustAddMore) {
                for (int i = 0; i < (_tableInfo.rowNums - col.Length); i++) {
                    _internalTable[_currentCol].Add(" ");
                }
            }
            _currentCol += 1;
            return true;
        }
        private string PadCentred(string item, int col, bool isCol) {
            
            var diff = _tableInfo.maxLenCol[col] - item.Length;
          
            if(diff == 0) 
                return item;
            var n = diff / 2;
          
            var padChar = isCol ? '═' : ' ';
            var padStrFront = new String(padChar,n);
            var padStrBack  = diff % 2 == 0? padStrFront : new String(padChar, _tableInfo.maxLenCol[col] - (n + item.Length)) ;
            return $"{padStrFront}{item}{padStrBack}"; 

        }
        private int LengthOfCell() {
            return 2 + _tableInfo.maxLenCol.Sum() + _tableInfo.colHeadings.Length - 1;
        }
        //Only way of viewing the table, all you would have to do is call `table.ToString()'
        public override string ToString() {
            var table = new StringBuilder();
            //Setup header
            table.Append(TABLE_CHARS.LEFT_TOP_EDGE);
            for (int i = 0; i < _tableInfo.colHeadings.Length; i++) {
                if(i == _tableInfo.colHeadings.Length - 1) {
                    table.Append(PadCentred(_tableInfo.colHeadings[i],i,true));
                    table.AppendLine(TABLE_CHARS.RIGHT_TOP_EDGE.ToString());
                    break;
                }
                var current_heading = _tableInfo.colHeadings[i];
                var padded_col = PadCentred(current_heading,i,true);
                table.Append(padded_col);
                table.Append(TABLE_CHARS.TOP_COL_DIV);
            }
            for(int row = 0; row < _tableInfo.rowNums; row++) {
                //If we are at the last row, put a '└' 
                table.Append(TABLE_CHARS.LEFT_MID_EDGE);
                for (int col = 0; col < _currentCol; col++) {
                    var currentItem = _internalTable[col][row];
                    var paddedItem = PadCentred(currentItem,col,false);
                    table.Append(paddedItem);
                    if (col != _currentCol - 1)
                    {
                        table.Append(TABLE_CHARS.MID_ROW_COL_DIV);
                        
                    }
                    else
                    {
                        table.AppendLine(TABLE_CHARS.RIGHT_MID_EDGE.ToString());
                    }
                }
                //Just print the bottom of the table, (scuffed as hell) since we must have appended a new line before
                if(row == _tableInfo.rowNums - 1) {
                    table.Append(TABLE_CHARS.LEFT_BOTTOM_EDGE + new String('-',LengthOfCell() - 2) + TABLE_CHARS.RIGHT_BOTTOM_EDGE);
                }
            }
            return table.ToString();
        }
    }

}
