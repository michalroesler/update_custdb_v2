Option Explicit

Sub update_custdb_from_id_number_to_the_right()
   Application.ScreenUpdating = False
   'Application.DisplayAlerts = False
    
   Dim wordApp As Word.Application            'it is assigned to button #2    'user-defined type not defined
   Dim wordDoc As Word.Document               '[0-9]{1;5}[ ]{1;2}/[0-9]{4}
   Dim excelApp As Excel.Application
   Dim mySheet As Excel.Worksheet
   Dim Para As Word.Paragraph
   Dim wordRng As Word.Range
   Dim wordRng2 As Word.Range
   Dim fullName As Word.Range
   Dim exclRng As Excel.Range
   Dim scndRng As Excel.Range
   Dim address As Excel.Range
    
   Dim pStart As Long
   Dim pEnd As Long
   Dim Length As Long
   Dim startPos As Long
   Dim endPos As Long
   Dim parNmbr As Long
   Dim lngRow As Long
   Dim x As Long
   Dim flag As Boolean
   Dim intCol As Integer
   
   Dim sCell As String
   Dim idNmbr As String
   Dim peselFromContract As String
   Dim nonFormatedDate As String
   Dim city As String
   Dim street As String
   Dim homeNmbr As String
   Dim apNmbr As String
   Dim textToFind1 As String
   Dim textToFind2 As String
   Dim textToFind31 As String
   Dim textToFind32 As String
   Dim textToFind41 As String
   Dim textToFind42 As String
   
   'Assigning object variables and values
   Set wordApp = GetObject(, "Word.Application")       'At its simplest, CreateObject creates an instance of an object,
   Set excelApp = GetObject(, "Excel.Application")     'whereas GetObject gets an existing instance of an object.
   
   Set wordDoc = wordApp.ActiveDocument
   Set mySheet = excelApp.ActiveWorkbook.ActiveSheet
   
   Set wordRng = wordApp.ActiveDocument.Content
   Set scndRng = ActiveSheet.Range("A1")
   
   Set address = Application.InputBox(Prompt:="Pick the Cell", Type:=8)
   
   'Assigning textsToFind?? variables
   textToFind1 = "ważnym do dnia "
   textToFind2 = "roku,"
   textToFind31 = "zamieszkały pod adresem: "
   textToFind32 = "zamieszkały jak podaje: "
   textToFind41 = "zamieszkała pod adresem: "
   textToFind42 = "zamieszkała jak podaje: "
   
   'sCell = ActiveWorkbook.ActiveSheet.Range(Application.InputBox(Prompt:="Pick the Cell", Type:=8)).Value              'sCell = Range(Application.ActiveWorkbook.ActiveSheet.InputBox(Prompt:="Pick the Cell", Type:=8)).Value
   'Application-defined or object-defined error
   'sCell = Application.InputBox(Prompt:="Pick the Cell", Type:=8).Value
   'lngRow = Application.InputBox(Prompt:="Pick the Cell", Type:=8).Row
   'MsgBox ActiveCell.Row
   
   sCell = address.Value
   sCell = Trim$(sCell)
   Debug.Print sCell
   lngRow = address.Row
   Debug.Print lngRow
   
   x = 11
   
   'InStr function returns a Variant (Long) specifying the position of the first occurrence of one string within another.
   'In this section I'm narrowing the whole ActiveDocument.Content range, into only the first 18 or 21 paragraphs.
   Set wordRng = wordApp.ActiveDocument.Content                'assign all docx contents into wordRng variable
   startPos = InStr(1, wordRng, sCell) - 1                     'here we get 2070, we're looking 4 PESEL nmbr;
   endPos = InStr(startPos, wordRng, textToFind1) - 1          'here we get 2203, we're looking 4 "ważnym do dnia";
   wordRng.SetRange Start:=0, End:=endPos                      'wordrng.SetRange(=0, =endpos)
   parNmbr = wordRng.Paragraphs.Count
   Debug.Print "Total # of paragraphs = " & parNmbr

    wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
    Debug.Print wordRng
    Debug.Print sCell
    Debug.Print lngRow
    'sCell = ActiveWorkbook.Sheets("data").Cells(lngRow, 2).Value
    
    'With wordDoc.wordRng.Find
    With wordRng.Find
      .Text = "<[A-Z]{3} [0-9]{6}>"           'you can use the tags < and > to mark the start and end of a word, respectively.
      .MatchWildcards = True                  'extracting  ID number.
      .MatchCase = False                      'extracting  ID number.
      .Wrap = wdFindStop
      .Forward = True
      .Execute
      If .Found = True Then
        idNmbr = wordRng
        idNmbr = Trim$(idNmbr)
        intCol = Application.WorksheetFunction.match("nr_dokumentu", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
        mySheet.Cells(lngRow, intCol) = idNmbr                                                              'lngRow = lngRow + 1.
      End If
   End With
   
   'InStr function returns a Variant (Long) specifying the position of the first occurrence of one string within another.
   wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
   Debug.Print wordRng                   'extracting valid until... date.
   startPos = InStr(1, wordRng, textToFind1) + Len(textToFind1)               'here we get 217, we're looking 4 id validity date;
   endPos = InStr(startPos, wordRng, textToFind2) - 1                         'here we get 247, we're looking 4 id validity date;
   'Mid function returns a Variant (String) containing a specified number of characters from a string.
   nonFormatedDate = Mid(wordRng, startPos, endPos - startPos)
   nonFormatedDate = Trim$(nonFormatedDate)
   Debug.Print nonFormatedDate
   intCol = Application.WorksheetFunction.match("data_waznosci_slownie", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
   mySheet.Cells(lngRow, intCol) = nonFormatedDate
   
   'extracting city of residence
   wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
   intCol = Application.WorksheetFunction.match("plec", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
   If mySheet.Cells(lngRow, intCol).Value = "m" Then
      With wordRng
         With .Find
            .Text = textToFind31
            .MatchWildcards = True
            .MatchCase = False
            .Wrap = wdFindStop
            .Forward = True
            .Execute
            If .Found = True Then
               wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
               startPos = InStr(1, wordRng, textToFind31) + Len(textToFind31)            'here we get 123, we're looking 4 "zamieszkały pod adresem: ";
               endPos = InStr(startPos, wordRng, "(") - 1                                'here we get 131, we're looking 4 "(";
               'Mid function returns a Variant (String) containing a specified number of characters from another string.
               city = Mid(wordRng, startPos, endPos - startPos)
               city = Trim$(city)
               Debug.Print city
               intCol = Application.WorksheetFunction.match("zamieszk_miasto", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = city
            ElseIf .Found = False Then
               wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
               startPos = InStr(1, wordRng, textToFind32) + Len(textToFind32)            'here we get 123, we're looking 4 "zamieszkały jak podaje: ";
               endPos = InStr(startPos, wordRng, "(") - 1                                'here we get 131, we're looking 4 "(";
               'Mid function returns a Variant (String) containing a specified number of characters from another string.
               city = Mid(wordRng, startPos, endPos - startPos)
               city = Trim$(city)
               Debug.Print city
               intCol = Application.WorksheetFunction.match("zamieszk_miasto", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = city
            End If
            
            .Text = "<([0-9]{2}-[0-9]{3})>"               'you can use the tags < and > to mark the start and end of a word, respectively.
            .MatchWildcards = True                        'find postal code
            .MatchCase = False
            .MatchWholeWord = True                        'this was added recently
            .Wrap = wdFindStop
            .Forward = True
            .Execute
            If .Found = True Then
               intCol = Application.WorksheetFunction.match("zamieszk_kod", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               'mySheet.Cells(lngRow, intCol) = .Text     'this line writes "<([0-9]{2}-[0-9]{3})>" into the cell
               mySheet.Cells(lngRow, intCol) = wordRng
            End If
         End With
      End With
   ElseIf mySheet.Cells(lngRow, intCol).Value = "k" Then
      wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
      Debug.Print wordRng
      With wordRng
         With .Find
            .Text = textToFind41
            .MatchWildcards = True
            .MatchCase = False
            .Wrap = wdFindStop
            .Forward = True
            .Execute
            If .Found = True Then
               wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
               startPos = InStr(1, wordRng, textToFind41) + Len(textToFind41)            'here we get 129, we're looking 4 "zamieszkała pod adresem: ";
               endPos = InStr(startPos, wordRng, "(") - 1                                'here we get 137, we're looking 4 "(";
               'Mid function returns a Variant (String) containing a specified number of characters from another string.
               city = Mid(wordRng, startPos, endPos - startPos)
               city = Trim$(city)
               Debug.Print city
               intCol = Application.WorksheetFunction.match("zamieszk_miasto", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = city
            ElseIf .Found = False Then
               wordRng.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
               startPos = InStr(1, wordRng, textToFind42) + Len(textToFind42)            'here we get 123, we're looking 4 "zamieszkała jak podaje: ";
               endPos = InStr(startPos, wordRng, "(") - 1                                'here we get 131, we're looking 4 "(";
               'Mid function returns a Variant (String) containing a specified number of characters from another string.
               city = Mid(wordRng, startPos, endPos - startPos)
               city = Trim$(city)
               Debug.Print city
               intCol = Application.WorksheetFunction.match("zamieszk_miasto", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = city
            End If
            
            .Text = "<([0-9]{2}-[0-9]{3})>"               'you can use the tags < and > to mark the start and end of a word, respectively.
            .MatchWildcards = True                        'find postal code
            .MatchCase = False
            .Wrap = wdFindStop
            .Forward = True
            .Execute
            If .Found = True Then
               intCol = Application.WorksheetFunction.match("zamieszk_kod", Worksheets("data").Range("1:1"), 0)    'Range("1:1") is row 1.
               'mySheet.Cells(lngRow, intCol) = .Text     'this line writes "<([0-9]{2}-[0-9]{3})>" into the cell
               mySheet.Cells(lngRow, intCol) = wordRng
            End If
         End With
      End With
   End If
   
   'extracting street of residence.
   Set wordRng = ActiveDocument.Paragraphs(parNmbr).Range              'extracting street of residence.
   Debug.Print wordRng                                                 'extracting street of residence.
   startPos = InStr(1, wordRng, "przy ulicy ") + Len("przy ulicy ")
   endPos = InStr(startPos, wordRng, "nr") - 1
   street = Mid(wordRng, startPos, endPos - startPos)
   street = Trim$(street)
   Debug.Print street
   intCol = Application.WorksheetFunction.match("zamieszk_ulica_aleja", Worksheets("data").Range("1:1"), 0)      'Range("1:1") is row 1.
   mySheet.Cells(lngRow, intCol) = street                         'writing street value into the cell.
   'the wordRng is not getting changed as a result of the procedure above
   
   startPos = 0
   endPos = 0
   Set wordRng2 = ActiveDocument.Paragraphs(parNmbr).Range
   startPos = InStr(1, wordRng2, street & " nr ") + Len(street & " nr ")         'extracting residence home number.
   endPos = InStr(startPos, wordRng2, "m.") - 1                                  'extracting residence home number.
   If endPos = -1 Then endPos = InStr(startPos, wordRng2, "lok.") - 1
   If endPos = -1 Then endPos = InStr(startPos, wordRng2, ",")
   Debug.Print endPos
   homeNmbr = Mid(wordRng2, startPos, endPos - startPos)
   homeNmbr = Trim$(homeNmbr)
   Debug.Print homeNmbr
   intCol = Application.WorksheetFunction.match("nr_domu", Worksheets("data").Range("1:1"), 0)      'Range("1:1") is row 1.
   mySheet.Cells(lngRow, intCol) = homeNmbr                       'writing home number into the cell.
   'the wordrng2 is not getting changed as a result of the procedure above
   
   'extract the apartment number
   With wordRng2
         With .Find
            .Text = "m."           'you can use the tags < and > to mark the start and end of a word, respectively.
            .MatchWildcards = True                  'extracting  ap number.
            .MatchCase = False                      'extracting  ap number.
            .MatchWholeWord = True
            .Wrap = wdFindStop
            .Forward = True
            .Execute
         End With
         'Debug.Print wordRng2
         Debug.Print .Text
         wordRng2.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
         If .Find.Found = True Then
            startPos = InStr(1, wordRng2, "m. ") + Len("m. ")                  'extracting residence home number.
            endPos = InStr(startPos, wordRng2, ",")                            'extracting residence home number.
            apNmbr = Mid(wordRng2, startPos, endPos - startPos)
            apNmbr = Trim$(apNmbr)
            Debug.Print apNmbr
            intCol = Application.WorksheetFunction.match("nr_lok", Worksheets("data").Range("1:1"), 0)      'Range("1:1") is row 1.
            mySheet.Cells(lngRow, intCol) = apNmbr
         Else
            With .Find
               .Text = "lok."             'you can use the tags < and > to mark the start and end of a word, respectively.
               .MatchWildcards = True                  'extracting  ap number.
               .MatchCase = False                      'extracting  ap number.
               .MatchWholeWord = True
               .Wrap = wdFindStop
               .Forward = True
               .Execute
            End With
            'Debug.Print wordRng2
            'Debug.Print .Text
            wordRng2.SetRange Start:=ActiveDocument.Paragraphs(parNmbr).Range.Start, End:=ActiveDocument.Paragraphs(parNmbr).Range.End
            If .Find.Found = True Then
               startPos = InStr(1, wordRng2, "lok. ") + Len("lok. ")              'extracting residence home number.
               endPos = InStr(startPos, wordRng2, ",")                            'extracting residence home number.
               apNmbr = Mid(wordRng2, startPos, endPos - startPos)
               apNmbr = Trim$(apNmbr)
               Debug.Print apNmbr
               intCol = Application.WorksheetFunction.match("nr_lok", Worksheets("data").Range("1:1"), 0)      'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = apNmbr
            Else
               apNmbr = "n/a"
               intCol = Application.WorksheetFunction.match("nr_lok", Worksheets("data").Range("1:1"), 0)      'Range("1:1") is row 1.
               mySheet.Cells(lngRow, intCol) = apNmbr
            End If
         End If
   End With
End Sub


Sub WhereAmI()

    'This is a collection of three small functions that provide the line number on the_
    'current page, the abolute line number in the document, and the paragraph number in the document.
                                                                                     'http://www.vbaexpress.com/kb/getarticle.php?kb_id=59
    MsgBox "Paragraph number: " & GetParNum(Selection.Range) & vbCrLf & _            'https://www.automateexcel.com/vba/new-line-carriage-return/
    "Absolute line number: " & GetAbsoluteLineNum(Selection.Range) & vbCrLf & _      'When working with strings in VBA, use vbNewLine, vbCrLf or vbCR to insert a line break / new paragraph.
    "Relative line number: " & GetLineNum(Selection.Range)                           'The following code shows you how you would use vbNewLine in order to put the second text string on a new line in the Immediate window.
End Sub
