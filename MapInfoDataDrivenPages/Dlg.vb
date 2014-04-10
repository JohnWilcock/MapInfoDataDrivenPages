'/*****************************************************************************
'*       Author J.Wilcock 2014
'*      Data Driven Pages For Mapinfo v1.0
'*****************************************************************************


Imports System
Imports System.Windows.Forms
Imports System.Threading
Imports System.Xml
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports MapInfo.MiPro.Interop
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Drawing.Printing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports System.Xml.Linq
Imports System.Linq





Namespace MapInfoDataDrivenPages
    Partial Public Class Dlg

        Inherits UserControl
        ' some string in xml file 
        'Private Const STR_NAME As String = "Name"
        Private Const STR_ROOT As String = "root"
        Private Const STR_DIALOG As String = "Dialog"
        Private Const STR_NAMEDVIEWS As String = "DDP"
        'Private Const STR_VIEWS As String = "Views"
        Private Const STR_PATH_DIALOG As String = "/DDP/Dialog"
        Private Const STR_PATH_ROOT_FOLDER As String = "/DDP/Views"
        Private Const STR_LEFT As String = "Left"
        Private Const STR_TOP As String = "Top"
        Private Const STR_WIDTH As String = "Width"
        Private Const STR_HEIGHT As String = "Height"

        ' The controller class which uses this dialog class ensures 
        ' * a single instance of this dialog class. However different 
        ' * running instance of MapInfo Professional will have their 
        ' * own copy of dll. To make sure that read/write from/to xml 
        ' * file which is going to be a single file on the disk, is 
        ' * smooth and we have the synchronized access to the xml file, 
        ' * the Mutexes will be used. 
        ' 


        ' Name of the mutex 
        Private sXMLFile As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\MapInfo\MapInfo\DDP.xml"
        Private dialogLeft As Integer, dialogTop As Integer, dialogWidth As Integer, dialogHeight As Integer
        ' flag indicating whether it is the first time the form is being loaded
        Dim firstLoad As Boolean = True
        Private _controller As Controller  ' represents the window that owns this dialog (main MI Pro window)

        'min/max x/y
        Dim minX, minY, maxX, maxY As Double
        Dim isMapper As Integer
        Dim coordsys As String = "CoordSys Earth Projection 8, 79, " & Chr(34) & "m" & Chr(34) & ", -2, 49, 0.9996012717, 400000, -100000 Bounds (-7845061.1011, -15524202.1641) (8645061.1011, 4470074.53373)"

        ''' <summary> 
        ''' Construction 
        ''' </summary> 
        ''' 
        Public tableNames As New List(Of String)
        Public ColumnList As New List(Of String)
        Public MapperList As New List(Of String)
        Public MapperIDList As New List(Of String)
        Public LayoutList As New List(Of String)
        Public LayoutIDList As New List(Of String)
        Public pageList As New List(Of String)

        Public LayoutTableRowCount As Integer = 1

        Public pageDrivenQueryList As New List(Of String)
        Public pageDrivenQueryTables As New List(Of String)
        Public pageDrivenQueryListSub As New List(Of String)
        Public pageDrivenQueryForms As New List(Of queryInfo)

        Public pageTextRowIdList As New List(Of String)
        Public pageTextColumnHeaderList As New List(Of String)
        Public pageTextPackedRowIdList As New List(Of String)
        Public pageTextPackedColumnHeaderList As New List(Of String)

        Public CD As New ColorDialog

        'lists for interval xy
        Public Shared xIntervalList As New List(Of Double)
        Public Shared yIntervalList As New List(Of Double)
        Public Shared zIntervalList As New List(Of Double)
        Public Shared ELEVarray As New List(Of Double)

        Dim exportType As Integer '1 for image, 2 for print , 3 for multi tif
        Dim tiffarray() As Bitmap

        Dim windowColour As Color
        Dim validRows As Integer

        'sort list - uses custom class
        Dim sortList As New List(Of pageSortData)

        'delete list
        Dim deleteList As New List(Of String)







        Public Sub New()
            InitializeComponent()
            'mut = New Mutex(False, mutexName)

            'set standard text in combo boxes
            ComboBox7.Text = "JPG"
            NumericUpDown4.Value = 200   'resolution
            'ComboBox10.Text = "mm" ' layout units for layout buffer
            NumericUpDown1.Value = 125
            NumericUpDown2.Value = 125
            ComboBox5.SelectedIndex = 0
            ComboBox6.SelectedIndex = 0

            'check for blank file used in search operations - this is used as a flag to see if OSTools has been installed, if not it is autoregestered by the MBX
            CreateBlank()



        End Sub



        ''' <summary>
        ''' Parameterised Construction
        ''' <param name="controller"></param>
        ''' </summary>
        Public Sub New(ByVal controller As Controller)
            Me.New()
            _controller = controller
        End Sub




#Region "[DIALOG EVENT HANDLERS]"
        ''' <summary> 
        ''' Named View dialog Load event handler 
        ''' </summary> 
        ''' <param name="sender"></param> 
        ''' <param name="e"></param> 
        Private Sub NViewDlg_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            CreateBlank()
            InteropHelper.theDlg = Me
            windowColour = ComboBox1.BackColor
            refreshDDP()
            If firstLoad = True Then
                firstLoad = False

                If dialogWidth >= Me.MinimumSize.Width AndAlso dialogWidth <= Screen.PrimaryScreen.WorkingArea.Width Then
                    Me.Width = dialogWidth
                End If
                If dialogHeight >= Me.MinimumSize.Height AndAlso dialogHeight <= Screen.PrimaryScreen.WorkingArea.Height Then
                    Me.Height = dialogHeight
                End If
                If dialogLeft > -Me.Width AndAlso dialogLeft < Screen.PrimaryScreen.WorkingArea.Width Then
                    Me.Left = dialogLeft
                End If
                If dialogTop > -Me.Top AndAlso dialogTop < Screen.PrimaryScreen.WorkingArea.Height Then
                    Me.Top = dialogTop
                End If
            End If
        End Sub
        ' This call to the WIN32 API function SetFocus is used in NViewDlg_FormClosing below
        <DllImport("User32.dll")> _
        Private Shared Function SetFocus(ByVal hWnd As IntPtr)
        End Function




#End Region


        Public Sub CloseDockWindow()
            ''Write out the XML file that stores the Named Views info
            _controller.DockWindowClose()
        End Sub
        ''' <summary>
        ''' Set the dialog position and docking state 
        ''' </summary>
        Public Sub SetDockPosition()
            _controller.SetDockWindowPositionFromFile()
        End Sub




#Region "[HELPERS]"

        Sub CreateBlank()
            'create blank tab is it does not exist
            Dim MIfolderDir As String = InteropServices.MapInfoApplication.Eval("GetFolderPath$(-3)")
            If Not System.IO.File.Exists(MIfolderDir & "\ProfileTool.tab") Then

                InteropServices.MapInfoApplication.Do("Create Table blank(blank Char(30)) file " & Chr(34) & MIfolderDir & "\blank.tab" & Chr(34))
                InteropServices.MapInfoApplication.Do("open table " & Chr(34) & MIfolderDir & "\blank.tab" & Chr(34) & " as blank hide readonly")
                InteropServices.MapInfoApplication.Do("create map for blank CoordSys Earth Projection 8, 79, " & Chr(34) & "m" & Chr(34) & ", -2, 49, 0.9996012717, 400000, -100000 Bounds (-7845061.1011, -15524202.1641) (8645061.1011, 4470074.53373)")
                InteropServices.MapInfoApplication.Do("close table blank")
            End If
        End Sub


#End Region



        'functions called from MapBasic ***************************************

        Public Shared Function nextGrid(ByVal x As Integer) As Boolean
            Return True
        End Function





        Private Sub ToolStripContainer1_ContentPanel_Load(sender As Object, e As EventArgs)

        End Sub

        Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

        End Sub

        Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
            Dim result As Integer = MessageBox.Show("This will remove all settings and refresh the list of layers, mappers and layouts", "Continue ?", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                refreshDDP()
            End If

        End Sub

        Sub refreshDDP()
            populateListofIndexLayers()
            resetMapperAndLayoutPickers()
            resetSetupFields()


            'clear page text lists
            pageTextRowIdList.Clear()
            pageTextColumnHeaderList.Clear()
            pageList.Clear()
            ToolStripDropDownButton1.DropDownItems.Clear()
            ToolStripComboBox1.Items.Clear()
            pageList.Clear()
            'LayoutTableRowCount = 1
            'need to wipe any existing page text

            pageDrivenQueryForms.Clear()
            pageDrivenQueryList.Clear()
            pageDrivenQueryListSub.Clear()
            pageDrivenQueryTables.Clear()

            DataGridView1.Rows.Clear()

            CD.Color = Color.Red

            'set combo box colors to none
            ComboBox1.Enabled = True
            ComboBox8.Enabled = True
            ComboBox2.Enabled = True

            'auto fill combo boxes if data availble - show red if error
            'index layer
            If ComboBox1.Items.Count > 0 Then
                ComboBox1.SelectedIndex = 0
            Else
                ComboBox1.Items.Add("No Vaild Index Layers open")
                ComboBox1.SelectedIndex = 0
                ComboBox1.Enabled = False
            End If

            'mapper layer
            If ComboBox8.Items.Count > 0 Then
                ComboBox8.SelectedIndex = 0
            Else
                ComboBox8.Items.Add("No Vaild Mappers open")
                ComboBox8.SelectedIndex = 0
                ComboBox8.Enabled = False
            End If

            'layout layer
            If ComboBox2.Items.Count > 0 Then
                ComboBox2.SelectedIndex = 0
            Else
                If ComboBox8.Text <> "No Vaild Mappers open" Then
                    'open a new layout
                    Dim currentCoordSys As String = InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & " ,17)")
                    InteropServices.MapInfoApplication.Do("Layout")
                    InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & "in" & Chr(34) & "")
                    InteropServices.MapInfoApplication.Do("Create Frame (0.5,0.5) (5,5) Pen (1,2,0) Brush (2,16777215,16777215) From Window " & MapperIDList(ComboBox8.SelectedIndex) & " FillFrame On")
                    InteropServices.MapInfoApplication.Do("Set " & currentCoordSys)

                    populateListOfLayouts()
                    ComboBox2.Items.AddRange(LayoutList.ToArray)
                    ComboBox2.SelectedIndex = 0

                Else
                    ComboBox2.Items.Add("No Vaild Layouts open")
                    ComboBox2.SelectedIndex = 0
                    ComboBox2.Enabled = False
                End If
            End If

            'lear sorting
            sortList.Clear()
        End Sub

        Sub resetSetupFields()
            ComboBox1.Text = "Pick an index layer"

            ComboBox8.Text = "Pick a mapper"
            ComboBox2.Text = "Pick a layout"

            ComboBox3.Enabled = False
            ComboBox4.Enabled = False
            ComboBox3.Text = "Pick an index layer above"
            ComboBox4.Text = "Pick an index layer above"
        End Sub

        Sub resetColumnSetupFields()
            ComboBox3.Enabled = True
            ComboBox4.Enabled = True
            ComboBox3.Text = "Pick a column to name pages by"
            ComboBox4.Text = "Pick a sort column"
        End Sub

        Public Sub resetMapperAndLayoutPickers()
            ComboBox8.Items.Clear()
            ComboBox2.Items.Clear()
            ComboBox10.Items.Clear() 'overview

            populateListOfMappers()
            populateListOfLayouts()

            ComboBox8.Items.AddRange(MapperList.ToArray)
            ComboBox2.Items.AddRange(LayoutList.ToArray)
            ComboBox10.Items.AddRange(MapperList.ToArray)
        End Sub

        Public Sub populateListOfMappers()
            MapperList.Clear()
            MapperIDList.Clear()

            Dim NumberOfWindowsOpen As Integer

            'find number of open windows of all types
            NumberOfWindowsOpen = InteropServices.MapInfoApplication.Eval("NumWindows()")

            'cycle through all open windows to find mappers
            For i As Integer = 1 To NumberOfWindowsOpen
                'if mapper add to combobox
                If InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 3)") = 1 Then '3= win type
                    MapperList.Add(InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 1)")) ' 1=win_info_name
                    MapperIDList.Add(InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 13)")) ' 13 = winID
                End If

            Next



        End Sub

        Public Sub populateListOfLayouts()
            LayoutList.Clear()
            LayoutIDList.Clear()

            Dim NumberOfWindowsOpen As Integer

            'find number of open windows of all types
            NumberOfWindowsOpen = InteropServices.MapInfoApplication.Eval("NumWindows()")

            'cycle through all open windows to find mappers
            For i As Integer = 1 To NumberOfWindowsOpen
                'if mapper add to combobox
                If InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 3)") = 3 Then '3= win type, 3 =layout win
                    LayoutList.Add(InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 1)")) ' 1=win_info_name
                    LayoutIDList.Add(InteropServices.MapInfoApplication.Eval("WindowInfo(" & i & ", 13)")) ' 13 = winID
                End If

            Next

        End Sub

        Public Sub populateListofIndexLayers()
            Dim NumberOfTablesOpen As Integer
            tableNames.Clear()

            'find number of open tables
            NumberOfTablesOpen = InteropServices.MapInfoApplication.Eval("NumTables()")


            'cycle through all open tables (don't need to be in mapper) to find vector layers
            For i As Integer = 1 To NumberOfTablesOpen
                'check layer is valid

                'if valid add to combobox
                If isLayerValid(InteropServices.MapInfoApplication.Eval("tableinfo(" & i & ", 1)")) Then
                    tableNames.Add(InteropServices.MapInfoApplication.Eval("tableinfo(" & i & ", 1)")) ' 1=tab_info_name
                End If

            Next
            ComboBox1.Items.Clear()
            ComboBox1.Items.AddRange(tableNames.ToArray)

        End Sub

        Sub populateListofColumnsFromLayer(ByVal tableName As String)
            ComboBox3.Items.Clear()
            ComboBox4.Items.Clear()
            ComboBox9.Items.Clear()

            getColumnsOfChosenTable(tableName)

            ComboBox3.Items.AddRange(ColumnList.ToArray)
            ComboBox4.Items.AddRange(ColumnList.ToArray)
            ComboBox9.Items.AddRange(ColumnList.ToArray)

            'add list of columns to page text 
            'TODO: - also add object information posibilities e.g area, length, scale

            Dim y As Integer = 0
            ToolStripDropDownButton1.DropDownItems.Clear()
            For x As Integer = 0 To ColumnList.Count - 1
                ToolStripDropDownButton1.DropDownItems.Add(ColumnList(x))
                'ToolStripDropDownButton1.DropDownItems(x).
                AddHandler ToolStripDropDownButton1.DropDownItems(x).Click, AddressOf addPageText
                y = x
            Next

            'add standard items. scale, page number & date
            ToolStripDropDownButton1.DropDownItems.Add("-")
            ToolStripDropDownButton1.DropDownItems.Add("Scale")
            AddHandler ToolStripDropDownButton1.DropDownItems(y + 2).Click, AddressOf addStandardPageText

            ToolStripDropDownButton1.DropDownItems.Add("Page")
            AddHandler ToolStripDropDownButton1.DropDownItems(y + 3).Click, AddressOf addStandardPageText

            ToolStripDropDownButton1.DropDownItems.Add("Date")
            AddHandler ToolStripDropDownButton1.DropDownItems(y + 4).Click, AddressOf addStandardPageText

        End Sub

        Sub addStandardPageText(ByVal sender As System.Object, ByVal e As System.EventArgs)
            'standard items are those which change between pages but are not associated with attribute data. i.e. date printed.
            Dim textType As String = CType(sender, ToolStripMenuItem).Text
            Dim tempString As String = returnStandardText("DDP$" & textType)

            'get the next layout row id
            Dim layoutID As String = LayoutIDList(ComboBox2.SelectedIndex)
            Dim LayoutN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 10 )")
            Dim currentLayoutRow As Integer = getNextLayoutRow(LayoutN)

            'create a text element in selected layout with the contents set to the above
            InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & "mm" & Chr(34))

            InteropServices.MapInfoApplication.Do("Create Text into Window " & layoutID & " " & tempString & " ( 10, 10 ) ( 20, 10 ) ")

            'add to varibles
            pageTextRowIdList.Add(InteropServices.MapInfoApplication.Eval("tableInfo(" & LayoutN & ", 8 )"))
            pageTextColumnHeaderList.Add("DDP$" & textType)
        End Sub

        Function returnStandardText(ByVal textType As String) As String
            Dim tempstring As String = ""
            Select Case textType
                Case "DDP$Scale"
                    tempstring = Chr(34) & Math.Floor(getScale()) & Chr(34)
                Case "DDP$Page"
                    tempstring = Chr(34) & ToolStripComboBox1.Text & Chr(34)
                Case "DDP$Date"
                    tempstring = Chr(34) & Date.Now.ToShortDateString & Chr(34)
            End Select
            Return tempstring
        End Function

        Sub addPageText(ByVal sender As System.Object, ByVal e As System.EventArgs)
            'find out the text of the item clicked (this will be the column name) 
            Dim column As String = CType(sender, ToolStripMenuItem).Text

            'get the row number of the curent DDP - put as 1 untill implemented
            Dim row As String = ToolStripComboBox1.SelectedIndex + 1

            'get the contents of the above column and row for the index layer
            InteropServices.MapInfoApplication.Do("Fetch rec " & row & " From " & ComboBox1.Text)

            'get the next layout row id
            Dim layoutID As String = LayoutIDList(ComboBox2.SelectedIndex)
            Dim LayoutN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 10 )")
            Dim currentLayoutRow As Integer = getNextLayoutRow(LayoutN)

            'create a text element in selected layout with the contents set to the above
            InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & "mm" & Chr(34))
            Dim tempString As String = Chr(34) & InteropServices.MapInfoApplication.Eval(ComboBox1.Text & "." & column) & Chr(34)
            InteropServices.MapInfoApplication.Do("Create Text into Window " & layoutID & " " & tempString & " ( 10, 10 ) ( 20, 10 ) ")

            'add to varibles
            'pageTextRowIdList.Add(currentLayoutRow)
            pageTextRowIdList.Add(InteropServices.MapInfoApplication.Eval("tableInfo(" & LayoutN & ", 8 )"))
            pageTextColumnHeaderList.Add(column)

            'check for xml file and add it.

        End Sub

        Function getScale()
            'get  units of MAP
            Dim Units As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & MapperIDList(ComboBox8.SelectedIndex) & ", 11)")

            getMapperFrameInLayout(LayoutIDList(ComboBox2.SelectedIndex), MapperIDList(ComboBox8.SelectedIndex))
            InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & Units & Chr(34))
            Dim frameWidth As Double = InteropServices.MapInfoApplication.Eval("objectgeography(mainFrame,3) - objectgeography(mainFrame,1)")

            'set coord system back - based on mapper window. 
            InteropServices.MapInfoApplication.Do("Dim coord As String")
            InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ", 17)")
            InteropServices.MapInfoApplication.Do("Run Command coord")

            'assuming metres map units and mm layout units
            Return Math.Floor(Math.Round(CDbl((CDbl(InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ",1)")) / frameWidth) * 1000) / NumericUpDown3.Value, 0)) * NumericUpDown3.Value

        End Function

        Sub getColumnsOfChosenTable(ByVal tableName As String)
            ColumnList.Clear()

            Dim numberOfColumns As Integer
            numberOfColumns = InteropServices.MapInfoApplication.Eval("numcols(" & tableName & ")")

            For i As Integer = 1 To numberOfColumns
                ColumnList.Add(InteropServices.MapInfoApplication.Eval("columninfo(" & tableName & ", COL" & i & ", 1)"))

            Next

        End Sub

        Function isLayerValid(ByVal tableNameOrID As String) As Boolean
            Dim fail As Boolean = True

            'check layer is a vector layer
            If InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 3)") <> 1 Then fail = False ' 3 is table type.  1 is base table

            'check layer has > 0 features i.e. not blank
            If hasObjectRecords(tableNameOrID) = False Then fail = False

            'check layer is mappable
            If InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 5)") = "F" Then fail = False

            Return fail
        End Function

        Sub setupRecordNumbers(ByVal tableNameOrID As String)
            'populate page numbers
            Dim numRec As String = numberOfRecords(tableNameOrID)
            Label10.Text = numRec & " Features"

            'check for unpacked table (must be first - i.e. can't check for objectless rows before packing)
            If numRec <> InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 8)") Then
                Dim PT As New PackTable
                PT.Label4.Text = ComboBox1.Text

                PT.Label7.Text = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 8)")
                PT.Label8.Text = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 8)") - numRec
                PT.ShowDialog()
            End If

            'check for rows without objects
            If validRows <> InteropServices.MapInfoApplication.Eval("tableinfo(" & tableNameOrID & ", 8)") Then
                Dim ObLessRows As New ObjectlessRows
                ObLessRows.Label4.Text = ComboBox1.Text

                ObLessRows.Label7.Text = numRec
                ObLessRows.Label8.Text = validRows
                ObLessRows.ShowDialog()
            End If

            ToolStripComboBox1.Items.Clear()
            CheckedListBox1.Items.Clear()
            ToolStripComboBox1.Items.AddRange(pageList.ToArray)
            CheckedListBox1.Items.AddRange(pageList.ToArray)
        End Sub

        Function numberOfRecords(ByVal tableName As String) As Integer
            'fetch only understands tablename
            If IsNumeric(tableName) Then tableName = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableName & ", 1)") ' 1=table name

            validRows = 0
            Dim i As Integer = 0
            Dim tempValue As String = ""
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                tempValue = InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 1)")
                If tempValue = 1 Or tempValue = 2 Or tempValue = 3 Or tempValue = 4 Or tempValue = 5 Or tempValue = 6 Or tempValue = 7 Or tempValue = 8 Or tempValue = 9 Then
                    validRows = validRows + 1
                End If
                i = i + 1 '11=OBJ_INFO_NONEMPTY   |||  MI only returns strings
                pageList.Add(i)
                'End If
                InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)

            End While

            Return i
        End Function

        Function hasObjectRecords(ByVal tableName As String) As Boolean
            'fetch only understands tablename
            If IsNumeric(tableName) Then tableName = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableName & ", 1)") ' 1=table name

            Dim i As Integer = 0
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                If InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 1)") <> 10 Then Return True '11=OBJ_INFO_NONEMPTY   |||  MI only returns strings
                InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)
            End While

            Return False
        End Function

        Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged

        End Sub

        Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
            'check if tale is packed - if not insist it must be to continue
            'do this by checking total rows against rows with object data
            'TODO:see above

            If ComboBox1.Text <> "No Vaild Index Layers open" Then
                ToolStripComboBox1.Items.Clear()
                pageList.Clear()

                populateListofColumnsFromLayer(ComboBox1.Text)
                resetColumnSetupFields()
                setupRecordNumbers(ComboBox1.Text)
                ToolStripComboBox1.SelectedIndex = 0
                pageSelectAll()
            End If
        End Sub




        '////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

            If CheckBox1.Checked Then
                exportType = 3
            Else
                exportType = 1
            End If

            ExportToImages()
        End Sub

        Public Sub ExportToImages()
            cycleThroughFeatures()
        End Sub

        Public Sub cycleThroughFeatures()

            'set up vars for multi page tif
            Dim MasterBitmap As Bitmap
            Dim currentImage As Image
            Dim encoder As Encoder = encoder.SaveFlag
            Dim info As ImageCodecInfo = Nothing

            Dim iCodecInfo As ImageCodecInfo
            For Each iCodecInfo In ImageCodecInfo.GetImageEncoders()
                If iCodecInfo.MimeType = "image/tiff" Then
                    info = iCodecInfo
                End If
            Next iCodecInfo
            Dim ep As New EncoderParameters(1)
            ep.Param(0) = New EncoderParameter(encoder, CLng(EncoderValue.MultiFrame))


            deleteList.Clear()
            'set up MI dim for MBR object (.net cannot accept this
            InteropServices.MapInfoApplication.Do("dim currentObject as object")


            'get selected index table
            Dim tableName As String = ComboBox1.Text
            Dim mapperID As Integer = MapperIDList(ComboBox8.SelectedIndex)
            'get selected layout name
            Dim layoutName As String = ComboBox2.Text
            Dim layoutID As Integer = LayoutIDList(ComboBox2.SelectedIndex)

            'get  units of MAP
            Dim Units As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim LayoutUnits As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim existingMapperScale As Double = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 2)")

            'must set coordinate system based on map options - see profile tool
            'set coord system - based on mapper window. 
            InteropServices.MapInfoApplication.Do("Dim coord As String")
            InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & mapperID & ", 17)")
            InteropServices.MapInfoApplication.Do("Run Command coord")

            'get layout page size to pass to exportjpg
            'get current page dims
            'windowinfo papersize = 24
            'windowinfo page orientaion =22
            'PrinterSettings.PaperSizes rawkind

            Dim outName As String
            Dim MapInfoPageSize As Integer = InteropServices.MapInfoApplication.Eval("windowinfo(" & layoutID & ", 24)")
            Dim layoutPageSize As New PaperSize
            'layoutPageSize.RawKind = MapInfoPageSize

            Dim pageWidth, pageHeight As Double
            Dim settings As New PrinterSettings()
            For Each size As PaperSize In settings.PaperSizes
                If size.RawKind = MapInfoPageSize Then
                    layoutPageSize = size
                End If
            Next


            'is portrait or landscape
            If InteropServices.MapInfoApplication.Eval("windowinfo(" & layoutID & ", 22)") = 1 Then 'if potrait
                pageWidth = layoutPageSize.Width
                pageHeight = layoutPageSize.Height
            Else
                pageHeight = layoutPageSize.Width
                pageWidth = layoutPageSize.Height
            End If

            'set up counter
            Dim i As Integer = 1

            'output name column
            Dim nameColumn As String
            If ComboBox3.Text <> "" Then
                'if a name column is selected
                'are values unique (if not the files will override themselfs as the pages are exported)
                If areColumnValuesUnique(ComboBox3.Text) Then
                    nameColumn = ComboBox3.Text
                Else
                    nameColumn = "rowid"
                End If


            Else
                'if no name column selected default to row id .  i.e. 1,2,3,4 etc...
                nameColumn = "rowid"
            End If

            'set up progress bars
            ProgressBar1.Maximum = CheckedListBox1.CheckedItems.Count
            ProgressBar2.Maximum = CheckedListBox1.CheckedItems.Count


            'for each feature
            'get feature MBR and zoom to it
            Dim tempValue As String = ""
            tempValue = InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 1)")

            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"

                'override "Next page" if sort field is chosen
                If sortList.Count > 0 Then
                    If i > sortList.Count Then
                        Exit While
                    End If
                    InteropServices.MapInfoApplication.Do("Fetch rec " & sortList(CInt(ToolStripComboBox1.Text) - 1).number & " From " & tableName)
                End If

                'check if valid object (not all rows may have an object attached to them
                'If tempValue = 1 Or tempValue = 2 Or tempValue = 3 Or tempValue = 4 Or tempValue = 5 Or tempValue = 6 Or tempValue = 7 Or tempValue = 8 Or tempValue = 9 Then
                If i = CheckedListBox1.Items.Count + 1 Then Exit While
                If CheckedListBox1.GetItemChecked(i - 1) Then
                    'recNumber = InteropServices.MapInfoApplication.Eval(tableName & ".rowid")

                    'move page number on 
                    'this will trigger pagetext update
                    ToolStripComboBox1.SelectedIndex = i - 1 'ToolStripComboBox1.SelectedIndex + 1


                    'InteropServices.MapInfoApplication.Do("Fetch rec " & recNumber & " From " & tableName)


                    'NOW MOVED TO MOVE EXTENT''''''''''''''''''''''''''''''
                    'execute any page driven queries
                    'If pageDrivenQueryList.Count > 0 Then
                    'place index page values into query
                    'substituteIndexValues()

                    'run queries
                    'executeQueries()
                    'End If
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''


                    'requires re-seting after updatePageText
                    'InteropServices.MapInfoApplication.Do("Fetch Rec " & i & " From " & tableName)
                    ' MsgBox(InteropServices.MapInfoApplication.Eval(tableName & ".rowid"))
                    Select Case exportType
                        Case 1
                            'export chosen layout to image
                            'MsgBox(nameColumn & ": " & InteropServices.MapInfoApplication.Eval(tableName & "." & nameColumn))
                            exportJPG(layoutID, Path.Combine(TextBox1.Text, InteropServices.MapInfoApplication.Eval(tableName & "." & nameColumn)), pageWidth, pageHeight)
                            If i < ProgressBar1.Maximum Then ProgressBar1.Value = i
                        Case 2
                            'send to printer
                            InteropServices.MapInfoApplication.Do("PrintWin  Window " & layoutID)
                            If i < ProgressBar2.Maximum Then ProgressBar2.Value = i
                        Case 3
                            'multipage tif
                            outName = InteropServices.MapInfoApplication.Eval(tableName & "." & nameColumn)
                            exportJPG(layoutID, Path.Combine(TextBox1.Text, outName), pageWidth, pageHeight)
                            'if first image
                            If IsNothing(MasterBitmap) Then
                                'master image
                                MasterBitmap = Bitmap.FromFile(Path.Combine(TextBox1.Text, outName) & ".tif")
                                MasterBitmap.Save(Path.Combine(TextBox1.Text, "multiPageTiff.tif"), info, ep)
                                ep.Param(0) = New EncoderParameter(encoder, CLng(EncoderValue.FrameDimensionPage))
                            Else
                                'other image
                                currentImage = CType(Bitmap.FromFile(Path.Combine(TextBox1.Text, InteropServices.MapInfoApplication.Eval(tableName & "." & nameColumn) & ".tif")), Image)
                                MasterBitmap.SaveAdd(currentImage, ep)

                            End If
                            deleteList.Add(Path.Combine(TextBox1.Text, InteropServices.MapInfoApplication.Eval(tableName & "." & nameColumn) & ".tif"))

                            If i < ProgressBar1.Maximum Then ProgressBar1.Value = i
                    End Select


                End If
                i = i + 1



                'only fetch next if no sort field
                'InteropServices.MapInfoApplication.Do("Fetch rec " & recNumber & " From " & tableName)
                If sortList.Count = 0 Then
                    InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)
                End If

            End While

            If exportType = 3 Then
                'if multipage tiff -  finish
                ep.Param(0) = New EncoderParameter(encoder, CLng(EncoderValue.Flush))
                'close out the file.
                MasterBitmap.SaveAdd(ep)
            End If

            ProgressBar1.Value = 0
            ProgressBar2.Value = 0
        End Sub

        'still can't use ... can't detach bmps from process
        Sub cleanUpTiffs()
            If deleteList.Count > 0 Then
                For Each fPath As String In deleteList
                    'delete original
                    If File.Exists(fPath) Then
                        File.Delete(fPath)
                    End If
                Next

                deleteList.Clear()
            End If
        End Sub

        Sub moveExtent()
            'remove queries 
            If pageDrivenQueryList.Count > 0 And pageDrivenQueryTables.Count > 0 Then
                removeQueries()
            End If

            'set up MI dim for MBR object (.net cannot accept this
            InteropServices.MapInfoApplication.Do("dim currentObject as object")

            'for MBR 
            Dim minX, maxX, minY, maxY As Double

            'get selected index table
            Dim tableName As String = ComboBox1.Text
            Dim mapperID As Integer = MapperIDList(ComboBox8.SelectedIndex)
            'get selected layout name
            Dim layoutName As String = ComboBox2.Text
            Dim layoutID As Integer = LayoutIDList(ComboBox2.SelectedIndex)

            'get  units of MAP
            Dim Units As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim LayoutUnits As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim existingMapperScale As Double = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 2)")

            Dim ZoomOrScaleString As String = ""
            Dim ZoomOrScale As Double
            Dim frameWidth As Double = 0
            Dim frameScale As Double = 0

            'override "Next page" if sort field is chosen
            If sortList.Count > 0 Then
                InteropServices.MapInfoApplication.Do("Fetch rec " & sortList(CInt(ToolStripComboBox1.Text) - 1).number & " From " & tableName)
            End If

            'move page number on 
            InteropServices.MapInfoApplication.Do("currentObject = MBR(" & tableName & ".obj)")

            'initialise coord values
            minX = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 1) ")
            maxX = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 3) ")
            minY = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 2) ")
            maxY = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 4) ")

            If RadioButton1.Checked Then
                'if best fit scaling
                If InteropServices.MapInfoApplication.Eval("ObjectInfo(" & tableName & ".obj, 1 )") <> 5 Then
                    'if line or polygon
                    Select Case ComboBox6.Text
                        Case "Percentage"
                            ZoomOrScale = ((maxX - minX) / 100) * NumericUpDown2.Value
                        Case "Map Units"
                            ZoomOrScale = (maxX - minX) + (NumericUpDown2.Value * 2)
                        Case "Page Units"
                            'todo
                    End Select
                Else
                    'if point
                    Select Case ComboBox5.Text
                        Case "Percentage"
                            'there is no percentage for points as they have no MBR
                        Case "Map Units"
                            ZoomOrScale = (maxX - minX) + (NumericUpDown1.Value * 2)
                        Case "Page Units"
                            'todo
                    End Select
                End If
                'round 
                getMapperFrameInLayout(LayoutIDList(ComboBox2.SelectedIndex), MapperIDList(ComboBox8.SelectedIndex))
                InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & Units & Chr(34))
                frameWidth = InteropServices.MapInfoApplication.Eval("objectgeography(mainFrame,3) - objectgeography(mainFrame,1)")

                'set coord system - based on mapper window. 
                InteropServices.MapInfoApplication.Do("Dim coord As String")
                InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & mapperID & ", 17)")
                InteropServices.MapInfoApplication.Do("Run Command coord")
                'frameScale = InteropServices.MapInfoApplication.Eval("mapperinfo(" & mapperID & ",1)") / frameWidth
                frameScale = ZoomOrScale / frameWidth
                'round frame scale
                frameScale = Math.Floor(Math.Round(CDbl(frameScale) / NumericUpDown3.Value, 0)) * NumericUpDown3.Value
                'convert back to zoom width
                ZoomOrScale = frameWidth * CDbl(frameScale)

                ZoomOrScaleString = " Zoom " & ZoomOrScale & " units " & Chr(34) & Units & Chr(34)

            Else
                'map set by set scale 
                If RadioButton2.Checked Then
                    'static scale
                    ZoomOrScaleString = " Scale 1 for " & existingMapperScale
                Else
                    'dynamic scale based on attribute column
                    'get frame and put in mainFrame
                    getMapperFrameInLayout(LayoutIDList(ComboBox2.SelectedIndex), MapperIDList(ComboBox8.SelectedIndex))
                    InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & Units & Chr(34))
                    frameWidth = InteropServices.MapInfoApplication.Eval("objectgeography(mainFrame,3) - objectgeography(mainFrame,1)")
                    InteropServices.MapInfoApplication.Do("Dim coord As String")
                    InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & mapperID & ", 17)")
                    InteropServices.MapInfoApplication.Do("Run Command coord")
                    'set mapper width to produce correct layout scale (may need to set to map units ?)
                    ZoomOrScale = frameWidth * CDbl(InteropServices.MapInfoApplication.Eval(tableName & "." & ComboBox9.Text))
                    ZoomOrScaleString = " Zoom " & ZoomOrScale & " units " & Chr(34) & Units & Chr(34)


                End If
            End If


            'see: Changing the Current View of the Map 
            InteropServices.MapInfoApplication.Do("Set Map window " & mapperID & " Center(" & minX + ((maxX - minX) / 2) & ", " & minY + ((maxY - minY) / 2) & ") " & ZoomOrScaleString)

            'draw an extent indicator on an overview map
            setupOverviewMapbox()

            'execute any page driven queries
            If pageDrivenQueryList.Count > 0 Then
                'place index page values into query
                substituteIndexValues()

                'run queries
                executeQueries()
            End If

        End Sub


        Sub exportJPG(ByVal theLayoutNum As Integer, ByVal outName As String, ByVal pageX As Double, ByVal pageY As Double)
            'units ? ... set to mm ???
            Dim recNumber As String
            recNumber = InteropServices.MapInfoApplication.Eval(ComboBox1.Text & ".rowid")
            outName = outName & "." & ComboBox7.Text

            Dim outputType As String = "JPEG"
            Select Case ComboBox7.Text
                Case "JPG"
                    outputType = "JPEG"
                Case "PNG"
                    outputType = "PNG"

            End Select

            'function to create map jpg
            InteropServices.MapInfoApplication.Do("Save Window windowID(" & theLayoutNum & ") As " & Chr(34) & outName & Chr(34) & " Type " & Chr(34) & outputType & Chr(34) & " Width " & pageX & "  Units " & Chr(34) & "mm" & Chr(34) & " Height " & pageY & "  Units " & Chr(34) & "mm" & Chr(34) & " Resolution " & NumericUpDown4.Value)
            InteropServices.MapInfoApplication.Do("Fetch rec " & recNumber & " From " & ComboBox1.Text)
        End Sub

        Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
            TabControl1.SelectedIndex = 1
        End Sub

        Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
            TabControl1.SelectedIndex = 2
        End Sub

        Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
            TabControl1.SelectedIndex = 3
        End Sub

        Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

            loadXML()
        End Sub


        Private Sub ToolStripDropDownButton1_Click(sender As Object, e As EventArgs) Handles ToolStripDropDownButton1.Click
            If ComboBox2.Enabled = False Then
                MsgBox("Layout window must be defined before page driven text can be added")
                Exit Sub
            End If
        End Sub


        'page text function

        Sub changeSaveWorkspaceMenu()
            'check if save workspace is disabled - if it is then DDP save is already there

            'alter the save workspace menu item to also save the xml
            InteropServices.MapInfoApplication.Do("Alter Menu " & Chr(34) & "File" & Chr(34) & " Add " & Chr(34) & "Save DDP Workspace" & Chr(34) & "  Calling saveDDPWorkspace")

            'change the save workspace button


        End Sub

        Sub saveWORKSPACE()
            'see if workspace is a DDP workspace (i.e. has an xml file)
            'if so place flag in workspace text file

            'autostart the tracking MBX when the workspace is loaded

        End Sub

        Function getNextLayoutRow(ByVal layoutName As String)
            'fetch only understands name

            Dim i As Integer = 0
            InteropServices.MapInfoApplication.Do("Fetch First From " & layoutName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & layoutName & ")") = "F"
                InteropServices.MapInfoApplication.Do("Fetch Next From " & layoutName)
                i = i + 1
            End While

            Return i + 1
        End Function

        Sub updatePageText()

            'exit if no page text
            If pageTextColumnHeaderList.Count = 0 Then Exit Sub

            Dim layoutID As String = LayoutIDList(ComboBox2.SelectedIndex)
            Dim LayoutN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 10 )")

            'set up temp object for exchange
            InteropServices.MapInfoApplication.Do("dim objectTemp as object")
            InteropServices.MapInfoApplication.Do("dim newValueTemp as string")

            'cycle through all rows pageTextRowIdList 
            For i As Integer = 0 To pageTextRowIdList.Count - 1

                'fetch correct row
                InteropServices.MapInfoApplication.Do("Fetch Rec " & pageTextRowIdList(i) & " From " & LayoutN)

                'put current text object into temp object variable
                InteropServices.MapInfoApplication.Do("objectTemp = " & LayoutN & ".obj")

                'if page driven text (not standard text)
                If pageTextColumnHeaderList(i).ToString.Contains("$") = False Then

                    'fetch correct record from index table
                    InteropServices.MapInfoApplication.Do("Fetch Rec " & ToolStripComboBox1.Text & " From " & ComboBox1.Text)
                    'override "Next page" if sort field is chosen
                    If sortList.Count > 0 Then
                        InteropServices.MapInfoApplication.Do("Fetch rec " & sortList(CInt(ToolStripComboBox1.Text) - 1).number & " From " & ComboBox1.Text)
                    End If

                    InteropServices.MapInfoApplication.Do("newValueTemp = " & ComboBox1.Text & "." & pageTextColumnHeaderList(i))
                Else
                    'if a standard item (i.e. scale, date etc...)
                    InteropServices.MapInfoApplication.Do("newValueTemp = " & returnStandardText(pageTextColumnHeaderList(i)))
                End If

                'update the object with new values
                InteropServices.MapInfoApplication.Do("alter object objectTemp info 3, newValueTemp")

                'update the layout text with the new object
                InteropServices.MapInfoApplication.Do("update " & LayoutN & " set obj = objectTemp Where Rowid = " & pageTextRowIdList(i))
            Next

        End Sub




        Sub CreateXML()
            'create xml to hold rowid,positions and contents of page text

            'place data in xml based on "pageText" array - which contains the data column and assoicated layout rowid

        End Sub

        Sub loadXML()
            'reset page text
            pageTextRowIdList.Clear()
            pageTextColumnHeaderList.Clear()

            'reset queries
            pageDrivenQueryList.Clear()
            DataGridView1.Rows.Clear()
            pageDrivenQueryForms.Clear()

            'find the xml file and load its contents into "pageText"
            Dim OFD As New OpenFileDialog
            OFD.Filter = "DDP xml files (*.ddp)|*.DDP"
            OFD.Title = "Pick a saved Data driven pages file"
            OFD.ShowDialog()

            If OFD.FileName = "" Then Exit Sub

            'does wor exist
            If File.Exists(OFD.FileName.Substring(0, OFD.FileName.Length - 3) & "wor") Then
                'workspace file exists - check dates of both
                Dim ddpINFO As Date = IO.File.GetLastWriteTime(OFD.FileName)
                Dim worINFO As Date = IO.File.GetLastWriteTime(OFD.FileName.Substring(0, OFD.FileName.Length - 3) & "wor")
                'are dates within a min of each other (i.e. has the wor been altered since saving)
                If (ddpINFO - worINFO).TotalSeconds > 5 Or (ddpINFO - worINFO).TotalSeconds < 5 Then
                    'warn user
                    Dim result As Integer = MessageBox.Show("Workspace has been altered since saving Data Driven Pages, loading may result in errors", "Continue ?", MessageBoxButtons.YesNo)
                    If result <> DialogResult.Yes Then Exit Sub
                End If

                'open wor
                'close all currently open files
                InteropServices.MapInfoApplication.Do("Close All")

                'open worspace
                InteropServices.MapInfoApplication.Do("Run Application " & Chr(34) & OFD.FileName.Substring(0, OFD.FileName.Length - 3) & "wor" & Chr(34))

            Else
                'only settings file exists
                Dim openResult As Integer = MessageBox.Show("No corrisponding workspace file found. An attempt can be made to match Data Driven Page settings to currently loaded files, errors may occur", "Continue ?", MessageBoxButtons.YesNo)
                If openResult <> DialogResult.Yes Then Exit Sub
            End If

            refreshDDP()

            'load xml
            Dim DDP As XDocument = XDocument.Load(OFD.FileName)
            'Dim DDP As XDocument = theFile...<DDP>

            ComboBox1.Text = DDP.<DDP>.<INDEXLAYER>.Value
            ComboBox8.Text = DDP.<DDP>.<INDEXMAPPER>.Value
            ComboBox2.Text = DDP.<DDP>.<INDEXLAYOUT>.Value
            ComboBox3.Text = DDP.<DDP>.<INDEXNAME>.Value
            ComboBox4.Text = DDP.<DDP>.<INDEXSORT>.Value

            RadioButton1.Checked = CBool(DDP.<DDP>.<MEBESTFIT>.Value)
            RadioButton2.Checked = CBool(DDP.<DDP>.<MEMAINTAIN>.Value)
            RadioButton3.Checked = CBool(DDP.<DDP>.<MEDDPSCALE>.Value)

            NumericUpDown1.Value = DDP.<DDP>.<MEPOINTBUFFER>.Value
            NumericUpDown2.Value = DDP.<DDP>.<MEPOLYBUFFER>.Value
            NumericUpDown3.Value = DDP.<DDP>.<MESCALEROUND>.Value

            ComboBox10.Text = DDP.<DDP>.<OVERVIEWMAPPER>.Value
            Panel1.BackColor = ColorTranslator.FromHtml(DDP.<DDP>.<OVERVIEWCOLOUR>.Value)
            NumericUpDown5.Value = DDP.<DDP>.<OVERVIEWWIDTH>.Value

            'page text
            Dim allTEXT As System.Collections.Generic.IEnumerable(Of System.Xml.Linq.XElement) = DDP.<DDP>.<PAGETEXT>.<TEXT>
            For Each pageTextItem As System.Xml.Linq.XElement In allTEXT
                pageTextRowIdList.Add(pageTextItem.<ROWID>.Value)
                pageTextColumnHeaderList.Add(pageTextItem.<COLNAME>.Value)
            Next

            'ddp queries
            Dim QI As New queryInfo
            Dim allQUERIES As System.Collections.Generic.IEnumerable(Of System.Xml.Linq.XElement) = DDP.<DDP>.<PAGEQUERIES>.<QUERY>
            For Each pageQItem As System.Xml.Linq.XElement In allQUERIES
                DataGridView1.Rows.Add()
                DataGridView1.Rows.Item(DataGridView1.Rows.Count - 1).Cells(0).Value = pageQItem.<QUERYID>.Value
                DataGridView1.Rows.Item(DataGridView1.Rows.Count - 1).Cells(1).Value = pageQItem.<QUERYLAYER>.Value
                pageDrivenQueryList.Add(pageQItem.<QUERYDEF>.Value)

                QI = New queryInfo
                QI.ComboBox1.Items.AddRange(tableNames.ToArray)
                QI.ComboBox1.Text = pageQItem.<QUERYLAYER>.Value

                'add table name to box to allow query of columns
                QI.tablename = ComboBox1.Text
                QI.TextBox1.Text = pageQItem.<QUERYDEF>.Value
                QI.setPageColumns()

                MapInfoDataDrivenPages.InteropHelper.theDlg.pageDrivenQueryForms.Add(QI)

            Next

        End Sub





        Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
            If ComboBox2.Text <> "" And ComboBox2.Text <> "No Vaild Layouts open" Then
                If areSettingsValid() = False Then
                    Exit Sub
                End If
            End If
            updatePageTextToSelectedNumber()
        End Sub

        Sub updatePageTextToSelectedNumber()
            'check layout name is still correct (only an issue for saving as DDP uses ID)
            hasLayoutNameChanged()

            If ToolStripComboBox1.Text <> "" And ComboBox2.Text <> "" Then
                InteropServices.MapInfoApplication.Do("Fetch Rec " & ToolStripComboBox1.Text & " From " & ComboBox1.Text)
                'override "Next page" if sort field is chosen
                If sortList.Count > 0 Then
                    InteropServices.MapInfoApplication.Do("Fetch rec " & sortList(CInt(ToolStripComboBox1.Text) - 1).number & " From " & ComboBox1.Text)
                End If

                'get selected index table
                Dim tableName As String = ComboBox1.Text
                Dim mapperID As Integer = MapperIDList(ComboBox8.SelectedIndex)

                'must set coordinate system based on map options - see profile tool
                'set coord system - based on mapper window. 
                InteropServices.MapInfoApplication.Do("Dim coord As String")
                InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & mapperID & ", 17)")
                InteropServices.MapInfoApplication.Do("Run Command coord")



                'move extent
                moveExtent()

                'do last as map must be moved first if scale is used
                updatePageText()
            End If
        End Sub

        Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
            'move 1 page on
            If areSettingsValid() = False Then
                Exit Sub
            End If

            If ComboBox8.Enabled = False Or ComboBox2.Enabled = False Then
                MsgBox("Both mapper and layout window must be defined before pages can be selected")
                Exit Sub
            End If
            If ToolStripComboBox1.Items.Count - 1 > ToolStripComboBox1.SelectedIndex Then
                ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.SelectedIndex + 1
            End If
        End Sub

        Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
            'move 1 page back
            If areSettingsValid() = False Then
                Exit Sub
            End If

            If ComboBox8.Enabled = False Or ComboBox2.Enabled = False Then
                MsgBox("Both mapper and layout window must be defined before pages can be selected")
                Exit Sub
            End If
            If ToolStripComboBox1.SelectedIndex > 0 Then
                ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.SelectedIndex - 1
            End If
        End Sub

        Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
            'last page
            If areSettingsValid() = False Then
                Exit Sub
            End If
            If ComboBox8.Enabled = False Or ComboBox2.Enabled = False Then
                MsgBox("Both mapper and layout window must be defined before pages can be selected")
                Exit Sub
            End If
            ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.Items.Count - 1
        End Sub

        Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
            'first page
            If areSettingsValid() = False Then
                Exit Sub
            End If
            If ComboBox8.Enabled = False Or ComboBox2.Enabled = False Then
                MsgBox("Both mapper and layout window must be defined before pages can be selected")
                Exit Sub
            End If
            ToolStripComboBox1.SelectedIndex = 0
        End Sub

        Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
            loadQueryInfo()
        End Sub

        Sub loadQueryInfo()
            Dim QI As New queryInfo
            QI.ComboBox1.Items.AddRange(tableNames.ToArray)
            'remove index layer - can't query against itslf !
            QI.ComboBox1.Items.RemoveAt(ComboBox1.SelectedIndex)

            'add table name to box to allow query of columns
            QI.tablename = ComboBox1.Text
            QI.setPageColumns()

            QI.ShowDialog()
        End Sub

        Sub executeQueries()
            'for each query
            Dim RandomClass As New Random()


            For i As Integer = 0 To pageDrivenQueryList.Count - 1

                'excute query and place into rand
                pageDrivenQueryTables.Add(RandomClass.Next(1000000, 9999999))
                'MsgBox("Select * from " & MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Item(i).Cells(1).Value & " where " & pageDrivenQueryListSub(i) & " into _" & pageDrivenQueryTables(i))
                InteropServices.MapInfoApplication.Do("Select * from " & MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Item(i).Cells(1).Value & " where " & pageDrivenQueryListSub(i) & " into _" & pageDrivenQueryTables(i))

                'add to mapper
                InteropServices.MapInfoApplication.Do("Add Map Window " & MapperIDList(ComboBox8.SelectedIndex) & " Auto Layer _" & pageDrivenQueryTables(i))

                'disable to prevent medeling
                InteropServices.MapInfoApplication.Do("Set Table " & pageDrivenQueryTables(i) & " ReadOnly UserMap off UserBrowse Off UserClose Off UserEdit Off UserRemoveMap Off UserDisplayMap Off ")
            Next

            'update the chosen mapper window in the setup tab - so saving produces the correct name
            Dim MapperIDExisting As String = MapperIDList(ComboBox8.SelectedIndex)
            ComboBox8.Items.Clear()
            populateListOfMappers()
            ComboBox8.Items.AddRange(MapperList.ToArray)
            ComboBox8.SelectedIndex = getMapperIndex(MapperIDExisting)





        End Sub

        Sub removeQueries()
            'cycle through random number table and remove each query from mapper
            For i As Integer = 0 To pageDrivenQueryTables.Count - 1
                InteropServices.MapInfoApplication.Do("Remove Map Window " & MapperIDList(ComboBox8.SelectedIndex) & " Layer _" & pageDrivenQueryTables(i))
                InteropServices.MapInfoApplication.Do("close table _" & pageDrivenQueryTables(i))
            Next
            'clear query lists
            pageDrivenQueryTables.Clear()
            pageDrivenQueryListSub.Clear()
        End Sub

        Sub substituteIndexValues()
            Dim queryTerms() As String
            pageDrivenQueryListSub.Clear()

            For i As Integer = 0 To pageDrivenQueryList.Count - 1
                queryTerms = pageDrivenQueryList(i).Split(" ")
                For j As Integer = 0 To queryTerms.Length - 1
                    If queryTerms(j).Contains(ComboBox1.Text & ".obj") Then
                        'if an object query get obj and put it in a variable
                        InteropServices.MapInfoApplication.Do("dim queryVar as object")
                        InteropServices.MapInfoApplication.Do("queryVar = " & queryTerms(j))
                        pageDrivenQueryListSub.Add(pageDrivenQueryList(i).Replace(queryTerms(j), " queryVar "))
                    ElseIf queryTerms(j).Contains(ComboBox1.Text & ".") Then
                        pageDrivenQueryListSub.Add(pageDrivenQueryList(i).Replace(queryTerms(j), Chr(34) & InteropServices.MapInfoApplication.Eval(queryTerms(j)) & Chr(34)))
                    End If

                Next

            Next
        End Sub

        'function to return new mapper name after page driven query has been added - this may be identical if window helper has overridden the window name
        Function getMapperIndex(ByVal mapperID As String) As Integer
            For i As Integer = 0 To MapperIDList.Count - 1
                If MapperIDList(i) = mapperID Then
                    getMapperIndex = i
                End If
            Next


        End Function

        Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
            pageDrivenQueryForms(DataGridView1.CurrentCellAddress.Y).caller = DataGridView1.CurrentCellAddress.Y
            pageDrivenQueryForms(DataGridView1.CurrentCellAddress.Y).ShowDialog()

        End Sub

        Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

            pageDrivenQueryList.RemoveAt(DataGridView1.CurrentCellAddress.Y)
            pageDrivenQueryForms.RemoveAt(DataGridView1.CurrentCellAddress.Y)
            DataGridView1.Rows.RemoveAt(DataGridView1.CurrentCellAddress.Y)
        End Sub

        Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged

        End Sub

        Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
            InteropServices.MapInfoApplication.Do("Run Menu Command 111")
        End Sub

        Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
            'kill queries first - because if they are object queries they will be based on a variable
            removeQueries()


            'save
            ' Create XmlWriterSettings.
            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.Indent = True
            settings.OmitXmlDeclaration = True
            settings.ConformanceLevel = ConformanceLevel.Auto

            Dim xmlstring As New System.Text.StringBuilder


            'pick a file to save to (default to workspace filepath)
            Dim FSA As New SaveFileDialog
            FSA.Filter = "DDP xml files (*.ddp)|*.DDP"
            FSA.Title = "choose a file name to save this data driven pages setup"
            FSA.ShowDialog()

            'is a valid filenme
            If FSA.FileName.Length < 3 Then
                Exit Sub
            End If

            'condense list of dynamic row ids (page text) as this table is packed on a " save workspace" removing all empty rows
            setPageTextPackedRowIdList()

            'create string for xml
            ' Create XmlWriter.
            Using writer As XmlWriter = XmlWriter.Create(xmlstring, settings)
                ' Begin writing.
                writer.WriteStartDocument()
                writer.WriteStartElement("DDP")

                'write basic info
                writer.WriteElementString("INDEXLAYER", ComboBox1.Text)
                writer.WriteElementString("INDEXMAPPER", ComboBox8.Text) '-NO!
                'index mapper may have data driven queries added to it - hmmmm.
                'solution - update chosen mapper after every page driven query - see executequeries
                writer.WriteElementString("INDEXLAYOUT", ComboBox2.Text)
                writer.WriteElementString("INDEXNAME", ComboBox3.Text)
                writer.WriteElementString("INDEXSORT", ComboBox4.Text)

                writer.WriteElementString("MEBESTFIT", RadioButton1.Checked)
                writer.WriteElementString("MEMAINTAIN", RadioButton2.Checked)
                writer.WriteElementString("MEDDPSCALE", RadioButton3.Checked)

                writer.WriteElementString("MEPOINTBUFFER", NumericUpDown1.Value)
                writer.WriteElementString("MEPOLYBUFFER", NumericUpDown2.Value)
                writer.WriteElementString("MESCALEROUND", NumericUpDown3.Value)

                writer.WriteElementString("MEDDPCOL", ComboBox9.Text) 'ddp scle column

                writer.WriteElementString("OVERVIEWMAPPER", ComboBox10.Text)
                writer.WriteElementString("OVERVIEWCOLOUR", ColorTranslator.ToHtml(CD.Color))
                writer.WriteElementString("OVERVIEWWIDTH", NumericUpDown5.Value)

                'write page text xml
                writer.WriteStartElement("PAGETEXT")
                For i As Integer = 0 To pageTextPackedRowIdList.Count - 1
                    writer.WriteStartElement("TEXT")
                    writer.WriteElementString("ROWID", pageTextPackedRowIdList(i))
                    writer.WriteElementString("COLNAME", pageTextPackedColumnHeaderList(i))
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                'write page query xml
                writer.WriteStartElement("PAGEQUERIES")
                For i As Integer = 0 To DataGridView1.Rows.Count - 1
                    writer.WriteStartElement("QUERY")
                    writer.WriteElementString("QUERYID", i)
                    writer.WriteElementString("QUERYLAYER", DataGridView1.Rows.Item(i).Cells(1).Value)
                    writer.WriteElementString("QUERYDEF", pageDrivenQueryList(i))
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndDocument()
            End Using
            'write page file
            Dim fileToSave As New System.IO.StreamWriter(FSA.FileName)
            fileToSave.WriteLine(xmlstring.ToString)
            fileToSave.Close()

            'write workspace - this will force pack the layoutN table to bring it in line with the DDP Xml
            InteropServices.MapInfoApplication.Do("Save Workspace As " & Chr(34) & FSA.FileName.Substring(0, FSA.FileName.Length - 3) & "wor" & Chr(34))

        End Sub



        Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
            'show index table
            If ComboBox1.Text <> "" Then
                InteropServices.MapInfoApplication.Do("Browse * From " & ComboBox1.Text)
            Else
                MsgBox("No index layer selected")
            End If

        End Sub

        Sub setPageTextPackedRowIdList()
            Dim layoutID As String = LayoutIDList(ComboBox2.SelectedIndex)
            Dim layoutName As String = ComboBox2.Text
            Dim LayoutN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 10 )")
            Dim validRowCount As Integer = 1
            pageTextPackedRowIdList.Clear()
            pageTextPackedColumnHeaderList.Clear()

            InteropServices.MapInfoApplication.Do("Fetch first From " & LayoutN)
            While InteropServices.MapInfoApplication.Eval("EOT(" & LayoutN & ")") = "F"
                'is it a dynamic row ?  
                If isDynamicRow(InteropServices.MapInfoApplication.Eval(LayoutN & ".rowid")) <> -1 Then
                    pageTextPackedRowIdList.Add(validRowCount)
                    pageTextPackedColumnHeaderList.Add(pageTextColumnHeaderList(isDynamicRow(InteropServices.MapInfoApplication.Eval(LayoutN & ".rowid"))))
                End If
                validRowCount = validRowCount + 1

                InteropServices.MapInfoApplication.Do("Fetch next From " & LayoutN)
            End While

        End Sub

        Function isDynamicRow(ByVal rowNum As Integer) As Integer
            'cycle through PageTextRowIdList and return value indicating if the row number in question is present
            isDynamicRow = -1
            For x As Integer = 0 To pageTextRowIdList.Count - 1
                'MsgBox("d row to compare to:" & pageTextRowIdList(x) & ", " & rowNum)
                If pageTextRowIdList(x) = rowNum Then
                    isDynamicRow = x
                End If
            Next

        End Function

        Sub getMapperFrameInLayout(ByVal layoutID As String, ByVal mapperID As String)
            'sets an object var in MB to the first frame with the mapper id linked

            Dim LayoutN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 10 )")


            InteropServices.MapInfoApplication.Do("Fetch First From " & LayoutN)
            For i As Integer = 1 To InteropServices.MapInfoApplication.Eval("tableinfo(" & LayoutN & " , 8)")
                If InteropServices.MapInfoApplication.Eval("EOT(" & LayoutN & ")") = "F" Then
                    If InteropServices.MapInfoApplication.Eval("objectinfo(" & LayoutN & ".obj,1)") = 6 Then
                        'if its a frame (need to get its size in page units to get frame scale
                        If InteropServices.MapInfoApplication.Eval("objectinfo(" & LayoutN & ".obj,4)") = mapperID Then
                            'if it points to the chosen mapper
                            'set as mainFrame object in MB
                            InteropServices.MapInfoApplication.Do("dim mainFrame as object")
                            InteropServices.MapInfoApplication.Do("mainFrame = " & LayoutN & ".obj")
                        End If

                    End If
                End If
                InteropServices.MapInfoApplication.Do("Fetch Next From " & LayoutN)
            Next

        End Sub


        Sub setupOverviewMapbox()

            If ComboBox10.Text = "" Then Exit Sub

            Dim frameWidth, frameheight As String
            Dim minX, maxX, minY, maxY As Double
            Dim Units As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & MapperIDList(ComboBox10.SelectedIndex) & ", 11)")

            'get frame in ayout and put in MB mainframe
            getMapperFrameInLayout(LayoutIDList(ComboBox2.SelectedIndex), MapperIDList(ComboBox8.SelectedIndex))

            'get frame dimensions
            InteropServices.MapInfoApplication.Do("Set CoordSys Layout Units " & Chr(34) & Units & Chr(34))
            frameWidth = InteropServices.MapInfoApplication.Eval("objectgeography(mainFrame,3) - objectgeography(mainFrame,1)")
            frameheight = InteropServices.MapInfoApplication.Eval("objectgeography(mainFrame,4) - objectgeography(mainFrame,2)")
            'set coord system - based on mapper window. 
            InteropServices.MapInfoApplication.Do("Dim coord As String")
            InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & MapperIDList(ComboBox10.SelectedIndex) & ", 17)")
            InteropServices.MapInfoApplication.Do("Run Command coord")

            'get viewport min/max x/y
            'frame width represents mapper width, so min/max x can be taken directly from mapper extents
            minX = InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ",5)")
            maxX = InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ",7)")
            maxY = InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ",4)") + (((maxX - minX) * (frameheight / frameWidth)) / 2)
            minY = InteropServices.MapInfoApplication.Eval("mapperinfo(" & MapperIDList(ComboBox8.SelectedIndex) & ",4)") - (((maxX - minX) * (frameheight / frameWidth)) / 2)

            'delete all from cosmetic layer
            Dim CosmeticN As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & MapperIDList(ComboBox10.SelectedIndex) & ", 10 )")
            InteropServices.MapInfoApplication.Do("delete object from " & CosmeticN)

            'set cosmetic layer editable
            InteropServices.MapInfoApplication.Do("set map window " & MapperIDList(ComboBox10.SelectedIndex) & " layer 0 editable")

            'draw new box on cosmetic layer
            InteropServices.MapInfoApplication.Do("Create Pline Into Window " & MapperIDList(ComboBox10.SelectedIndex) & "  Multiple 1 5 ( " & minX & "," & minY & ") (" & minX & "," & maxY & " ) (" & maxX & ", " & maxY & " ) (" & maxX & ", " & minY & " )( " & minX & ", " & minY & ") Pen makepen(" & NumericUpDown5.Value & ", 2, " & InteropServices.MapInfoApplication.Eval("RGB(" & CD.Color.R & "," & CD.Color.G & "," & CD.Color.B & ")") & " )")


        End Sub


        Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
            CD.ShowDialog()
            Panel1.BackColor = CD.Color
        End Sub


        Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
            printPages()
        End Sub

        Sub printPages()
            exportType = 2
            cycleThroughFeatures()
        End Sub

        Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
            If CheckBox1.Checked Then
                ComboBox7.SelectedIndex = 2
            End If
        End Sub

        Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
            pageSelectAll()
        End Sub

        Sub pageSelectAll()
            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemChecked(i, True)
            Next
        End Sub

        Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemChecked(i, False)
            Next
        End Sub

        Private Sub ComboBox8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox8.SelectedIndexChanged

        End Sub


        Function areColumnValuesUnique(ByVal colName As String) As Boolean
            Dim tablename As String = ComboBox1.Text
            Dim allValues As New List(Of String)

            Dim i As Integer = 0
            Dim tempValue As String = ""
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                allValues.Add(InteropServices.MapInfoApplication.Eval(tablename & "." & colName))
                i = i + 1
                InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)
            End While

            Dim uniqueVals As Long = allValues.Distinct.LongCount
            If uniqueVals = i Then
                Return True
            Else
                Return False
            End If

        End Function

        Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
            Dim OFD As New FolderBrowserDialog
            OFD.ShowDialog()

            TextBox1.Text = OFD.SelectedPath

        End Sub

        Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
            setSortOrder()
        End Sub

        Sub setSortOrder()
            Dim sortField As String = ComboBox4.Text
            Dim tableName As String = ComboBox1.Text
            Dim row As Integer = 1


            sortList.Clear()
            Dim currentSortItem As pageSortData

            'get the sort field values for each row and put in new pageSortData class, then add to list
            InteropServices.MapInfoApplication.Do("Fetch first From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                ' 
                currentSortItem = New pageSortData
                currentSortItem.number = row
                currentSortItem.sortField = InteropServices.MapInfoApplication.Eval(tableName & "." & sortField)

                sortList.Add(currentSortItem)

                InteropServices.MapInfoApplication.Do("Fetch next From " & tableName)
                row = row + 1
            End While

            'sort based on sortfield
            sortList = sortList.OrderBy(Function(x) x.sortField).ToList

            'For x As Integer = 0 To sortList.Count - 1
            'MsgBox(sortList(x).number & ", " & sortList(x).sortField)
            'Next

        End Sub

        Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
            Dim pdf As New PDFHelp
            pdf.Show()
        End Sub

        Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
            pageDrivenQueryForms(DataGridView1.CurrentCellAddress.Y).caller = DataGridView1.CurrentCellAddress.Y
            pageDrivenQueryForms(DataGridView1.CurrentCellAddress.Y).ShowDialog()
        End Sub


        Sub hasLayoutNameChanged()
            If ComboBox2.SelectedItem <> "" Then
                Dim layoutID As Integer = LayoutIDList(ComboBox2.SelectedIndex)
                'this sub determines if the layout name (title) has changed.... if it has it will update combobox2.
                'layout names change if the user changes the window title (i.e. PHBs' Window helper) or if another layout of the same name is added. i.e 'layout' becomes layout:2

                'get layout name from list (layoutname when DDp were created)
                Dim theLayoutName As String = LayoutList(ComboBox2.SelectedIndex)

                'get layout name from ID (in layoutIDList) - i.e. the layout name now
                Dim theNEWLayoutName As String = InteropServices.MapInfoApplication.Eval("WindowInfo(" & layoutID & ", 1)")

                'if layout has been closed
                If IsNothing(theNEWLayoutName) Then
                    Dim currentIndexLayer As String = ComboBox1.Text
                    refreshDDP()
                    ComboBox1.Text = currentIndexLayer
                    Exit Sub
                End If

                'if different update combobox.
                If theLayoutName <> theNEWLayoutName Then
                    ComboBox2.Items.Item(ComboBox2.SelectedIndex) = theNEWLayoutName
                End If
            End If
        End Sub

        'validate data driven scale
        Sub validateScaleColumn()
            InteropServices.MapInfoApplication.Do("Fetch first From " & ComboBox1.Text)
            While InteropServices.MapInfoApplication.Eval("EOT(" & ComboBox1.Text & ")") = "F"
                If IsNumeric(InteropServices.MapInfoApplication.Eval(ComboBox1.Text & "." & ComboBox9.Text)) = False Then
                    MsgBox("Non numeric or invalid scales detected in column:" & ComboBox9.Text & vbNewLine & "Errors may occur if you choose to use this field as a scale source")
                    Exit Sub
                End If
                InteropServices.MapInfoApplication.Do("Fetch next From " & ComboBox1.Text)
            End While
        End Sub

        'more Bl*!$y validation
        Private Sub ComboBox9_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox9.SelectedIndexChanged
            validateScaleColumn()
        End Sub

        Function isIndexLayerStillOpen() As Boolean
            'cycle through all layers
            Dim NumberOfTablesOpen As Integer
            'find number of open tables
            NumberOfTablesOpen = InteropServices.MapInfoApplication.Eval("NumTables()")

            'cycle through all open tables (don't need to be in mapper) to find vector layers
            For i As Integer = 1 To NumberOfTablesOpen
                If InteropServices.MapInfoApplication.Eval("tableinfo(" & i & ", 1)") = ComboBox1.Text Then Return True
            Next

            Return False
        End Function

        Function isMapperStillOpen() As Boolean
            'cycle through all layers
            Dim NumberOfWinOpen As Integer
            'find number of open tables
            NumberOfWinOpen = InteropServices.MapInfoApplication.Eval("NumWindows()")

            'cycle through all open tables (don't need to be in mapper) to find vector layers
            For i As Integer = 1 To NumberOfWinOpen
                If InteropServices.MapInfoApplication.Eval("windowid(" & i & ")") = MapperIDList(ComboBox8.SelectedIndex) Then Return True
            Next

            Return False
        End Function

        Function isLayoutStillOpen() As Boolean
            'cycle through all layers
            Dim NumberOfWinOpen As Integer
            'find number of open tables
            NumberOfWinOpen = InteropServices.MapInfoApplication.Eval("NumWindows()")

            'cycle through all open tables (don't need to be in mapper) to find vector layers
            For i As Integer = 1 To NumberOfWinOpen
                If InteropServices.MapInfoApplication.Eval("windowid(" & i & ")") = LayoutIDList(ComboBox2.SelectedIndex) Then Return True
            Next

            Return False
        End Function

        Function isOverviewStillOpen() As Boolean
            'cycle through all layers
            Dim NumberOfWinOpen As Integer
            'find number of open tables
            NumberOfWinOpen = InteropServices.MapInfoApplication.Eval("NumWindows()")

            'cycle through all open tables (don't need to be in mapper) to find vector layers
            For i As Integer = 1 To NumberOfWinOpen
                If InteropServices.MapInfoApplication.Eval("windowinfo(" & i & ",1)") = ComboBox10.Text Then Return True
            Next

            Return False
        End Function

        Function areSettingsValid() As Boolean
            Dim errorMSG As String = ""

            If isIndexLayerStillOpen() = False Then
                errorMSG = errorMSG & "Index Layer not found" & vbNewLine
            End If

            If isMapperStillOpen() = False Then
                errorMSG = errorMSG & "Mapper Window not found" & vbNewLine
            End If

            If isLayoutStillOpen() = False Then
                errorMSG = errorMSG & "Layout Window not found" & vbNewLine
            End If

            If errorMSG.Length > 0 Then
                MsgBox("The following errors were found:" & vbNewLine & errorMSG)
                Return False
            End If

            Return True
        End Function
    End Class












    Public Class pageSortData
        Private _number As String
        Public Property number() As String
            Get
                Return _number
            End Get
            Set(ByVal value As String)
                _number = value
            End Set
        End Property

        Private _sortField As String
        Public Property sortField() As String
            Get
                Return _sortField
            End Get
            Set(ByVal value As String)
                _sortField = value
            End Set
        End Property


    End Class
End Namespace