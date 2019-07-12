<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ServerBrowser
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ServerBrowser))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ServerList = New System.Windows.Forms.ListView()
        Me.ServerName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Ping = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Players = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Map = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Version = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.IP = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Queryport = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Refreshh = New System.Windows.Forms.Button()
        Me.Connect_btn = New System.Windows.Forms.Button()
        Me.notifylabel = New System.Windows.Forms.Label()
        Me.addserver = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ServerList)
        Me.Panel1.Location = New System.Drawing.Point(12, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(679, 399)
        Me.Panel1.TabIndex = 0
        '
        'ServerList
        '
        Me.ServerList.AllowColumnReorder = True
        Me.ServerList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ServerName, Me.Ping, Me.Players, Me.Map, Me.Version, Me.IP, Me.Queryport})
        Me.ServerList.GridLines = True
        Me.ServerList.Location = New System.Drawing.Point(4, 4)
        Me.ServerList.MultiSelect = False
        Me.ServerList.Name = "ServerList"
        Me.ServerList.Size = New System.Drawing.Size(660, 392)
        Me.ServerList.TabIndex = 0
        Me.ServerList.UseCompatibleStateImageBehavior = False
        Me.ServerList.View = System.Windows.Forms.View.Details
        '
        'ServerName
        '
        Me.ServerName.Text = "Server Name"
        Me.ServerName.Width = 160
        '
        'Ping
        '
        Me.Ping.Text = "Ping"
        Me.Ping.Width = 35
        '
        'Players
        '
        Me.Players.Text = "Players"
        Me.Players.Width = 47
        '
        'Map
        '
        Me.Map.Text = "Map"
        Me.Map.Width = 118
        '
        'Version
        '
        Me.Version.Text = "Version"
        Me.Version.Width = 102
        '
        'IP
        '
        Me.IP.Text = "IP"
        Me.IP.Width = 128
        '
        'Queryport
        '
        Me.Queryport.Text = "Query Port"
        Me.Queryport.Width = 66
        '
        'Refresh
        '
        Me.Refreshh.Location = New System.Drawing.Point(520, 417)
        Me.Refreshh.Name = "Refresh"
        Me.Refreshh.Size = New System.Drawing.Size(75, 23)
        Me.Refreshh.TabIndex = 0
        Me.Refreshh.Text = "Refresh"
        Me.Refreshh.UseVisualStyleBackColor = True
        '
        'Connect_btn
        '
        Me.Connect_btn.Location = New System.Drawing.Point(601, 417)
        Me.Connect_btn.Name = "Connect_btn"
        Me.Connect_btn.Size = New System.Drawing.Size(75, 23)
        Me.Connect_btn.TabIndex = 1
        Me.Connect_btn.Text = "Connect"
        Me.Connect_btn.UseVisualStyleBackColor = True
        '
        'notifylabel
        '
        Me.notifylabel.AutoSize = True
        Me.notifylabel.Location = New System.Drawing.Point(21, 422)
        Me.notifylabel.Name = "notifylabel"
        Me.notifylabel.Size = New System.Drawing.Size(0, 13)
        Me.notifylabel.TabIndex = 3
        '
        'addserver
        '
        Me.addserver.Location = New System.Drawing.Point(419, 417)
        Me.addserver.Name = "addserver"
        Me.addserver.Size = New System.Drawing.Size(95, 23)
        Me.addserver.TabIndex = 2
        Me.addserver.Text = "Add New Server"
        Me.addserver.UseVisualStyleBackColor = True
        '
        'ServerBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(703, 447)
        Me.Controls.Add(Me.addserver)
        Me.Controls.Add(Me.notifylabel)
        Me.Controls.Add(Me.Connect_btn)
        Me.Controls.Add(Me.Refreshh)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ServerBrowser"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Nightfire Server Browser (-tobrowser Command Line Argument)"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ServerList As System.Windows.Forms.ListView
    Friend WithEvents ServerName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Ping As System.Windows.Forms.ColumnHeader
    Friend WithEvents Players As System.Windows.Forms.ColumnHeader
    Friend WithEvents Map As System.Windows.Forms.ColumnHeader
    Friend WithEvents Version As System.Windows.Forms.ColumnHeader
    Friend WithEvents Refreshh As System.Windows.Forms.Button
    Friend WithEvents IP As System.Windows.Forms.ColumnHeader
    Friend WithEvents Queryport As System.Windows.Forms.ColumnHeader
    Friend WithEvents Connect_btn As System.Windows.Forms.Button
    Friend WithEvents notifylabel As System.Windows.Forms.Label
    Friend WithEvents addserver As System.Windows.Forms.Button
End Class
