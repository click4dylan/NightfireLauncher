<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SplashScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashScreen))
        Me.nightfirepclink = New System.Windows.Forms.LinkLabel()
        Me.nfsourcelink = New System.Windows.Forms.LinkLabel()
        Me.browseservers_btn = New System.Windows.Forms.Button()
        Me.options_btn = New System.Windows.Forms.Button()
        Me.exit_btn = New System.Windows.Forms.Button()
        Me.Hide_btn = New System.Windows.Forms.Button()
        Me.SplashScreenInfoLabel = New System.Windows.Forms.Label()
        Me.NightfireSourceBTN = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'nightfirepclink
        '
        Me.nightfirepclink.ActiveLinkColor = System.Drawing.Color.Transparent
        Me.nightfirepclink.AutoSize = True
        Me.nightfirepclink.BackColor = System.Drawing.Color.Transparent
        Me.nightfirepclink.Cursor = System.Windows.Forms.Cursors.Hand
        Me.nightfirepclink.DisabledLinkColor = System.Drawing.Color.Transparent
        Me.nightfirepclink.ForeColor = System.Drawing.Color.Transparent
        Me.nightfirepclink.LinkColor = System.Drawing.Color.Transparent
        Me.nightfirepclink.Location = New System.Drawing.Point(12, 0)
        Me.nightfirepclink.Name = "nightfirepclink"
        Me.nightfirepclink.Size = New System.Drawing.Size(153, 17)
        Me.nightfirepclink.TabIndex = 7
        Me.nightfirepclink.TabStop = True
        Me.nightfirepclink.Text = "nightfirepc.com                        "
        Me.nightfirepclink.UseCompatibleTextRendering = True
        Me.nightfirepclink.VisitedLinkColor = System.Drawing.Color.Transparent
        '
        'nfsourcelink
        '
        Me.nfsourcelink.ActiveLinkColor = System.Drawing.Color.Transparent
        Me.nfsourcelink.AutoSize = True
        Me.nfsourcelink.BackColor = System.Drawing.Color.Transparent
        Me.nfsourcelink.Cursor = System.Windows.Forms.Cursors.Hand
        Me.nfsourcelink.DisabledLinkColor = System.Drawing.Color.Transparent
        Me.nfsourcelink.ForeColor = System.Drawing.Color.Transparent
        Me.nfsourcelink.LinkColor = System.Drawing.Color.Transparent
        Me.nfsourcelink.Location = New System.Drawing.Point(12, 17)
        Me.nfsourcelink.Name = "nfsourcelink"
        Me.nfsourcelink.Size = New System.Drawing.Size(151, 17)
        Me.nfsourcelink.TabIndex = 8
        Me.nfsourcelink.TabStop = True
        Me.nfsourcelink.Text = "nightfiresource.com                "
        Me.nfsourcelink.UseCompatibleTextRendering = True
        Me.nfsourcelink.VisitedLinkColor = System.Drawing.Color.Transparent
        '
        'browseservers_btn
        '
        Me.browseservers_btn.BackColor = System.Drawing.Color.Transparent
        Me.browseservers_btn.BackgroundImage = Global.Bond.My.Resources.Resources.serverlist
        Me.browseservers_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.browseservers_btn.FlatAppearance.BorderSize = 0
        Me.browseservers_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.browseservers_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.browseservers_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.browseservers_btn.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.browseservers_btn.ForeColor = System.Drawing.Color.Black
        Me.browseservers_btn.Location = New System.Drawing.Point(576, 72)
        Me.browseservers_btn.Name = "browseservers_btn"
        Me.browseservers_btn.Size = New System.Drawing.Size(64, 24)
        Me.browseservers_btn.TabIndex = 9
        Me.browseservers_btn.UseVisualStyleBackColor = False
        '
        'options_btn
        '
        Me.options_btn.BackColor = System.Drawing.Color.Transparent
        Me.options_btn.BackgroundImage = Global.Bond.My.Resources.Resources.options
        Me.options_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.options_btn.FlatAppearance.BorderSize = 0
        Me.options_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.options_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.options_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.options_btn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.options_btn.ForeColor = System.Drawing.Color.Black
        Me.options_btn.Location = New System.Drawing.Point(576, 48)
        Me.options_btn.Name = "options_btn"
        Me.options_btn.Size = New System.Drawing.Size(64, 24)
        Me.options_btn.TabIndex = 10
        Me.options_btn.UseVisualStyleBackColor = False
        '
        'exit_btn
        '
        Me.exit_btn.BackColor = System.Drawing.Color.Transparent
        Me.exit_btn.BackgroundImage = CType(resources.GetObject("exit_btn.BackgroundImage"), System.Drawing.Image)
        Me.exit_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.exit_btn.FlatAppearance.BorderSize = 0
        Me.exit_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.exit_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.exit_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.exit_btn.ForeColor = System.Drawing.Color.Black
        Me.exit_btn.Location = New System.Drawing.Point(576, 0)
        Me.exit_btn.Name = "exit_btn"
        Me.exit_btn.Size = New System.Drawing.Size(64, 24)
        Me.exit_btn.TabIndex = 12
        Me.exit_btn.UseVisualStyleBackColor = False
        '
        'Hide_btn
        '
        Me.Hide_btn.BackColor = System.Drawing.Color.Transparent
        Me.Hide_btn.BackgroundImage = Global.Bond.My.Resources.Resources.minimize
        Me.Hide_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.Hide_btn.FlatAppearance.BorderSize = 0
        Me.Hide_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.Hide_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.Hide_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Hide_btn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Hide_btn.ForeColor = System.Drawing.Color.Black
        Me.Hide_btn.Location = New System.Drawing.Point(576, 24)
        Me.Hide_btn.Name = "Hide_btn"
        Me.Hide_btn.Size = New System.Drawing.Size(64, 24)
        Me.Hide_btn.TabIndex = 11
        Me.Hide_btn.UseVisualStyleBackColor = False
        '
        'SplashScreenInfoLabel
        '
        Me.SplashScreenInfoLabel.AutoEllipsis = True
        Me.SplashScreenInfoLabel.BackColor = System.Drawing.Color.Transparent
        Me.SplashScreenInfoLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplashScreenInfoLabel.ForeColor = System.Drawing.Color.White
        Me.SplashScreenInfoLabel.Location = New System.Drawing.Point(117, 433)
        Me.SplashScreenInfoLabel.Name = "SplashScreenInfoLabel"
        Me.SplashScreenInfoLabel.Size = New System.Drawing.Size(438, 29)
        Me.SplashScreenInfoLabel.TabIndex = 13
        Me.SplashScreenInfoLabel.Text = "Initializing Launcher"
        Me.SplashScreenInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'NightfireSourceBTN
        '
        Me.NightfireSourceBTN.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.NightfireSourceBTN.Cursor = System.Windows.Forms.Cursors.Hand
        Me.NightfireSourceBTN.Enabled = False
        Me.NightfireSourceBTN.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.NightfireSourceBTN.FlatAppearance.BorderSize = 5
        Me.NightfireSourceBTN.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.NightfireSourceBTN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.NightfireSourceBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.NightfireSourceBTN.Font = New System.Drawing.Font("Arial", 8.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NightfireSourceBTN.Location = New System.Drawing.Point(6, 295)
        Me.NightfireSourceBTN.Name = "NightfireSourceBTN"
        Me.NightfireSourceBTN.Size = New System.Drawing.Size(209, 71)
        Me.NightfireSourceBTN.TabIndex = 14
        Me.NightfireSourceBTN.Text = "Click Here for Nightfire Source:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The new HD remake of Nightfire for PC with free" & _
    " Online Multiplayer!"
        Me.NightfireSourceBTN.UseVisualStyleBackColor = False
        Me.NightfireSourceBTN.Visible = False
        '
        'SplashScreen
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(640, 480)
        Me.Controls.Add(Me.NightfireSourceBTN)
        Me.Controls.Add(Me.SplashScreenInfoLabel)
        Me.Controls.Add(Me.browseservers_btn)
        Me.Controls.Add(Me.options_btn)
        Me.Controls.Add(Me.exit_btn)
        Me.Controls.Add(Me.Hide_btn)
        Me.Controls.Add(Me.nfsourcelink)
        Me.Controls.Add(Me.nightfirepclink)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SplashScreen"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Nightfire Launcher"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents nightfirepclink As System.Windows.Forms.LinkLabel
    Friend WithEvents nfsourcelink As System.Windows.Forms.LinkLabel
    Friend WithEvents browseservers_btn As System.Windows.Forms.Button
    Friend WithEvents options_btn As System.Windows.Forms.Button
    Friend WithEvents exit_btn As System.Windows.Forms.Button
    Friend WithEvents Hide_btn As System.Windows.Forms.Button
    Friend WithEvents SplashScreenInfoLabel As System.Windows.Forms.Label
    Friend WithEvents NightfireSourceBTN As System.Windows.Forms.Button

End Class
