Imports System.IO
Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports System.Runtime.InteropServices
Imports System.Net

Public Class Form1
    Public commandoutput As String
    Public commandoutputscript As String
    Public treeview1isclicked As Integer
    Public currentpath As String = ""
    Public historyback(0 To 5) As String
    Public historyforward(0 To 5) As String

    <DllImport("uxtheme", CharSet:=CharSet.Unicode)> _
    Public Shared Function SetWindowTheme(ByVal hWnd As IntPtr, ByVal textSubAppName As String, ByVal textSubIdList As String) As Integer
    End Function
    '
    '
    '
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles Me.Load
reload:
        ToolStripStatusLabel1.Text = "loading...."
        If Device_connected(1) = True Then
            Process2.StartInfo.Arguments = device & " shell"
            Process2.Start()
            'Remount system RW
            adb_command("remount")
            'retrive all data about apps
            getdataapps()
            'retrive all data from device's root dir
            explorerexplore("/", False)
            ToolStripTextBox1.Text = "Home"
            'instruction's for opening apk file's with program
            If Not Command() = Nothing Then
                Dialog1.APKhandler.FileName = Command()
                Dialog1.ShowDialog()
            End If
            'device stay's connected checker
            Timer1.Enabled = True
            'listview theme
            SetWindowTheme(ListView1.Handle, "explorer", Nothing)
            'listview select first item (for having no error)
            Me.ListView1.Focus()
            Me.ListView1.Items(0).Selected = True

        Else
            If Not device = Nothing Then
                GoTo reload
            Else
                MsgBox("No device connected" & vbNewLine & "Enable USB-debugging on device" & vbNewLine & "Press Refresh to try again")
                ToolStripStatusLabel1.Text = "No device connected!"
            End If
        End If
        BackgroundWorker1.RunWorkerAsync()
    End Sub
    '
    '
    '
    Private Sub Form1_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        TreeView1.Size = New Size(Width - 130, Height - 129)
        Button1.Location = New Point(Width - 120, 6)
        Button2.Location = New Point(Width - 120, 35)
        Button3.Location = New Point(Width - 120, 64)
        ListView1.Size = New Size(Width - 33, Height - 140)
    End Sub
    '
    '
    '
    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Device_connected(0) = True Then
            Process2.Kill()
        End If

    End Sub
    '
    '
    '
    Private Sub BackgroundWorker1_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        checkversion(False)
        If ToolStripStatusLabel1.Text = "Checking for updates..." Then
            ToolStripStatusLabel1.Text = "Done!"
        End If
    End Sub
    '
    '
    'events from application tabs
    Private Sub Button1_Click_1(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        pullapp()
    End Sub
    '
    '
    '
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        installapk()
    End Sub
    '
    '
    '
    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        uninstallapp()
    End Sub
    '
    '
    '
    'events from menustrip
    Private Sub InstallApplicationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles InstallApplicationToolStripMenuItem.Click
        installapk()
    End Sub
    '
    '
    '
    Private Sub RefreshToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        Form1_Load(Nothing, Nothing)
    End Sub
    '
    '
    '
    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub
    '
    '
    '
    Private Sub RestoreDeletedSystemAppsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RestoreDeletedSystemAppsToolStripMenuItem.Click
        Restore_system_apps.ShowDialog()
    End Sub
    '
    '
    '
    Private Sub CheckForUpdatesToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CheckForUpdatesToolStripMenuItem.Click
        If BackgroundWorker1.IsBusy = False Then
            checkversion(True)
        Else
            ToolStripStatusLabel1.Text = "Checking for updates..."
        End If
    End Sub
    '
    '
    '
    Private Sub AboutEasyADBToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AboutEasyADBToolStripMenuItem.Click
        AboutBox1.ShowDialog()
    End Sub
    '
    '
    'event from device explorer
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If Process2.HasExited Then
            Timer1.Enabled = False
            MsgBox("Device Disconected" & vbNewLine & "Press ""refresh"" if device is connected again")
            ListView1.Items.Clear()
            TreeView1.Nodes.Clear()
        End If
    End Sub
    '
    '
    '
    Private Sub ListView1_DoubleClick(sender As Object, e As System.EventArgs) Handles ListView1.DoubleClick
        Dialog2.ListView1.Items.Clear()
        Select Case ListView1.SelectedItems(0).SubItems(1).Text
            Case "Link"
                currentpath = ListView1.SelectedItems(0).SubItems(4).Text
                ToolStripTextBox1.Text = currentpath
                explorerexplore(currentpath, False)
            Case "Folder"
                currentpath = currentpath & "/" & ListView1.SelectedItems.Item(0).Text
                explorerexplore(currentpath, False)
                If currentpath = Nothing Or currentpath = "/" Then
                    ToolStripTextBox1.Text = "Home"
                Else
                    ToolStripTextBox1.Text = currentpath
                End If
            Case "Android application package"
                Dialog2.ListView1.Items.Add(New ListViewItem("Pull to Folder", 0, Dialog2.ListView1.Groups.Item(0)))
                If Dialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Select Case Dialog2.dialog2result
                        Case "Open with ADB APK installer"
                            Form2.path = """" & currentpath & "/" & ListView1.SelectedItems(0).Text & """"
                            Form2.Show()
                        Case "Pull to Folder"
                            ToolStripButton3_Click(Nothing, Nothing)
                    End Select
                End If
            Case Else
                Dialog2.ListView1.Items.Add(New ListViewItem("Pull to Folder", 0, Dialog2.ListView1.Groups.Item(0)))
                Dialog2.ListView1.Items.Add(New ListViewItem("Open with ADB Textviewer", 1, Dialog2.ListView1.Groups.Item(0)))
                If Dialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Select Case Dialog2.dialog2result
                        Case "Open with ADB Textviewer"
                            Form2.path = """" & currentpath & "/" & ListView1.SelectedItems(0).Text & """"
                            Form2.Show()
                        Case "Pull to Folder"
                            ToolStripButton3_Click(Nothing, Nothing)
                    End Select
                End If
        End Select
        'listview select first item (for having no error)
        Try
            Me.ListView1.Focus()
            Me.ListView1.Items(0).Selected = True
        Catch
        End Try
    End Sub
    '
    '
    '
    Private Sub TreeView1_Click(sender As Object, e As System.EventArgs) Handles TreeView1.Click
        treeview1isclicked = True
    End Sub
    '
    '
    '
    Private Sub ToolStripButton1_Click(sender As Object, e As System.EventArgs) Handles ToolStripButton1.Click
        If Not currentpath = Nothing Then
            Try
                Dim count As Integer = currentpath.Split("/").Length - 1
                currentpath = currentpath.Remove(currentpath.Length - currentpath.Split("/")(count).Length - 1)
                explorerexplore(currentpath, False)
                ToolStripTextBox1.Text = currentpath
            Catch
                MsgBox("Can not return")
            End Try
        End If
    End Sub
    '
    '
    '
    Private Sub ToolStripButton2_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton2.Click
        currentpath = ToolStripTextBox1.Text
        explorerexplore(currentpath, False)
    End Sub
    '
    '
    '
    Private Sub ToolStripButton3_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton3.Click
        If APKpuller.ShowDialog() = Windows.Forms.DialogResult.OK Then
            adb_command("pull """ & currentpath & "/" & ListView1.SelectedItems(0).Text & """ """ & APKpuller.SelectedPath & """")
        End If
    End Sub
    '
    '
    '
    Private Sub ToolStripButton4_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton4.Click
        pushfilexplorer.TextBox2.Text = currentpath
        If pushfilexplorer.ShowDialog() = Windows.Forms.DialogResult.OK Then
            adb_command("shell [ -f """ & pushfilexplorer.TextBox2.Text & "/" & My.Computer.FileSystem.GetFileInfo(pushfilexplorer.TextBox1.Text).Name & """ ] && echo ""File exists"" || echo ""File does not exists""")
            If commandoutput = "File exists" Then
                If MsgBox("Warning, The file already exist! Do you want to overwrite?", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                    GoTo verder
                End If
            Else
verder:
                ToolStripStatusLabel1.Text = "Pushing File, Please wait ...."
                adb_command("push """ & pushfilexplorer.TextBox1.Text & """ """ & pushfilexplorer.TextBox2.Text & """")
                ToolStripStatusLabel1.Text = "Chmoding File, Please wait ...."
                adb_command("shell chmod " & pushfilexplorer.Label10.Text & " """ & pushfilexplorer.TextBox2.Text & "/" & My.Computer.FileSystem.GetFileInfo(pushfilexplorer.TextBox1.Text).Name & """")
                ToolStripStatusLabel1.Text = "Releading, Please wait ...."
                explorerexplore(currentpath, False)
                ToolStripStatusLabel1.Text = "Done!"
            End If
        End If
    End Sub
    '
    '
    '
    Private Sub ToolStripButton5_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton5.Click
        Try
            If ListView1.SelectedItems(0).SubItems(1).Text = "Folder" Then
                If MsgBox("Are you sure you wan to delete this folder", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                    ToolStripStatusLabel1.Text = "Deleting folder, Please wait..."
                    adb_command("remount")
                    adb_command("shell rm -r """ & currentpath & "/" & ListView1.SelectedItems(0).Text & """")
                    explorerexplore(currentpath, False)
                    ToolStripStatusLabel1.Text = "Done!"
                End If
            Else
                If MsgBox("Are you sure you wan to delete this file", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                    ToolStripStatusLabel1.Text = "Deleting file, Please wait..."
                    adb_command("remount")
                    adb_command("shell rm """ & currentpath & "/" & ListView1.SelectedItems(0).Text & """")
                    adb_command("shell [ -f """ & currentpath & "/" & ListView1.SelectedItems(0).Text & """ ] && echo ""File exists"" || echo ""File does not exists""")
                    If commandoutput = "File exists" Then
                        MsgBox("Failed")
                    End If
                    explorerexplore(currentpath, False)
                    ToolStripStatusLabel1.Text = "Done!"
                End If
            End If
        Catch
            MsgBox("No File selected!")
        End Try
    End Sub
    '
    '
    '
    Private Sub ToolStripButton6_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton6.Click
        adb_command("shell mkdir " & currentpath & "/" & InputBox("Give name to new folder"))
        explorerexplore(currentpath, False)
    End Sub
    '
    '
    '
    Private Sub ToolStripButton7_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton7.Click
        Try
            If ListView1.SelectedItems(0).ToString.Length > 0 Then
                If Permission.ShowDialog = Windows.Forms.DialogResult.OK Then
                    ToolStripStatusLabel1.Text = "Chmoding File, Please wait ...."
                    adb_command("shell chmod " & Permission.Label10.Text & " """ & currentpath & "/" & ListView1.SelectedItems(0).Text & """")
                    ToolStripStatusLabel1.Text = "Releading, Please wait ...."
                    explorerexplore(currentpath, False)
                    ToolStripStatusLabel1.Text = "Done!"
                End If
            End If
        Catch
            MsgBox("You need to select a file or folder!")
        End Try
    End Sub
    '
    '
    '
    Private Sub ToolStripButton8_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton8.Click
        Dim output As String = InputBox("search after?" & vbNewLine & "NOTE: Exact filename")
        If Not output = Nothing Then
            explorerexplore(output, True)
        End If
    End Sub
    '
    '
    'Functions
    Sub explorerexplore(path As String, search As Boolean)
        path.Replace("//", "/")
        If path = "Home" Or path = "home" Then
            path = "/"
        End If
        Process1.StartInfo.Arguments = device & " shell"
        Dim stopper As Integer = 0
        Process1.Start()
        If search = True Then
            Process1.StandardInput.WriteLine("cd """ & currentpath & """ ")
            Process1.StandardInput.WriteLine("find -name """ & path & """ -exec ls -1e --color=never {} \;")
        Else
            Process1.StandardInput.WriteLine("cd """ & path & """ ")
            Process1.StandardInput.WriteLine("busybox ls -le --color=never | grep ^d ")
            Process1.StandardInput.WriteLine("busybox ls -le --color=never | grep ^l ")
            Process1.StandardInput.WriteLine("busybox ls -le --color=never | grep ^- ")
        End If

        Process1.StandardInput.WriteLine("exit ")
        Dim count As Integer = 0
        ListView1.Items.Clear()
        Do Until Process1.StandardOutput.EndOfStream
            Dim output As String = Process1.StandardOutput.ReadLine
            If output.Contains(" ") And output.Contains("can't cd to") Then
                MsgBox("Error Path doesn't exits")
                currentpath = "/"
                ToolStripTextBox1.Text = currentpath
                explorerexplore(currentpath, False)
                GoTo errorline
            End If
            If Not output = Nothing And Not output.EndsWith(" ") Then
                If search = True Then
                    MsgBox(output)
                    Dim objectname As String = output.Split(" ")(output.Split(" ").Length - 1)
                    Dim objectpermissions As String = output.Substring(1, 9)
                    Dim filepic As Integer = 0
                    Dim filetype As String = ""
                    Dim filedatebuilder As String =
                        output.Substring(output.Length - objectname.Length - 21, 21).Replace("Jan", "January").Replace("Feb", "February").Replace("Mar", "March").Replace("Apr", "April").Replace("Jun", "June").Replace("Jul", "July").Replace("Aug", "August").Replace("Sep", "September").Replace("Oct", "October").Replace("Nov", "November").Replace("Dec", "December")
                    Dim filedate As String = filedatebuilder.Replace("  ", " ").Split(" ")(1) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(0) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(3) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(2)
                    Select Case objectname.Split(".")(objectname.Split(".").Length - 1)
                        Case "apk"
                            filepic = 3
                            filetype = "Android application package"
                        Case "PNG", "png"
                            filepic = 4
                            filetype = "PNG Image"
                        Case "jpg", "JPG"
                            filepic = 4
                            filetype = "JPG Image"
                        Case "prop"
                            filepic = 5
                            filetype = "System Info File"
                        Case "zip", "rar", "ZIP", "RAR", "2zip", "2ZIP", "7z"
                            filepic = 6
                            filetype = "Archive"
                        Case "odex"
                            filepic = 5
                            filetype = "Application Cache File"
                        Case "wav"
                            filepic = 7
                            filetype = "Music File"
                        Case "wav"
                            filepic = 10
                            filetype = "Music File"
                        Case "ogg"
                            filepic = 11
                            filetype = "Ringtone"
                        Case "ttf"
                            filepic = 8
                            filetype = "Font"
                        Case "GIF", "gif"
                            filepic = 9
                            filetype = "GIF Image"
                        Case "JAR", "jar"
                            filepic = 12
                            filetype = "java file"
                        Case "LOG", "log"
                            filepic = 13
                            filetype = "Log File"
                        Case "list", "LIST"
                            filepic = 13
                            filetype = "List File"
                        Case "TXT", "txt"
                            filepic = 13
                            filetype = "Text File"
                        Case "XML", "xml"
                            filepic = 14
                            filetype = "XML File"
                        Case "swf", "SWF", "avi", "AVI", "3gp", "3GP", "mp4", "MP4"
                            filepic = 15
                            filetype = "Movie File"
                        Case Else
                            filepic = 2
                            filetype = "File"
                    End Select
                    Dim item = ListView1.Items.Add(objectname, filepic)
                    item.SubItems.Add(filetype)
                    item.SubItems.Add("")
                    item.SubItems.Add(objectpermissions)
                Else
                    Dim objectname As String = output.Split(" ")(output.Split(" ").Length - 1)
                    Dim objectpermissions As String = output.Substring(1, 9)
                    Select Case output.Substring(0, 1)
                        Case "d"
                            Dim filedatebuilder As String =
                            output.Substring(output.Length - objectname.Length - 21, 21).Replace("Jan", "January").Replace("Feb", "February").Replace("Mar", "March").Replace("Apr", "April").Replace("Jun", "June").Replace("Jul", "July").Replace("Aug", "August").Replace("Sep", "September").Replace("Oct", "October").Replace("Nov", "November").Replace("Dec", "December")
                            Dim objectdate As String = filedatebuilder.Replace("  ", " ").Split(" ")(1) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(0) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(3) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(2)
                            ListView1.Items.Add(New ListViewItem(New String() {objectname, "Folder", objectdate, objectpermissions}, 0))
                        Case "l"
                            Dim filedatebuilder As String =
                            output.Substring(output.Length - objectname.Length - output.Split(" ")(output.Split(" ").Length - 3).Length - 25, 21).Replace("Jan", "January").Replace("Feb", "February").Replace("Mar", "March").Replace("Apr", "April").Replace("Jun", "June").Replace("Jul", "July").Replace("Aug", "August").Replace("Sep", "September").Replace("Oct", "October").Replace("Nov", "November").Replace("Dec", "December")
                            Dim objectdate As String = filedatebuilder.Replace("  ", " ").Split(" ")(1) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(0) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(3) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(2)
                            ListView1.Items.Add(New ListViewItem(New String() {output.Split(" ")(output.Split(" ").Length - 3), "Link", objectdate, objectpermissions, objectname}, 1))

                        Case "-"
                            Dim filepic As Integer = 0
                            Dim filetype As String = ""
                            Dim filedatebuilder As String =
                                output.Substring(output.Length - objectname.Length - 21, 21).Replace("Jan", "January").Replace("Feb", "February").Replace("Mar", "March").Replace("Apr", "April").Replace("Jun", "June").Replace("Jul", "July").Replace("Aug", "August").Replace("Sep", "September").Replace("Oct", "October").Replace("Nov", "November").Replace("Dec", "December")
                            Dim filedate As String = filedatebuilder.Replace("  ", " ").Split(" ")(1) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(0) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(3) & " " & filedatebuilder.Replace("  ", " ").Split(" ")(2)
                            Select Case objectname.Split(".")(objectname.Split(".").Length - 1)
                                Case "apk"
                                    filepic = 3
                                    filetype = "Android application package"
                                Case "PNG", "png"
                                    filepic = 4
                                    filetype = "PNG Image"
                                Case "jpg", "JPG"
                                    filepic = 4
                                    filetype = "JPG Image"
                                Case "prop"
                                    filepic = 5
                                    filetype = "System Info File"
                                Case "zip", "rar", "ZIP", "RAR", "2zip", "2ZIP", "7z"
                                    filepic = 6
                                    filetype = "Archive"
                                Case "odex"
                                    filepic = 5
                                    filetype = "Application Cache File"
                                Case "wav"
                                    filepic = 7
                                    filetype = "Music File"
                                Case "wav"
                                    filepic = 10
                                    filetype = "Music File"
                                Case "ogg"
                                    filepic = 11
                                    filetype = "Ringtone"
                                Case "ttf"
                                    filepic = 8
                                    filetype = "Font"
                                Case "GIF", "gif"
                                    filepic = 9
                                    filetype = "GIF Image"
                                Case "JAR", "jar"
                                    filepic = 12
                                    filetype = "java file"
                                Case "LOG", "log"
                                    filepic = 13
                                    filetype = "Log File"
                                Case "list", "LIST"
                                    filepic = 13
                                    filetype = "List File"
                                Case "TXT", "txt"
                                    filepic = 13
                                    filetype = "Text File"
                                Case "XML", "xml"
                                    filepic = 14
                                    filetype = "XML File"
                                Case "swf", "SWF", "avi", "AVI", "3gp", "3GP", "mp4", "MP4"
                                    filepic = 15
                                    filetype = "Movie File"
                                Case Else
                                    filepic = 2
                                    filetype = "File"
                            End Select
                            Dim item = ListView1.Items.Add(objectname, filepic)
                            item.SubItems.Add(filetype)
                            item.SubItems.Add(filedate)
                            item.SubItems.Add(objectpermissions)
                    End Select
                End If
            End If
        Loop
errorline:
        Process1.WaitForExit()
    End Sub
    '
    '
    '
    Sub uninstallapp()
        ToolStripStatusLabel1.Text = "unistalling application, please wait..."
        If treeview1isclicked = True Then
            If TreeView1.SelectedNode.Level = 1 Then
                Dim sort As Integer = TreeView1.SelectedNode.Parent.Index
                Select Case sort
                    Case 0
                        If MsgBox("Are You sure that you wanna uninstall """ & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & """", MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation) = MsgBoxResult.Yes Then
                            adb_command("uninstall " & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text.Replace("-1", "").Replace("-2", ""))
                            MsgBox(commandoutput)
                            getdataapps()
                        End If
                    Case 1
                        If MsgBox("Are You sure that you wanna uninstall """ & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & """", MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation) = MsgBoxResult.Yes Then
                            adb_command("uninstall " & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text.Replace("-1", "").Replace("-2", ""))
                            MsgBox(commandoutput)
                            getdataapps()
                        End If
                    Case 2
                        If MsgBox("Warning this is a system application, uninstalling this may cause malfunctioning of your device!!  are you sure you want to uninstall " & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text, MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                            adb_command("pull /system/app/" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk backup\" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk")
                            adb_command("remount")
                            adb_command("shell rm /system/app/" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk")
                            MsgBox("Done")
                        End If
                End Select
            End If
        End If
        getdataapps()
        ToolStripStatusLabel1.Text = "Done!"
    End Sub
    '
    '
    '
    Sub getdataapps()
        ToolStripStatusLabel1.Text = "Loading application data..."
        TreeView1.Nodes.Clear()
        TreeView1.Nodes.Add("DATA")
        adb_shell_script("script\GetappDATA.eadbss", 1, 0, "")
        TreeView1.Nodes.Add("SDCARD")
        adb_shell_script("script\GetappSDCARD.eadbss", 1, 1, "")
        TreeView1.Nodes.Add("SYSTEM")
        adb_shell_script("script\GetappSYSTEM.eadbss", 1, 2, "")
        ToolStripStatusLabel1.Text = "Done!"
        TreeView1.ExpandAll()
        TreeView1.TopNode = TreeView1.Nodes(0)
    End Sub
    '
    '
    '
    Sub pullapp()
        If treeview1isclicked = True Then
            If TreeView1.SelectedNode.Level = 1 Then
                ToolStripStatusLabel1.Text = "pulling application from device, please wait..."
                If APKpuller.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    Dim sort As Integer = TreeView1.SelectedNode.Parent.Index
                    Select Case sort
                        Case 0
                            adb_command("pull /data/app/" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk """ & APKpuller.SelectedPath & """")
                        Case 1
                            adb_command("pull /mnt/asec/" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & "/pkg.apk """ & APKpuller.SelectedPath & """")
                            My.Computer.FileSystem.RenameFile(APKpuller.SelectedPath & "\pkg.apk", TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk")
                        Case 2
                            adb_command("pull /system/app/" & TreeView1.Nodes(sort).Nodes(TreeView1.SelectedNode.Index).Text & ".apk """ & APKpuller.SelectedPath & """")
                    End Select
                End If
                ToolStripStatusLabel1.Text = "Done!"
            End If
        End If
    End Sub
    '
    '
    '
    Sub installapk()
        'install application
        If Dialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            ToolStripStatusLabel1.Text = "installing application, please wait..."
            If installcomand.Contains("#INSTALLSYSTEM#") Then
                Dim filename As String = My.Computer.FileSystem.GetFileInfo(installcomand.Replace("#INSTALLSYSTEM#", "")).Name
                adb_command("remount")
                adb_command("shell [ -f /system/app/" & filename & " ] && echo ""File exists"" || echo ""File does not exists""")
                If commandoutput = "File exists" Then
                    If MsgBox("Warning, The application already exist! Do you want to overwrite", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                        GoTo verder
                    End If
                Else
verder:
                    adb_command("push """ & installcomand.Replace("#INSTALLSYSTEM#", "") & """ /system/app")
                    adb_command("shell chmod 644 /system/app/" & filename)
                    If MsgBox("It is recommended that you reboot your device. do you want to reboot the device?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        adb_command("reboot")
                    End If
                End If
            Else
                adb_command(installcomand)
                Select Case commandoutput
                    Case "Failure [INSTALL_FAILED_ALREADY_EXISTS]"
                        ToolStripStatusLabel1.Text = "failed, application already exist"
                        MsgBox("failed, application already exist", MsgBoxStyle.Critical)
                    Case Else
                        MsgBox(commandoutput)
                        ToolStripStatusLabel1.Text = "done"
                        Clipboard.SetText(commandoutput)
                End Select
            End If
            getdataapps()
        End If
    End Sub
    '
    '
    '
    Sub adb_command(command As String)
        Process1.StartInfo.Arguments = device & " " & command
        Process1.Start()
        Do Until Process1.StandardOutput.EndOfStream
            Application.DoEvents()
            Dim output As String = Process1.StandardOutput.ReadLine
            If Not output = Nothing Then
                commandoutput = output
            End If
        Loop
        Process1.WaitForExit()
    End Sub

    Sub adb_shell_script(path As String, show As Integer, categorie As Integer, filename As String)
        Process1.StartInfo.Arguments = device & " shell"
        Dim script As FileStream = New FileStream(My.Computer.FileSystem.GetFileInfo(Application.ExecutablePath).Directory.FullName & "\" & path, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim scriptreader As StreamReader = New StreamReader(script)
        Process1.Start()
        Do Until scriptreader.EndOfStream
            Application.DoEvents()
            Dim input As String = scriptreader.ReadLine()
            Process1.StandardInput.WriteLine(input.Replace("#FILENAME#", filename) & " ")

        Loop
        scriptreader.Close()
        Do Until Process1.StandardOutput.EndOfStream
            Application.DoEvents()
            Dim output As String = Process1.StandardOutput.ReadLine
            If Not output = Nothing Then
                Select Case show
                    Case 0
                        If Not output.Contains(" ") And Not output.Contains(".tmp") And Not output.Contains("odex") Then
                            commandoutput = output
                            MsgBox(output)
                        End If
                    Case 1
                        If Not output.Contains(" ") And Not output.Contains(".tmp") And Not output.Contains("odex") Then
                            TreeView1.Nodes(categorie).Nodes.Add(output.Replace(".apk", ""))
                        End If
                End Select

            End If
        Loop
        script.Close()
        scriptreader.Close()
        Process1.WaitForExit()
    End Sub
    '
    '
    '
    Sub checkversion(message As Boolean)
        Try
            Dim version As WebClient = New WebClient
            Dim versionstream As Stream = version.OpenRead("https://dl.dropbox.com/u/23703101/Easy%20ADB/Version.txt")
            Dim versionreader As StreamReader = New StreamReader(versionstream)
            Dim newversion As String = versionreader.ReadLine
            If newversion.Replace(".", Nothing) > Application.ProductVersion.Replace(".", Nothing) Then
                If MsgBox("Update founded! Would you open the webbrouwser to install the Update" & vbNewLine & "Current version : " & Application.ProductVersion & vbNewLine & "New version : " & newversion, MsgBoxStyle.YesNo Or MsgBoxStyle.Information, ) = MsgBoxResult.Yes Then
                    Try
                        System.Diagnostics.Process.Start(versionreader.ReadLine())
                    Catch
                        MsgBox("On Error Occured", MsgBoxStyle.Critical)
                    End Try
                End If
            End If
        Catch
            If message = True Then
                MsgBox("On Error Occured", MsgBoxStyle.Critical)
            End If
        End Try
    End Sub


   
End Class
