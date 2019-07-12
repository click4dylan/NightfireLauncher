<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdateMessageBox
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UpdateMessageBox))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.textbox = New System.Windows.Forms.RichTextBox()
        Me.no = New System.Windows.Forms.Button()
        Me.yes = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.textbox)
        Me.Panel1.Location = New System.Drawing.Point(13, 13)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(445, 339)
        Me.Panel1.TabIndex = 0
        '
        'textbox
        '
        Me.textbox.BackColor = System.Drawing.Color.White
        Me.textbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.textbox.Location = New System.Drawing.Point(0, 0)
        Me.textbox.Name = "textbox"
        Me.textbox.ReadOnly = True
        Me.textbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical
        Me.textbox.ShortcutsEnabled = False
        Me.textbox.Size = New System.Drawing.Size(445, 339)
        Me.textbox.TabIndex = 0
        Me.textbox.Text = "                  There is a new update available for James Bond 007: Nightfire. " & _
    "" & Global.Microsoft.VisualBasic.ChrW(10) & " Press yes to download the latest patch, or press no to continue. Details are l" & _
    "isted below:" & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'no
        '
        Me.no.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.no.Location = New System.Drawing.Point(383, 362)
        Me.no.Name = "no"
        Me.no.Size = New System.Drawing.Size(75, 23)
        Me.no.TabIndex = 1
        Me.no.Text = "No"
        Me.no.UseVisualStyleBackColor = True
        '
        'yes
        '
        Me.yes.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.yes.Location = New System.Drawing.Point(302, 362)
        Me.yes.Name = "yes"
        Me.yes.Size = New System.Drawing.Size(75, 23)
        Me.yes.TabIndex = 2
        Me.yes.Text = "Yes"
        Me.yes.UseVisualStyleBackColor = True
        '
        'UpdateMessageBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.no
        Me.ClientSize = New System.Drawing.Size(470, 397)
        Me.Controls.Add(Me.yes)
        Me.Controls.Add(Me.no)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "UpdateMessageBox"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "A New Update is Available"
        Me.TopMost = True
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents textbox As System.Windows.Forms.RichTextBox
    Friend WithEvents no As System.Windows.Forms.Button
    Friend WithEvents yes As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
