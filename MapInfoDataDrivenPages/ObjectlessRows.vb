Imports MapInfo.MiPro.Interop

Public Class ObjectlessRows

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim i As Integer = 1
        Dim tempValue As String = ""
        InteropServices.MapInfoApplication.Do("Fetch First From " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text)
        While InteropServices.MapInfoApplication.Eval("EOT(" & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text & ")") = "F"
            tempValue = InteropServices.MapInfoApplication.Eval("objectinfo(" & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text & ".obj, 1)")
            If tempValue = 1 Or tempValue = 2 Or tempValue = 3 Or tempValue = 4 Or tempValue = 5 Or tempValue = 6 Or tempValue = 7 Or tempValue = 8 Or tempValue = 9 Then
                'is a valid row
            Else
                'invalid (no object data)
                InteropServices.MapInfoApplication.Do("delete from " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text & " Where Rowid = " & i)
            End If
            i = i + 1

            'End If
            InteropServices.MapInfoApplication.Do("Fetch Next From " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text)

        End While

        'now pack table
        InteropServices.MapInfoApplication.Do("Commit Table " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text)
        InteropServices.MapInfoApplication.Do("Pack Table " & MapInfoDataDrivenPages.InteropHelper.theDlg.ComboBox1.Text & " Graphic Data ")


        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
    End Sub
End Class