Imports MapInfo.MiPro.Interop

Public Class PackTable


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'get mapper id

        'is table only layer in mapper


        InteropServices.MapInfoApplication.Do("Commit Table " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text)
        'InteropServices.MapInfoApplication.Do("print " & Chr(34) & "Commit Table " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text & Chr(34))
        InteropServices.MapInfoApplication.Do("Pack Table " & Label4.Text & " Graphic Data ")
        Me.Hide()
        'MapInfoDataDrivenPages.InteropHelper.theDlg.refreshDDP()
        MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text = Label4.Text

        'if only layer in mapper open new mapper, add layer and set combobox8 to the new mapper

        'else add to previous mapper

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        Me.Hide()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Items.RemoveAt(MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.SelectedIndex)
        Me.Hide()
    End Sub
End Class