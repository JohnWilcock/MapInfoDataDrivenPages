'/*****************************************************************************
'*       Author JWilcock 2014
'*      Profile Tool for native mapinfo Grids v1.0
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

        Public pageTextRowIdList As New List(Of String)
        Public pageTextColumnHeaderList As New List(Of String)

        'lists for interval xy
        Public Shared xIntervalList As New List(Of Double)
        Public Shared yIntervalList As New List(Of Double)
        Public Shared zIntervalList As New List(Of Double)
        Public Shared ELEVarray As New List(Of Double)






        Public Sub New()
            InitializeComponent()
            'mut = New Mutex(False, mutexName)

            'set standard text in combo boxes
            ComboBox7.Text = "JPG"
            NumericUpDown4.Value = 200   'resolution
            ComboBox10.Text = "mm" ' layout units for layout buffer
            NumericUpDown1.Value = 125
            NumericUpDown2.Value = 125
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
            populateListofIndexLayers()
            resetMapperAndLayoutPickers()
            resetSetupFields()


            'clear page text lists
            pageTextRowIdList.Clear()
            pageTextColumnHeaderList.Clear()
            pageList.Clear()
            ToolStripDropDownButton1.DropDownItems.Clear()
            ToolStripComboBox1.Items.Clear()
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

            populateListOfMappers()
            populateListOfLayouts()

            ComboBox8.Items.AddRange(MapperList.ToArray)
            ComboBox2.Items.AddRange(LayoutList.ToArray)
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

            getColumnsOfChosenTable(tableName)

            ComboBox3.Items.AddRange(ColumnList.ToArray)
            ComboBox4.Items.AddRange(ColumnList.ToArray)

            'add list of columns to page text 
            'TODO - also add object information posibilities e.g area, length
            ToolStripDropDownButton1.DropDownItems.Clear()
            For x As Integer = 0 To ColumnList.Count - 1
                ToolStripDropDownButton1.DropDownItems.Add(ColumnList(x))
                'ToolStripDropDownButton1.DropDownItems(x).
                AddHandler ToolStripDropDownButton1.DropDownItems(x).Click, AddressOf addPageText

            Next


        End Sub

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
            pageTextRowIdList.Add(currentLayoutRow)
            pageTextColumnHeaderList.Add(column)

            'check for xml file and add it.

        End Sub

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

            ToolStripComboBox1.Items.AddRange(pageList.ToArray)

        End Sub

        Function numberOfRecords(ByVal tableName As String) As Integer
            'fetch only understands tablename
            If IsNumeric(tableName) Then tableName = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableName & ", 1)") ' 1=table name

            Dim i As Integer = 0
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                If InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 11)") = "T" Then i = i + 1 '11=OBJ_INFO_NONEMPTY   |||  MI only returns strings
                InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)
                pageList.Add(i)
            End While

            Return i
        End Function

        Function hasObjectRecords(ByVal tableName As String) As Boolean
            'fetch only understands tablename
            If IsNumeric(tableName) Then tableName = InteropServices.MapInfoApplication.Eval("tableinfo(" & tableName & ", 1)") ' 1=table name

            Dim i As Integer = 0
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                If InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 11)") = "T" Then Return True '11=OBJ_INFO_NONEMPTY   |||  MI only returns strings
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

            populateListofColumnsFromLayer(ComboBox1.Text)
            resetColumnSetupFields()
            setupRecordNumbers(ComboBox1.Text)
            ToolStripComboBox1.SelectedIndex = 0
        End Sub




        '////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
            ExportToImages()
        End Sub

        Public Sub ExportToImages()
            cycleThroughFeatures()
        End Sub

        Public Sub cycleThroughFeatures()
            'set up MI dim for MBR object (.net cannot accept this
            InteropServices.MapInfoApplication.Do("dim currentObject as object")

            'set first page
            ToolStripComboBox1.SelectedIndex = 0

            'get selected index table
            Dim tableName As String = ComboBox1.Text
            Dim mapperID As Integer = MapperIDList(ComboBox1.SelectedIndex)
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

            Dim i As Integer = 1
            'for each feature
            'get feature MBR and zoom to it
            InteropServices.MapInfoApplication.Do("Fetch First From " & tableName)
            While InteropServices.MapInfoApplication.Eval("EOT(" & tableName & ")") = "F"
                'check if valid object (not all rows may have an object attached to them
                If InteropServices.MapInfoApplication.Eval("objectinfo(" & tableName & ".obj, 11)") = "T" Then

                    'move page number on 
                    'this will trigger pagetext update
                    If ToolStripComboBox1.SelectedIndex + 1 <> ToolStripComboBox1.Items.Count And i > 1 Then
                        ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.SelectedIndex + 1
                    End If



                End If
                'requires re-seting after updatePageText
                InteropServices.MapInfoApplication.Do("Fetch Rec " & i & " From " & tableName)


                'export chosen layout to image
                exportJPG(layoutID, Path.Combine(TextBox1.Text, InteropServices.MapInfoApplication.Eval(tableName & "." & ComboBox3.Text)), pageWidth, pageHeight)
                i = i + 1
                InteropServices.MapInfoApplication.Do("Fetch Next From " & tableName)
            End While






        End Sub



        Sub moveExtent()
            'set up MI dim for MBR object (.net cannot accept this
            InteropServices.MapInfoApplication.Do("dim currentObject as object")

            'for MBR 
            Dim minX, maxX, minY, maxY As Double

            'get selected index table
            Dim tableName As String = ComboBox1.Text
            Dim mapperID As Integer = MapperIDList(ComboBox1.SelectedIndex)
            'get selected layout name
            Dim layoutName As String = ComboBox2.Text
            Dim layoutID As Integer = LayoutIDList(ComboBox2.SelectedIndex)

            'get  units of MAP
            Dim Units As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim LayoutUnits As String = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 11)")
            Dim existingMapperScale As Double = InteropServices.MapInfoApplication.Eval("MapperInfo(" & mapperID & ", 2)")

            Dim ZoomOrScaleString As String = ""
            Dim ZoomOrScale As Double

            'move page number on 
            InteropServices.MapInfoApplication.Do("currentObject = MBR(" & tableName & ".obj)")

            'initialise coord values
            minX = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 1) ")
            maxX = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 3) ")
            minY = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 2) ")
            maxY = InteropServices.MapInfoApplication.Eval("ObjectGeography(" & tableName & ".obj, 4) ")

            If RadioButton1.Checked Then
                'if best fit scaling
                Select Case ComboBox6.Text
                    Case "Percentage"
                        ZoomOrScale = ((maxX - minX) / 100) * NumericUpDown2.Value
                    Case "Map Units"
                        ZoomOrScale = (maxX - minX) + (NumericUpDown2.Value * 2)
                    Case "Page Units"

                End Select

                ZoomOrScaleString = " Zoom " & ZoomOrScale & " units " & Chr(34) & Units & Chr(34)


            Else
                'map set by set scale 
                If RadioButton2.Checked Then
                    'static scale
                    ZoomOrScaleString = " Scale 1 for " & existingMapperScale
                Else
                    'dynamic scale based on attribute column
                    ZoomOrScaleString = " Scale 1 for " & InteropServices.MapInfoApplication.Eval(tableName & "." & ComboBox9.Text)

                End If
            End If


            'see: Changing the Current View of the Map 
            InteropServices.MapInfoApplication.Do("Set Map window " & mapperID & " Center(" & minX + ((maxX - minX) / 2) & ", " & minY + ((maxY - minY) / 2) & ") " & ZoomOrScaleString)



        End Sub


        Sub exportJPG(ByVal theLayoutNum As Integer, ByVal outName As String, ByVal pageX As Double, ByVal pageY As Double)
            'units ? ... set to mm ???

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

        End Sub

        Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
            TabControl1.SelectedIndex = 1
        End Sub

        Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
            TabControl1.SelectedIndex = 2
        End Sub

        Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

        End Sub

        Private Sub ToolStripDropDownButton1_Click(sender As Object, e As EventArgs) Handles ToolStripDropDownButton1.Click

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

                'fetch correct record from index table
                InteropServices.MapInfoApplication.Do("Fetch Rec " & ToolStripComboBox1.Text & " From " & ComboBox1.Text)

                InteropServices.MapInfoApplication.Do("newValueTemp = " & ComboBox1.Text & "." & pageTextColumnHeaderList(i))

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
            'find the xml file and load its contents into "pageText"


        End Sub





        Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
            updatePageTextToSelectedNumber()
        End Sub

        Sub updatePageTextToSelectedNumber()
            If ToolStripComboBox1.Text <> "" And ComboBox2.Text <> "" Then
                InteropServices.MapInfoApplication.Do("Fetch Rec " & ToolStripComboBox1.Text & " From " & ComboBox1.Text)

                'get selected index table
                Dim tableName As String = ComboBox1.Text
                Dim mapperID As Integer = MapperIDList(ComboBox1.SelectedIndex)

                'must set coordinate system based on map options - see profile tool
                'set coord system - based on mapper window. 
                InteropServices.MapInfoApplication.Do("Dim coord As String")
                InteropServices.MapInfoApplication.Do("coord = " & Chr(34) & "Set " & Chr(34) & " + mapperinfo(" & mapperID & ", 17)")
                InteropServices.MapInfoApplication.Do("Run Command coord")

                updatePageText()

                'move extent
                moveExtent()
            End If
        End Sub

        Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
            'move 1 page on
            If ToolStripComboBox1.Items.Count - 1 > ToolStripComboBox1.SelectedIndex Then
                ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.SelectedIndex + 1
            End If
        End Sub

        Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
            'move 1 page back
            If ToolStripComboBox1.SelectedIndex > 0 Then
                ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.SelectedIndex - 1
            End If
        End Sub

        Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
            'last page
            ToolStripComboBox1.SelectedIndex = ToolStripComboBox1.Items.Count - 1
        End Sub

        Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
            'first page
            ToolStripComboBox1.SelectedIndex = 0
        End Sub
    End Class
End Namespace