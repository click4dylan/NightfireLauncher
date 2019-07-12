<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddAServer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AddAServer))
        Me.addserverdone = New System.Windows.Forms.Button()
        Me.ipqueryport = New System.Windows.Forms.TextBox()
        Me.iplabel = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'addserverdone
        '
        Me.addserverdone.Location = New System.Drawing.Point(201, 34)
        Me.addserverdone.Name = "addserverdone"
        Me.addserverdone.Size = New System.Drawing.Size(75, 23)
        Me.addserverdone.TabIndex = 1
        Me.addserverdone.Text = "Add Server"
        Me.addserverdone.UseVisualStyleBackColor = True
        '
        'ipqueryport
        '
        Me.ipqueryport.Location = New System.Drawing.Point(84, 8)
        Me.ipqueryport.Name = "ipqueryport"
        Me.ipqueryport.Size = New System.Drawing.Size(192, 20)
        Me.ipqueryport.TabIndex = 0
        '
        'iplabel
        '
        Me.iplabel.AutoSize = True
        Me.iplabel.Location = New System.Drawing.Point(12, 11)
        Me.iplabel.Name = "iplabel"
        Me.iplabel.Size = New System.Drawing.Size(66, 13)
        Me.iplabel.TabIndex = 2
        Me.iplabel.Text = "IP:Queryport"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(12, 63)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(264, 13)
        Me.label2.TabIndex = 3
        Me.label2.Text = "Adding a server also notifies the master of its existence"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 39)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(161, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Example: 124.123.456.120:6550"
        '
        'AddAServer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(289, 80)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.iplabel)
        Me.Controls.Add(Me.ipqueryport)
        Me.Controls.Add(Me.addserverdone)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AddAServer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add a server"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents addserverdone As System.Windows.Forms.Button
    Friend WithEvents ipqueryport As System.Windows.Forms.TextBox
    Friend WithEvents iplabel As System.Windows.Forms.Label
    Friend WithEvents label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
