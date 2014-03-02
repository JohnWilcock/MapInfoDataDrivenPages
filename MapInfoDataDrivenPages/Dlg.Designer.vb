Imports System.io
Imports System.Reflection

Namespace MapInfoDataDrivenPages
    Partial Class Dlg
        ''' <summary> 
        ''' Required designer variable. 
        ''' </summary> 
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used. 
        ''' </summary> 
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param> 
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary> 
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor. 
        ''' </summary> 
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Dlg))
            Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
            Me.TabControl1 = New System.Windows.Forms.TabControl()
            Me.TabPage1 = New System.Windows.Forms.TabPage()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.Label12 = New System.Windows.Forms.Label()
            Me.ComboBox8 = New System.Windows.Forms.ComboBox()
            Me.Label11 = New System.Windows.Forms.Label()
            Me.Button4 = New System.Windows.Forms.Button()
            Me.Label10 = New System.Windows.Forms.Label()
            Me.Button1 = New System.Windows.Forms.Button()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.ComboBox2 = New System.Windows.Forms.ComboBox()
            Me.ComboBox4 = New System.Windows.Forms.ComboBox()
            Me.ComboBox1 = New System.Windows.Forms.ComboBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.ComboBox3 = New System.Windows.Forms.ComboBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.TabPage2 = New System.Windows.Forms.TabPage()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.Button6 = New System.Windows.Forms.Button()
            Me.ComboBox9 = New System.Windows.Forms.ComboBox()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.ComboBox10 = New System.Windows.Forms.ComboBox()
            Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
            Me.ComboBox6 = New System.Windows.Forms.ComboBox()
            Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.ComboBox5 = New System.Windows.Forms.ComboBox()
            Me.RadioButton3 = New System.Windows.Forms.RadioButton()
            Me.RadioButton2 = New System.Windows.Forms.RadioButton()
            Me.RadioButton1 = New System.Windows.Forms.RadioButton()
            Me.TabPage3 = New System.Windows.Forms.TabPage()
            Me.GroupBox5 = New System.Windows.Forms.GroupBox()
            Me.Label13 = New System.Windows.Forms.Label()
            Me.ProgressBar2 = New System.Windows.Forms.ProgressBar()
            Me.ComboBox11 = New System.Windows.Forms.ComboBox()
            Me.Button3 = New System.Windows.Forms.Button()
            Me.GroupBox4 = New System.Windows.Forms.GroupBox()
            Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
            Me.Button5 = New System.Windows.Forms.Button()
            Me.TextBox1 = New System.Windows.Forms.TextBox()
            Me.NumericUpDown4 = New System.Windows.Forms.NumericUpDown()
            Me.Label9 = New System.Windows.Forms.Label()
            Me.Label8 = New System.Windows.Forms.Label()
            Me.ComboBox7 = New System.Windows.Forms.ComboBox()
            Me.Button2 = New System.Windows.Forms.Button()
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
            Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
            Me.ToolStripButton6 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton7 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
            Me.TabPage4 = New System.Windows.Forms.TabPage()
            Me.ToolStripContainer1.ContentPanel.SuspendLayout()
            Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
            Me.ToolStripContainer1.SuspendLayout()
            Me.TabControl1.SuspendLayout()
            Me.TabPage1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox3.SuspendLayout()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TabPage3.SuspendLayout()
            Me.GroupBox5.SuspendLayout()
            Me.GroupBox4.SuspendLayout()
            CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStripContainer1
            '
            Me.ToolStripContainer1.BottomToolStripPanelVisible = False
            '
            'ToolStripContainer1.ContentPanel
            '
            Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.TabControl1)
            resources.ApplyResources(Me.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel")
            resources.ApplyResources(Me.ToolStripContainer1, "ToolStripContainer1")
            Me.ToolStripContainer1.LeftToolStripPanelVisible = False
            Me.ToolStripContainer1.Name = "ToolStripContainer1"
            Me.ToolStripContainer1.RightToolStripPanelVisible = False
            '
            'ToolStripContainer1.TopToolStripPanel
            '
            Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.ToolStrip1)
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(Me.TabPage1)
            Me.TabControl1.Controls.Add(Me.TabPage2)
            Me.TabControl1.Controls.Add(Me.TabPage4)
            Me.TabControl1.Controls.Add(Me.TabPage3)
            resources.ApplyResources(Me.TabControl1, "TabControl1")
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            '
            'TabPage1
            '
            Me.TabPage1.Controls.Add(Me.GroupBox2)
            resources.ApplyResources(Me.TabPage1, "TabPage1")
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.UseVisualStyleBackColor = True
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Label12)
            Me.GroupBox2.Controls.Add(Me.ComboBox8)
            Me.GroupBox2.Controls.Add(Me.Label11)
            Me.GroupBox2.Controls.Add(Me.Button4)
            Me.GroupBox2.Controls.Add(Me.Label10)
            Me.GroupBox2.Controls.Add(Me.Button1)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Controls.Add(Me.ComboBox2)
            Me.GroupBox2.Controls.Add(Me.ComboBox4)
            Me.GroupBox2.Controls.Add(Me.ComboBox1)
            Me.GroupBox2.Controls.Add(Me.Label3)
            Me.GroupBox2.Controls.Add(Me.ComboBox3)
            Me.GroupBox2.Controls.Add(Me.Label1)
            Me.GroupBox2.Controls.Add(Me.Label2)
            resources.ApplyResources(Me.GroupBox2, "GroupBox2")
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.TabStop = False
            '
            'Label12
            '
            resources.ApplyResources(Me.Label12, "Label12")
            Me.Label12.Name = "Label12"
            '
            'ComboBox8
            '
            Me.ComboBox8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox8.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox8, "ComboBox8")
            Me.ComboBox8.Name = "ComboBox8"
            '
            'Label11
            '
            resources.ApplyResources(Me.Label11, "Label11")
            Me.Label11.Name = "Label11"
            '
            'Button4
            '
            resources.ApplyResources(Me.Button4, "Button4")
            Me.Button4.Name = "Button4"
            Me.Button4.UseVisualStyleBackColor = True
            '
            'Label10
            '
            resources.ApplyResources(Me.Label10, "Label10")
            Me.Label10.Name = "Label10"
            '
            'Button1
            '
            resources.ApplyResources(Me.Button1, "Button1")
            Me.Button1.Name = "Button1"
            Me.Button1.UseVisualStyleBackColor = True
            '
            'Label4
            '
            resources.ApplyResources(Me.Label4, "Label4")
            Me.Label4.Name = "Label4"
            '
            'ComboBox2
            '
            Me.ComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox2.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox2, "ComboBox2")
            Me.ComboBox2.Name = "ComboBox2"
            '
            'ComboBox4
            '
            Me.ComboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox4.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox4, "ComboBox4")
            Me.ComboBox4.Name = "ComboBox4"
            '
            'ComboBox1
            '
            Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox1.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox1, "ComboBox1")
            Me.ComboBox1.Name = "ComboBox1"
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'ComboBox3
            '
            Me.ComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox3.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox3, "ComboBox3")
            Me.ComboBox3.Name = "ComboBox3"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'TabPage2
            '
            Me.TabPage2.Controls.Add(Me.GroupBox1)
            resources.ApplyResources(Me.TabPage2, "TabPage2")
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.UseVisualStyleBackColor = True
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Button6)
            Me.GroupBox1.Controls.Add(Me.ComboBox9)
            Me.GroupBox1.Controls.Add(Me.Label7)
            Me.GroupBox1.Controls.Add(Me.NumericUpDown3)
            Me.GroupBox1.Controls.Add(Me.GroupBox3)
            Me.GroupBox1.Controls.Add(Me.RadioButton3)
            Me.GroupBox1.Controls.Add(Me.RadioButton2)
            Me.GroupBox1.Controls.Add(Me.RadioButton1)
            resources.ApplyResources(Me.GroupBox1, "GroupBox1")
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.TabStop = False
            '
            'Button6
            '
            resources.ApplyResources(Me.Button6, "Button6")
            Me.Button6.Name = "Button6"
            Me.Button6.UseVisualStyleBackColor = True
            '
            'ComboBox9
            '
            Me.ComboBox9.FormattingEnabled = True
            resources.ApplyResources(Me.ComboBox9, "ComboBox9")
            Me.ComboBox9.Name = "ComboBox9"
            '
            'Label7
            '
            resources.ApplyResources(Me.Label7, "Label7")
            Me.Label7.Name = "Label7"
            '
            'NumericUpDown3
            '
            resources.ApplyResources(Me.NumericUpDown3, "NumericUpDown3")
            Me.NumericUpDown3.Name = "NumericUpDown3"
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.ComboBox10)
            Me.GroupBox3.Controls.Add(Me.NumericUpDown2)
            Me.GroupBox3.Controls.Add(Me.ComboBox6)
            Me.GroupBox3.Controls.Add(Me.NumericUpDown1)
            Me.GroupBox3.Controls.Add(Me.Label6)
            Me.GroupBox3.Controls.Add(Me.Label5)
            Me.GroupBox3.Controls.Add(Me.ComboBox5)
            resources.ApplyResources(Me.GroupBox3, "GroupBox3")
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.TabStop = False
            '
            'ComboBox10
            '
            Me.ComboBox10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox10.FormattingEnabled = True
            Me.ComboBox10.Items.AddRange(New Object() {resources.GetString("ComboBox10.Items"), resources.GetString("ComboBox10.Items1")})
            resources.ApplyResources(Me.ComboBox10, "ComboBox10")
            Me.ComboBox10.Name = "ComboBox10"
            '
            'NumericUpDown2
            '
            resources.ApplyResources(Me.NumericUpDown2, "NumericUpDown2")
            Me.NumericUpDown2.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.NumericUpDown2.Name = "NumericUpDown2"
            '
            'ComboBox6
            '
            Me.ComboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox6.FormattingEnabled = True
            Me.ComboBox6.Items.AddRange(New Object() {resources.GetString("ComboBox6.Items"), resources.GetString("ComboBox6.Items1"), resources.GetString("ComboBox6.Items2")})
            resources.ApplyResources(Me.ComboBox6, "ComboBox6")
            Me.ComboBox6.Name = "ComboBox6"
            '
            'NumericUpDown1
            '
            resources.ApplyResources(Me.NumericUpDown1, "NumericUpDown1")
            Me.NumericUpDown1.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.NumericUpDown1.Name = "NumericUpDown1"
            '
            'Label6
            '
            resources.ApplyResources(Me.Label6, "Label6")
            Me.Label6.Name = "Label6"
            '
            'Label5
            '
            resources.ApplyResources(Me.Label5, "Label5")
            Me.Label5.Name = "Label5"
            '
            'ComboBox5
            '
            Me.ComboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox5.FormattingEnabled = True
            Me.ComboBox5.Items.AddRange(New Object() {resources.GetString("ComboBox5.Items"), resources.GetString("ComboBox5.Items1")})
            resources.ApplyResources(Me.ComboBox5, "ComboBox5")
            Me.ComboBox5.Name = "ComboBox5"
            '
            'RadioButton3
            '
            resources.ApplyResources(Me.RadioButton3, "RadioButton3")
            Me.RadioButton3.Name = "RadioButton3"
            Me.RadioButton3.UseVisualStyleBackColor = True
            '
            'RadioButton2
            '
            resources.ApplyResources(Me.RadioButton2, "RadioButton2")
            Me.RadioButton2.Name = "RadioButton2"
            Me.RadioButton2.UseVisualStyleBackColor = True
            '
            'RadioButton1
            '
            resources.ApplyResources(Me.RadioButton1, "RadioButton1")
            Me.RadioButton1.Checked = True
            Me.RadioButton1.Name = "RadioButton1"
            Me.RadioButton1.TabStop = True
            Me.RadioButton1.UseVisualStyleBackColor = True
            '
            'TabPage3
            '
            Me.TabPage3.Controls.Add(Me.GroupBox5)
            Me.TabPage3.Controls.Add(Me.GroupBox4)
            resources.ApplyResources(Me.TabPage3, "TabPage3")
            Me.TabPage3.Name = "TabPage3"
            Me.TabPage3.UseVisualStyleBackColor = True
            '
            'GroupBox5
            '
            Me.GroupBox5.Controls.Add(Me.Label13)
            Me.GroupBox5.Controls.Add(Me.ProgressBar2)
            Me.GroupBox5.Controls.Add(Me.ComboBox11)
            Me.GroupBox5.Controls.Add(Me.Button3)
            resources.ApplyResources(Me.GroupBox5, "GroupBox5")
            Me.GroupBox5.Name = "GroupBox5"
            Me.GroupBox5.TabStop = False
            '
            'Label13
            '
            resources.ApplyResources(Me.Label13, "Label13")
            Me.Label13.Name = "Label13"
            '
            'ProgressBar2
            '
            resources.ApplyResources(Me.ProgressBar2, "ProgressBar2")
            Me.ProgressBar2.Name = "ProgressBar2"
            '
            'ComboBox11
            '
            Me.ComboBox11.FormattingEnabled = True
            Me.ComboBox11.Items.AddRange(New Object() {resources.GetString("ComboBox11.Items"), resources.GetString("ComboBox11.Items1")})
            resources.ApplyResources(Me.ComboBox11, "ComboBox11")
            Me.ComboBox11.Name = "ComboBox11"
            '
            'Button3
            '
            resources.ApplyResources(Me.Button3, "Button3")
            Me.Button3.Name = "Button3"
            Me.Button3.UseVisualStyleBackColor = True
            '
            'GroupBox4
            '
            Me.GroupBox4.Controls.Add(Me.ProgressBar1)
            Me.GroupBox4.Controls.Add(Me.Button5)
            Me.GroupBox4.Controls.Add(Me.TextBox1)
            Me.GroupBox4.Controls.Add(Me.NumericUpDown4)
            Me.GroupBox4.Controls.Add(Me.Label9)
            Me.GroupBox4.Controls.Add(Me.Label8)
            Me.GroupBox4.Controls.Add(Me.ComboBox7)
            Me.GroupBox4.Controls.Add(Me.Button2)
            resources.ApplyResources(Me.GroupBox4, "GroupBox4")
            Me.GroupBox4.Name = "GroupBox4"
            Me.GroupBox4.TabStop = False
            '
            'ProgressBar1
            '
            resources.ApplyResources(Me.ProgressBar1, "ProgressBar1")
            Me.ProgressBar1.Name = "ProgressBar1"
            '
            'Button5
            '
            resources.ApplyResources(Me.Button5, "Button5")
            Me.Button5.Name = "Button5"
            Me.Button5.UseVisualStyleBackColor = True
            '
            'TextBox1
            '
            resources.ApplyResources(Me.TextBox1, "TextBox1")
            Me.TextBox1.Name = "TextBox1"
            '
            'NumericUpDown4
            '
            resources.ApplyResources(Me.NumericUpDown4, "NumericUpDown4")
            Me.NumericUpDown4.Maximum = New Decimal(New Integer() {400, 0, 0, 0})
            Me.NumericUpDown4.Name = "NumericUpDown4"
            Me.NumericUpDown4.Value = New Decimal(New Integer() {200, 0, 0, 0})
            '
            'Label9
            '
            resources.ApplyResources(Me.Label9, "Label9")
            Me.Label9.Name = "Label9"
            '
            'Label8
            '
            resources.ApplyResources(Me.Label8, "Label8")
            Me.Label8.Name = "Label8"
            '
            'ComboBox7
            '
            Me.ComboBox7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox7.FormattingEnabled = True
            Me.ComboBox7.Items.AddRange(New Object() {resources.GetString("ComboBox7.Items"), resources.GetString("ComboBox7.Items1")})
            resources.ApplyResources(Me.ComboBox7, "ComboBox7")
            Me.ComboBox7.Name = "ComboBox7"
            '
            'Button2
            '
            resources.ApplyResources(Me.Button2, "Button2")
            Me.Button2.Name = "Button2"
            Me.Button2.UseVisualStyleBackColor = True
            '
            'ToolStrip1
            '
            resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripButton2, Me.ToolStripButton3, Me.ToolStripSeparator1, Me.ToolStripButton4, Me.ToolStripButton5, Me.ToolStripComboBox1, Me.ToolStripButton6, Me.ToolStripButton7, Me.ToolStripSeparator2, Me.ToolStripDropDownButton1})
            Me.ToolStrip1.Name = "ToolStrip1"
            '
            'ToolStripButton1
            '
            Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton1, "ToolStripButton1")
            Me.ToolStripButton1.Name = "ToolStripButton1"
            '
            'ToolStripButton2
            '
            Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton2, "ToolStripButton2")
            Me.ToolStripButton2.Name = "ToolStripButton2"
            '
            'ToolStripButton3
            '
            Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton3, "ToolStripButton3")
            Me.ToolStripButton3.Name = "ToolStripButton3"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'ToolStripButton4
            '
            Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.ToolStripButton4, "ToolStripButton4")
            Me.ToolStripButton4.Name = "ToolStripButton4"
            '
            'ToolStripButton5
            '
            Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.ToolStripButton5, "ToolStripButton5")
            Me.ToolStripButton5.Name = "ToolStripButton5"
            '
            'ToolStripComboBox1
            '
            resources.ApplyResources(Me.ToolStripComboBox1, "ToolStripComboBox1")
            Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
            '
            'ToolStripButton6
            '
            Me.ToolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.ToolStripButton6, "ToolStripButton6")
            Me.ToolStripButton6.Name = "ToolStripButton6"
            '
            'ToolStripButton7
            '
            Me.ToolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.ToolStripButton7, "ToolStripButton7")
            Me.ToolStripButton7.Name = "ToolStripButton7"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
            '
            'ToolStripDropDownButton1
            '
            resources.ApplyResources(Me.ToolStripDropDownButton1, "ToolStripDropDownButton1")
            Me.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
            '
            'TabPage4
            '
            resources.ApplyResources(Me.TabPage4, "TabPage4")
            Me.TabPage4.Name = "TabPage4"
            Me.TabPage4.UseVisualStyleBackColor = True
            '
            'Dlg
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ToolStripContainer1)
            Me.MinimumSize = New System.Drawing.Size(300, 200)
            Me.Name = "Dlg"
            Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
            Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
            Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
            Me.ToolStripContainer1.ResumeLayout(False)
            Me.ToolStripContainer1.PerformLayout()
            Me.TabControl1.ResumeLayout(False)
            Me.TabPage1.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.TabPage2.ResumeLayout(False)
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TabPage3.ResumeLayout(False)
            Me.GroupBox5.ResumeLayout(False)
            Me.GroupBox5.PerformLayout()
            Me.GroupBox4.ResumeLayout(False)
            Me.GroupBox4.PerformLayout()
            CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton2 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton3 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripButton4 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton5 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripComboBox1 As System.Windows.Forms.ToolStripComboBox
        Friend WithEvents ToolStripButton6 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton7 As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents Button1 As System.Windows.Forms.Button
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents ComboBox4 As System.Windows.Forms.ComboBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents ComboBox3 As System.Windows.Forms.ComboBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents ComboBox2 As System.Windows.Forms.ComboBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
        Friend WithEvents ComboBox6 As System.Windows.Forms.ComboBox
        Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents ComboBox5 As System.Windows.Forms.ComboBox
        Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton
        Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
        Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
        Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
        Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
        Friend WithEvents Button3 As System.Windows.Forms.Button
        Friend WithEvents NumericUpDown4 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents ComboBox7 As System.Windows.Forms.ComboBox
        Friend WithEvents Button2 As System.Windows.Forms.Button
        Friend WithEvents Label10 As System.Windows.Forms.Label
        Friend WithEvents Button4 As System.Windows.Forms.Button
        Friend WithEvents ComboBox8 As System.Windows.Forms.ComboBox
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents Label12 As System.Windows.Forms.Label
        Friend WithEvents ComboBox9 As System.Windows.Forms.ComboBox
        Friend WithEvents ComboBox10 As System.Windows.Forms.ComboBox
        Friend WithEvents Label13 As System.Windows.Forms.Label
        Friend WithEvents ProgressBar2 As System.Windows.Forms.ProgressBar
        Friend WithEvents ComboBox11 As System.Windows.Forms.ComboBox
        Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
        Friend WithEvents Button5 As System.Windows.Forms.Button
        Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
        Friend WithEvents Button6 As System.Windows.Forms.Button
        Friend WithEvents TabPage4 As System.Windows.Forms.TabPage

#End Region




    End Class
End Namespace