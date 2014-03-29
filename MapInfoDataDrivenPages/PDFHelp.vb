Public Class PDFHelp

    Dim PDFpics As New List(Of Image)
    Dim PDFInstructions As New List(Of String)
    Dim num As Integer = 0

    Private Sub PDFHelp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '= Global.My.Resources.Resources.GotoLastRow_288

        PDFpics.Add(Global.My.Resources.Resources.pdf1)
        PDFpics.Add(Global.My.Resources.Resources.pdf2)
        PDFpics.Add(Global.My.Resources.Resources.pdf3)
        PDFpics.Add(Global.My.Resources.Resources.pdf4)

        PDFpics.Add(Global.My.Resources.Resources.pdf5)
        PDFpics.Add(Global.My.Resources.Resources.pdf6)
        PDFpics.Add(Global.My.Resources.Resources.pdf7)
        PDFpics.Add(Global.My.Resources.Resources.pdf8)


        PDFInstructions.Add("Select the 'Print to PDF' Button from the menubar")
        PDFInstructions.Add("Select the button called 'PDF' from this dialog box")
        PDFInstructions.Add("UN-CHECK the box called 'Open PDF Automaticly' and press OK, then exit the 'Print to PDF' dialog box")
        PDFInstructions.Add("Select the 'Print properties' button on the Data Driven Pages 'Export' Tab")

        PDFInstructions.Add("Then select the 'Printer' button")
        PDFInstructions.Add("1. Ensure the printer is set to the MapInfo PDF Printer " & vbNewLine & "2. Press the 'Properties' button.")
        PDFInstructions.Add("1. Select the 'Destination' Tab" & vbNewLine & "2. Ensure the 'Preview' and 'Prompt for destination' boxes are UN-CHECKED" & vbNewLine & "3. Select 'file system' and then press 'Options'")
        PDFInstructions.Add("Ensure the options in this box match those shown above - 'Default filename' should be the path and name of your output file")

        TextBox1.Text = PDFInstructions(0)
        PictureBox1.Image = PDFpics(0)

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If num < 7 Then
            num = num + 1
            PictureBox1.Image = PDFpics(num)
            TextBox1.Text = PDFInstructions(num)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If num > 0 Then
            num = num - 1
            PictureBox1.Image = PDFpics(num)
            TextBox1.Text = PDFInstructions(num)
        End If
    End Sub
End Class