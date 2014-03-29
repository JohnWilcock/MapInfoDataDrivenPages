Imports MapInfo.MiPro.Interop

Public Class queryInfo

    Public caller As Integer = 9999

    Public selColumnList As New List(Of String)
    Public tablename As String

    Private Sub queryInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'disable textbox
        TextBox1.Enabled = False

        'only add handlers if first load
        If caller = 9999 Then
            'add event handlers to operators drop down
            For i As Integer = 0 To ToolStripDropDownButton1.DropDownItems.Count - 1
                AddHandler ToolStripDropDownButton1.DropDownItems(i).Click, AddressOf addQOperator
            Next
        End If
    End Sub

    Sub addQOperator(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim theOperator As String = CType(sender, ToolStripMenuItem).Text
        TextBox1.Text = TextBox1.Text & " " & theOperator & " "
    End Sub


    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        'get columns
        getColumnsOfChosenTable(ComboBox1.Text)
        ToolStripButton2.DropDownItems.Clear()
        For x As Integer = 0 To selColumnList.Count - 1
            ToolStripButton2.DropDownItems.Add(selColumnList(x))
            'ToolStripDropDownButton1.DropDownItems(x).
            AddHandler ToolStripButton2.DropDownItems(x).Click, AddressOf addQLayerText

        Next
    End Sub

    Sub setPageColumns()
        getColumnsOfChosenTable(tablename)
        ToolStripButton1.DropDownItems.Clear()
        For x As Integer = 0 To selColumnList.Count - 1
            ToolStripButton1.DropDownItems.Add(selColumnList(x))
            'ToolStripDropDownButton1.DropDownItems(x).
            AddHandler ToolStripButton1.DropDownItems(x).Click, AddressOf addQPageText

        Next


    End Sub

    Sub addQLayerText(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim column As String = CType(sender, ToolStripMenuItem).Text
        TextBox1.Text = TextBox1.Text & " " & ComboBox1.Text & "." & column
    End Sub

    Sub addQPageText(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim column As String = CType(sender, ToolStripMenuItem).Text
        TextBox1.Text = TextBox1.Text & " " & tablename & "." & column
    End Sub


    'getcolumns
    Sub getColumnsOfChosenTable(ByVal tableName As String)
        selColumnList.Clear()

        Dim numberOfColumns As Integer
        numberOfColumns = InteropServices.MapInfoApplication.Eval("numcols(" & tableName & ")")

        For i As Integer = 1 To numberOfColumns
            selColumnList.Add(InteropServices.MapInfoApplication.Eval("columninfo(" & tableName & ", COL" & i & ", 1)"))

        Next
        selColumnList.Add("obj")

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        addQuery()
    End Sub

    Sub addQuery()
        If caller = 9999 Then ' if a new query 
            MapInfoDataDrivenPages.InteropHelper.theDlg.pageDrivenQueryList.Add(TextBox1.Text)

            MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Add(1)
            'MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Count-1
            MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Item(MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Count - 1).Cells(0).Value = MapInfoDataDrivenPages.InteropHelper.theDlg.pageDrivenQueryList.Count
            MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Item(MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Count - 1).Cells(1).Value = ComboBox1.Text

            MapInfoDataDrivenPages.InteropHelper.theDlg.pageDrivenQueryForms.Add(Me)
            Me.Hide()
        Else ' if an existing query called by a click
            MapInfoDataDrivenPages.InteropHelper.theDlg.DataGridView1.Rows.Item(caller).Cells(1).Value = ComboBox1.Text
            MapInfoDataDrivenPages.InteropHelper.theDlg.pageDrivenQueryList(caller) = TextBox1.Text
            Me.Hide()
        End If
    End Sub




    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        TextBox1.Enabled = True

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
    End Sub
End Class