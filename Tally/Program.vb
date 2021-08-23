Imports System
Imports Tisch 'A library I wrote to make a table, written in C#
'Program by Bokang Mogomotsi
Module Program
    Structure StoreItem
        Public Barcode As String 
        Public Price As Double
        Public Quantity As Integer
    End Structure
    Structure Result 'A structure to store the results of error checking operations for productivity reasons
        Public sItem As StoreItem
        Public IsEmpty As Boolean 'If the result is empty then using sItem is technically an error
    End Structure
    Function Valid(arr As String()) As Result
        Dim si As StoreItem
        Dim res As Result = Nothing
        dim price As Double
        dim quant As Integer 

        If arr.Length <> 3 Then 
            Console.WriteLine("Not enough data!")
            res.IsEmpty = True 
            Return res
        End If
        If  Not Double.TryParse(arr(1),price) Or Not Integer.TryParse(arr(2),quant) Then
            Console.WriteLine("Incorrect formatting of input! Re-Enter")
            res.IsEmpty = True
            Return res
        End If
        si.Barcode = arr(0)
        si.Price   = price 
        si.Quantity = quant 
        res.sItem = si 
        res.IsEmpty = False
        Return res
    End Function
    Const N = 5
    Sub Main(args As String())
        Dim items(N - 1) As StoreItem ' Creating an array of storeitems 
        Dim runningTotal As Double
        For i = 0 to N - 1
            Dim _input() As String
            Dim res As Result 
            Do
                Console.WriteLine("Enter a barcode, price and quantity seperated by commas in that order")
                _input = Console.ReadLine().Split(",")
                res = Valid(_input)
            Loop While res.IsEmpty
            items(i) = res.sItem 
            runningTotal += items(i).Quantity * items(i).Price
        Next
        
        Dim amountForEach = items.Select(Function(i) (i.Quantity * i.Price).ToString())
        Dim Barcodes      = items.Select(Function(i) i.Barcode)
        Dim Quantities    = items.Select(Function(i) i.Quantity.ToString())
        Dim t             = New Table("Barcode","Quantity","Amount($)")
        t.AddColData("Barcode",Barcodes.ToArray())
        t.AddColData("Quantity",Quantities.ToArray())
        t.AddColData("Amount($)",amountForEach.ToArray() )
        Console.WriteLine(t)
        Console.WriteLine($"The Total amount is ${runningTotal}")
    End Sub
End Module
